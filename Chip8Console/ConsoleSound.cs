using System;
using System.Collections.Generic;
using System.Text;

using Chip8;

namespace Chip8Console
{
    public class ConsoleSound : ISound
    {
        public void Play()
        {
            Console.Beep();
        }
    }
}
