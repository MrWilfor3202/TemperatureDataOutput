using System.Collections;
using System;

namespace TemperatureDataOutput.Model
{
    public class TemperatureByteConverter : IByteConverter
    {
        public double GetTemperatureFromBytes(byte[] temperatureBytes)
        {
            if (temperatureBytes.Length < 2)
                throw new ArgumentException();

            bool negativeSign = false;
            BitArray bitsBuffer = new BitArray(temperatureBytes);
            double result = 0;

            if ((bitsBuffer[11]) == true)
            {
                int[] number = new int[1];
                negativeSign = true;
                bitsBuffer.Not();
                bitsBuffer.CopyTo(number, 0);
                number[0] += 1;
                bitsBuffer = new BitArray(number);
            }

            for (int i = 0; i < 11; i++)
                result += Convert.ToByte(bitsBuffer[i]) * Math.Pow(2, i - 4);

            result *= Math.Pow(-1, Convert.ToByte(negativeSign));

            return result;
        }
    }
}
