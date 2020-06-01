using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Chip8
{
    public class Chip8Emulator
    {
        const int MEMORY_SIZE = 0x1000;

        public Chip8Memory Memory { get; private set; }

        public Chip8CPU CPU { get; private set; }

        public DisplayModel DisplayModel { get; private set; }

        public IKeyboardInput KeyboardInput { get; private set; }

        public ISound SoundPlayer { get; private set; }

        private Random random;

        public Chip8Emulator(IKeyboardInput keyboardInput, ISound audioPlayer)
        {
            DisplayModel = new DisplayModel();

            Memory = new Chip8Memory();

            CPU = new Chip8CPU(Memory);

            random = new Random();

            KeyboardInput = keyboardInput;

            SoundPlayer = audioPlayer;
        }

        public void Tick()
        {
            var opCode = CPU.GetNextInstruction();
            var parsedOpCode = new ParsedOpCode(opCode);

            DispatchOpcode(parsedOpCode);
        }

        public void Tick60hz()
        {
            CPU.Tick60hz();

            if (CPU.SoundTimer.CountdownCompleted)
            {
                CPU.SoundTimer.Reset();
                SoundPlayer.Play();
            }
        }

        public void LoadROM(string filePath)
        {
            Memory.LoadROM(filePath);
            CPU.ProgramCounter = 0x0200;

        }

        private void DispatchOpcode(ParsedOpCode parsedOpCode)
        {
            switch (parsedOpCode.Instruction)
            {
                case 0x0:
                    // 0xE0 Clear Display
                    if (parsedOpCode.NN == 0xE0)
                    {
                        DisplayModel.Clear();
                    }
                    // 0xEE Return from Subroutine
                    else if (parsedOpCode.NN == 0xEE)
                    {
                        CPU.Ret();
                    }
                    break;

                case 0x01:
                    // 0x1NNN Jump to address NNN
                    CPU.Jump(parsedOpCode.NNN);
                    break;

                case 0x02:
                    // 0x2NNN Execute subroutine at address NNN
                    CPU.Subroutine(parsedOpCode.NNN);
                    break;

                case 0x03:
                    // 0x3XNN Skip the next instruction if regoster VX == NN
                    CPU.SkipIfEqual(parsedOpCode.X, parsedOpCode.NN);
                    break;

                case 0x04:
                    // 0x4XNN Skip the next instruction if register VX != NN
                    CPU.SkipIfNotEqual(parsedOpCode.X, parsedOpCode.NN);
                    break;

                case 0x05:
                    // 0x5XY0 Skip the next instruction if register VX == VY
                    CPU.SkipIfXEqY(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x06:
                    // 0x6XNN Store the number NN in register VX
                    CPU.StoreInRegister(parsedOpCode.X, parsedOpCode.NN);
                    break;

                case 0x07:
                    // 0x7XNN Add the number NN to register VX
                    CPU.AddToRegister(parsedOpCode.X, parsedOpCode.NN);
                    break;

                case 0x08:
                    // Handle opcodes starting with 0x08 in a separate function
                    Dispatch0x08(parsedOpCode);
                    break;

                case 0x09:
                    // 0x9XY0 Skip the next instruction if register VX != VY
                    if (parsedOpCode.N == 0)
                    {
                        CPU.SkipNextInstructionIfVXneVY(parsedOpCode.X, parsedOpCode.Y);
                    }
                    break;

                case 0x0A:
                    // 0xANNN Store memeory at address NNN in register I
                    CPU.Index = parsedOpCode.NNN;
                    break;

                case 0x0B:
                    // 0xBNNN Jump to address NNN plus V0
                    CPU.JumpToAddressPlusV0(parsedOpCode.NNN);
                    break;

                case 0x0C:
                    // 0xCXNN Set VX to a random number with a mask of NN
                    byte number = (byte)(random.Next(parsedOpCode.NN));
                    CPU.V[parsedOpCode.X] = number;
                    break;

                case 0x0D:
                    // 0xDXYN Draw sprite at position VX, VY with N bytes of data at the address stored in I
                    // Set VF to 1 if any set pixels become unset. Set VF to 0 otherwise.
                    var unsetPixels = DisplayModel.DrawSprite(CPU.V[parsedOpCode.X], CPU.V[parsedOpCode.Y], Memory, CPU.Index, parsedOpCode.N);
                    CPU.V[0x0F] = (byte)(unsetPixels ? 1 : 0);
                    break;

                case 0x0E:
                    // Handle opcodes begining with 0xE
                    Dispatch0x0E(parsedOpCode);
                    break;

                case 0x0F:
                    // Handle opcodes begining with 0xF
                    Dispatch0x0F(parsedOpCode);
                    break;

            }
        }

        private void Dispatch0x08(ParsedOpCode parsedOpCode)
        {
            switch (parsedOpCode.N)
            {
                case 0x0:
                    // 0x8XY0 Store the value of register VY in VX
                    CPU.CopyRegisterYtoX(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x01:
                    // 0x8XY1 Set VX to VX OR VY
                    CPU.OrRegisterXY(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x02:
                    // 0x8XY2 Set VX to VX AND VY
                    CPU.AndRegisterXY(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x03:
                    // 0x8XY3 Set VX to VX XOR VY
                    CPU.XorRegisterXY(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x04:
                    // 0x8XY4 Add the value of VY to VX. 
                    CPU.AddRegisterYtoX(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x05:
                    // 0x8XY5 Subtract the value of VY from VX
                    CPU.SubtractRegisterYfromX(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x06:
                    // 0x8XY6 Store the value of register VY shifted to the right by one bit in register VX
                    CPU.ShiftVxBy1Right(parsedOpCode.X);
                    break;

                case 0x07:
                    // Set register VX to the value of VY minus VX
                    CPU.SubtractRegisterXFromY(parsedOpCode.X, parsedOpCode.Y);
                    break;

                case 0x0E:
                    // Store the value of register VY shifted left one bit in register VX.
                    // Set VF to the MSB prior to the shift
                    CPU.ShiftVxBy1Left(parsedOpCode.X);
                    break;

            }

        }

        private void Dispatch0x0E(ParsedOpCode parsedOpCode)
        {
            switch (parsedOpCode.NN)
            {
                case 0x9E:
                    // Skip the next instruction if the key mapped to the hex value in register VX is pressed
                    if (KeyboardInput.IsKeyPressed(CPU.V[parsedOpCode.X]))
                    {
                        CPU.ProgramCounter += 2;
                    }
                    break;

                case 0xA1:
                    // Skip the next instruction if the key mapped to the hex value in register VX is not pressed
                    if (!KeyboardInput.IsKeyPressed(CPU.V[parsedOpCode.X]))
                    {
                        CPU.ProgramCounter += 2;
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void Dispatch0x0F(ParsedOpCode parsedOpCode)
        {
            switch (parsedOpCode.NN)
            {
                case 0x07:
                    // Store the current value of the delay timer in register VX
                    CPU.V[parsedOpCode.X] = CPU.DelayTimer.Value;
                    break;

                case 0x0A:
                    // Wait for a keypress and store the result in register VX
                    var hexKey = KeyboardInput.WaitForKey();
                    CPU.V[parsedOpCode.X] = hexKey;

                    break;

                case 0x15:
                    // Set the delay timer to the value of register VX
                    CPU.DelayTimer.Value = parsedOpCode.X;
                    break;

                case 0x18:
                    // Set the sound timer to the value of register VX
                    CPU.SoundTimer.Value = parsedOpCode.X;
                    break;

                case 0x1E:
                    // Add the value stored in register VX to register I
                    CPU.Index += CPU.V[parsedOpCode.X];
                    break;

                case 0x29:
                    // Set I to the memory address of the sprite data stored in register VX
                    CPU.Index = (UInt16)(CPU.V[parsedOpCode.X] * 5 + Memory.StartFontLocation);
                    break;

                case 0x33:
                    // Store the binary-coded decimal equivalent of the value stored in register VX at addresses I, I + 1, and I + 2
                    var decimalNum = CPU.V[parsedOpCode.X];

                    Memory[CPU.Index] = (byte)(decimalNum / 100);
                    Memory[CPU.Index + 1] = (byte)((decimalNum % 100) / 10);
                    Memory[CPU.Index + 2] = (byte)((decimalNum % 10) % 10);
                    break;

                case 0x55:
                    // Store the values of registers V0 to VX inclusive in memory starting at address I.
                    // X + 1 is added to I after completion
                    for (int i = 0; i <= parsedOpCode.X; i += 1)
                    {
                        Memory[CPU.Index + i] = CPU.V[i];
                    }

                    CPU.Index += (UInt16)(parsedOpCode.X + 1);
                    break;

                case 0x65:
                    // Fill registers V0 to VX inclusive with the values stored in memory starting at address I
                    // X + 1 is added to I after operation
                    for (int i = 0; i <= parsedOpCode.X; i += 1)
                    {
                        CPU.V[i] = Memory[CPU.Index + i];
                    }

                    CPU.Index += (UInt16)(parsedOpCode.X + 1);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
