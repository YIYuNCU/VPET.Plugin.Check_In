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
    public partial class MessageBox : Window
    {
        Check_In vts;


        public MessageBox(Check_In vts)
        {
            InitializeComponent();
            Content.Text = vts.Content.Translate().ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

    }
}
