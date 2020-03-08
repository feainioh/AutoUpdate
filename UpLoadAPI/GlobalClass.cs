using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpLoadAPI
{
    /// <summary>
    /// 调用返回结果类
    /// </summary>
    public class ResultObj
    {
        private bool success;
        /// <summary>
        /// 调用结果
        /// </summary>
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }


        private string msg;
        /// <summary>
        /// 调用消息
        /// </summary>
        public string Msg
        {
            get { return msg; }
            set { msg = value; }
        }

    }



    public class LogHelper
    {
        /// <summary>
        /// 更新日志
        /// </summary>
        /// <param name="Err"></param>
        internal static void AppendUpdate_LOG(string Err)
        {
            try
            {
                string filename = DateTime.Now.ToString("yyyyMMddHH");
                string dirName = AppDomain.CurrentDomain.BaseDirectory + "\\LOG\\Update\\";
                if (!Directory.Exists(dirName))
                { Directory.CreateDirectory(dirName); }

                string _logfile = dirName + filename + ".txt";
                FileStream FS = new FileStream(_logfile, FileMode.Append);
                string str_record = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t\t" + Err;
                StreamWriter SW = new StreamWriter(FS);
                SW.WriteLine(str_record);
                SW.Close(); 
                SW.Dispose();
            }
            catch { }
        }
    }
}
