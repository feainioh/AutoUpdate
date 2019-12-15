
using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        internal string RemoteHttp_Port = "8080";


        public MainWindow()
        {
            InitializeComponent();
        }

        private const string directory = "SoftUpdate";
      public void Upload(string file)
      {
          FileInfo info = new FileInfo(file);
          string url = string.Format("http://{0}:{1}/UploadFile.aspx?d={2}&n={3}", RemoteHttp_IPAddr, RemoteHttp_Port, directory, info.Name);
            url = string.Format("http://{0}/",txt_webUri.Text.Trim());

            WebClient client = new WebClient();
          client.Credentials = CredentialCache.DefaultCredentials; 
          client.UploadFileAsync(new Uri(url), file);
          client.UploadFileCompleted += new UploadFileCompletedEventHandler(result_UploadFileCompleted);
      }
      private void result_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
      {
          if (e.Error != null)
          {
              Notice.Show("上传失败：" + e.Error.Message,"错误",4,Panuon.UI.Silver.MessageBoxIcon.Error);
          }
          else
          {
               Notice.Show("上传成功！","消息",3,Panuon.UI.Silver.MessageBoxIcon.Info);
          }
       }

       private void UploadFile_Click(object sender, RoutedEventArgs e)
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
                        foreach (var item in _files)
                        {
                            Upload(item);
                        }
                    }
                }
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
        }
    }
}
