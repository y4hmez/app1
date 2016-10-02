using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;
using App1.Annotations;

namespace App1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private const Double TOLERANCE = 0.01;
        private readonly Compass _compass;
        private readonly Barometer _barometer;

        private static readonly Lazy<ViewModel> lazy = new Lazy<ViewModel>(() => new ViewModel(Compass.GetDefault(), Barometer.GetDefault()));

        //public static ViewModel Instance { get; } = new ViewModel();
        //if we have to we can fake some injection at this point..
        public static ViewModel Instance { get { return lazy.Value; } }


        private ViewModel(Compass compass, Barometer barometer)
        {
            _compass = compass;
            _barometer = barometer;

            _compass.ReadingChanged += _compass_ReadingChanged;
            _barometer.ReadingChanged += _barometer_ReadingChanged;

        }

        public ViewModel() { } //public default for XAML

        private int angleToNorth;

        public int AngleToNorth
        {
            get { return angleToNorth; }
            set { angleToNorth = value; }
        }

        private double heading;
        public double Heading
        {
            get { return heading; }
            set
            {
                if (Math.Abs(heading - value) > TOLERANCE)
                {
                    heading = value;
                    OnPropertyChanged();
                }
            }
        }

        private double pressure;
        public double Pressure
        {
            get { return pressure; }
            set
            {
                if (Math.Abs(pressure - value) > TOLERANCE)
                {
                    pressure = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
            () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private void _barometer_ReadingChanged(Barometer sender, BarometerReadingChangedEventArgs args)
        {
            Pressure = args.Reading.StationPressureInHectopascals;
        }

        private void _compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            if (args.Reading.HeadingTrueNorth.HasValue)
            {
                //Heading = args.Reading.HeadingTrueNorth.Value;

                Heading = (360 - args.Reading.HeadingTrueNorth.Value + 180) % 360;


                //deg = (deg + 360) % 360;
            }
        }
    }
}