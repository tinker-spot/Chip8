using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class ParsedOpCode
    {
        public UInt16 OpCode { get; private set; } // The original opcode

        public UInt16 Instruction { get; private set; } // the highest 4 bits

        public UInt16 NNN { get; private set; } // the lowest 12 bits

        public byte NN { get; private set; } // the lowest 8 bits 

        public byte N { get; private set; } // the lowest 4 bits

        public byte X { get; private set; } // the lower 4 bits of the high byte

        public byte Y { get; private set; } // the upper 4 bits of the low byte

        public ParsedOpCode(UInt16 opCode)
        {
            OpCode = opCode;

            Instruction = (UInt16)((opCode & 0xF000) >> 12);
            NNN = (UInt16)(opCode & 0x0FFF);
            NN = (byte)(opCode & 0x00FF);
            N = (byte)(opCode & 0x00F);
            X = (byte)((opCode & 0x0F00) >> 8);
            Y = (byte)((opCode & 0x00F0) >> 4);
        }
    }
}
