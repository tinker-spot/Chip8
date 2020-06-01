using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Timer
    {
        private byte value;

        public byte Value { get => value; set { SetValue(value); } }

        public bool CountdownCompleted { get; private set; }

        public Timer()
        {
            Reset();
        }

        public void Tick()
        {
            if (CountdownCompleted || Value == 0)
                return;

            Value -= 1;
            if (Value == 0)
                CountdownCompleted = true;
        }

        public void Reset()
        {
            Value = 0;
            CountdownCompleted = false;
        }

        private void SetValue(byte _value)
        {
            value = _value;
            CountdownCompleted = (value != 0) ? false : true;
        }
    }
}
