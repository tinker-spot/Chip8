using System;
using System.Collections.Generic;
using System.Text;

using Chip8;

namespace Chip8Console
{
    public class RunChip8EmulatorTest
    {
        private Chip8Emulator chip8Emulator;
        private ConsoleDisplayView displayView;

        public RunChip8EmulatorTest(Chip8Emulator _emulator, ConsoleDisplayView _displayView)
        {
            chip8Emulator = _emulator;
            displayView = _displayView;
        }

        public void Run()
        {
            int loopCount = 31;
            while (loopCount > 0)
            {
                chip8Emulator.Tick();
                if (chip8Emulator.DisplayModel.RefreshRequested)
                    displayView.Refresh();
                loopCount -= 1;
            }

            chip8Emulator.Tick60hz();
        }

        public void Stop()
        {

        }

    }
}
