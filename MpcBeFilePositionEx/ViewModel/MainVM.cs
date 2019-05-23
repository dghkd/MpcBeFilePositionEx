using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Diagnostics;

using MpcBeFilePositionEx.Common;
using MpcBeFilePositionEx.RegOperation;

namespace MpcBeFilePositionEx.ViewModel
{
    public class MainVM : ViewModelBase
    {
        #region Private Member

        private ObservableCollection<string> _extColle;
        private object _lockExtColleObj;

        #endregion Private Member

        #region Constructor

        public MainVM()
        {
            _lockExtColleObj = new object();
            _extColle = new ObservableCollection<string>();
            this.ExtCollection = CollectionViewSource.GetDefaultView(_extColle);
            BindingOperations.EnableCollectionSynchronization(_extColle, _lockExtColleObj);
        }

        #endregion Constructor

        #region Public Member

        public ICollectionView ExtCollection { get; private set; }

        #endregion Public Member

        #region Command

        public enum CommandKey
        {
            Attach,
            Detach,
        }

        private CommandBase _cmdAttach;

        public CommandBase CmdAttach
        {
            get
            {
                return _cmdAttach ?? (_cmdAttach = new CommandBase(x => ExecuteCommand(CommandKey.Attach)));
            }
        }

        private CommandBase _cmdDetach;

        public CommandBase CmdDetach
        {
            get
            {
                return _cmdDetach ?? (_cmdDetach = new CommandBase(x => ExecuteCommand(CommandKey.Detach)));
            }
        }

        public delegate void CommandActionMethod(MainVM sender, CommandKey cmdKey);

        public event CommandActionMethod CommandAction;

        private void ExecuteCommand(CommandKey cmdKey)
        {
            if (this.CommandAction != null)
            {
                this.CommandAction(this, cmdKey);
            }
        }

        #endregion Command

        #region Public Method

        public bool AttachExt(string extName)
        {
            if (RegMethod.AttachMpcBeExt(extName))
            {
                //Save ext.
                _extColle.Add(extName);
                MainVM.SaveToFile(this);
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool AttachExt(List<string> extList)
        {
            foreach (string extName in extList)
            {
                if (RegMethod.AttachMpcBeExt(extName))
                {
                    //Save ext.
                    _extColle.Add(extName);
                }
            }

            MainVM.SaveToFile(this);

            return true;
        }

        public bool DetachExt(string extName)
        {
            if (RegMethod.DetachMpcBeExt(extName))
            {
                _extColle.Remove(extName);
                MainVM.SaveToFile(this);
                return true;
            }
            return false;
        }

        public bool ReattachExt(string extName)
        {
            if (RegMethod.DetachMpcBeExt(extName))
            {
                if (RegMethod.AttachMpcBeExt(extName))
                {
                    return true;
                }
            }

            return false;
        }

        public static MainVM LoadSaveFile()
        {
            MainVM ret = new MainVM();
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + AppEnum.SAVE_EXT_FILE_NAME;
            if (!File.Exists(filePath))
            {
                return ret;
            }
            foreach (string ext in File.ReadAllLines(filePath))
            {
                ret._extColle.Add(ext);
            }
            return ret;
        }

        public static void SaveToFile(MainVM vm)
        {
            string content = "";
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + AppEnum.SAVE_EXT_FILE_NAME;
            foreach (string ext in vm._extColle)
            {
                content += ext + Environment.NewLine;
            }
            File.WriteAllText(filePath, content);
        }

        #endregion Public Method
    }
}