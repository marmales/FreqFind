using FreqFind.Common.Interfaces;
using System;
using System.Numerics;

namespace FreqFind.Lib.Helpers
{
    public static class FFTHelpers
    {

        /* Performs a Bit Reversal Algorithm on a postive integer 
         * for given number of bits
         * e.g. 011 with 3 bits is reversed to 110 */
        public static int BitReverse(int n, int bits)
        {
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0)
            {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return ((reversedN << count) & ((1 << bits) - 1));
        }

        /* Uses Cooley-Tukey iterative in-place algorithm with radix-2 DIT case
         * assumes no of points provided are a power of 2 */
        public static void FFT(Complex[] buffer)
        {

            int bits = (int)Math.Log(buffer.Length, 2);
            for (int j = 1; j < buffer.Length / 2; j++)
            {

                int swapPos = BitReverse(j, bits);
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {

                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / (double)N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;

                    }
                }
            }
        }


        public static void GetFrequencyValues(ref double[] result, Complex[] fftData)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];
            for (int i = 0; i < targetArrayLength; i++)
            {
                //var im2 = Math.Pow(fftData[i].Imaginary, 2);
                //var re2 = Math.Pow(fftData[i].Real, 2);
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);//System.Math.Sqrt(im2 + re2)
            }
        }

        public static void SendStereoSamples(ISampleAggregator<float> aggregator, short[] data, float leftVolume = 0.5f)
        {
            if (leftVolume < 0 || leftVolume > 1)
                throw new InvalidOperationException("Volume value bust be between 0 and 1");

            var rigthVolume = 1 - leftVolume;

            float tmpValue;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            for (int i = 0; i < data.Length; i += 2) //2 channels
            {
                tmpValue = leftVolume * data[i] + rigthVolume * data[i + 1];
                if (tmpValue > maxIntValue)
                    aggregator.AddSample(maxFloatValue);
                else if (tmpValue < minIntValue)
                    aggregator.AddSample(minFloatValue);
                else
                    aggregator.AddSample(tmpValue / divisior); //[-1;1]

            }
        }
        public static void SendMonoSamples(ISampleAggregator<float> aggregator, short[] data)
        {
            float tmpValue;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            for (int i = 0; i < data.Length; i++)
            {
                tmpValue = data[i];
                if (tmpValue > maxIntValue)
                    aggregator.AddSample(maxFloatValue);
                else if (tmpValue < minIntValue)
                    aggregator.AddSample(minFloatValue);
                else
                    aggregator.AddSample(tmpValue / divisior); //[-1;1]

            }
        }
    }
}
