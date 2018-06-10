using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FreqFind.Lib.Helpers
{
    public static class FFTProcessor
    {
        public static IEnumerable<Complex> ChirpTransform(Complex[] input, LocalRange range, int sampleRate)
        {
            var samplesLength = input.Length;
            var NM1 = samplesLength + range.ZoomOptions.TargetNumberOfSamples - 1;
            var A = Complex.Exp(new Complex(0, -2 * Math.PI * range.LeftThreshold / sampleRate));
            var W = Complex.Exp(new Complex(0, -2 * Math.PI * ((range.RightThreshold - range.LeftThreshold) / (2 * (range.ZoomOptions.TargetNumberOfSamples - 1)) / sampleRate)));

            var y1 = new Complex[NM1];
            var y2 = new Complex[NM1];

            for (int k = 0; k < NM1 - 1; k++)
            {
                if (k < samplesLength)
                    y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * input[k];
                else
                    y1[k] = 0;

                if (k < range.ZoomOptions.TargetNumberOfSamples)
                    y2[k] = Complex.Pow(W, -Math.Pow(k, 2));
                else
                    y2[k] = Complex.Pow(W, (-Math.Pow((NM1 - k), 2)));
            }

            Convolve(y1, y2);

            //that can be improved by move scaling from Convolve to this and process only target number of samples
            for (int k = 0; k < range.ZoomOptions.TargetNumberOfSamples; k++)
                y1[k] *= Complex.Pow(W, Math.Pow(k, 2));

            return y1.Take(range.ZoomOptions.TargetNumberOfSamples);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xvector">This parameter represent one of the input functions and processed value at the end</param>
        /// <param name="yvector">This parameter represent one of the input functions</param>
        static void Convolve(Complex[] xvector, Complex[] yvector)
        {
            int n = xvector.Length;
            if (n != yvector.Length)
                throw new ArgumentException("Mismatched lengths");
            Transform(xvector, false);
            Transform(yvector, false);
            for (int i = 0; i < n; i++)
                xvector[i] *= yvector[i];
            Transform(xvector, true);

            for (int i = 0; i < n; i++)  // Scaling (because this FFT implementation omits it)
                xvector[i] /= n;
        }

        public static void Transform(Complex[] vector, bool inverse)
        {
            int n = vector.Length;
            if (n == 0)
                return;
            else if ((n & (n - 1)) == 0)  // Is power of 2
                TransformRadix2(vector, inverse);
            else
                TransformBluestein(vector, inverse);
        }

        static void TransformRadix2(Complex[] vector, bool inverse)
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
                int j = (int)((uint)FFTHelpers.ReverseBits(i) >> (32 - levels));
                if (j > i)
                {
                    Complex temp = vector[i];
                    vector[i] = vector[j];
                    vector[j] = temp;
                }

            }
            #region WIKI
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
            #endregion
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

        static void TransformBluestein(Complex[] vector, bool inverse)
        {
            // Find a power-of-2 convolution length m such that m >= n * 2 + 1
            int n = vector.Length;
            if (n >= 0x20000000)
                throw new ArgumentException("Array too large");
            int m = 1;
            while (m < n * 2 + 1)
                m *= 2;

            // Trignometric table
            Complex[] expTable = new Complex[n];
            double coef = Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n; i++)
            {
                int j = (int)((long)i * i % (n * 2));  // This is more accurate than j = i * i
                expTable[i] = Complex.Exp(new Complex(0, j * coef));
            }

            // Temporary vectors and preprocessing
            Complex[] avector = new Complex[m];
            for (int i = 0; i < n; i++)
                avector[i] = vector[i] * expTable[i];
            Complex[] bvector = new Complex[m];
            bvector[0] = expTable[0];
            for (int i = 1; i < n; i++)
                bvector[i] = bvector[m - i] = Complex.Conjugate(expTable[i]);

            // Convolution
            Convolve(avector, bvector); //avecotr will be processed vector

            // Postprocessing
            for (int i = 0; i < n; i++)
                vector[i] = avector[i] * expTable[i];
        }
    }
}
