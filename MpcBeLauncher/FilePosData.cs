using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpcBeLauncher
{
    public class FilePosData
    {
        public string FullPath { get; set; }
        public long Position { get; set; }
        public string Name { get; set; }
        public long FileSize { get; set; }
        public int AudioTrack { get; set; }
        public int Subtitle { get; set; }
    }
}