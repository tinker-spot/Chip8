using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

using Chip8;

namespace Chip8Console
{
    public class RunChip8EmulatorWithTimers : IDisposable
    {

        private System.Timers.Timer emulatorClock;
        private System.Timers.Timer timer60hzClock;

        private const int timer60hzSpeedMs = 16; // 16ms, or about 1/60 of a second
        private const int defaultEmulatorSpeedHertz = 500;

        uint emulatorSpeedHertz;

        public uint EmulatorSpeedHertz { get => emulatorSpeedHertz; set { SetEmulatorSpeedHertz(value); } }

        private bool singleStep = false;
        public bool SingleStep { get => singleStep; set { SetSingleStep(value); } }

        private Chip8Emulator chip8Emulator;
        private ConsoleDisplayView displayView;

        public RunChip8EmulatorWithTimers(Chip8Emulator _emulator, ConsoleDisplayView _displayView)
        {
            chip8Emulator = _emulator;
            displayView = _displayView;

            emulatorClock = new System.Timers.Timer();
            timer60hzClock = new System.Timers.Timer(timer60hzSpeedMs);

            EmulatorSpeedHertz = defaultEmulatorSpeedHertz;

            emulatorClock.Elapsed += EmulatorClock_Elapsed;
            timer60hzClock.Elapsed += Timer60hzClock_Elapsed;
        }

        public void Run()
        {
            emulatorClock.AutoReset = true;
            timer60hzClock.AutoReset = true;

            emulatorClock.Enabled = true;
            timer60hzClock.Enabled = true;
        }

        public void Stop()
        {
            emulatorClock.Enabled = false;
            timer60hzClock.Enabled = false;
        }

        private void EmulatorClock_Elapsed(object sender, ElapsedEventArgs e)
        {
            chip8Emulator.Tick();
//            displayView.Refresh();
        }

        private void Timer60hzClock_Elapsed(object sender, ElapsedEventArgs e)
        {
            chip8Emulator.Tick60hz();
            displayView.Refresh();
        }

        private double ConvertHertzToMilliseconds(double hertz)
        {
            return (1 / hertz) * 1000;
        }

        private void SetEmulatorSpeedHertz(uint hertz)
        {
            emulatorSpeedHertz = hertz;
            var msSpeed = ConvertHertzToMilliseconds(hertz);
            emulatorClock.Interval = msSpeed;
        }

        private void SetSingleStep(bool _singlestep)
        {
            singleStep = _singlestep;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    emulatorClock.Dispose();
                    timer60hzClock.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
