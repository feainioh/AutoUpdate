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
using Panuon.UI.Silver;
using System.Xml;
using System.Net.NetworkInformation;

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

        /// <summary>
        /// 上位机服务器Http地址
        /// </summary>
        internal string RemoteHttp_IPAddr = "127.0.0.1";
        /// <summary>
        /// 上位机服务器Http端口号
        /// </summary>
        internal string RemoteHttp_Port = "8080";
        /// <summary>
        /// 服务器网址
        /// </summary>
        private string IIS_Url = string.Empty;
        /// <summary>
        /// 当前程序运行的路径，用以拷贝升级程序到此文件夹
        /// </summary>
        private string ApplicationRunPath = string.Empty;
        /// <summary>
        /// 升级完后运行的程序
        /// </summary>
        private string RunExe = string.Empty;
        /// <summary>
        /// 下载升级文件临时存放地
        /// </summary>
        private string UpdateTempFile = string.Empty;
        /// <summary>
        /// 备份当前程序的文件夹
        /// </summary>
        private string BackupFile = string.Empty;
        /// <summary>
        /// 当前程序运行的版本
        /// </summary>
        public int Current_Ver = 1;
        /// <summary>
        /// 已经下载到本机的版本
        /// </summary>
        public int Download_Ver = 1;
        /// <summary>
        /// 服务器上存放的版本(服务器未启用时，使用默认值1)
        /// </summary>
        public int Server_Ver = 1;

        /// <summary>
        /// 是否需要升级，如果有新版本，回复HOST的THU
        /// </summary>
        public bool NeedUpdate
        {
            get
            {
                if (Download_Ver != 1)
                {
                    if (Download_Ver != Current_Ver) return true;
                    else return false;
                }
                else return false;
            }
        }
        /// <summary>
        /// 是否运行检测服务器版本线程
        /// </summary>
        private bool CheckVerRun = false;
        /// <summary>
        /// 当前窗口弹窗的标题
        /// </summary>
        private string MsgTitle = "服务器更新";
        /// <summary>
        /// 异步下载完成
        /// </summary>
        private AutoResetEvent Downloadcompleted = new AutoResetEvent(false);

        /// <summary>
        /// 显示字符串
        /// </summary>
        /// <param name="msg"></param>
        public delegate void dele_ShowStr(string msg);
        public event dele_ShowStr EventShowStr;

        #region XML文档模块
        /// <summary>
        /// 字符串"update"
        /// </summary>
        string _Update = "Update";
        /// <summary>
        /// 字符串"Version"
        /// </summary>
        string _Version = "Version";
        /// <summary>
        ///  字符串"_File"
        /// </summary>
        string _File = "File";
        List<XmlNode> list_XN = new List<XmlNode>();//读取到File下的所有节点值
        #endregion

        MyFunction myFunction = new MyFunction();



        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ////获取服务器信息
            //GetServerConfig();
            ////地址
            //ServerIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectCallback), null);

            LoadConfig();
            this.ApplicationRunPath = AppDomain.CurrentDomain.BaseDirectory;;
            this.CheckVerRun = true;

            this.IIS_Url = string.Format("http://{0}:{1}/SoftUpdate/AutoUpdate.xml", RemoteHttp_IPAddr, RemoteHttp_Port);
            this.UpdateTempFile = "temp";
            this.BackupFile = "backup";

            InitIIS();
        }


        /// <summary>
        /// 读取配置文件里的相关信息
        /// </summary>
        private void LoadConfig()
        {
            Dictionary<string, string> dic_conf = myFunction.GetConfigSettings();
            RemoteHttp_IPAddr = dic_conf["WebUri"];
            RemoteHttp_Port = dic_conf["WebPort"];
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitIIS()
        {
            MyFunction MyFunction = new MyFunction();
            this.Current_Ver = GetVerInt(MyFunction.GetVersion());

            Thread m_CheckVer = new Thread(Thread_CheckVer);
            m_CheckVer.IsBackground = true;
            m_CheckVer.Name = "检查服务器是否有新版本";
            m_CheckVer.Start();
        }

        private void Thread_CheckVer()
        {
            if (ApplicationRunPath == string.Empty)
            {
#if DEBUG
                MessageBox.Show("服务器更新 路径为空");
#endif
                Console.WriteLine("服务器更新 路径为空");
                return;
            }
            int Interval = 60;//时间间隔，单位秒
            Interval *= 10;//分钟
            Interval *= 1000;//毫秒
            while (true)
            {
                try
                {
                    if (!Ping(RemoteHttp_IPAddr))
                    {
                        Thread.Sleep(Interval);
                        continue;
                    }
                    Dictionary<string, string> Path;
                    if (CheckVersion(out Path))
                    {
                        Console.WriteLine(string.Format("{0}\t需要下载", DateTime.Now.ToString("HH:mm:ss:fff")));
                        MyFunction.AppendUpdate_LOG(string.Format("{0}\t需要下载", DateTime.Now.ToString("HH:mm:ss:fff")));
                        //开始下载
                        Download(Path, ApplicationRunPath);
                    }
                    else
                    {
                        MyFunction.AppendUpdate_LOG(string.Format("{0}\t不需要下载", DateTime.Now.ToString("HH:mm:ss:fff")));
                        Console.WriteLine(string.Format("{0}\t不需要下载", DateTime.Now.ToString("HH:mm:ss:fff")));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    MyFunction.AppendUpdate_LOG("升级模块异常：" + ex.Message + "\r\n" + ex.StackTrace);
                }
                finally
                {
                    Thread.Sleep(Interval);
                }
            }
        }

        /// <summary>
        /// ping测试服务器地址
        /// </summary>
        /// <param name="str">服务器IP地址</param>
        /// <returns></returns>
        private bool Ping(string str)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(str, 1000);
                bool reply = false;
                if (pingReply.Status == IPStatus.Success) reply = true;
                Console.WriteLine(string.Format("{2}\t服务器更新 Ping {0}\t{1}", str, reply ? "成功" : "失败", DateTime.Now.ToString("HH:mm:ss:fff")));
                return reply;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{2}\t{1} PING异常:{0}", ex.Message, MsgTitle, DateTime.Now.ToString("HH:mm:ss:fff")));
                return false;
            }
        }


        /// <summary>
        /// 检查版本是否为最新版本
        /// </summary>
        /// <param name="DownloadPath_Name">下载文件路径 文件名</param>
        /// <returns></returns>
        public bool CheckVersion(out Dictionary<string, string> DownloadPath_Name)
        {
            bool NewVersion = false;
            DownloadPath_Name = new Dictionary<string, string>();
            try
            {
                OpXML xml = new OpXML();
                list_XN = OpXML.GetChildNodes(IIS_Url, _Version);
                foreach (XmlNode node in list_XN)
                {
                    this.Server_Ver = GetVerInt(node.Value);
                    Console.WriteLine(string.Format("{0}\t服务器版本:{1}\t已经下载版本:{2}\t当前版本:{3}",
                        DateTime.Now.ToString("HH:mm:ss:fff"),
                        this.Server_Ver,
                        this.Download_Ver,
                        this.Current_Ver));
                    MyFunction.AppendUpdate_LOG(string.Format("{0}\t服务器版本:{1}\t已经下载版本:{2}\t当前版本:{3}",
                        DateTime.Now.ToString("HH:mm:ss:fff"),
                        this.Server_Ver,
                        this.Download_Ver,
                        this.Current_Ver));
                }
                list_XN = OpXML.GetChildNodes(IIS_Url, _Update);
                foreach (XmlNode node in list_XN)
                {
                    if (node.Name != _File) continue;
                    string webpath, filename;
                    if (GetUpdateInf(node, out webpath, out filename)) DownloadPath_Name.Add(webpath, filename);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(string.Format("检查版本:{0}", ex.Message), MsgTitle);
#endif
                Console.WriteLine(string.Format("{1} 检查版本:{0}", ex.Message, MsgTitle));
                MyFunction.AppendUpdate_LOG(string.Format("{1} 检查版本:{0}", ex.Message, MsgTitle));
            }
            //以网络的版本比对当前运行的版本，及下载的版本
            if (this.Server_Ver != this.Current_Ver && this.Server_Ver != this.Download_Ver) NewVersion = true;
            return NewVersion;
        }
        private int GetVerInt(string Str_Ver)
        {
            return Convert.ToInt32(Str_Ver.Replace(".", ""));
        }


        /// <summary>
        /// 判断该节点下的子节点是否符合（下载网址，名称）
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="path">网址</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        private bool GetUpdateInf(XmlNode node, out string path, out string filename)
        {
            path = string.Empty;
            filename = string.Empty;
            if (node.ChildNodes.Count == 2)
            {
                XmlNodeList child = node.ChildNodes;
                XmlNode item1 = child.Item(0);//默认情况，第一个为路径
                XmlNode item2 = child.Item(1);//默认情况，第二个为名称
                if (item1.Name == "Url" && item2.Name == "FileName")
                {
                    path = item1.InnerXml;
                    filename = item2.InnerXml;
                    return true;
                }
                else if (item2.Name == "Url" && item1.Name == "FileName")
                {
                    path = item2.InnerXml;
                    filename = item1.InnerXml;
                    return true;
                }
                else return false;
            }
            else return false;
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="WebUrlPath_Name">下载文件路径 文件名</param>
        /// <param name="Destination">保存文件夹名称</param>
        /// <returns></returns>
        public void Download(Dictionary<string, string> WebUrlPath_Name, string Destination)
        {
            bool isSuccess = true;//是否下载成功

            if (Directory.Exists(UpdateTempFile))
            {
                Directory.Delete(UpdateTempFile, true);
            }
            Directory.CreateDirectory(UpdateTempFile);

            List<string> backFile = new List<string>();
            foreach (KeyValuePair<string, string> sub in WebUrlPath_Name)
            {
                backFile.Add(sub.Value);
                if (!Download(sub.Key, sub.Value))
                {
                    ShowStr(sub.Value + "\t下载失败");
                    isSuccess = false;
                }
                else ShowStr(sub.Value + "\t下载成功");
            }
            if (isSuccess)
            {
                ShowStr("下载成功，开始备份");
                AutoResetEvent BackupFile = new AutoResetEvent(false);
                Thread Thd_Back = new Thread(new ThreadStart(delegate
                {
                    BackupFiles(backFile);
                    BackupFile.Set();
                }));
                Thd_Back.IsBackground = true;
                Thd_Back.Start();

                if (!BackupFile.WaitOne(10 * 1000))
                {
                    ShowStr("备份文件超时");
                    Thread.Sleep(2000);
                }

                this.Download_Ver = this.Server_Ver;
                if (MessageBox.Show("程序有新版本,是否升级？\r\n", "升级", MessageBoxButton.OKCancel) == MessageBoxResult.OK) PCUpdate();
                else Thread.Sleep(Timeout.Infinite);
            }
        }

        /// <summary>
        /// 备份文件(只保留近十个文件)
        /// </summary>
        /// <param name="FileName">需要备份的文件</param>
        private void BackupFiles(List<string> FileName)
        {
            if (!Directory.Exists(BackupFile))
            {
                Directory.CreateDirectory(BackupFile);
            }

            Dictionary<string, DateTime> File_CreateTime = new Dictionary<string, DateTime>();
            string[] dirs = Directory.GetDirectories(BackupFile);//获取子文件夹
            foreach (string dir in dirs)
            {
                File_CreateTime.Add(dir, File.GetCreationTime(dir));
            }
            var dicSort = from objDic in File_CreateTime orderby objDic.Value descending select objDic;//Dictonary排序（降序） 如果需要升序  descending 去掉即可
            int FilesCount = 10;
            foreach (KeyValuePair<string, DateTime> kvp in dicSort)
            {
                if (--FilesCount < 0) Directory.Delete(kvp.Key, true);
            }

            MyFunction MyFunction = new MyFunction();
            string savepath = MyFunction.GetVersion();
            string createfloder = CreateFolder(BackupFile + @"\", savepath);
            while (FileName.Count > 0)
            {
                try
                {
                    string file = FileName[0];
                    if (!File.Exists(file))
                    {
                        FileName.RemoveAt(0);
                        continue;
                    }
                    File.Copy(file, createfloder + @"\" + file);
                    FileName.RemoveAt(0);
                }
                catch (Exception ex)
                { }
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path">创建的文件夹的路径(包含\)</param>
        /// <param name="FolderName">希望文件夹名称</param>
        /// <returns>实际创建的文件夹名称</returns>
        private string CreateFolder(string path, string FolderName)
        {
            string foldername = path + FolderName;
            string[] dirs = Directory.GetDirectories(path);//获取子文件夹
            List<string> folder = new List<string>();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].IndexOf(FolderName) > 0) folder.Add(dirs[i]);
            }
            int BigNum = 0;
            while (folder.Count > 0)
            {
                try
                {
                    int num = 0;
                    if (folder[0].IndexOf("_") > 0) num = Convert.ToInt16(folder[0].Substring(folder[0].IndexOf("_") + 1));
                    BigNum = num > BigNum ? num : BigNum;
                    folder.RemoveAt(0);
                }
                catch (Exception ex) { }
            }
            if (BigNum == 0 && !Directory.Exists(path + FolderName)) Directory.CreateDirectory(path + FolderName);
            else foldername = CreateFolder(path, FolderName + "_" + (++BigNum));

            return foldername;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="WebUrl">远程路径</param>
        /// <param name="SaveName">保存的名称</param>
        /// <returns></returns>
        private bool Download(string WebUrl, string SaveName)
        {
            WebClient wc = new WebClient();
            try
            {
                ShowStr(string.Format("下载开始:{0}\t文件大小:{1}", SaveName, GetLength(WebUrl)));
                txt_FileName.Text = SaveName;
                txt_FileAmt.Text = SizeToString( GetLength(WebUrl));
                //调试
                if (!Directory.Exists(UpdateTempFile))
                {
                    return false;
                }

                string DownFileName = UpdateTempFile + @"\" + SaveName.TrimStart('\\');//下载到本地的文件
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(DownFileName))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DownFileName));

                #region 同步下载
                wc.DownloadFile(WebUrl, DownFileName);//同步下载
                Console.WriteLine(string.Format("{0}\t{1} 下载结束", DateTime.Now.ToString("HH:mm:ss:fff"), SaveName));
                wc.Dispose();
                #endregion

                #region 异步下载
                //wc.DownloadFileAsync(new Uri(WebUrl), file + @"\" + savePath);//异步下载
                //wc.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                //wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                //Downloadcompleted.WaitOne();//等待异步下载完成
                #endregion
                return true;
            }
            catch (Exception ex)
            {

                MyFunction.AppendUpdate_LOG(string.Format("下载异常：{0}", ex.Message));
                wc.Dispose();
                return false;
            }
        }

        private long GetLength(string uri)
        {
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
            long len = res.ContentLength;
            res.Close();
            return len;
        }
        private void ShowStr(string str)
        {
            Console.WriteLine("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss"), str);
            txt_CurrentFile.Text = string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss"), str);
            if (EventShowStr != null) EventShowStr(str);
        }


        /// <summary>
        /// PC软件升级方法
        /// </summary>
        public void PCUpdate()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            try
            {
                string UpdateProgram = "UpdateTool.exe";
                if (!File.Exists(UpdateProgram))
                {
                    ShowStr("升级程序丢失");
                    return;
                }
                string tempcopyname = "%($)%.exe";//临时复制的文件，不需要复制此文件
                File.Copy(UpdateProgram, UpdateTempFile + @"\" + tempcopyname);

                string filePath = this.ApplicationRunPath + @"\" + UpdateTempFile + @"\" + tempcopyname;
                startInfo.FileName = filePath;
                //参数0为源文件夹，参数1为目标文件夹，参数2为复制完毕后运行程序
                string parameter = string.Format("{0} {1} {2}"
                                    , this.ApplicationRunPath + @"\" + UpdateTempFile
                                    , this.ApplicationRunPath
                                    , RunExe);
                startInfo.Arguments = parameter;
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message, "升级故障");
#endif
            }
            finally
            {
                Environment.Exit(0);
            }
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
