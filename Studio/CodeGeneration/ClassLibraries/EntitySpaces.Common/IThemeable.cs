using System;
using System.Collections.Generic;
using System.Text;

namespace EntitySpaces.Common
{
    // Interface to define that a class (Form or UserControl) can update its theme
    public interface IThemeable
    {
        void ApplyTheme();
    }
}
