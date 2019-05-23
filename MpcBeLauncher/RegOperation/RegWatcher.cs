using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RegistryUtils;

namespace MpcBeLauncher.RegOperation
{
    public class RegWatcher
    {
        private RegistryMonitor _monitor;
        private List<FilePosData> _lastFileList;

        public RegWatcher()
        {
            _lastFileList = new List<FilePosData>();
        }

        public void Start()
        {
            if (_monitor != null)
            {
                return;
            }
            _monitor = new RegistryMonitor(Microsoft.Win32.RegistryHive.CurrentUser, RegString.REG_KEY_MPC_BE_RECENT_FILES);
            _monitor.RegChanged += On_MpcBe_RegChanged;
            _monitor.Start();
            _lastFileList.Clear();
            _lastFileList.AddRange(RegMethod.GetRecentFilePostDataList());
        }

        public void Stop()
        {
            _monitor.Dispose();
            _monitor = null;
        }

        public delegate void DelegateRegChangedMethod(RegWatcher sender, List<FilePosData> changedData);

        public event DelegateRegChangedMethod RegRecentFileChanged;

        private void On_MpcBe_RegChanged(object sender, EventArgs e)
        {
            if (this.RegRecentFileChanged != null)
            {
                List<FilePosData> curRecentFileList = RegMethod.GetRecentFilePostDataList();
                curRecentFileList.RemoveAll(x => _lastFileList.Exists(y =>
                y.FullPath == x.FullPath
                && y.Position == x.Position
                && y.AudioTrack == x.AudioTrack
                && y.Subtitle == x.Subtitle));

                this.RegRecentFileChanged(this, curRecentFileList);
            }
        }
    }
}