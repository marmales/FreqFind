using FreqFind.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioReader<T>
    {
        Action<T[]> OnDataReceived { get; set; }
        void Setup(IAudioSettings settings);
        void Start();
        void Stop();
    }
}
