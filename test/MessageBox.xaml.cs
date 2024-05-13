using LinePutScript.Converter;
using LinePutScript.Localization.WPF;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VPet_Simulator.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Panuon.WPF.UI;
using LinePutScript;

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
            this.vts = vts;
            Content.Text = vts.Content.ToString();
            if (vts.Administrator == true && vts.AdEnable == false)
            {
                Enable.Visibility = Visibility.Visible;
                Content.Visibility = Visibility.Collapsed;
                vts.Administrator = false;
            }
            else if(vts.Administrator == false)
            {
                Enable.Visibility = Visibility.Collapsed;
                Content.Visibility = Visibility.Visible;
            }
            if (vts.AdEnable == true)
            {
                ImageUseNum.Visibility = Visibility.Visible;
                ImageUseNumText.Visibility = Visibility.Visible;
                IsFinished.Visibility = Visibility.Visible;
                IsFinishedText.Visibility = Visibility.Visible;
                CheckType.Visibility = Visibility.Visible;
                CheckTypeText.Visibility = Visibility.Visible;
                IfShow.Visibility = Visibility.Visible;
                IfShowText.Visibility = Visibility.Visible;
                AdSave.Visibility = Visibility.Visible;
                AdClear.Visibility = Visibility.Visible;
                AdExit.Visibility = Visibility.Visible;
                Content.Visibility = Visibility.Collapsed;
                Check_In_Num.Visibility = Visibility.Visible;
                Check_In_NumText.Visibility = Visibility.Visible;
            }
            else
            {
                ImageUseNum.Visibility = Visibility.Collapsed;
                ImageUseNumText.Visibility = Visibility.Collapsed;
                IsFinished.Visibility = Visibility.Collapsed;
                IsFinishedText.Visibility = Visibility.Collapsed;
                CheckType.Visibility = Visibility.Collapsed;
                CheckTypeText.Visibility = Visibility.Collapsed;
                IfShow.Visibility = Visibility.Collapsed;
                IfShowText.Visibility = Visibility.Collapsed;
                AdSave.Visibility = Visibility.Collapsed;
                AdClear.Visibility = Visibility.Collapsed;
                AdExit.Visibility = Visibility.Collapsed;
                Check_In_NumText.Visibility = Visibility.Collapsed;
                Check_In_Num.Visibility= Visibility.Collapsed;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void Enable_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(Enable.Text.ToString() == "Check_In_Evian".ToString())
            {
                vts.AdEnable = true;
                ImageUseNum.Visibility = Visibility.Visible;
                ImageUseNumText.Visibility = Visibility.Visible;
                IsFinished.Visibility = Visibility.Visible;
                IsFinishedText.Visibility = Visibility.Visible;
                CheckType.Visibility = Visibility.Visible;
                CheckTypeText.Visibility = Visibility.Visible;
                IfShow.Visibility = Visibility.Visible;
                IfShowText.Visibility = Visibility.Visible;
                Enable.Visibility = Visibility.Collapsed;
                AdSave.Visibility = Visibility.Visible;
                AdClear.Visibility = Visibility.Visible;
                AdExit.Visibility= Visibility.Visible;
                Check_In_Num.Visibility = Visibility.Visible;
                Check_In_NumText.Visibility = Visibility.Visible;
            }
        }

        private void AdSave_Click(object sender, RoutedEventArgs e)
        {
            if(ImageUseNum.Text != "")
            {
                vts.AdChange(Convert.ToInt32(ImageUseNum.Text.ToString()));
            }
            if (IsFinished.Text != "")
            {
                if(IsFinished.Text.ToString() == 0.ToString())
                    vts.AdChange(-1, 0);
                else if (IsFinished.Text.ToString() == 1.ToString())
                    vts.AdChange(-1, 1);
            }
            if (CheckType.Text != "")
            {
                vts.AdChange(-1,-1, Convert.ToInt32(CheckType.Text.ToString()));
            }
            if (IfShow.Text != "")
            {
                if (IfShow.Text.ToString() == 0.ToString())
                    vts.AdChange(-1, -1,-1,0);
                else if (IfShow.Text.ToString() == 1.ToString())
                    vts.AdChange(-1, -1,-1,1);
                
            }
        }

        private void AdClear_Click(object sender, RoutedEventArgs e)
        {
            vts.OpenTime =DateTime.Now.AddDays(-1).Date;
            System.Windows.MessageBox.Show("移除数据成功");
        }

        private void AdExit_Click(object sender, RoutedEventArgs e)
        {
            vts.Administrator = false;
            vts.AdEnable = false;
            System.Windows.MessageBox.Show("退出成功");
            Close();
        }
    }
}
