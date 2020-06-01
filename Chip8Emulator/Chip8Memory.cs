using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Chip8
{
    public class Chip8Memory
    {
        public UInt16 StartFontLocation { get => 0x0000; }

        public UInt16 StartProgramLocation { get => 0x0200; }

        public UInt16 MemorySize { get => 0x1000; }

        public byte[] Memory { get; private set; }

        static byte[] builtinFonts =
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public Chip8Memory()
        {
            Memory = new byte[MemorySize];
            LoadFonts();
        }

        public byte this[int index]
        {
            get { return Memory[index]; }

            set { Memory[index] = value; }

        }

        public UInt16 ReadWord(UInt16 address)
        {
            var word1 = Memory[address];

            var word2 = Memory[address + 1];

            return (UInt16)(word1 << 8 | word2);
        }

        public void LoadROM(string fileName)
        {
            using (var file = File.OpenRead(fileName))
            {
                if (file.Length >= (MemorySize - StartProgramLocation))
                    throw new Exception("ROM too long!");

                file.Read(Memory, StartProgramLocation, MemorySize - StartProgramLocation);
            }

        }

        private void LoadFonts()
        {
            builtinFonts.CopyTo(Memory, StartFontLocation);
        }
    }
}
