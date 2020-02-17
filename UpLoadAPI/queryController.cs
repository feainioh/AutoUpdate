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
using System.Linq;
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

    public class PersonController : ApiController
    {
        Person[] personList = new Person[] {
            new Person { Id= 1,Age = 2,Name="DANNY"},
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
        [Route("api/person/Get")]
        public List<Person> Get(string id)
        {
            return personList.ToList();
        }
    }
}
