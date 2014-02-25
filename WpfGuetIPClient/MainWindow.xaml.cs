using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfGuetIPClient.ViewModels;

namespace WpfGuetIPClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        public MainWindow()
        {
            InitializeComponent();


            model = new MainWindowViewModel();
            this.DataContext = model;


            //设置托盘的各个属性
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            //notifyIcon.BalloonTipText = "程序开始运行";
            notifyIcon.Text = "出校器";
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.Visible = true;
            //notifyIcon.ShowBalloonTip(2000);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);

            //设置菜单项
            //System.Windows.Forms.MenuItem menu1 = new System.Windows.Forms.MenuItem("菜单项1");
            //System.Windows.Forms.MenuItem menu2 = new System.Windows.Forms.MenuItem("菜单项2");
            //System.Windows.Forms.MenuItem menu = new System.Windows.Forms.MenuItem("菜单", new System.Windows.Forms.MenuItem[] { menu1 , menu2 });

            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);

            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] {  exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            //窗体状态改变时候触发
            //this.StateChanged += new EventHandler(SysTray_StateChanged);
        }

        private void SysTray_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// 退出选项
        ///
        ///

        ///

        private void exit_Click(object sender, EventArgs e)
        {
            closeButtonClick(null, null);
        }

        ///
        /// 鼠标单击
        ///
        ///

        ///

        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    bool bl = this.Activate();
                }
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private Storyboard leafStoryboard;
        private Storyboard cloudStoryboard;

        private bool b;


        private void miniButtonClick(object sender, RoutedEventArgs e)
        {
//            this.WindowState = System.Windows.WindowState.Minimized;

            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Hidden;

            }

            leafStoryboard.Stop();

            cloudStoryboard.Stop();

            b = false;
            
        }

        private void closeButtonClick(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("确定要关闭吗?",
                                               "退出",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question,
                                                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                model.saveData();
                notifyIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            }
            
            //Thread t = new Thread(model.IPClose);
            ////t.IsBackground = true;
            //t.Start();
            //this.Close();
        }

        private void userNameChanged(object sender, TextChangedEventArgs e)
        {
            model.userNameTextChanged(userNameTextBox.Text);
        }

        private MainWindowViewModel model;

        private void activated(object sender, EventArgs e)
        {
            if (!b)
            {
                leafStoryboard = Resources["leafLeave"] as Storyboard;
                leafStoryboard.Begin();

                cloudStoryboard = Resources["cloudMove"] as Storyboard;
                cloudStoryboard.Begin();


                b = true;
            }
        }

    }
}
