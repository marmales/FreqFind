
using FreqFind.Common.Extensions;
using NAudio.Wave;
using System;

namespace FreqFind.Lib.Helpers
{
    public static class DeviceHelpers
    {
        public static BufferedWaveProvider SetWaveFormat(this BufferedWaveProvider provider, WaveIn newDevice, bool restart = false)
        {
            if (restart || provider == null)
                provider = CreateDefaultWaveProvider(newDevice);
            else
            {
                //cache old settings
                var bufferLength = provider.BufferLength;
                var discardOnOverflow = provider.DiscardOnBufferOverflow;
                //  ... etc

                provider = new BufferedWaveProvider(newDevice.WaveFormat)
                {
                    BufferLength = bufferLength,
                    DiscardOnBufferOverflow = discardOnOverflow
                };
            }

            return provider;
        }

        internal static BufferedWaveProvider CreateDefaultWaveProvider(WaveIn device, bool discardOnOverflow = true)
        {
            return new BufferedWaveProvider(device.WaveFormat)
            {
                BufferLength = SoundCard.BufferSize,
                DiscardOnBufferOverflow = discardOnOverflow
            };
        }
    }
}
