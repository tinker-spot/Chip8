using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class DisplayModel
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte[,] Pixels { get; private set; }

        private DisplayMode _displayMode;
        public DisplayMode DisplayMode { get => _displayMode; set { SetDisplayMode(value); } }

        public bool RefreshRequested { get; set; }

        public DisplayModel()
        {
            InitDisplay();
        }

        private void SetDisplayMode(DisplayMode displayMode)
        {
            if (DisplayMode == displayMode)
                return;

            if (displayMode == DisplayMode.Enhanced)
                throw new NotSupportedException();

            _displayMode = displayMode;
            InitDisplay();

        }

        public void InitDisplay()
        {
            Width = 64;
            Height = 32;

            if (DisplayMode == DisplayMode.Enhanced)
            {
                Width *= 2;
                Height *= 2;
            }

            Pixels = new byte[Height, Width];
            //            Pixels.Initialize();
            Clear();

        }

        public void Clear()
        {
            for (int height = 0; height < Height; height += 1)
                for (int width = 0; width < Width; width += 1)
                {
                    Pixels[height, width] = 0;
                }

            RefreshRequested = true;
        }

        public bool DrawSprite(int posX, int posY, Chip8Memory memory, UInt16 spriteAddress, byte spriteHeight)
        {
            bool pixelChanged = false;

            for (int spriteRow = 0; spriteRow < spriteHeight; spriteRow += 1)
            {
                byte currentBit = 128;

                var spriteData = memory[spriteAddress + spriteRow];
                var row = posY + spriteRow;
                var column = posX;

                if (row >= Height)
                    continue;

                for (int width = 0; width < 8; width += 1)
                {
                    if (column >= Width)
                        continue;

                    var spriteBit = spriteData & currentBit;

                    var newValue = (spriteBit != 0) ? 0xff : 0;

                    var oldValue = Pixels[row, column];
                    Pixels[row, column] ^= (byte)newValue;

                    if (oldValue != 0 && newValue == 0)
                        pixelChanged = true;

                    column += 1;
                    currentBit /= 2;                    
                }
            }

            RefreshRequested = true;

            return pixelChanged;
        }
    }
}
