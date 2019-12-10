using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量
        //服务器地址
        private string serverIP = string.Empty;
        //服务器端口号
        private int port = 80;
        //软件名称
        private string softName;

        public IPEndPoint ServerIpEndPoint { get; set; }
        public bool IsSystemDownLoad { get;  set; }
        public bool CanWindowClose { get;  set; }
        public string FilePath { get;  set; }


        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //获取服务器信息
            GetServerConfig();
            //地址
            ServerIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectCallback), null);
        }

        /// <summary>
        /// 下载方法
        /// </summary>
        /// <param name="state"></param>
        private void ConnectCallback(object state)
        {
            Thread.Sleep(400);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ServerIpEndPoint);
            }
            catch
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("连接服务器失败，稍后重试。");
                }));
                return;
            }

            try
            {
                //控制版本号的文件最后一个写入，来确保所有文件更新成功
                string ExecuteFileName = string.Empty;
                byte[] ExecuteFileContent = null;

                int Command = 0x1002;

                if (IsSystemDownLoad)
                {
                    Command = 0x1001;
                }

                socket.Send(BitConverter.GetBytes(Command));


                CanWindowClose = false;
                int _ReceiveLenght = 4;
                int ReceivedLenght = 0;
                byte[] ReceiveByte = new byte[_ReceiveLenght];
                while (ReceivedLenght < _ReceiveLenght)
                {
                    int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                    ReceivedLenght += temp;
                }

                //接收的文件个数
                int FileCount = BitConverter.ToInt32(ReceiveByte, 0);

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    txt_FileCount.Text = FileCount.ToString("00");
                }));

                int ReceivedFiles = 0;

                for (int i = 0; i < FileCount; i++)
                {
                    _ReceiveLenght = 8;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];

                    //分三次接收数据
                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                        ReceivedLenght += temp;
                    }
                    int FileNameLenght = BitConverter.ToInt32(ReceiveByte, 0) - 8;
                    int FileLenght = BitConverter.ToInt32(ReceiveByte, 4);

                    _ReceiveLenght = FileNameLenght;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];


                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                        ReceivedLenght += temp;
                    }
                    string fileName = Encoding.Unicode.GetString(ReceiveByte);

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        txt_FileName.Text = StringShort(fileName);
                        txt_FileAmt.Text = SizeToString(FileLenght);
                        pb_Total.Value = 100 * (ReceivedFiles + 1) / FileCount;
                        pb_Current.Value = 0;
                        pb_Current.Maximum = FileLenght;
                        txt_CurrentFile.Text = (ReceivedFiles + 1).ToString("00");
                    }));

                    Thread.Sleep(100);
                    _ReceiveLenght = FileLenght;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];


                    //文件选择分割成4K数据进行接收
                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int 本次接收 = (_ReceiveLenght - ReceivedLenght) > 4096 ? 4096 : _ReceiveLenght - ReceivedLenght;
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, 本次接收, SocketFlags.None);
                        ReceivedLenght += temp;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            pb_Current.Value = ReceivedLenght;
                        }));
                    }
                    //数据保存

                    string FullName = FilePath + @"\" + fileName;

                    if (FullName.Contains(softName + ".exe") && string.IsNullOrEmpty(ExecuteFileName))
                    {
                        ExecuteFileName = FullName;
                        ExecuteFileContent = ReceiveByte;
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllBytes(FullName, ReceiveByte);
                        }
                        catch
                        {
                            //事实上在此处如果出现异常，将会导致应用程序出现无法预知的失败
                            throw;
                        }
                    }

                    Thread.Sleep(100);

                    ReceivedFiles++;
                }


                if (!string.IsNullOrEmpty(ExecuteFileName) && ExecuteFileContent != null)
                {
                    try
                    {
                        File.WriteAllBytes(ExecuteFileName, ExecuteFileContent);
                    }
                    catch
                    {
                        //此处异常没有影响，重新运行程序时，将重新更新
                        throw new Exception("更新软件时出现了异常，请稍后重试！");
                    }
                }


                socket.Send(BitConverter.GetBytes(1));
                Thread.Sleep(20);
                socket.Close();

                Thread.Sleep(500);


                Thread.Sleep(500);

                if (IsSystemDownLoad)
                {
                    string temp = @"ECHO OFF
ECHO Set WshShell = Wscript.CreateObject(""Wscript.Shell"") >%temp%\tmp.vbs
CMD /c ""ECHO ^Set MyLink = WshShell.CreateShortcut(""" +
Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\" + softName + @".lnk"")"" >>%temp%\tmp.vbs""
ECHO MyLink.TargetPath = """ + FilePath + @"\" + softName + @".exe"" >>%temp%\tmp.vbs
ECHO MyLink.Save >>%temp%\tmp.vbs
cscript /nologo %temp%\tmp.vbs
DEL /q /s %temp%\tmp.vbs 2>nul 1>nul";
                    string FullName = FilePath + @"\生成快捷方式.cmd";
                    try
                    {
                        File.WriteAllBytes(FullName, Encoding.Default.GetBytes(temp));
                        //启动生成快捷方式
                        System.Diagnostics.Process.Start(FullName);
                    }
                    catch
                    {

                    }
                }

                //启动指定的应用程序
                System.Diagnostics.Process.Start(FilePath + @"\" + softName + ".exe");
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CanWindowClose = true;
                    Close();
                }));
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }));
            }
            finally
            {
                CanWindowClose = true;
            }
        }

        private void GetServerConfig()
        {
            serverIP = ConfigurationManager.AppSettings.Get("IP");
            port = int.Parse(ConfigurationManager.AppSettings.Get("Port"));
            softName = ConfigurationManager.AppSettings.Get("SoftName");
            FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\"+softName;
        }

        private string SizeToString(long size)
        {
            if (size < 1024)
            {
                return size + " B";
            }
            else if (size < 1024 * 1024)
            {
                return ((float)size / 1024).ToString("F1") + " K";
            }
            else
            {
                return ((float)size / (1024 * 1024)).ToString("F1") + " M";
            }
        }
        private string StringShort(string str)
        {
            if (str.Length > 30)
            {
                return str.Substring(0, 20) + "...";
            }
            else
            {
                return str;
            }
        }
    }
}
