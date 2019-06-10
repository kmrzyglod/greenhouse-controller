﻿using Windows.Devices.I2c;

namespace greenhouse_controller.Core.I2c
{
    public static class I2cExtensions
    {
        //Method to read bytes from registers
        public static int ReadBytes(this I2cDevice device, byte register, short numOfBytes = 1, short retries = 2)
        {
            byte[] writeBuffer = new byte[] { 0x00 };
            byte[] readBuffer = new byte[numOfBytes];

            writeBuffer[0] = register;
            var status = I2cTransferStatus.UnknownError;
            for (short i = 0; i < retries; i++)
            {
                status = device.WriteReadPartial(writeBuffer, readBuffer).Status;
            }

            if (status == I2cTransferStatus.SlaveAddressNotAcknowledged || status == I2cTransferStatus.UnknownError)
            {
                throw new I2cTransferException("Error during read byte", status);
            }

            int value = readBuffer[0];
            for (short j = 1; j < numOfBytes; j++)
            {
                value += readBuffer[j] << (8 * j);
            }
            return value;
        }
    }
}