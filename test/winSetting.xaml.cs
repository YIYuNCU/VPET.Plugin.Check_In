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
using System.Windows.Controls;

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
;        }

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
            var path = System.IO.Path.Combine(vts.LoaddllPath("Check_In") , "Resources" , "Image");
            for (var i = 0; i <= vts.ImageNum; i++)
            {
                if (!string.IsNullOrEmpty(vts.PSave["ERRImage"][(gstr)i.ToString()]))
                {
                    if (vts.PSave["ERRImage"][(gbol)i.ToString()] == true)  ///需要改变的图片
                    {
                        if (i < vts.ImageUseNum)
                        {
                            var pathU = System.IO.Path.Combine(path, "Unencrypted_State", "Gift" + i.ToString() + @".png");
                            var pathE = System.IO.Path.Combine(path, "Encryption_State", "Gift" + i.ToString() + @".png");
                            Check_In.DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4="));
                        }
                        vts.PSave["ERRImage"].Remove(i.ToString());
                    }
                    else if (vts.PSave["ERRImage"][(gbol)i.ToString()] == false)
                    {
                        for (var j = 0; j < Math.Min(vts.ImageUseNum, i + 1); j++) 
                        {
                            var pathU = System.IO.Path.Combine(path, "Unencrypted_State", "Gift" + j.ToString() + @".png");
                            var pathE = System.IO.Path.Combine(path , "Encryption_State" ,  "Gift" + j.ToString() + @".png");
                            Check_In.DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4="));
                        }
                        vts.PSave["ERRImage"].Remove(i.ToString());
                    }
                }
            }
            var pathUF = path + @"\Unencrypted_State";
            if (Directory.Exists(pathUF))
            {
                Process.Start("explorer.exe", pathUF);
            }
        }

        private void ReSave_Click(object sender, RoutedEventArgs e)
        {
            vts.Resave();
            MessageBoxX.Show("已读取存档备份".Translate(), "提示".Translate(),
                    MessageBoxButton.OK, MessageBoxIcon.Info, DefaultButton.YesOK, 5);
        }
    }
}
