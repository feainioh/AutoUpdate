using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZIPTestDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        //压缩文件
        private void button2_Click(object sender, EventArgs e)
        {
            CompressZipFile(openFileDialog1.FileName,@"D:\Temp");
        }

        public void CompressZipFile(string SourceFileName, string FileName)
        {
            WriterOptions options = new WriterOptions(CompressionType.Deflate);
            options.ArchiveEncoding.Default = Encoding.UTF8;
            //指定要压缩的文件夹路径
            using (var zip = File.OpenWrite(FileName+"\\test.zip"))
            using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, options))
            {
                    zipWriter.Write(Path.GetFileName(SourceFileName), SourceFileName);
                
            }
        }

        public bool CompressZipFiles(string SourceFileName,string FileName)
        {
            try
            {
                //设置属性
                WriterOptions options = new WriterOptions(CompressionType.Deflate);
                options.ArchiveEncoding.Default = Encoding.UTF8;
                //指定要压缩的文件夹路径
                using (var zip = File.OpenWrite(FileName + "\\test.zip"))
                using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, options))
                {
                    zipWriter.Write(Path.GetFileName(SourceFileName), SourceFileName);

                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var archive = ArchiveFactory.Open(openFileDialog1.FileName);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(@"D:\Temp", new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
        }
    }
}
