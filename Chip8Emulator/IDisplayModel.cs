using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public enum DisplayMode { Standard, Enhanced};

    public interface IDisplayModel
    {
        void Clear();

        int Width { get; }

        int Height { get; }

        DisplayMode DisplayMode { get; set; }
        
    }
}
