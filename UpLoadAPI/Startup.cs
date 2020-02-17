/**************************************************************************
*   
*   =================================
*   CLR版本     ：4.0.30319.42000
*   命名空间    ：UpLoadAPI
*   文件名称    ：Startup.cs
*   =================================
*   创 建 者    ：LQZ
*   创建日期    ：2020-1-15 10:18:27 
*   功能描述    ：
*   =================================
*   修 改 者    ：
*   修改日期    ：
*   修改内容    ：
*   =================================
*  
***************************************************************************/
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace UpLoadAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            //创建Web API 的配置
            var config = new HttpConfiguration();
            //启动标记路由
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}"
            );
            //将路有配置附加到appBuilder
            appBuilder.UseWebApi(config);
        }
    }
}
