using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

namespace wrike_timer
{
    public class CustomStopwatch : Stopwatch, INotifyPropertyChanged
    {
        private TimeSpan _elapsed;
        private Timer _timer;

        public event PropertyChangedEventHandler PropertyChanged;

        public new TimeSpan Elapsed
        {
            get
            {
                return _elapsed + base.Elapsed;
            }
        }

        public new long ElapsedMilliseconds
        {
            get
            {
                return (_elapsed.Ticks / TimeSpan.TicksPerMillisecond) + base.ElapsedMilliseconds;
            }
        }

        public new long ElapsedTicks
        {
            get
            {
                return _elapsed.Ticks + base.ElapsedTicks;
            }
        }

        public CustomStopwatch()
        {
            this._elapsed = new TimeSpan();
            this._timer = new Timer(1000);
            this._timer.AutoReset = true;
            this._timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Elapsed));
        }

        public CustomStopwatch(TimeSpan elapsed)
        {
            this._elapsed = elapsed;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public new void Reset()
        {
            base.Reset();
            this._elapsed = new TimeSpan();
            this._timer.Stop();
            OnPropertyChanged(nameof(IsRunning));
        }

        public new void Restart()
        {
            base.Restart();
            this._elapsed = new TimeSpan();
            this._timer.Start();
            OnPropertyChanged(nameof(IsRunning));
        }

        public new void Start()
        {
            base.Start();
            this._timer.Start();
            OnPropertyChanged(nameof(IsRunning));
        }

        public new void Stop()
        {
            base.Stop();
            this._timer.Stop();
            OnPropertyChanged(nameof(IsRunning));
        }
    }
}