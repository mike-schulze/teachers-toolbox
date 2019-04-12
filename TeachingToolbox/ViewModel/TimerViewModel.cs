using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Windows.Threading;

namespace TeachingToolbox.ViewModel
{
    public class TimerViewModel : ViewModelBase
    {
        public TimerViewModel()
        {
            mTimer = new DispatcherTimer ();
            mTimer.Interval = TimeSpan.FromMilliseconds( TimerDelay );
            mTimer.Tick += HandleTimerTick;

            mTimerDuration = TimeSpan.FromTicks( 0 );

            StartCommand = new RelayCommand ( StartTimer, () => !IsRunning );
            StopCommand = new RelayCommand( StopTimer, () => IsRunning );
            ResetCommand = new RelayCommand ( ResetTimer );
        }

        private void HandleTimerTick( object sender, EventArgs e )
        {
            mTimerDuration = mTimerDuration.Add( TimeSpan.FromMilliseconds( TimerDelay ) );
            RaisePropertyChanged( TimerStringPropertyName );
        }

        private void StartTimer()
        {
            mTimer.Start();
            IsRunning = true;
        }

        private void StopTimer()
        {
            mTimer.Stop();
            IsRunning = false;
        }

        private void ResetTimer()
        {
            mTimerDuration = TimeSpan.FromTicks( 0 );
            RaisePropertyChanged( TimerStringPropertyName );
        }

        public string TimerString
        {
            get
            {
                return String.Format( "{0:00}:{1:00}:{2:00}", mTimerDuration.Hours, mTimerDuration.Minutes, mTimerDuration.Seconds );
            }
        }
        public const string TimerStringPropertyName = "TimerString";

        public bool IsRunning
        {
            get
            {
                return mIsRunning;
            }

            set
            {
                if( Set( IsRunningPropertyName, ref mIsRunning, value ) )
                {
                    StartCommand.RaiseCanExecuteChanged();
                    StopCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool mIsRunning = false;
        public const string IsRunningPropertyName = "IsRunning";

        public RelayCommand StartCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }

        private TimeSpan mTimerDuration;
        private readonly DispatcherTimer mTimer;

        private const int TimerDelay = 250;
    }
}
