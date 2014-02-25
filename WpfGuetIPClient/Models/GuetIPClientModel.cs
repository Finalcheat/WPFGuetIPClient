using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfGuetIPClient.Models
{
    public class MoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double money = (double)value;
            return money.ToString("f2") + "元";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class FlowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double money = (double)value;
            return ((int)(money / 1024)).ToString() + "KB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    class GuetIPClientModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        private string version;

        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                this.RaisePropertyChanged("Version");
            }
        }



        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                this.RaisePropertyChanged("IpAddress");
            }
        }


        private string connectInformation;

        public string ConnectInformation
        {
            get { return connectInformation; }
            set
            {
                connectInformation = value;
                this.RaisePropertyChanged("ConnectInformation");
            }
        }



        private double flow;

        public double Flow
        {
            get { return flow; }
            set
            {
                flow = value;
                this.RaisePropertyChanged("Flow");
            }
        }


        private double money;

        public double Money
        {
            get { return money; }
            set
            {
                money = value;
                this.RaisePropertyChanged("Money");
            }
        }

        public GuetIPClientModel()
        {
            // This constructor arbitrarily assigns the local port number.
            udp = new UdpClient(5201);
            udp2 = new UdpClient(5200);
            ip = new IPEndPoint(IPAddress.Parse("172.16.12.11"), 5301);
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("172.16.12.11"), 5200);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("172.16.12.11"), 5300);
            udp2.Connect(ipEndPoint);

            //Creates a TCPClient using a local end point.
            //IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            //IPEndPoint ipLocalEndPoint = new IPEndPoint(IPAddress.Parse("10.21.146.149"), 0);
            //tcp = new TcpClient(ipLocalEndPoint);

            //tcp.Connect(IPAddress.Parse("172.16.12.11"), 80);
            

            //IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            //for (int i = 0; i < addressList.Length; ++i)
            //{
            //    if (addressList[i].AddressFamily.ToString() == "InterNetwork")
            //    {
            //        IPAddress IpAddress = addressList[i];
            //        IPEndPoint ipLocalEndPoint = new IPEndPoint(IpAddress, 5200);
            //        tcp = new TcpClient(ipLocalEndPoint);
            //        break;
            //    }
            //}
            
            //udp.Send();

            Version = "Version 1.85";

            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            for (int i = 0; i < addressList.Length; ++i)
            {
                if (addressList[i].AddressFamily.ToString() == "InterNetwork")
                {
                    IpAddress = addressList[i].ToString();
                    break;
                }
            }
            ConnectInformation = "连接服务器中......";
        }


        public bool isConnect()
        {

            try
            {
                udp.Connect(ip);
                //udpClient.Connect("www.contoso.com", 5301);


                byte[] data = get0x05Pack();

                //log(data, 0x05);

                udp.Send(data, data.Length);

                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 5201);


                //udp.BeginReceive(new AsyncCallback(ReceiveCallback), udp);
                //string returnData = Encoding.ASCII.GetString(receiveBytes);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            int time = 0;

            while (time < 30)
            {
                if (udp.Available > 0)
                {
                    //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 5201);
                    byte[] data = udp.Receive(ref ip);
                    if (data[2] == 0x06)
                    {
                        //log(data, 0x06);
                        int informationLength = BitConverter.ToInt32(data, 0x25);
                        ConnectInformation = Encoding.Default.GetString(data, 0x29, informationLength);
                       
                    }
                    //log(re, 0x06);

                    return true;
                }
                
                ++time;
                Thread.Sleep(100);
            }

            //udp.EndReceive(udp, ip);
            ConnectInformation = "连接服务器失败";
            return false;
        }


        private void log(byte[] bytes, int num)
        {
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str = str + bytes[i].ToString("X2") + " ";
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"H:\VS2012\WpfGuetIPClient\WpfGuetIPClient\1.txt", true))
            {
                file.Write(num.ToString("X2"));
                file.Write("\r\n");
                file.WriteLine(str);
                file.Write("\r\n");
            }

            //System.IO.FileStream fs = new System.IO.FileStream(@"H:\VS2012\WpfGuetIPClient\WpfGuetIPClient\1.txt", System.IO.FileMode.Append);
            //fs.Write(bytes, 0, bytes.Length);


            //fs.Close();
        }


        //private void ReceiveCallback(IAsyncResult ar)
        //{
        //    UdpClient u = (UdpClient)(ar.AsyncState);
        //    //IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

        //    Byte[] receiveBytes = udp.EndReceive(ar, ref ip);
        //    string receiveString = Encoding.ASCII.GetString(receiveBytes);

        //    Console.WriteLine("Received: {0}", receiveString);
        //    messageReceived = true;
        //}


        private byte[] get0x05Pack()
        {
            Byte[] head = new Byte[86] { 0x82,  0x23,  0x05,  0x43,  0x28,  0x02,  00,  00,  00,  00,  00,  00,  00,  00,  00 , 0x40  
                                              , 0x9c,  0x05,  0x41,  00,  00,  00,  00,  0xf0,  0x86,  0x1b,  0x41,  0x01,  00,  00,  00,  0x61
                                              , 0x01,  00,  00,  00,  0x62,  0x29,  00,  00,  00,  0x4a,  0x46,  0x5d,  0x6b,  0x76,  0x53,  0x10  
                                              , 0x3b , 0x4e,  0x43,  0x3f,  0x66,  0x6d , 0x71,  0x25,  0x11,  0x1d,  0x5c,  0x0a,  0x19, 0x4f,  0x40,  0x5b  
                                              , 0x6f,  0x52,  0x26,  0x47,  0x6f,  0x58,  0x64,  0x39,  0x6e,  0x33 , 0x4f,  0x52,  0x77,  0x75,  0x24,  0x20  
                                              , 0x27, 00,  00,  00 , 00,  00 };

            Byte[] data = new Byte[500];
            for (int i = 0; i < 86; ++i)
            {
                data[i] = head[i];
            }
            for (int j = 86; j < 500; ++j)
            {
                data[j] = 0xff;
            }

            return data;
        }


        private byte[] get0x14Pack()
        {
            byte[] head = new byte[] { 0x82, 0x23, 0x14 };
            byte[] data = new byte[300];

            int i;
            for (i = 0; i < head.Length; i++)
            {
                data[i] = head[i];
            }

            for (int j = 0; j < 8; j++)
            {
                data[i + j] = 0;
            }

            i += 8;

            byte[] userNameByte = Encoding.ASCII.GetBytes(_userName);

            data[i++] = (byte)userNameByte.Length;
            data[i++] = 0;
            data[i++] = 0;
            data[i++] = 0;

            for (int j = 0; j < userNameByte.Length; j++)
            {
                data[i + j] = (byte)(userNameByte[j] - 0x0f);
            }

            i += userNameByte.Length;

            data[i++] = 0x13;

            for (int j = 0; j < 22; j++)
            {
                data[i + j] = 0;
            }

            i += 22;

            byte[] tail = new byte[] { 01 , 00 , 00 , 00 , 0x61 , 0xbe , 0x96 , 0xd4 , 0x03 , 00 , 00 ,
                                       00 , 00 , 00 , 00 , 0xf0 , 0x3f , 01 , 00 , 00 , 00 , 0x62 };

            for (int j = 0; j < tail.Length; j++)
            {
                data[i + j] = tail[j];
            }

            i += tail.Length;

            for (;  i < 300; i++)
            {
                data[i] = 0xff;
            }


            return data;
        }

        private byte[] get0x16Pack(Int16 key)
        {
            byte[] head = new byte[] { 0x82, 0x23, 0x16 };
            byte[] data = new byte[300];
            int i;
            for (i = 0; i < head.Length; i++)
            {
                data[i] = head[i];
            }

            i += head.Length;

            for (int j = 0; j < 8; j++)
            {
                data[i + j] = 0;
            }

            i += 8;

            data[i++] = 0x13;
            data[i++] = 0;
            data[i++] = 0;
            data[i++] = 0;

            byte[] temp = new byte[] { 0x6c , 0x14 , 0x58 , 0x33 , 0x13 , 0x52 , 0x0a,  0x21 , 0x2c , 0x28,
                                        0x5a , 0x5b , 0x38 , 0x74 , 0x1a , 0x28, 0x31 , 0x37 , 00 };

            for (int j = 0; j < temp.Length; j++)
            {
                data[i + j] = temp[j];
            }

            i += temp.Length;


            string keyStr = "";
            for (int j = 0; j < _password.Length; j++)
            {
                Int16 keyValue = ((Int16)((key >> 8) ^ (Int16)_password[j]));
                keyStr += keyValue.ToString("X");
                key = (Int16)((key + keyValue) * 0xce6b + 0x58bf);
            }

            byte[] keyByte = Encoding.ASCII.GetBytes(keyStr);

            data[i++] = (byte)keyByte.Length;
            data[i++] = 0;
            data[i++] = 0;
            data[i++] = 0;

            for (int j = 0; j < keyByte.Length; j++)
            {
                data[i + j] = keyByte[j];
            }

            i += keyByte.Length;

            data[i++] = 0x11;
            data[i++] = 0;
            data[i++] = 0;
            data[i++] = 0;


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

            byte[] macByte = Encoding.ASCII.GetBytes(mac);

            for (int j = 0; j < macByte.Length; ++j)
            {
                if (macByte[j] == 0x3a)
                {
                    data[i + j] = 0x2d;
                }
                else
                {
                    data[i + j] = macByte[j];
                }
            }

            i += macByte.Length;


            byte[] tail = new byte[] { 0xbe , 0x10 , 00 , 00 , 00 , 00 , 00 , 00 , 00 , 00 , 0xf0 , 0x3f , 0x01 , 0x00 , 0x00  ,
                                       0x00 , 0x62 };

            for (int j = 0; j < tail.Length; j++)
            {
                data[i + j] = tail[j];
            }

            i += tail.Length;

            for (; i < 300; i++)
            {
                data[i] = 0xff;
            }

            return data;

        }

        public bool IPClose()
        {
            ConnectInformation = "关闭IP中......";
            byte[] data = get0x14Pack();
            udp2.Send(data, data.Length);
            int time = 0;

            bool isReceive = false;
            while (time < 30)
            {
                if (udp2.Available > 0)
                {
                    data = udp2.Receive(ref RemoteIpEndPoint);
                    isReceive = true;
                    break;
                }
                ++time;
                Thread.Sleep(100);
            }

            if (!isReceive)
            {
                ConnectInformation = "关闭IP失败";
                return false;
            }

            int offset = 0x35 + _userName.Length - 10;
            Int16 key = (Int16)(data[offset] * 0x100 + data[offset - 1] - 0x2382);
            data = get0x16Pack(key);
            udp2.Send(data, data.Length);

            time = 0;
            while (time < 30)
            {
                if (udp2.Available > 0)
                {
                    data = udp2.Receive(ref RemoteIpEndPoint);
                    if (data[2] == 0x17)
                    {
                        ConnectInformation = "关闭IP成功";
                        return true;
                    }
                }
                ++time;
                Thread.Sleep(100);
            }

            ConnectInformation = "关闭IP失败";
            return false;

        }



        private byte[] get0x1fPack(string userName)
        {
            Byte[] head = new Byte[] { 0x82, 0x23, 0x1f, 0x00, 0x00, 0x00, 00, 00, 00, 00, 00 };
            Byte[] tail = new Byte[] { 0x13, 00, 00 , 00 ,0x5c,  0x76,  0x1a  ,
                                           0x6e,  0x7a, 0x55 , 0x44,  0x64,  0x0f,  0x6b,  0x3e,  0x38,  0x66,
                                           0x4b,  0x77, 0x57,  00,  00 , 00 , 0x01,  00 , 00 , 00 , 0x61,  0xe6,
                                           0xee,  0x98, 0x03,  00 , 00 , 00 , 00 , 00 , 00,  0xf0 , 0x3f,  01,  00,  00 , 00 , 0x62 };

            Byte[] userNameAscii = Encoding.ASCII.GetBytes(userName);


            Byte[] data = new Byte[300];

            int i;
            for (i = 0; i < 11; ++i)
            {
                data[i] = head[i];
            }

            data[i] = (byte)userName.Length;
            for (i = 12; i < 15; ++i)
            {
                data[i] = 0;
            }
            int j;
            for (j = 0; j < userNameAscii.Length; ++j)
            {
                data[i + j] = (byte)(userNameAscii[j] - 0x0a);
            }
            i += userNameAscii.Length;
            for (j = 0; j < tail.Length; ++j)
            {
                data[i + j] = tail[j];
            }
            i += j;
            for (; i < 300; ++i)
            {
                data[i] = 0xff;
            }

            return data;
        }


        private byte[] get0x21Pack(Int16 keyNum)
        {
            _key = (Int16)(keyNum - 0x0d10);
            string key = (_key).ToString();
            //string key = "3028";

            MD5 md5 = MD5.Create();
            //byte[] test = md5.ComputeHash(Encoding.ASCII.GetBytes("3603123456"));
            byte[] md5Byte = md5.ComputeHash(Encoding.ASCII.GetBytes(key + _password));
            string s1 = md5Byte[0].ToString("X");
            string s2 = md5Byte[1].ToString("X");
            string s3 = (md5Byte[2] >> 4).ToString("X");
            string key2 = s1 + s2 + s3 + _userName;
            byte[] keyData = md5.ComputeHash(Encoding.ASCII.GetBytes(key2));


            //string str1 = System.Text.Encoding.ASCII.GetString(keyData);


            byte[] data = new byte[300];

            byte[] head = new byte[] { 0x82,  0x23,  0x21,  0x00  ,00 , 00  ,00 , 00  ,00  ,00 , 00 , 0x13 , 00 , 00 , 00 , 0x39  ,
                                            0x4a , 0x4e , 0x14 , 0x4a , 0x61 , 0x2f , 0x0c  ,0x1b  ,0x2c , 0x43  ,0x64 , 0x67 , 0x32 ,
                                            0x76 , 0x3f, 0xd8 , 0x06 , 00 , 0x1e , 00, 00, 00 };

            byte[] tail = new byte[] { 0x80 , 0x6e , 0xf8 , 0x03 , 00  ,00 , 00  ,
                                             00 , 00 , 00 , 0xf0  ,0x3f  ,01 , 00 , 00  ,00 , 0x62 };

            int i, j;
            for (i = 0; i < 0x26; ++i)
            {
                data[i] = head[i];
            }


            key = "";
            for (j = 0; j < 15; ++j)
            {
                int num1 = keyData[j] / 16;
                key += num1.ToString("X");

                int num2 = keyData[j] % 16;
                key += num2.ToString("X");

            }

            byte[] tempKeyData = keyData;
            keyData = Encoding.ASCII.GetBytes(key);

            for (j = 0; j < keyData.Length; ++j)
            {
                data[i + j] = keyData[j];
            }

            i += 30;

            data[i++] = 0x11;
            data[i++] = 0x00;
            data[i++] = 0x00;
            data[i++] = 0x00;


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

            byte[] macByte = Encoding.ASCII.GetBytes(mac);

            for (j = 0; j < macByte.Length; ++j)
            {
                if (macByte[j] == 0x3a)
                {
                    data[i + j] = 0x2d;
                }
                else
                {
                    data[i + j] = macByte[j];
                }
            }

            i += j;

            for (j = 0; j < tail.Length; ++j)
            {
                data[i + j] = tail[j];
            }

            for (i = 0x6a; i < 300; ++i)
            {
                data[i] = 0xff;
            }

            return data;
        }


        public bool IPOpen(string userName, string password)
        {
            if (userName != null)
            {
                _userName = userName;
            }
            if (password != null)
            {
                _password = password;
            }

            ConnectInformation = "IP开放中......";
            try
            {

                byte[] data = get0x1fPack(_userName);

                //log(data, 0x1f);
                udp2.Send(data, data.Length);

                int time = 0;

                while (time < 30)
                {
                    if (udp2.Available > 0)
                    {
                        
                        data = udp2.Receive(ref RemoteIpEndPoint);
                        //log(data, 0x20);

                        int offset = 0x36 + _userName.Length - 10;
                        Int16 keyNum = (Int16)(data[offset] * 0x100 + data[offset - 1]);
                        data = get0x21Pack(keyNum);


                        //log(data, 0x21);
                        udp2.Send(data, data.Length);
                        data = udp2.Receive(ref RemoteIpEndPoint);
                        //log(data, 0x22);
                        //string str = data[3].ToString("X");

                        if (data[2] == 0x22)
                        {
                            int informationLength = BitConverter.ToInt32(data, 0x64);
                            ConnectInformation = Encoding.Default.GetString(data, 0x68, informationLength);
                            return data[3] == 0;
                        }
                        
                    }

                    ++time;
                    Thread.Sleep(100);
                }

                //s.Write(data, 0, data.Length);

                //byte[] serverData = new byte[300];
                //s.Read(serverData, 0, serverData.Length);
         
            }
            catch (SocketException)
            {
                udp.Close();
                return false;
            }

            ConnectInformation = "开放IP失败";
            return false;
        }


        public void getInformation()
        {
            byte[] b = udp.Receive(ref ip);

            //log(b, 0x1);


            if (b[2] == 0x1f)
            {
                Flow = System.BitConverter.ToDouble(b, 0x0b);
                Money = System.BitConverter.ToDouble(b, 0x13);
            }

        }


        public void setInformation()
        {
            byte[] head = new byte[] { 0x82, 0x23, 0x1e };

            byte[] data = new byte[500];

            int i;
            for (i = 0; i < head.Length; i++)
            {
                data[i] = head[i];
            }

            data[i++] = (byte)((_key + 0x05dc) % 0x100);
            data[i++] = (byte)((_key + 0x05dc) / 0xff);

            byte[] temp = new byte[] { 00 , 00 , 00 , 00 , 00 , 00 , 00 , 00 , 00 , 00 , 0x2e  ,
                                       0xf5 , 0x27 , 0x41 , 00 , 00 , 00 , 00 , 0xba , 0x2c , 0x22 , 0x41 };

            for (int j = 0; j < temp.Length; j++)
            {
                data[i + j] = temp[j];
            }

            i += temp.Length;

            byte[] userNameByte = Encoding.ASCII.GetBytes(_userName);
            data[i++] = (byte)userNameByte.Length;
            data[i++] = 0;
            data[i++] = 0;
            data[i++] = 0;

            for (int j = 0; j < userNameByte.Length; j++)
            {
                data[i + j] = userNameByte[j];
            }

            i += userNameByte.Length;

            byte[] tail = new byte[] { 01 , 00 , 00 , 00 , 0x62 , 0x0c , 00  ,
                                        00 , 00 , 0xbd , 0xf1 , 0xcc,  0xec , 0xcc , 0xec , 0xc6 , 0xf8 ,
                                       0xd5 , 0xe6 , 0xba , 0xc3 , 0x01 , 00 , 00 , 00 , 0x20 };

            for (int j = 0; j < tail.Length; j++)
            {
                data[i + j] = tail[j];
            }

            i += tail.Length;

            for (; i < 500; i++)
            {
                data[i] = 0xff;
            }

            udp.Send(data, data.Length);
            //log(data, 0x1e);
        }


        private string _userName = "";
        private string _password = "";
        private Int16 _key;
        //private bool messageReceived = false;
        private UdpClient udp;
        private UdpClient udp2;
        //private TcpClient tcp;
        //private Socket server;
        IPEndPoint ip;
        IPEndPoint RemoteIpEndPoint;

    }


}
