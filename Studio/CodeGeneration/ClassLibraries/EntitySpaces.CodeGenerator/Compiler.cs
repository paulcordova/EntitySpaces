using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace EntitySpaces.CodeGenerator
{
    /// <summary>
    /// Compiles a parsed template in the form of a CodeBuilder object and returns the compiler results.
    /// Uses Roslyn compiler instead of deprecated CodeDom for .NET 8 / .NET 10 compatibility
    /// </summary>
    internal class Compiler
    {
        internal static string TemplateCachePath = string.Empty;
        internal static string CompilerAssemblyPath = string.Empty;

        /// <summary>
        /// Compiles a parsed template in the form of a CodeBuilder object and returns the compiler results.
        /// </summary>
        internal static CompilerResults Compile(CodeBuilder code)
        {
            try
            {
                CompilerResults results = new CompilerResults(new System.CodeDom.Compiler.TempFileCollection());

                if (code == null || code.ToString().Length == 0)
                {
                    results.Errors.Add(new CompilerError("", 0, 0, "", "El código a compilar está vacío"));
                    return results;
                }

                if (code.CompileInMemory)
                {
                    return CompileInMemory(code);
                }
                else
                {
                    return CompileToFile(code);
                }
            }
            catch (Exception ex)
            {
                CompilerResults results = new CompilerResults(new System.CodeDom.Compiler.TempFileCollection());
                results.Errors.Add(new CompilerError("", 0, 0, "", $"Error de compilación: {ex.Message}"));
                return results;
            }
        }

        /// <summary>
        /// Compila el código en memoria usando Roslyn
        /// </summary>
        private static CompilerResults CompileInMemory(CodeBuilder code)
        {
            CompilerResults results = new CompilerResults(new System.CodeDom.Compiler.TempFileCollection());

            try
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code.ToString());
                var references = CollectReferences(code);
                string assemblyName = "esTemplate_" + Guid.NewGuid().ToString("N");

                var compilation = CSharpCompilation.Create(assemblyName)
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(references)
                    .AddSyntaxTrees(syntaxTree);

                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);

                    if (!result.Success)
                    {
                        var diagnostics = result.Diagnostics
                            .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                            .ToList();

                        foreach (var diagnostic in diagnostics)
                        {
                            string errorCode = diagnostic.Id;
                            string message = diagnostic.GetMessage();
                            int line = 0;
                            int column = 0;

                            try
                            {
                                var lineSpan = diagnostic.Location.GetLineSpan();
                                line = lineSpan.StartLinePosition.Line + 1;
                                column = lineSpan.StartLinePosition.Character + 1;
                            }
                            catch { }

                            results.Errors.Add(new CompilerError("", line, column, errorCode, message));
                        }

                        return results;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] assemblyBytes = ms.ToArray();

                    try
                    {
                        var assembly = System.Reflection.Assembly.Load(assemblyBytes);
                        results.CompiledAssembly = assembly;
                    }
                    catch (Exception ex)
                    {
                        results.Errors.Add(new CompilerError("", 0, 0, "", $"Error cargando ensamblado: {ex.Message}"));
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                results.Errors.Add(new CompilerError("", 0, 0, "", $"Excepción en compilación: {ex.Message}"));
                return results;
            }
        }

        /// <summary>
        /// Compila el código a archivo usando Roslyn
        /// </summary>
        private static CompilerResults CompileToFile(CodeBuilder code)
        {
            CompilerResults results = new CompilerResults(new System.CodeDom.Compiler.TempFileCollection());
            string codeFileName = null;
            string assemblyFileName = null;

            try
            {
                string baseName = "esCompiledTemplate_" + Guid.NewGuid().ToString().Replace("-", "");
                codeFileName = Path.Combine(TemplateCachePath, baseName + ".cs");
                assemblyFileName = Path.Combine(TemplateCachePath, baseName + ".dll");

                if (!Directory.Exists(TemplateCachePath))
                {
                    Directory.CreateDirectory(TemplateCachePath);
                }

                File.WriteAllText(codeFileName, code.ToString());
                results.TempFiles.AddFile(codeFileName, false);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code.ToString(), path: codeFileName);
                var references = CollectReferences(code);

                var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(assemblyFileName))
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(references)
                    .AddSyntaxTrees(syntaxTree);

                using (var fs = new FileStream(assemblyFileName, FileMode.Create))
                {
                    EmitResult result = compilation.Emit(fs);

                    if (!result.Success)
                    {
                        var diagnostics = result.Diagnostics
                            .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                            .ToList();

                        foreach (var diagnostic in diagnostics)
                        {
                            string errorCode = diagnostic.Id;
                            string message = diagnostic.GetMessage();
                            int line = 0;
                            int column = 0;

                            try
                            {
                                var lineSpan = diagnostic.Location.GetLineSpan();
                                line = lineSpan.StartLinePosition.Line + 1;
                                column = lineSpan.StartLinePosition.Character + 1;
                            }
                            catch { }

                            results.Errors.Add(new CompilerError(codeFileName, line, column, errorCode, message));
                        }

                        return results;
                    }
                }

                results.PathToAssembly = assemblyFileName;
                results.TempFiles.AddFile(assemblyFileName, false);

                try
                {
                    var assembly = System.Reflection.Assembly.LoadFrom(assemblyFileName);
                    results.CompiledAssembly = assembly;
                }
                catch (Exception ex)
                {
                    results.Errors.Add(new CompilerError("", 0, 0, "", $"Error cargando ensamblado: {ex.Message}"));
                }

                return results;
            }
            catch (Exception ex)
            {
                results.Errors.Add(new CompilerError("", 0, 0, "", $"Excepción en compilación: {ex.Message}\n{ex.StackTrace}"));

                try
                {
                    if (codeFileName != null && File.Exists(codeFileName))
                        File.Delete(codeFileName);
                    if (assemblyFileName != null && File.Exists(assemblyFileName))
                        File.Delete(assemblyFileName);
                }
                catch { }

                return results;
            }
        }

        /// <summary>
        /// Colecta las referencias de ensamblados asegurando que solo exista UNA versión 
        /// de cada ensamblado por nombre simple para evitar ambigüedades en Roslyn.
        /// </summary>
        private static List<MetadataReference> CollectReferences(CodeBuilder code)
        {
            // Diccionario para evitar registrar dos veces el mismo ensamblado (por nombre simple)
            var uniqueAssemblies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            void TryRegisterAssembly(string path)
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

                try
                {
                    string asmName = Path.GetFileNameWithoutExtension(path);

                    // Si el ensamblado no ha sido agregado, se registra.
                    // Esto prioriza las referencias explícitas agregadas primero.
                    if (!uniqueAssemblies.ContainsKey(asmName))
                    {
                        uniqueAssemblies[asmName] = path;
                    }
                }
                catch { }
            }

            // 1. Referencias explícitas definidas en CodeBuilder / CompilerAssemblyPath
            if (code.References != null && code.References.Count > 0)
            {
                foreach (string reference in code.References)
                {
                    string assemblyPath = null;
                    string dllName = reference.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? reference : reference + ".dll";

                    if (!string.IsNullOrEmpty(CompilerAssemblyPath))
                    {
                        string testPath = Path.Combine(CompilerAssemblyPath, dllName);
                        if (File.Exists(testPath)) assemblyPath = testPath;
                    }

                    if (assemblyPath == null && File.Exists(dllName))
                    {
                        assemblyPath = dllName;
                    }

                    if (assemblyPath == null)
                    {
                        try
                        {
                            var asmName = Path.GetFileNameWithoutExtension(reference);
                            var asm = AppDomain.CurrentDomain.GetAssemblies()
                                .FirstOrDefault(a => string.Equals(a.GetName().Name, asmName, StringComparison.OrdinalIgnoreCase));

                            if (asm != null && !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location))
                            {
                                assemblyPath = asm.Location;
                            }
                        }
                        catch { }
                    }

                    if (assemblyPath != null)
                    {
                        TryRegisterAssembly(assemblyPath);
                    }
                }
            }

            // 2. Ensamblados cargados en el AppDomain actual
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location) && File.Exists(asm.Location))
                    {
                        TryRegisterAssembly(asm.Location);
                    }
                }
                catch { }
            }

            // 3. Referencias del Runtime de .NET (TRUSTED_PLATFORM_ASSEMBLIES)
            string trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (!string.IsNullOrEmpty(trustedAssemblies))
            {
                foreach (var path in trustedAssemblies.Split(Path.PathSeparator))
                {
                    TryRegisterAssembly(path);
                }
            }
            else
            {
                string runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                if (Directory.Exists(runtimeDir))
                {
                    foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
                    {
                        TryRegisterAssembly(dll);
                    }
                }
            }

            // Construir la lista final de MetadataReference
            var references = new List<MetadataReference>();
            foreach (var path in uniqueAssemblies.Values)
            {
                try
                {
                    references.Add(MetadataReference.CreateFromFile(path));
                }
                catch { }
            }

            return references;
        }

        /// <summary>
        /// Carga todos los ensamblados del Runtime de .NET (incluyendo mscorlib facade y BCL)
        /// </summary>
        private static void AddCoreReferences(Action<string> addRef)
        {
            // A. Cargar TRUSTED_PLATFORM_ASSEMBLIES (contiene todas las DLLs del runtime activo, ej. System.Private.CoreLib, mscorlib facade, netstandard, etc.)
            string trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (!string.IsNullOrEmpty(trustedAssemblies))
            {
                var paths = trustedAssemblies.Split(Path.PathSeparator);
                foreach (var path in paths)
                {
                    addRef(path);
                }
            }
            else
            {
                // Fallback: Buscar en el directorio del Runtime de .NET (.NET Core / 8 / 10)
                string runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                if (Directory.Exists(runtimeDir))
                {
                    foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
                    {
                        addRef(dll);
                    }
                }
            }

            // B. Cargar todos los ensamblados cargados actualmente en el AppDomain que no sean dinámicos
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location))
                    {
                        addRef(asm.Location);
                    }
                }
                catch { }
            }
        }
    }
}