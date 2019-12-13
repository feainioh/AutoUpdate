
using Microsoft.Win32;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<FileInfoModel> _fileList = new ObservableCollection<FileInfoModel>();

        public ObservableCollection<FileInfoModel> FileInfoList
        {
            get { return _fileList; }
            set { _fileList = value; }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private const string directory = "0123";
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        public void Upload(string file)
        {
            FileInfo info = new FileInfo(file);
            string url = txt_webUri.Text.Trim();
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            client.UploadFileAsync(new Uri(url), file);
            client.UploadFileCompleted += new UploadFileCompletedEventHandler(result_UploadFileCompleted);
        }
        private void result_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Notice.Show("上传失败：" + e.Error.Message, "错误", 4, Panuon.UI.Silver.MessageBoxIcon.Error);
            }
            else
            {
                Notice.Show("上传成功！", "消息", 3, Panuon.UI.Silver.MessageBoxIcon.Info);
            }
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (IsWebUri(txt_webUri.Text))
            {
                form.OpenFileDialog _dialog = new form.OpenFileDialog();
                _dialog.Multiselect = true;
                if (_dialog.ShowDialog() == form.DialogResult.OK)
                {
                    FileInfoList.Clear();
                    string[] _files = _dialog.FileNames;
                    if (_files != null && _files.Length > 0)
                    {
                        foreach (var item in _files)
                        {
                            FileInfo info = new FileInfo(item);
                            FileInfoModel model = new FileInfoModel();
                            model.FileName = info.Name;
                            model.FilePath = info.FullName;
                            FileInfoList.Add(model);
                            Upload(item);
                        }
                        dg_ServerFiles.ItemsSource = FileInfoList;
                    }
                }
            }
            else
            {
                Notice.Show("请输入正确的网络地址！", "警告", 4, Panuon.UI.Silver.MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 判读地址是否是网址
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsWebUri(string text)
        {
            if (text.Length < 8) return false;
            if ((text.Substring(0, 7) == "http://") || (text.Substring(0, 8) == "https://"))
                return true;
            return false;
        }


    }

    public class FileInfoModel : PropertyChangedBase
    {
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

    }
}
