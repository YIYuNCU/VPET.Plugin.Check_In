using LinePutScript.Converter;
using LinePutScript.Localization.WPF;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VPet_Simulator.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.Windows.Shapes;

namespace VPET.Evian.Check_In
{
    /// <summary>
    /// winSetting.xaml 的交互逻辑
    /// </summary>
    public partial class winSetting : Window
    {
        Check_In vts;


        public winSetting(Check_In vts)
        {
            InitializeComponent();
            this.vts = vts;
            SwitchOn.IsChecked = vts.Set.Enable;

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            vts.winSetting = null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (vts.Set.Enable != SwitchOn.IsChecked.Value)
            {
                vts.ChangeOpenState(Convert.ToInt16(SwitchOn.IsChecked.Value));
            }
            vts.MW.Set["Check_In"] = LPSConvert.SerializeObject(vts.Set, "Check_In");
            Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var path = "";
            path = vts.LoaddllPath("Check_In") + @"\Resources" + @"\Image";
            var pathU = path + @"\Unencrypted_State";
            if (Directory.Exists(pathU))
            {
                Process.Start("explorer.exe", pathU);
            }
        }
    }
}
