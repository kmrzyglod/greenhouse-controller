﻿using EspIot.Core.Gpio;
using System.Threading;
using Windows.Devices.Gpio;

namespace EspIot.Drivers.SeedstudioWaterFlowSensor
{
    public class WaterFlowSensor
    {
        private readonly GpioPin _pin;
        private readonly GpioController _gpioController;
        private readonly int _measureTime;
        private int _impulseCounter;
        private int _flowValue;
        private int _momentaryFlowValue;
        private bool _measurementInProgress;

        public WaterFlowSensor(GpioController gpioController, GpioPins pin, int measurementTime)
        {
            _gpioController = gpioController;
            _measureTime = measurementTime;
            _pin = _gpioController.OpenPin((int)pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pin.ValueChanged += (sender, e) =>
            {
                _impulseCounter++;
            };

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(measurementTime);
                    _momentaryFlowValue = (int)(_impulseCounter / (measurementTime / 1000f) / 5f);
                    _impulseCounter = 0;
                    if (_measurementInProgress)
                    {
                        _flowValue += (int)(_momentaryFlowValue * (measurementTime / 60000f));
                    }
                }

            }).Start();
        }

        //Get momentary flow value in liters/minute
        public int GetMomentaryFlowValue()
        {
            return _momentaryFlowValue;
        }

        public void StartMeasurement()
        {
            _measurementInProgress = true;
        }

        //Finish measurement and get result in liters
        public int FinishMeasurement()
        {
            _measurementInProgress = false;
            var flowValue = _flowValue;
            _flowValue = 0;
            return flowValue;
        }


    }
}
