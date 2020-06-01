using System;
using System.Collections.Generic;
using System.Text;

using Chip8;

namespace Chip8Console
{
    public class ConsoleDisplayView : IDisplayView
    {
        public ConsoleColor ForegroundColor { set; get; }

        public ConsoleColor BackgroundColor { get; set; }

        public DisplayModel DisplayModel { get; set; }

        public int RowOffset { get; set; }

        public int ColumnOffset { get; set; }

        public ConsoleDisplayView(Chip8Emulator emulator) : this(emulator.DisplayModel)
        {
        }

        public ConsoleDisplayView(DisplayModel _displayModel)
        {
            DisplayModel = _displayModel;

            RowOffset = 0;
            ColumnOffset = 0;
        }

        public void ClearAll()
        {
            Console.Clear();
        }

        public void Refresh()
        {
            if (!DisplayModel.RefreshRequested)
                return;

            if (DisplayModel.DisplayMode != DisplayMode.Standard)
                throw new NotImplementedException();

            StandardModeRefresh(DisplayModel);

            DisplayModel.RefreshRequested = false;
        }

        private void StandardModeRefresh(DisplayModel displayModel)
        {
            Console.CursorVisible = false;

            //            Console.ForegroundColor = ForegroundColor;
            //            Console.BackgroundColor = BackgroundColor;

            // Setting the cursor position is relatively slow, output a line at a time to speed things up.
            var sb = new StringBuilder(displayModel.Width);

            for (int row = 0; row < displayModel.Height; row += 1)
            {
                Console.SetCursorPosition(ColumnOffset, RowOffset + row);

                for (int col = 0; col < displayModel.Width; col += 1)
                {
                    if (displayModel.Pixels[row, col] != 0)
                    {
                        sb.Append("█");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                Console.Write(sb);
                Console.Out.Flush();

                sb.Clear();
            }
        }
    }
}
