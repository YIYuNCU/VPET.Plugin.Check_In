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
using LinePutScript;

namespace VPET.Evian.Check_In
{
    /// <summary>
    /// winSetting.xaml 的交互逻辑
    /// </summary>
    public partial class winSetting : Window
    {
        Check_In vts;


        public winSetting(Check_In vts)
        {///前两行代码不可缺少
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
            var path = vts.LoaddllPath("Check_In") + @"\Resources" + @"\Image";
            for (var i = 0; i <= vts.ImageNum; i++)
            {
                if (vts.MSave["ERRImage"][(gstr)i.ToString()] != null)
                {
                    if (vts.MSave["ERRImage"][(gbol)i.ToString()] == true)  ///需要改变的图片
                    {
                        if (i < vts.ImageUseNum)
                        {
                            var pathU = path + @"\Unencrypted_State" + @"\" + "Gift" + i.ToString() + @".png";
                            var pathE = path + @"\Encryption_State" + @"\" + "Gift" + i.ToString() + @".png";
                            Check_In.DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4="));
                        }
                        vts.MSave["ERRImage"].Remove(i.ToString());
                    }
                    else if (vts.MSave["ERRImage"][(gbol)i.ToString()] == false)
                    {
                        for (var j = 0; j < Math.Min(vts.ImageUseNum, i + 1); j++) 
                        {
                            var pathU = path + @"\Unencrypted_State" + @"\" + "Gift" + j.ToString() + @".png";
                            var pathE = path + @"\Encryption_State" + @"\" + "Gift" + j.ToString() + @".png";
                            Check_In.DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4="));
                        }
                        vts.MSave["ERRImage"].Remove(i.ToString());
                    }
                }
            }
            var pathUF = path + @"\Unencrypted_State";
            if (Directory.Exists(pathUF))
            {
                Process.Start("explorer.exe", pathUF);
            }
        }
    }
}
