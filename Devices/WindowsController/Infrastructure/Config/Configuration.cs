﻿using EspIot.Core.Configuration;
using EspIot.Core.Gpio;

namespace WindowsController.Infrastructure.Config
{
    public class Configuration: IConfiguration 
    {
        public string MqttBrokerAddress { get; } = "192.168.2.108";
        //public string DeviceId { get; } = "esp32-motor-driver";
        //public string InboundMessagesTopic { get; } = "devices/esp32-motor-driver/messages/devicebound/#";
        public string DeviceId { get; } = "esp32-greenhouse";
        public string InboundMessagesTopic { get; } = "devices/esp32-greenhouse/messages/devicebound/#";
        
        //Status led config
        public GpioPins WifiStatusLed { get; } = GpioPins.GPIO_NUM_12;
        public GpioPins MqttStatusLed { get; } = GpioPins.GPIO_NUM_13;

        //Buttons 

        //First channel config
        public GpioPins FirstChannelEnable { get; } = GpioPins.GPIO_NUM_4;
        public GpioPins FirstChannelInput1 { get; } = GpioPins.GPIO_NUM_18;
        public GpioPins FirstChannelInput2 { get; } = GpioPins.GPIO_NUM_5;
        public GpioPins FirstChannelReedSwitch { get; } = GpioPins.GPIO_NUM_21;
        public GpioPins FirstChannelControlSwitch { get; } = GpioPins.GPIO_NUM_32;


        //First channel config
        public GpioPins SecondChannelEnable { get; } = GpioPins.GPIO_NUM_16;
        public GpioPins SecondChannelInput1 { get; } = GpioPins.GPIO_NUM_22;
        public GpioPins SecondChannelInput2 { get; } = GpioPins.GPIO_NUM_23;
        public GpioPins SecondChannelReedSwitch { get; } = GpioPins.GPIO_NUM_27;
        public GpioPins SecondChannelControlSwitch { get; } = GpioPins.GPIO_NUM_33;
    }
}