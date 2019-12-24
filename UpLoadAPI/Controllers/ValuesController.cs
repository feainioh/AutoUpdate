using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UpLoadAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(int count) //接收参数
        {
            try
            {
                var time = DateTime.Now;
                string[] arr = new string[] { time.Year.ToString(), time.Month.ToString(), time.Day.ToString() };
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/TempFiles/"), Path.Combine(arr));//在File文件夹下根据时间组成路径
                var save_Path = Path.Combine(HttpContext.Current.Server.MapPath("~/SoftUpdate/"));//文件最终保存位置
                if (!Directory.Exists(path))//判断路径是否存在
                {
                    Directory.CreateDirectory(path);
                }

                var provider = new MultipartFormDataStreamProvider(path);
                await Request.Content.ReadAsMultipartAsync(provider);//获取

                foreach (MultipartFileData file in provider.FileData) //保存
                {
                    var guid = Guid.NewGuid().ToString("N");
                    var name = file.Headers.ContentDisposition.FileName.Replace("\"", "");//获取后缀
                    var suffix = (name.Split('.'))[1];
                    var newFileName = guid + Path.GetExtension(name);
                    var newpath = Path.Combine(path, newFileName);//组成新的路径
                    File.Move(file.LocalFileName, newpath); //这个地方采用File.Move 移动到指定位子赋值指定名称(原因可以调式~~~~~)
                }
                //更新文件的文件夹爱
                DirectoryInfo saveDir = new DirectoryInfo(save_Path); 
                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误:ex=" + ex.ToString());
                return InternalServerError(ex);
            }
        }
    }
}
