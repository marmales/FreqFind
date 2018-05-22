using FreqFind.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FreqFind.Lib.Helpers
{
    public static class FFTProcessor
    {
        public static void ChirpTransform(Complex[] input, int startFrequency, int endFrequency, int prazkiCount, int sampleRate)
        {
            var samplesLength = input.Length;
            var NM1 = samplesLength + prazkiCount - 1;
            var A = Complex.Exp(new Complex(0, -2 * Math.PI * startFrequency / sampleRate));
            var W = Complex.Exp(new Complex(0, -2 * Math.PI * ((endFrequency - startFrequency) / (2 * (prazkiCount - 1)) / sampleRate)));

            var y1 = new Complex[samplesLength];
            var y2 = new Complex[samplesLength];


            for (int k = 0; k < NM1 - 1; k++)
            {
                if (k < samplesLength)
                    y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * input[k];
                else
                    y1[k] = 0;

                if (k < prazkiCount)
                    y2[k] = Complex.Pow(W, -k / 2);
                else
                    y2[k] = Complex.Pow(W, (-Math.Pow((NM1 - k), 2)));

            }
        }
        public static void TransformRadix2(Complex[] vector, bool inverse)
        {
            // Length variables
            int n = vector.Length;
            int levels = 0;  // compute levels = floor(log2(n))
            for (int temp = n; temp > 1; temp >>= 1)
                levels++;
            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric table
            Complex[] expTable = new Complex[n / 2];
            double coef = 2 * Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n / 2; i++)
                expTable[i] = Complex.Exp(new Complex(0, i * coef));

            // Bit-reversed addressing permutation
            for (int i = 0; i < n; i++)
            {
                //Debug.Write(string.Format($"Before: {i}"));
                int j = (int)((uint)ReverseBits(i) >> (32 - levels));
                if (j > i)
                {
                    Complex temp = vector[i];
                    vector[i] = vector[j];
                    vector[j] = temp;
                }
                //Debug.WriteLine($"\tAfter: {j}");

            }
            //IMPLEMENTED PSEUDOCODE FROM https://en.wikipedia.org/wiki/Cooley–Tukey_FFT_algorithm
            //for (int s = 1; s < Math.Log(n, 2); s++)
            //{
            //    var m = (int)Math.Pow(2, s);
            //    var Wm = Complex.Exp(new Complex(0, (-2 * Math.PI) / m));

            //    for (int k = 0; k < n - 1; k += m)
            //    {
            //        var W = new Complex(1, 0);
            //        for (int j = 0; j < m / 2 - 1; j++)
            //        {
            //            var t = W * vector[k + j + m / 2];
            //            var u = vector[k + j];
            //            vector[k + j] = u + t;
            //            vector[k + j + m / 2] = u - t;
            //            W = W * Wm;
            //        }
            //    }
            //}

            //Cooley - Tukey decimation -in-time radix - 2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;
                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        Complex temp = vector[j + halfsize] * expTable[k];
                        vector[j + halfsize] = vector[j] - temp;
                        vector[j] += temp;
                    }
                }
                if (size == n)  // Prevent overflow in 'size *= 2'
                    break;
            }
        }
        private static int ReverseBits(int val)
        {
            int result = 0;
            for (int i = 0; i < 32; i++, val >>= 1)
                result = (result << 1) | (val & 1);
            return result;
        }
    }
    public static class FFTHelpers
    {
        public static void GetFrequencyValues(ref double[] result, Complex[] fftData)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];
            for (int i = 0; i < targetArrayLength; i++)
            {
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);//System.Math.Sqrt(im2 + re2)
            }
        }

        public static void SendSamples(ISampleAggregator<float> aggregator, short[] data, IEnumerable<int> channelsVolume)
        {
            var volumeList = channelsVolume.ToList();
            if (volumeList.Count == 0)
                throw new ArgumentException("Channels not found!");

            float tmpValue = 0;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            for (int i = 0; i < data.Length; i += volumeList.Count)
            {
                int channelsIndex = 0;
                tmpValue = data.Skip(i).Take(volumeList.Count).Sum(x => (volumeList[channelsIndex++] / 100f) * x);
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
