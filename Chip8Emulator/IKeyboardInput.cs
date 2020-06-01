using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public interface IKeyboardInput
    {
        bool IsKeyPressed(byte key);

        byte WaitForKey();
    }
}
