
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private const string directory = "SoftUpdate";
      public void Upload(string file)
      {
          FileInfo info = new FileInfo(file);
          string url = string.Format("http://192.168.31.118:54040/UploadFile.aspx?d={0}&n={1}", directory, info.Name);
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
        }
    }
}
