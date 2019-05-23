using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MpcBeFilePositionEx.RegOperation;
using MpcBeFilePositionEx.ViewModel;

namespace MpcBeFilePositionEx
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = MainVM.LoadSaveFile();
            _vm.CommandAction += On_MainVM_CommandAction;
            GD_LayoutRoot.DataContext = _vm;
        }

        private void On_MainVM_CommandAction(MainVM sender, MainVM.CommandKey cmdKey)
        {
            switch (cmdKey)
            {
                case MainVM.CommandKey.Attach:
                    sender.AttachExt(TXTBOX_Ext.Text);
                    break;

                case MainVM.CommandKey.Detach:
                    if (LV_ExtList.SelectedItem != null)
                    {
                        string selExtName = LV_ExtList.SelectedItem as string;
                        sender.DetachExt(selExtName);
                    }

                    break;

                default:
                    break;
            }
        }
    }
}