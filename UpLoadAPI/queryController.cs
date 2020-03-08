/**************************************************************************
*   
*   =================================
*   CLR版本     ：4.0.30319.42000
*   命名空间    ：UpLoadAPI
*   文件名称    ：queryController.cs
*   =================================
*   创 建 者    ：LQZ
*   创建日期    ：2020-1-15 10:21:18 
*   功能描述    ：
*   =================================
*   修 改 者    ：
*   修改日期    ：
*   修改内容    ：
*   =================================
*  
***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace UpLoadAPI
{
    public class queryController : ApiController
    {
        //get api
        public string Get(string id)
        {
            return id;
        }
        // POST api
        public string Post([FromBody] string value)
        {
            return value;
        }
        // PUT api
        public void Put(int id, string value)
        {
        }
        // DELETE api
        public void Delete(int id)
        {
        }
    }

    public class UpdateController : ApiController
    {
        Person[] personList = new Person[] {
            new Person { Id= 1,Age = 2,Name="DANNY0"},
            new Person { Id = 2,Age = 3,Name = "Danny123"},
            new Person { Id =3,Age = 4,Name = "dANNY456"}
        };

        [HttpGet]
        [Route("api/person/getAll")]
        public List<Person> GetListAll()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GetPersonListAll");
            return personList.ToList();
        }

        [HttpGet]
        [Route("api/Files/GetApp")]
        public List<Person> GetApps()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GetPersonListAll");
            return personList.ToList();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="SaveAdd">上传文件保存的目录</param>
        /// <returns></returns>
        [HttpPost]
        [Route ("api/Files/UpLoadFile")]
        public async Task<ResultObj> UploadFile(string SaveAdd)
        {
            ResultObj resultObj = new ResultObj()
            {
                Success = false
            };

            var provider = new MultipartMemoryStreamProvider();

            //读取文件数据
            await Request.Content.ReadAsMultipartAsync(provider);

            //检查是否存在目录  
            if (!Directory.Exists(SaveAdd)) Directory.CreateDirectory(SaveAdd);

            if (provider.Contents.Count == 0)
            {
                resultObj.Msg = "没有文件";
            }
            else
            {
                var item = provider.Contents[0];
                // 判断是否是文件
                if (item.Headers.ContentDisposition.FileName != null)
                {
                    //获取到流
                    var ms = item.ReadAsStreamAsync().Result;
                    //进行流操作
                    using (var br = new BinaryReader(ms))
                    {
                        if (ms.Length <= 0)
                        {
                            resultObj.Msg = "文件长度为空";
                        }
                        //读取文件内容到内存中
                        byte[] data = br.ReadBytes((int)ms.Length);
                        //data就是取出的文件流啦
                        


                    }
                }
                else
                {
                    resultObj.Msg = "未知的上传内容";
                }
            }

            return resultObj;
        }
    }
}
