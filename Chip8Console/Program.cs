using System;
using System.Text;
using System.Timers;

using Chip8;

namespace Chip8Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Console.WindowWidth, 32);

            FillScreen();

            var consoleKeyboardInput = new ConsoleKeyboardInput();
            var consoleAudioPlayer = new ConsoleSound();

            Chip8Emulator emulator = new Chip8Emulator(consoleKeyboardInput, consoleAudioPlayer);
            var consoleDisplayView = new ConsoleDisplayView(emulator);

            if (args.Length > 0)
            {
                emulator.LoadROM(args[0]);

#if false
                var runChipEmulator = new RunChip8EmulatorTest(emulator, consoleDisplayView);
//                var runChipEmulator = new RunChip8EmulatorWithTimers(emulator, consoleDisplayView);

                try
                {
                    while (true)
                    {
                        runChipEmulator.Run();
                    }
                }
                catch (EndEmulationException)
                {

                }
#endif

#if true
                var runChipEmulator = new RunChip8EmulatorWithTimers(emulator, consoleDisplayView);

                while (true)
                {
                    runChipEmulator.Run();
                }

#endif

                Environment.ExitCode = 0;
                return;
            }

#if false
            var memory = emulator.Memory;
            memory[0x0200] = 0x62;
            memory[0x0201] = 0x0A;
            memory[0x0202] = 0x63;
            memory[0x0203] = 0x0C;
            memory[0x0204] = 0xA2;
            memory[0x0205] = 0x20;
            memory[0x0206] = 0xD2;
            memory[0x0207] = 0x36;
            memory[0x0208] = 0xD2;
            memory[0x0209] = 0x36;
            memory[0x020A] = 0x00; // Invalid!
            memory[0x0220] = 0xBA;
            memory[0x0221] = 0x7C;
            memory[0x0222] = 0xD6;
            memory[0x0223] = 0xFE;
            memory[0x0224] = 0x54;
            memory[0x0225] = 0xAA;
#endif


            Console.CursorVisible = true;
        }

        static void FillScreen()
        {
            Console.Clear();

            var sb = new StringBuilder(Console.WindowWidth);
            sb.Append('X', Console.WindowWidth);

            for (int row = 0; row < Console.WindowHeight; row += 1)
            {
                Console.SetCursorPosition(0, row);
                Console.Write(sb);
            }
        }
    }
}
