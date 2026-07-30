using System;
using System.IO;
using System.Text;
using Microsoft.Win32;

using EntitySpaces;

namespace AddInInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Use centralized registry path from VersionInfo
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(VersionInfo.RegistryPath, false))
                {
                    if (key != null)
                    {
                        string basePath = (string)key.GetValue("Install_Dir");

                        if (!basePath.EndsWith(@"\"))
                        {
                            basePath += @"\";
                        }

                        // Build source path dynamically with ReleaseName
                        string source = Path.Combine(basePath, @"CodeGeneration\Bin", $"{VersionInfo.ProductNameShort}.AddIn");

                        string dest = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                        string text = string.Empty;

                        using (TextReader reader = new StreamReader(source))
                        {
                            text = reader.ReadToEnd();
                        }

                        // Replace placeholder with actual installation path
                        text = text.Replace("[PATH]", basePath);

                        string dir = Path.Combine(dest, @"Microsoft\MSEnvShared\AddIns\");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Build destination path dynamically
                        string destFile = Path.Combine(dest, @"Microsoft\MSEnvShared\AddIns\", $"{VersionInfo.ProductNameShort}.AddIn");

                        // Write to destination and source (both with same content)
                        // Note: Encoding.BigEndianUnicode is preserved for compatibility
                        using (StreamWriter writer = new StreamWriter(destFile, false, Encoding.BigEndianUnicode))
                        {
                            writer.Write(text);
                        }

                        using (StreamWriter writer = new StreamWriter(source, false, Encoding.BigEndianUnicode))
                        {
                            writer.Write(text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}
