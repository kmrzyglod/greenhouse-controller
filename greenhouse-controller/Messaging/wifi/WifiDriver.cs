﻿using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace greenhouse_controller.Messaging.wifi
{
    public static class WifiDriver
    {
        private static string _ssid;
        private static string _password;
        private static Wireless80211Configuration.AuthenticationType _authenticationType;
        private static Wireless80211Configuration.EncryptionType _encryptionType;

        public static void ConnectToNetwork(string ssid, string password, Wireless80211Configuration.AuthenticationType authenticationType, Wireless80211Configuration.EncryptionType encryptionType)
        {
            _ssid = ssid;
            _password = password;
            _authenticationType = authenticationType;
            _encryptionType = encryptionType;

            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

            if (nis.Length > 0)
            {
                // get the first interface
                var ni = nis[0];

                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // network interface is Wi-Fi
                    Console.WriteLine("Network connection is: Wi-Fi");
                    var wc = Wireless80211Configuration.GetAllWireless80211Configurations()[ni.SpecificConfigId];

                    if (wc.Ssid != _ssid && wc.Password != _password)
                    {
                        // have to update Wi-Fi configuration
                        wc.Authentication = _authenticationType;
                        wc.Encryption = _encryptionType;
                        wc.Ssid = _ssid;
                        wc.Password = _password;
                        wc.SaveConfiguration();
                    }
                }
                else
                {
                    throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
                }

                // wait for DHCP to complete
                WaitIP();
            }
            else
            {
                throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
            }
        }

        private static void WaitIP()
        {
            Console.WriteLine("Waiting for IP...");

            while (true)
            {
                var ni = NetworkInterface.GetAllNetworkInterfaces()[0];
                if (ni.IPv4Address != null && ni.IPv4Address.Length > 0)
                {
                    if (ni.IPv4Address[0] != '0')
                    {
                        Console.WriteLine($"We have an IP: {ni.IPv4Address}");
                        break;
                    }
                }

                Thread.Sleep(300);
            }

             SetDateTime();
        }

        private static void SetDateTime()
        {
            Console.WriteLine("Setting up system clock...");

            // if SNTP is available and enabled on target device this can be skipped because we should have a valid date & time
            while (DateTime.UtcNow.Year < 2018)
            {
                Console.WriteLine("Waiting for valid date time...");
                // wait for valid date & time
                Thread.Sleep(1000);
            }

            Console.WriteLine($"System time is: {DateTime.UtcNow.ToString()}");
        }
    }
}