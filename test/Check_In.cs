using LinePutScript;
using LinePutScript.Localization.WPF;
using Panuon.WPF.UI;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VPet_Simulator.Core;
using VPet_Simulator.Windows.Interface;
using LinePutScript.Converter;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Shapes;
using System.Text;

namespace VPET.Evian.Check_In
{
    public class Check_In : MainPlugin
    {
        public Setting Set;
        /// <summary>
        /// 获取开启时间或当前时间(当开启超过一天后)
        /// </summary>
        public DateTime OpenTime = DateTime.Now.Date;
        /// <summary>
        /// 存储今日任务是否完成
        /// </summary>
        private bool IfFinished = false;
        /// <summary>
        /// 今日任务
        /// </summary>
        private int CheckType = -1;
        /// <summary>
        /// 图片个数
        /// </summary>
        private int ImageNum = 0;
        /// <summary>
        /// 摸身子次数
        /// </summary>
        private int TBNum = 0;
        /// <summary>
        /// 摸头次数
        /// </summary>
        private int THNum = 0;
        /// <summary>
        /// 购买次数
        /// </summary>
        private int BuyNum = 0;
        /// <summary>
        /// 使用过的个数
        /// </summary>
        private int ImageUseNum = 0;
        /// <summary>
        /// 保存文档
        /// </summary>
        public LpsDocument MSave;
        /// <summary>
        /// 任务保存文档
        /// </summary>
        LpsDocument MTask;
        /// <summary>
        /// 是否显示过任务
        /// </summary>
        private bool IfShow = false;
        /// <summary>
        /// 任务描述
        /// </summary>
        public string Content =  "";
        /// <summary>
        /// 错误状态标记
        /// </summary>
        private bool ERRFlag = false;
        /// <summary>
        /// 错误1标记
        /// </summary>
        private bool ERR1Flag = false;
        /// <summary>
        /// 管理员标记
        /// </summary>
        public bool Administrator = false;
        /// <summary>
        /// 管理员登录标记
        /// </summary>
        public bool AdEnable = false;
        /// <summary>
        /// OP权限
        /// </summary>
        /// <param name="IUN">ImageUseNum</param>
        /// <param name="IF">IfFinished</param>
        /// <param name="CT">CheckType</param>
        /// <param name="IS">IfShow</param>
        public void AdChange(int IUN = -1, short IF = -1,int CT = -1, short IS = -1)
        {
            if(IUN != -1)
            {
                ImageUseNum = IUN;
                MW.GameSavesData["Task"][(gint)"ImageUseNum"] = ImageUseNum;
            }
            if (IF == 0) 
            {
                IfFinished = false;
                MW.GameSavesData["Task"][(gbol)"IfFinished"] = IfFinished;
            }
            else if(IF == 1)
            {
                IfFinished = true;
                MW.GameSavesData["Task"][(gbol)"IfFinished"] = IfFinished;
            }
            if(CT != -1)
            {
                CheckType = CT;
                MW.GameSavesData["Task"][(gint)"CheckType"] = CheckType;
            }
            if (IS == 0)
            {
                IfShow = false;
                MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow;
            }
            else if (IS == 1) 
            {
                IfShow = true;
                MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow; 
            }
            System.Windows.MessageBox.Show("更改成功");
        }
        public override string PluginName => "Check_In";
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public Check_In(IMainWindow mainwin) : base(mainwin)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
        }
        /// <summary>
        /// 当桌宠开启，加载mod的时候会调用这个函数
        /// </summary>
        public override void LoadPlugin()
        {
            ///从Setting.lps中读取存储的设置
            Set = new Setting(MW.Set["Check_In"]);

            ///添加列表项
            MenuItem modset = MW.Main.ToolBar.MenuMODConfig;
            modset.Visibility = Visibility.Visible;
            var menuItem = new MenuItem()
            {
                Header = "每日任务".Translate(),
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
            menuItem.Click += (s, e) => { Setting(); };
            modset.Items.Add(menuItem);
            ///添加时间确定函数
            MW.Main.FunctionSpendHandle += Datesta;
            MW.Main.TimeUIHandle += Check_In_UI;
            ///读取游戏存档
            //////读取Setting文件
            Set = new Setting(MW.Set["Check_In"]);
            Set.Enable = MW.Set["Check_In"].GetBool("Enable");
            //////读取Save文件
            if (LoaddllPath("Check_In") == "")
            {
                MessageBoxX.Show("缺少文件，请验证文件完整性".Translate(), "错误".Translate(),
                    MessageBoxButton.OK, MessageBoxIcon.Error);
                return;
            }
            var pathM = LoaddllPath("Check_In") + @"\Resources" + @"\Save"+ @"\Save.lps";
            var pathT = LoaddllPath("Check_In") + @"\Resources" + @"\Task" + @"\Task.lps";
            MSave = new LpsDocument(File.ReadAllText(pathM));
            MTask = new LpsDocument(File.ReadAllText(pathT));
            //////从读取到的MSave中读取数据
            if (MSave["ImageNum"].GetString("ImageNum") != null)    ///表情总数
            {
                ImageNum = MSave["ImageNum"][(gint)"ImageNum"];
            }
            else
            {
                MSave["ImageNum"][(gint)"ImageNum"] = 0;
                MessageBoxX.Show("请确定图片个数是否为0".Translate(), "警告".Translate(),
                    MessageBoxButton.OK, MessageBoxIcon.Warning, DefaultButton.YesOK, 5);
            }
            if (MW.GameSavesData["Task"].GetString("CheckType") != null)  ///任务类型
            {
                CheckType = MW.GameSavesData["Task"][(gint)"CheckType"];
            }
            else
            {
                MW.GameSavesData["Task"][(gint)"CheckType"] = 0;
            }
            if (MW.GameSavesData["Task"].GetString("ImageUseNum") != null)  ///表情给予数量
            {
                ImageUseNum = MW.GameSavesData["Task"][(gint)"ImageUseNum"];
            }
            else
            {
                MW.GameSavesData["Task"][(gint)"ImageUseNum"] = 0;
            }
            if (MW.GameSavesData["Task"].GetString("IfShow") != null)  ///是否显示过
            {
                IfShow = MW.GameSavesData["Task"][(gbol)"IfShow"];
            }
            else
            {
                MW.GameSavesData["Task"][(gbol)"IfShow"] = false;
            }
            if (MSave["ERR1Flag"].GetString("ERR1Flag") != null)  ///是否显示过ERR1
            {
                ERR1Flag = MSave["ERR1Flag"][(gbol)"ERR1Flag"];
            }
            else
            {
                MSave["ERR1Flag"][(gbol)"ERR1Flag"] = false;
            }
            var path = LoaddllPath("Check_In") + @"\Resources" + @"\Image";
            for (var i = 0; i < ImageNum; i++) 
            {
                if(MSave["ERRImage"][(gstr)i.ToString()] !=null)
                {
                    if (MSave["ERRImage"][(gbol)i.ToString()] == true)  ///需要改变的图片
                    {
                        if(i < ImageUseNum )
                        {
                            var pathU = path + @"\Unencrypted_State" + @"\" + "Gift" + i.ToString() + @".png";
                            var pathE = path + @"\Encryption_State" + @"\" + "Gift" + i.ToString() + @".png";
                            DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4=")); 
                        }
                        MSave["ERRImage"].Remove(i.ToString());
                    }
                }
            }
            if (MW.GameSavesData["Task"].GetString("IfFinished") != null)  ///是否完成过
            {
                IfFinished = MW.GameSavesData["Task"][(gbol)"IfFinished"];
            }
            else
            {
                MW.GameSavesData["Task"][(gbol)"IfFinished"] = false;
            }
            if (MW.GameSavesData["Task"].GetString("TBNum") != null)  ///摸身子次数
            {
                TBNum = MW.GameSavesData["Task"][(gint)"TBNum"];
            }
            else
            {
                TBNum = MW.GameSavesData.Statistics[(gint)"stat_touch_body"];
                MW.GameSavesData["Task"][(gint)"TBNum"] = MW.GameSavesData.Statistics[(gint)"stat_touch_body"];
            }
            if (MW.GameSavesData["Task"].GetString("THNum") != null)  ///摸头次数
            {
                THNum = MW.GameSavesData["Task"][(gint)"THNum"];
            }
            else
            {
                THNum = MW.GameSavesData.Statistics[(gint)"stat_touch_head"];
                MW.GameSavesData["Task"][(gint)"THNum"] = MW.GameSavesData.Statistics[(gint)"stat_touch_head"];
            }
            if (MW.GameSavesData["Task"].GetString("BuyNum") != null)  ///购买次数
            {
                BuyNum = MW.GameSavesData["Task"][(gint)"BuyNum"];
            }
            else
            {
                BuyNum = MW.GameSavesData.Statistics[(gint)"stat_buytimes"]- MW.GameSavesData.Statistics[(gint)"stat_autobuy"];
                MW.GameSavesData["Task"][(gint)"BuyNum"] = MW.GameSavesData.Statistics[(gint)"stat_buytimes"] - MW.GameSavesData.Statistics[(gint)"stat_autobuy"];
            }
            ///确定今日任务
            ///0.工作一次   1.学习一次  2.玩耍一次  3.摸头三次  4.摸身子三次  5.手动购买一个商品
            DateTime store;
            store = MW.GameSavesData["Task"]["OpenTime"].GetDateTime();
            if (OpenTime.Date != store.Date)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                CheckType = random.Next(6);
                MW.GameSavesData["Task"][(gint)"CheckType"] = CheckType;
                IfShow = false;
                MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow;
                IfFinished = false;
                MW.GameSavesData["Task"][(gbol)"IfFinished"] = IfFinished;
                TBNum = MW.GameSavesData.Statistics[(gint)"stat_touch_body"];
                MW.GameSavesData["Task"][(gint)"TBNum"] = TBNum;
                THNum = MW.GameSavesData.Statistics[(gint)"stat_touch_head"];
                MW.GameSavesData["Task"][(gint)"THNum"] = THNum;
                BuyNum = MW.GameSavesData.Statistics[(gint)"stat_buytimes"] - MW.GameSavesData.Statistics[(gint)"stat_autobuy"];
                MW.GameSavesData["Task"][(gint)"BuyNum"] = BuyNum;
            }
            if (MW.GameSavesData["Task"]["OpenTime"].GetString() != null)    ///上次开启时间
            {
                MW.GameSavesData["Task"][(gdat)"OpenTime"] = OpenTime;
            }
            ///判断任务类型，挂handle
            if (CheckType >= 0 && CheckType < 3)
            {
                MW.Main.WorkTimer.E_FinishWork += Worksta;
            }
            else if (CheckType >= 3 && CheckType < 6)
            {
                MW.Main.FunctionSpendHandle += Buysta;
            }
            ///将任务输入Content变量
            if (MTask[(gstr)("Task" + CheckType.ToString())] != null && IfFinished == false)
            {
                Content = MTask[(gstr)("Task" + CheckType.ToString())];
            }
            else if (MTask[(gstr)("Task" + CheckType.ToString())] == null)
            {
                MTask[(gstr)("Task" + CheckType.ToString())] = CheckType.ToString();
            }
            else if(IfFinished == true)
            {
                Content = "今日已签到".Translate();
            }
            //ShowTask();
            ///base.LoadPlugin();
        }
        /// <summary>
        /// 添加自定
        /// </summary>
        public override void LoadDIY()
        {
            MW.Main.ToolBar.AddMenuButton(VPet_Simulator.Core.ToolBar.MenuType.DIY, "每日任务".Translate(), ShowTask);
        }
        /// <summary>
        /// 生成winSetting对话框资源
        /// </summary>
        public winSetting winSetting;
        /// <summary>
        /// 生成/显示winSetting对话框
        /// </summary>
        public override void Setting()
        {
            if (winSetting == null)
            {
                winSetting = new winSetting(this);
                winSetting.Show();
            }
            else
            {
                winSetting.Topmost = true;
            }
        }
        /// <summary>
        /// 生成messagebox对话框资源
        /// </summary>
        public MessageBox messagebox;
        /// <summary>
        /// 生成/显示对话框
        /// </summary>
        public void MTaskBox()
        {
            if (MSave[(gbol)"Administrator"])
            {
                MSave.Remove("Administrator");
                Administrator = true;  
            }
            if (MTask[(gstr)("Task" + CheckType.ToString())] != null && IfFinished == false)
            {
                Content = MTask[(gstr)("Task" + CheckType.ToString())];
            }
            else if (MTask[(gstr)("Task" + CheckType.ToString())] == null)
            {
                MTask[(gstr)("Task" + CheckType.ToString())] = CheckType.ToString();
            }
            if (messagebox == null)
            {
                messagebox = new MessageBox(this);
                messagebox.Show();
            }
            else
            {
                messagebox = null ;
                messagebox = new MessageBox(this);
                messagebox.Show();
            }
        }
        /// <summary>
        /// UI显示函数
        /// </summary>
        /// <param name="main"></param>
        private void Check_In_UI(Main main)
        {
            if (Set.Enable)
            {
                if(IfShow == false)
                {
                    ShowTask();
                }
                if (ERR() != 0 && ERRFlag == false) 
                {
                    ERRFlag = true;
                    if (ERR() == 1 &&ERR1Flag == false)
                    {
                        ERR1Flag = true;
                        ERRFlag = false;
                        MSave["ERR1Flag"][(gbol)"ERR1Flag"] = ERR1Flag;
                        MessageBoxX.Show("你已经获得了目前所有表情，期待下一次的更新吧\n如果你有想分享的表情图片，也可以联系作者楚依云(虚拟桌宠官方QQ群和Steam同名)".Translate()
                        , "恭喜".Translate(),
                        MessageBoxButton.OK, MessageBoxIcon.Success, DefaultButton.YesOK);
                    }
                    else if (ERR() == 2) 
                    {
                        MessageBoxX.Show("缺少文件，请验证文件完整性".Translate(), "错误".Translate(),
                        MessageBoxButton.OK, MessageBoxIcon.Error);
                    }
                    else if (ERR() == 3)
                    {
                        MessageBoxX.Show("图片缺失，请验证文件完整性".Translate(), "错误".Translate(),
                        MessageBoxButton.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// 判断是否出现异常情况
        /// </summary>
        /// <returns>如果一切正常返回1</returns>
        private int ERR()
        {
            if (ImageUseNum >= ImageNum)
            {
                return 1;
            }
            if (LoaddllPath("Check_In") == "")
            {
                return 2;
            }
            var path = LoaddllPath("Check_In") + @"\Resources" + @"\Image";
            if (!Directory.Exists(path))
            {
                return 2;
            }
            var pathE = path + @"\Encryption_State" + @"\Gift" + ImageUseNum.ToString() + @".png";
            if (!File.Exists(pathE))
            {
                return 3;
            }
            return 0;
        }
        /// <summary>
        /// 用于判断日期变化
        /// </summary>
        private async void Datesta()
        {
            if (DateTime.Now.Date > OpenTime.Date)
            {
                if (CheckType >= 0 && CheckType < 3 && IfFinished == false && Set.Enable == true) 
                {
                    MW.Main.WorkTimer.E_FinishWork -= Worksta;
                }
                else if (CheckType >= 3 && CheckType < 6 && IfFinished == false && Set.Enable == true) 
                {
                    MW.Main.FunctionSpendHandle -= Buysta;
                }
                OpenTime = DateTime.Now.Date;
                IfShow = false;
                MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow;
                IfFinished = false;
                MW.GameSavesData["Task"][(gbol)"IfFinished"] = IfFinished;
                TBNum = MW.GameSavesData.Statistics[(gint)"stat_touch_body"];
                MW.GameSavesData["Task"][(gint)"TBNum"] = TBNum;
                THNum = MW.GameSavesData.Statistics[(gint)"stat_touch_head"];
                MW.GameSavesData["Task"][(gint)"THNum"] = THNum;
                BuyNum = MW.GameSavesData.Statistics[(gint)"stat_buytimes"] - MW.GameSavesData.Statistics[(gint)"stat_autobuy"];
                MW.GameSavesData["Task"][(gint)"BuyNum"] = BuyNum;
                ///确定今日任务
                ///0.工作一次   1.学习一次  2.玩耍一次  3.摸头三次  4.摸身子三次  5.手动购买一个商品
                Random random = new Random(DateTime.Now.Millisecond);
                CheckType = random.Next(6);
                MW.GameSavesData["Task"][(gint)"CheckType"] = CheckType;
                if (CheckType >= 0 && CheckType < 3)
                {
                    MW.Main.WorkTimer.E_FinishWork += Worksta;
                }
                else if (CheckType >= 3 && CheckType < 6)
                {
                    MW.Main.FunctionSpendHandle += Buysta;
                }
                if (MTask[(gstr)("Task" + CheckType.ToString())] != null && IfFinished == false)
                {
                    Content = MTask[(gstr)("Task" + CheckType.ToString())];
                }
                else if (MTask[(gstr)("Task" + CheckType.ToString())] == null) 
                {
                    MTask[(gstr)("Task" + CheckType.ToString())] = CheckType.ToString();
                }
            }
        }
        /// <summary>
        /// 改变开启状态/判断今日任务种类
        /// </summary>
        public void ChangeOpenState(short change = -1)
        {
            if(change == -1)
            {
                Set.Enable = ! Set.Enable;
            }
            else if(change == 0)
            {
                Set.Enable = false;
            }
            else if(change == 1)
            {
                Set.Enable = true;
            }
            if(Set.Enable == true)
            {
                if (CheckType >= 0 && CheckType < 3)
                {
                    MW.Main.WorkTimer.E_FinishWork += Worksta;
                }
                else if (CheckType >= 3 && CheckType < 6)
                {
                    MW.Main.FunctionSpendHandle += Buysta;
                }
            }
        }
        /// <summary>
        /// 用于统计工作/学习/娱乐次数
        /// </summary>
        /// <param name="obj">obj变量中可获得工作种类</param>
        private void Worksta(WorkTimer.FinishWorkInfo obj)
        {
            if(IfFinished || (Set.Enable == false)||(CheckType >= 3 && CheckType < 6))
            {
                MW.Main.WorkTimer.E_FinishWork -= Worksta;
                return;
            }
            switch (CheckType)
            {
                case 0: ///工作一次
                    {
                        if (obj.work.Type == GraphHelper.Work.WorkType.Work && obj.spendtime >= 0.9 * obj.work.Time)
                        {
                            GiveBonus();
                        }
                        break;
                    }
                case 1: ///学习一次
                    {
                        if (obj.work.Type == GraphHelper.Work.WorkType.Study && obj.spendtime >= 0.9 * obj.work.Time)
                        {
                            GiveBonus();
                        }
                        break;
                    }
                case 2: ///玩耍一次
                    {
                        if (obj.work.Type == GraphHelper.Work.WorkType.Play && obj.spendtime >= 0.9 * obj.work.Time)
                        {
                            GiveBonus();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        /// <summary>
        /// 用于统计购买/摸头/摸身子次数
        /// </summary>
        private void Buysta()
        {
            if (IfFinished || (Set.Enable == false)|| (CheckType >= 0 && CheckType < 3))
            {
                MW.Main.FunctionSpendHandle -= Buysta;
                return;
            }
            switch (CheckType)
            {
                case 3: ///摸头三次
                    {
                        if (MW.GameSavesData.Statistics[(gint)"stat_touch_head"] >= (THNum + 3))  
                        {                           
                            GiveBonus();
                        }
                        break;
                    }
                case 4: ///摸身子三次
                    {
                        if (MW.GameSavesData.Statistics[(gint)"stat_touch_body"] >= (TBNum + 3))
                        {
                            GiveBonus();
                        }
                        break;
                    }
                case 5: ///手动购买一个商品
                    {
                        if(MW.GameSavesData.Statistics[(gint)"stat_buytimes"] - MW.GameSavesData.Statistics[(gint)"stat_autobuy"] >= BuyNum + 1)
                        {
                            GiveBonus();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        /// <summary>
        /// 用于显示今日任务
        /// </summary>
        public void ShowTask()
        {
            IfShow = true;
            MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow;
            MTaskBox();
        }
        /// <summary>
        /// 用于给予奖励
        /// </summary>
        private void GiveBonus()
        {
            IfFinished = true;
            MW.GameSavesData["Task"][(gbol)"IfFinished"] = IfFinished;
            IfShow = false;
            MW.GameSavesData["Task"][(gbol)"IfShow"] = IfShow;
            Random random = new Random(DateTime.Now.Millisecond);
            Random rand = new Random(DateTime.Now.Millisecond);
            ///随机奖励
            var BonusType = random.Next(5);
            ///如果等级高于900就考虑经验，避免溢出
            if(MW.GameSavesData.GameSave.Level > 900)
            {
                if(BonusType == 0|| BonusType == 2)
                {
                    BonusType += 1;
                }
            }
            ///理论获取奖励
            var gain = (MW.GameSavesData.GameSave.LevelUpNeed() + MW.GameSavesData.GameSave.Exp) * 100 / (100 + Math.Sqrt(MW.GameSavesData.GameSave.Level * 2));///确保不超模
            ///实际获取奖励
            var gain_actual = Math.Max(Math.Max(gain * 0.75, rand.Next(Convert.ToInt32(gain))), 200);
                if (BonusType == 0 || BonusType == 2) 
                {
                    Content = MTask[(gstr)("Bonus" + 0.ToString())] + gain_actual.ToString("0.00");
                }
                else if (BonusType == 1 || BonusType == 3) 
                {
                    Content = MTask[(gstr)("Bonus" + 1.ToString())] + (gain_actual/10).ToString("0.00");
                }
                else if(BonusType == 4)
                {
                    Content = MTask[(gstr)("Bonus" + 3.ToString())];
                    MW.GameSavesData["Task"][(gint)"ImageUseNum"] ++;

                }
                if (BonusType == 2 || BonusType == 3) 
                {
                    Content += MTask[(gstr)("Bonus" + 2.ToString())];
                    MW.GameSavesData["Task"][(gint)"ImageUseNum"] ++;
                }

            switch (BonusType)
            {
                case 0: ///升一级
                    {
                        MW.GameSavesData.GameSave.Exp += Math.Round(gain_actual, 2);
                        break;
                    }
                case 1: ///给钱
                    {
                        MW.GameSavesData.GameSave.Money += Math.Round(gain_actual / 10, 2);
                        break;
                    }
                case 2: ///升级加给图
                    {
                        MW.GameSavesData.GameSave.Exp += Math.Round(gain_actual, 2);
                        DecryptBonus("Gift");
                        break;
                    }
                case 3: ///给钱加给图
                    {
                        MW.GameSavesData.GameSave.Money += Math.Round(gain_actual / 10, 2);
                        DecryptBonus("Gift");
                        break;
                    }
                case 4: ///给图
                    {
                        DecryptBonus("Gift");
                        break;
                    }
                default:
                    {
                        break;
                    }
            } 
        }
        /// <summary>
        /// 用于解密奖励图
        /// </summary>
        /// <param name="ImageName">图片名称</param>
        private void DecryptBonus(string ImageName)
        {
            if (ERR() == 3)
            {
                return;
            }
            var path = "";
            path = LoaddllPath("Check_In") + @"\Resources" + @"\Image";
            var pathU = path + @"\Unencrypted_State" + @"\" + "Gift" + ImageUseNum.ToString() + @".png";
            var pathE = path + @"\Encryption_State" + @"\" + "Gift" + ImageUseNum.ToString() + @".png";
            if (!Directory.Exists(path + @"\Unencrypted_State"))
                Directory.CreateDirectory(path + @"\Unencrypted_State");
            DecryptImage(pathE, pathU, Base64Converter.ToBase64String("ZXZhaW4="));
            ImageUseNum = MW.GameSavesData["Task"][(gint)"ImageUseNum"];
            Process.Start("explorer.exe",pathU);
        }
        public override void EndGame()
        {
            if (ERR() != 0 && ERR() != 3 && ERR() != 1)   
            {
                return;
            }
            if (MSave["Administrator"].GetString() != null)
                MSave.Remove("Administrator");
            var pathS = LoaddllPath("Check_In") + @"\Resources" + @"\Save" + @"\Save.lps";
            var pathT = LoaddllPath("Check_In") + @"\Resources" + @"\Task" + @"\Task.lps";
            File.WriteAllText(pathS, MSave.ToString());
            File.WriteAllText(pathT, MTask.ToString());
            base.Save();
            base.EndGame();
        }
        /// <summary>
        /// 获取mod路径（By白草）
        /// </summary>
        /// <param name="dll">mod对应dll的名字</param>
        /// <returns>mod对应路径,或者""(如果没找到的话)</returns>
        public string LoaddllPath(string dll)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in loadedAssemblies)
            {
                string assemblyName = assembly.GetName().Name;

                if (assemblyName == dll)
                {
                    string assemblyPath = assembly.Location;

                    string assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyPath);

                    string parentDirectory = Directory.GetParent(assemblyDirectory).FullName;



                    return parentDirectory;
                }
            }
            return "";
        }
        /// <summary>
        /// 图片解密函数
        /// </summary>
        /// <param name="inputImagePath">加密的图片路径</param>
        /// <param name="outputImagePath">解密后保存路径</param>
        /// <param name="key">加密用的密钥</param>
        public static void DecryptImage(string inputImagePath, string outputImagePath, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                byte[] iv = { 0x09, 0x02, 0x01, 0x03, 0xFF, 0x22, 0x03, 0x15, 0x28, 0xA4, 0x2B, 0x5A, 0x9D, 0xEE, 0x0E, 0xAF };
                aesAlg.IV = iv; // 使用上述指定IV，可修改，但加密IV和解密IV应相同


                using (FileStream inputFile = new FileStream(inputImagePath, FileMode.Open))
                using (FileStream outputFile = new FileStream(outputImagePath, FileMode.Create))
                using (CryptoStream cryptoStream = new CryptoStream(inputFile, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputFile);
                }
            }
        }

    }
    public static class Base64Converter
    {
        public static string ToBase64String(string value)
        {
            if (value == null || value == "")
                return "";

            byte[] bytes = Encoding.Unicode.GetBytes(value); // 使用Unicode编码将UTF16字符串转换为字节数组
            return Convert.ToBase64String(bytes); // 将字节数组转换为base64编码的字符串
        }

        public static string FromBase64String(string value)
        {
            if (value == null || value == "")
                return "";

            byte[] bytes = Convert.FromBase64String(value); // 将base64编码的字符串转换为字节数组
            return Encoding.Unicode.GetString(bytes); // 使用Unicode编码将字节数组转换为UTF16字符串
        }
    }
}

