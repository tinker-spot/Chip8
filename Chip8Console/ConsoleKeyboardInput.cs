using System;
using System.Collections.Generic;
using System.Text;

using Chip8;

namespace Chip8Console
{
    public class ConsoleKeyboardInput : IKeyboardInput
    {
        private Dictionary<System.ConsoleKey, byte> ConsoleKeyToHexMap { get; set; }

        public ConsoleKeyboardInput()
        {
            InitHexMap();
        }

        public bool IsKeyPressed(byte hexKey)
        {
            if (Console.KeyAvailable == false)
                return false;

            // Filter out non-hex mapped keys
            byte mappedHexKey = CheckKeys();
            if (mappedHexKey == 0xFF)
                return false;

            return (mappedHexKey == hexKey);
        }

        public byte WaitForKey()
        {
            while (true)
            {
                var hexKey = CheckKeys();
                if (hexKey != 0xFF)
                    return hexKey;
            }
        }

        private byte CheckKeys()
        {
            var keyInfo = Console.ReadKey(true);
            var consoleKey = keyInfo.Key;

            // Exit the program if escape key pressed
            if (consoleKey == ConsoleKey.Escape)
            {
                throw new EndEmulationException();
            }

            // Map the pressed key to a hex value. If mapping is unsuccessful, then report invalid key
            bool valid = ConsoleKeyToHexMap.TryGetValue(consoleKey, out byte mappedHexKey);
            if (!valid)
                mappedHexKey = 0xff;

            return mappedHexKey;
        }

        private void InitHexMap()
        {
            ConsoleKeyToHexMap = new Dictionary<System.ConsoleKey, byte>()
            {
                { ConsoleKey.D1, 0x01 },
                { ConsoleKey.D2, 0x02 },
                { ConsoleKey.D3, 0x03 },
                { ConsoleKey.D4, 0x0C },
                { ConsoleKey.Q , 0x04 },
                { ConsoleKey.W , 0x05 },
                { ConsoleKey.E , 0x06 },
                { ConsoleKey.R , 0x0D },
                { ConsoleKey.A , 0x07 },
                { ConsoleKey.S , 0x08 },
                { ConsoleKey.D , 0x09 },
                { ConsoleKey.F , 0x0E },
                { ConsoleKey.Z , 0x0A },
                { ConsoleKey.X , 0x00 },
                { ConsoleKey.C,  0x0B },
                { ConsoleKey.V , 0x0F },

            };
        }
    }
}
