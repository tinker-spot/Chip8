using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Chip8CPU
    {
        const int REGISTER_COUNT = 16;
        const int START_PROGRAM_COUNTER_LOCATION = 0x200;
        const int STACK_SIZE = 16;

        public byte[] V { get; private set; }

        public UInt16 ProgramCounter { get; set; }

        public UInt16 Index { get; set; }

        private Chip8Memory Memory { get; set; }

        private Chip8Emulator Emulator { get; set; }

        public UInt16[] Stack { get; private set; }

        public UInt16 StackPointer { get; private set; }

        public Timer DelayTimer { get; private set; }

        public Timer SoundTimer { get; private set; }

        public Chip8CPU(Chip8Memory memory)
        {
            Memory = memory;

            V = new byte[REGISTER_COUNT];
            ProgramCounter = START_PROGRAM_COUNTER_LOCATION;
            Index = 0;
            Stack = new UInt16[STACK_SIZE];
            StackPointer = 0;

            DelayTimer = new Timer();
            SoundTimer = new Timer();
        }

        public void Tick60hz()
        {
            DelayTimer.Tick();
            SoundTimer.Tick();

        }

        public UInt16 GetNextInstruction()
        {
            var result = Memory.ReadWord(ProgramCounter);
            ProgramCounter += 2;

            return result;
        }

        public void Push(UInt16 addr)
        {
            Stack[StackPointer] = addr;
            StackPointer += 1;
        }

        public UInt16 Pop()
        {
            StackPointer -= 1;
            return Stack[StackPointer];
        }

        public void Ret()
        {
            var addr = Pop();
            ProgramCounter = addr;
        }

        public void Jump(UInt16 addr)
        {
            ProgramCounter = addr;
        }

        public void Subroutine(UInt16 addr)
        {
            Push((UInt16)(ProgramCounter /* + 2 */));
            Jump(addr);
        }

        public void SkipIfEqual(byte registerNum, byte value)
        {
            var registerValue = V[registerNum];
            if (registerValue == value)
            {
                ProgramCounter += 2;
            }
        }

        public void SkipIfNotEqual(byte registerNum, byte value)
        {
            var registerValue = V[registerNum];
            if (registerValue != value)
            {
                ProgramCounter += 2;
            }
        }

        public void SkipIfXEqY(byte registerX, byte registerY)
        {
            var xValue = V[registerX];
            var yValue = V[registerY];

            if (xValue == yValue)
            {
                ProgramCounter += 2;
            }
        }

        public void SkipIfXNotEqY(byte registerX, byte registerY)
        {
            var xValue = V[registerX];
            var yValue = V[registerY];

            if (xValue != yValue)
            {
                ProgramCounter += 2;
            }
        }

        public void StoreInRegister(byte registerX, byte value)
        {
            V[registerX] = value;
        }

        public void AddToRegister(byte registerX, byte value)
        {
            V[registerX] += value;
        }

        public void CopyRegisterYtoX(byte registerX, byte registerY)
        {
            V[registerX] = V[registerY];
        }

        public void OrRegisterXY(byte registerX, byte registerY)
        {
            V[registerX] |= V[registerY];
        }

        public void AndRegisterXY(byte registerX, byte registerY)
        {
            V[registerX] &= V[registerY];
        }

        public void XorRegisterXY(byte registerX, byte registerY)
        {
            V[registerX] ^= V[registerY];
        }

        public void AddRegisterYtoX(byte registerX, byte registerY)
        {
            int result = V[registerX] + V[registerY];
            V[0xF] = (byte)((result > 255) ? 1 : 0);
            V[registerX] = (byte)(result & 0xff);
        }

        public void SubtractRegisterYfromX(byte registerX, byte registerY)
        {
            int result = V[registerX] - V[registerY];
            V[15] = (byte)((result >= 0) ? 1 : 0);
            V[registerX] = (byte)(result & 0xff);
        }

        public void ShiftVxBy1Right(byte registerX)
        {
            V[0xF] = (byte)(V[registerX] & 0x01);
            V[registerX] >>= 1;
        }

        public void SubtractRegisterXFromY(byte registerX, byte registerY)
        {
            int result = V[registerY] - V[registerX];
            V[0x0F] = (byte)((result >= 0) ? 1 : 0);
            V[registerX] = (byte)(result & 0xff);
        }

        public void ShiftVxBy1Left(byte registerX)
        {
            V[0x0F] = (byte)(((V[registerX] & 0x80) == 0x80) ? 1 : 0);
            V[registerX] <<= 1;
        }

        public void SkipNextInstructionIfVXneVY(byte registerX, byte registerY)
        {
            if (V[registerX] != V[registerY])
                ProgramCounter += sizeof(UInt16);
        }

        public void JumpToAddressPlusV0(UInt16 address)
        {
            ProgramCounter = (UInt16)(address + V[0]);
        }

        public void SetDelayTimer(byte registerX)
        {
            DelayTimer.Value = V[registerX];
        }
    }
}
