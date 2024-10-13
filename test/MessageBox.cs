using LinePutScript.Localization.WPF;
using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using VPet_Simulator.Windows.Interface;

namespace VPET.Evian.Check_In
{
    public class MessageBoxCS
    {
        /// <summary>
        /// 生成messagebox对话框资源
        /// </summary>
        public MessageBox messagebox;
        public string Content { get; set; } = "";
        public string Title { get; set; } = "";
        public bool Administrator { get; set; } = false;
        public bool AdEnable { get; set; } = false;
        public string Password { get; } = "";
        public Check_In vts;
        public MessageBoxCS(Check_In vts,string content = "",string title = "", bool administrator = false, bool adEnable = false, string password = "")
        {
            if (vts == null)
            {
                throw new ArgumentNullException(nameof(vts), "Parameter vts cannot be null.");
            }
            messagebox = new MessageBox(this);
            this.vts = vts;
            Content = content;
            Title = title;
            Administrator = administrator;
            AdEnable = adEnable;
            Password = password;
        }


        /// <summary>
        /// 生成/显示对话框
        /// </summary>
        public void ShowBox()
        {
            Content = vts.Content;
            if (messagebox == null)
            {
                messagebox = new MessageBox(this);
            }
            else
            {
                if(!AdEnable)
                {
                    messagebox.Close();
                }
                messagebox = null;
                messagebox = new MessageBox(this);
            }
            if (Administrator == true)
            {
                messagebox.Show();
                vts.Administrator = false;
                return;
            }
            messagebox.Topmost = false;
            messagebox.Show();
        }


    }
}
