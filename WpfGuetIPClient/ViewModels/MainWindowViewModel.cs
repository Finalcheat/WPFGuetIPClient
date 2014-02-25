using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfGuetIPClient.Commands;
using WpfGuetIPClient.Models;

namespace WpfGuetIPClient.ViewModels
{

   

    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));


        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }





    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
          
        }


        private bool isChecked = true;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                this.RaisePropertyChanged("IsChecked");
            }
        }

        //private string version;

        //public string Version
        //{
        //    get { return version; }
        //    set
        //    {
        //        version = value;
        //        this.RaisePropertyChanged("Version");
        //    }
        //}



        //private string ipAddress;

        //public string IpAddress
        //{
        //    get { return ipAddress; }
        //    set
        //    {
        //        ipAddress = value;
        //        this.RaisePropertyChanged("IpAddress");
        //    }
        //}


        //private string connectInformation;

        //public string ConnectInformation
        //{
        //    get { return connectInformation; }
        //    set
        //    {
        //        connectInformation = value;
        //        this.RaisePropertyChanged("ConnectInformation");
        //    }
        //}



        //private double flow;

        //public double Flow
        //{
        //    get { return flow; }
        //    set
        //    {
        //        flow = value;
        //        this.RaisePropertyChanged("Flow");
        //    }
        //}


        //private double money;

        //public double Money
        //{
        //    get { return money; }
        //    set
        //    {
        //        money = value;
        //        this.RaisePropertyChanged("Money");
        //    }
        //}


        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                this.RaisePropertyChanged("UserName");
            }
        }


        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.RaisePropertyChanged("Password");
            }
        }


        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }


        private GuetIPClientModel guetIPModel;
        public GuetIPClientModel GuetIPModel
        {
            get { return guetIPModel; }
            set
            {
                guetIPModel = value;
                this.RaisePropertyChanged("GuetIPModel");
            }
        }

        public MainWindowViewModel()
        {
            GuetIPModel = new GuetIPClientModel();
            Thread t = new Thread(serverConnect);
            t.IsBackground = true;
            t.Start();

            
            //Flow = "1024";
            //Money = "5";
            //UserName = "1000360117";
            //Password = "123456";
            

            this.OpenCommand = new DelegateCommand();
            this.OpenCommand.ExecuteAction = new Action<object>(this.openClick);

            this.CloseCommand = new DelegateCommand();
            this.CloseCommand.ExecuteAction = new Action<object>(this.closeClick);

            userNameParam = new CspParameters();
            userNameParam.KeyContainerName = "WpfGuetIPClient";


            //获取网卡硬件地址
            string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }
            passwordParam = new CspParameters();
            passwordParam.KeyContainerName = mac;

            dataMap = new Dictionary<string, string>();

            readFileData();
        }


        private void openClick(object parameter)
        {
            //Random r = new Random();
            //Money = r.Next().ToString();

            //Money = Password;

            //ConnectInformation = "IP开放中......";
            Thread t = new Thread(IPOpen);
            t.IsBackground = true;
            t.Start();
        }

        private void IPOpen()
        {
            if (UserName == null)
                return;

            if (Password == null)
                return;

            //string str = System.Environment.CurrentDirectory;
           
            if (GuetIPModel.IPOpen(UserName, Password))
            {
                //ConnectInformation = "开放IP成功";

               
                if (IsChecked && (!userNameIsExist(UserName)))
                {
                    dataMap.Add(UserName, rsaPasswordEncode(Password));
                }
                else if (!isChecked)
                {
                    dataMap.Remove(UserName);
                }


                if (!isWork)
                {
                    Thread t1 = new Thread(sendThread);
                    t1.IsBackground = true;
                    t1.Start();
                    isWork = true;
                }
            }

        }

        //private void recvThread()
        //{
        //    while (true)
        //    {
        //        Information temp = model.getInformation();
        //        Money = temp.Money;
        //        Flow = temp.Flow;
        //        Thread.Sleep(30000);
        //    }
        //}

        private void sendThread()
        {
            while (true)
            {
                GuetIPModel.setInformation();
                GuetIPModel.getInformation();
                //Money = temp.Money;
                //Flow = temp.Flow;
                Thread.Sleep(30000);
            }
        }

        private void serverConnect()
        {
            GuetIPModel.isConnect();
        }

        private void closeClick(object parameter)
        {
            //ConnectInformation = "IP关闭中......";
            Thread t = new Thread(IPClose);
            t.IsBackground = true;
            t.Start();
        }

        public void IPClose()
        {
            if (GuetIPModel.IPClose())
            {
                isWork = false;
                //ConnectInformation = "关闭IP成功";
            }

        }


        private string rsaUserNameEncode(string userNameData)
        {
            //param = new CspParameters();
            //param.KeyContainerName = "Olive";//密匙容器的名称，保持加密解密一致才能解密成功
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(userNameParam))
            {
                byte[] plaindata = Encoding.Default.GetBytes(userNameData);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }

        private string rsaUserNameDecode(string key)
        {

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(userNameParam))
            {
                byte[] encryptdata = Convert.FromBase64String(key);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return Encoding.Default.GetString(decryptdata);
            }
        }

        private string rsaPasswordEncode(string passwordData)
        {
            //param = new CspParameters();
            //param.KeyContainerName = "Olive";//密匙容器的名称，保持加密解密一致才能解密成功
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(passwordParam))
            {
                byte[] plaindata = Encoding.Default.GetBytes(passwordData);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }

        private string rsaPasswordDecode(string key)
        {

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(passwordParam))
            {
                byte[] encryptdata = Convert.FromBase64String(key);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return Encoding.Default.GetString(decryptdata);
            }
        }


        private bool userNameIsExist(string userNameData)
        {

            bool b = false;
            foreach (KeyValuePair<string, string> pair in dataMap)
            {
                if (pair.Key == userNameData)
                {
                    b = true;
                    break;
                }
            }

            return b;
        }

        public void userNameTextChanged(string userNameData)
        {
            foreach (KeyValuePair<string, string> pair in dataMap)
            {
                if (pair.Key.Equals(userNameData))
                {
                    Password = rsaPasswordDecode(pair.Value);
                    break;
                }
            }
        }

        public void saveData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.Environment.CurrentDirectory + "\\WpfGuetIPClientData.txt", false))
            {
                foreach (KeyValuePair<string, string> pair in dataMap)
                {
                    file.Write(rsaUserNameEncode(pair.Key));
                    file.Write(' ');
                    file.WriteLine(pair.Value);
                    file.Write("\r\n");
                }
            }
        }

        private void readFileData()
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "\\WpfGuetIPClientData.txt"))
                {
                    // Read and display lines from the file until the end of 
                    // the file is reached.

                    string str = sr.ReadToEnd();
                    string[] line = str.Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);

                    int i;
                    string[] data = null;
                    for (i = 0; i < line.Length; i++)
                    {
                        data = line[i].Split(' ');
                        dataMap.Add(rsaUserNameDecode(data[0]), data[1]);
                    }

                    if (i != 0)
                    {
                        UserName = rsaUserNameDecode(data[0]);
                        Password = rsaPasswordDecode(data[1]);
                    }

                }
            }
            catch (Exception)
            {
                // Let the user know what went wrong.
                return;
            }
        }

        private CspParameters userNameParam;
        private CspParameters passwordParam;
        private Dictionary<string, string> dataMap;
        private bool isWork = false;
    }

}
