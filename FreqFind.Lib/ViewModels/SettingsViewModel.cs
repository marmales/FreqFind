using FreqFind.Common;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.ViewModels
{
    public class SettingsViewModel : BaseDialogViewModel
    {

        public SettingsViewModel(AudioSettings settings)
        {
            this.Settings = settings;
        }
        public IList<Device> Devices
        {
            get
            {
                if (devices == null)
                {
                    devices = GetDevices();
                    Settings.SelectedDevice = devices.FirstOrDefault();
                }

                return devices;
            }
        }
        IList<Device> devices;

        public IEnumerable<int> SampleRateList
        {
            get { return sampleRates; }
        }
        int[] sampleRates = new int[] {
            8000,
            11025,
            16000,
            22050,
            32000,
            37800,
            44100,
            47250,
            48000,
            50000,
            50400,
            88200,
            96000,
            176400,
            192000
        };

        public IEnumerable<int> BufferSizeList { get { return bufferSize; } }
        int[] bufferSize = GetBufferSizeCollection();

        private static int[] GetBufferSizeCollection()
        {
            var fftLimit = 14;
            var bufferList = new List<int>();
            for (int i = 10; i < fftLimit; i++)
            {
                bufferList.Add((int)Math.Pow(2, i));
            }
            return bufferList.ToArray();
        }

        public AudioSettings Settings { get; set; }

        public static IList<Device> GetDevices()
        {
            var list = new List<Device>();

            var waveInDevices = WaveIn.DeviceCount;
            for (int deviceIndex = 0; deviceIndex < waveInDevices; deviceIndex++)
                list.Add(DeviceMapper.Map(deviceIndex, WaveIn.GetCapabilities(deviceIndex)));

            return list;
        }

    }
}
