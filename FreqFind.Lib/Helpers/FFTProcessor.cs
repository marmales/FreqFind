using FreqFind.Lib.Models;
using System;
using System.Linq;
using System.Numerics;

namespace FreqFind.Lib.Helpers
{
    public static class FFTProcessor
    {
        public static void ChirpTransform(Complex[] input, MagnifierModel zoom, int sampleRate)
        {
            var samplesLength = input.Length;
            var NM1 = samplesLength + zoom.TargetNumberOfSamples - 1;
            var A = Complex.Exp(new Complex(0, -2 * Math.PI * zoom.LeftThreshold / zoom.RightThreshold));
            var W = Complex.Exp(new Complex(0, -2 * Math.PI * ((zoom.RightThreshold - zoom.LeftThreshold) / (2 * (zoom.TargetNumberOfSamples - 1)) / sampleRate)));

            var y1 = new Complex[samplesLength];
            var y2 = new Complex[samplesLength];


            for (int k = 0; k < NM1 - 1; k++)
            {
                if (k < samplesLength)
                    y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * input[k];
                else
                    y1[k] = 0;

                if (k < zoom.TargetNumberOfSamples)
                    y2[k] = Complex.Pow(W, -k / 2);
                else
                    y2[k] = Complex.Pow(W, (-Math.Pow((NM1 - k), 2)));

            }

            Transform(y1, false);
            Transform(y2, false);

            var outvector = new Complex[y1.Length];
            Convolve(y1, y2, outvector);

            for (int k = 0; k < zoom.TargetNumberOfSamples; k++)
                outvector[k] *= Complex.Pow(W, Math.Pow(k, 2));

            //outvector = outvector.Take(zoom.NumberOfSamples).ToArray();
        }
        static void Convolve(Complex[] xvector, Complex[] yvector, Complex[] outvector)
        {
            int n = xvector.Length;
            if (n != yvector.Length || n != outvector.Length)
                throw new ArgumentException("Mismatched lengths");
            xvector = (Complex[])xvector.Clone();
            yvector = (Complex[])yvector.Clone();
            Transform(xvector, false);
            Transform(yvector, false);
            for (int i = 0; i < n; i++)
                xvector[i] *= yvector[i];
            Transform(xvector, true);

            for (int i = 0; i < n; i++)  // Scaling (because this FFT implementation omits it)
                outvector[i] = xvector[i] / n;
        }

        public static void Transform(Complex[] vector, bool inverse)
        {
            int n = vector.Length;
            if (n == 0)
                return;
            else if ((n & (n - 1)) == 0)  // Is power of 2
                TransformRadix2(vector, inverse);
            else  // More complicated algorithm for arbitrary sizes
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

            //TEST THIS SHIT
            //for (int i = 0; i < n; i++)  // Scaling (because this FFT implementation omits it)
            //    vector[i] /= n;
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
            Complex[] cvector = new Complex[m];
            Convolve(avector, bvector, cvector);

            // Postprocessing
            for (int i = 0; i < n; i++)
                vector[i] = cvector[i] * expTable[i];
        }
    }
}
