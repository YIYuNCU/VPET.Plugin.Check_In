using LinePutScript.Converter;
using LinePutScript.Localization.WPF;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VPet_Simulator.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Panuon.WPF.UI;

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

    }
}
