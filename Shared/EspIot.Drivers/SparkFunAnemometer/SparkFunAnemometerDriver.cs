﻿using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Drivers.SparkFunAnemometer.Enums;

namespace EspIot.Drivers.SparkFunAnemometer
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunAnemometerDriver
    {
        private const uint ONE_HZ_PULSE_SPEED = 67; // 2,4 km/h -> 0,67 [m/s] -> 67 [m/s * 100];
        private readonly GpioController _gpioController;

        private readonly GpioPin _pin;
        private readonly GpioChangeCounter _pulseCounter;

        //[m/s * 100]
        private uint _averageWindSpeed;
        private uint _currentWindSpeed;

        private bool _isMeasuring;
        private uint _maxWindSpeed;
        private uint _measurementCounter;

        private Thread _measuringThread = new Thread(() => { });
        private uint _minWindSpeed;

        public SparkFunAnemometerDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Rising};
        }

        public void StartMeasurement(AnemometerMeasurementResolution measurementResolution)
        {
            if (_measuringThread.IsAlive)
            {
                //don't start new measuring when previous is pending
                throw new InvalidOperationException(
                    "Cannot start measurement because another measurement process is in progress.");
            }

            _isMeasuring = true;

            _measuringThread = new Thread(() =>
            {
                _pulseCounter.Start();

                while (_isMeasuring)
                {
                    Thread.Sleep((int) measurementResolution * 1000);
                    var pulseCount = _pulseCounter.Read();
                    _pulseCounter.Reset();

                    _currentWindSpeed =
                        (ushort) (pulseCount.Count / (double) measurementResolution * ONE_HZ_PULSE_SPEED);

                    _averageWindSpeed = (_averageWindSpeed * _measurementCounter + _currentWindSpeed) /
                                        ++_measurementCounter;

                    if (_currentWindSpeed > _maxWindSpeed)
                    {
                        _maxWindSpeed = _currentWindSpeed;
                    }

                    else if (_currentWindSpeed < _minWindSpeed)
                    {
                        _minWindSpeed = _currentWindSpeed;
                    }
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
            });

            _measuringThread.Start();
        }

        public uint GetCurrentWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _currentWindSpeed;
        }

        public uint GetAverageWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _averageWindSpeed;
        }

        public uint GetMaxWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _maxWindSpeed;
        }

        public uint GetMinWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _minWindSpeed;
        }

        public void Reset()
        {
            _measurementCounter = 0;
            _maxWindSpeed = 0;
            _averageWindSpeed = 0;
            _minWindSpeed = 0;
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
            Reset();
        }
    }
}