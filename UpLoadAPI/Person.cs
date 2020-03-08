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
namespace UpLoadAPI
{
    public class Person
    {
        #region 属性
        private int _id;
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name = string.Empty;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _age;

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        #endregion
    }
}