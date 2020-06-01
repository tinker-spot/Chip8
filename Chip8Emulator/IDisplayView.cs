using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public interface IDisplayView
    {
        DisplayModel DisplayModel { get; set; }

        void Refresh();
    }
}
