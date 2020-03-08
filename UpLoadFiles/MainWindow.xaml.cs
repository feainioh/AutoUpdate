
using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using form = System.Windows.Forms;
using Path = System.IO.Path;

namespace UpLoadFiles
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        MyFunction myFunction = new MyFunction();
        /// <summary>
        /// 程序名称
        /// </summary>
        internal string AppName = "";
        /// <summary>
        /// 上位机服务器Http地址
        /// </summary>
        internal string RemoteHttp_IPAddr = "127.0.0.1";
        /// <summary>
        /// 上位机服务器Http端口号
        /// </summary>
        internal string RemoteHttp_Port = "5906";

        ObservableCollection<FileInfoModel> fileInfoList =new ObservableCollection<FileInfoModel>();
        ObservableCollection<FileInfoModel> FileInfoList
        {
            get { return fileInfoList; }
            set { fileInfoList = value; }
        }

        private Uri APIEndPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        private const string directory = "SoftUpdate";
        public void Upload(string[] files)
        {
            APIEndPoint = new Uri(this.txt_webUri.Text);//获取链接
            var query = string.Format("?count={0}", files.Length); //自定义参数
            var queryUri = new Uri(APIEndPoint, query);//组合
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();//初始化 重点MultipartFormDataContent。。。。。
            foreach (var item in files)//选择的文件
            {
                string filename = Path.GetFileName(item);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                StreamContent streamConent = new StreamContent(new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read));
                multipartFormDataContent.Add(streamConent, filenameWithoutExtension, filename);
            }

            HttpResponseMessage responseMessage = httpClient.PostAsync(queryUri, multipartFormDataContent).Result;//已异步的post请求提交文件
            if (responseMessage.IsSuccessStatusCode)//判断是否成功
            {
                Notice.Show("上传成功！", "消息", 3, Panuon.UI.Silver.MessageBoxIcon.Info);
            }
            else
            {
                Notice.Show("上传失败!", "错误", 4, Panuon.UI.Silver.MessageBoxIcon.Error);
            }
        }
        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            if (txt_webUri.Text != "")
            {
                form.OpenFileDialog _dialog = new form.OpenFileDialog();
                _dialog.Multiselect = true;
                if (_dialog.ShowDialog() == form.DialogResult.OK)
                {
                    string[] _files = _dialog.FileNames;
                    if (_files != null && _files.Length > 0)
                    {
                        foreach(string file in _files)
                        {
                            FileInfoModel model = new FileInfoModel();
                            model.FilePath = file;
                            model.FileName = file.Split('\\')[file.Split('\\').Length-1];
                            FileInfoList.Add(model);
                        }
                        dg_ServerFiles.ItemsSource = FileInfoList;
                    }
                }
            }
        }

        public void TestURI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                APIEndPoint = new Uri(this.txt_webUri.Text);//链接
                HttpClient httpClient = new HttpClient();
                var result = httpClient.GetStringAsync(APIEndPoint);//判断是否可以获取到值
                Notice.Show("连接成功！", "消息", 3, Panuon.UI.Silver.MessageBoxIcon.Info);
            }
            catch (Exception ex)
            {
                Notice.Show("连接失败！"+ex.Message, "消息", 3, Panuon.UI.Silver.MessageBoxIcon.Info);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        /// <summary>
        /// 读取配置文件里的相关信息
        /// </summary>
        private void LoadConfig()
        {
            Dictionary<string, string> dic_conf = myFunction.GetConfigSettings();
            RemoteHttp_IPAddr = dic_conf["WebUri"];
            RemoteHttp_Port = dic_conf["WebPort"];
            AppName = dic_conf["AppName"];
            txt_webUri.Text = string.Format("http://{0}:{1}/api/values",RemoteHttp_IPAddr,RemoteHttp_Port);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FileInfoList != null && FileInfoList.Count > 0)
            {
                List<string> files = new List<string>();
                foreach(var file in FileInfoList)
                {
                    files.Add(file.FilePath);
                }
                //上传
                Upload(files.ToArray());
            }
            else
            {
                Notice.Show("当前未选择需要上传的文件！", "消息", 3, Panuon.UI.Silver.MessageBoxIcon.Info);
            }
        }
    }
}
