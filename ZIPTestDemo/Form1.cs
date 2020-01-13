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
            //CompressZipFile(openFileDialog1.FileName,@"D:\Temp");
            CompressZipFiles(openFileDialog1.FileName,@"D:\Temp\Test01.zip");
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

        /// <summary>
        /// 解压zip文件
        /// </summary>
        /// <param name="SourceFileName">需要解压的压缩包的文件路径</param>
        /// <param name="FileName">解压后保存的文件路径</param>
        /// <returns></returns>
        public bool ArchiveZipFile(string SourceFileName,string FileName)
        {
            try
            {
                var archive = ArchiveFactory.Open(SourceFileName);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(FileName, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 创建zip压缩文件
        /// </summary>
        /// <param name="SourceFileName">需要压缩的文件的路径</param>
        /// <param name="FileName">创建的压缩文件的路径</param>
        /// <returns></returns>
        public bool CompressZipFiles(string SourceFileName, string FileName)
        {
            try
            {
                //设置属性
                WriterOptions options = new WriterOptions(CompressionType.Deflate);
                options.ArchiveEncoding.Default = Encoding.UTF8;
                //指定要压缩的文件夹路径
                using (var zip = File.OpenWrite(FileName))
                using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, options))
                {
                    zipWriter.Write(Path.GetFileName(SourceFileName), SourceFileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
