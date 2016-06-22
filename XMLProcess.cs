using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
namespace Quartz
{
    public class XMLProcess
    {
        public XMLProcess()
        { }

        public XMLProcess(string strpath)
        {
            this._xmlpath = strpath;
        }

        private string _xmlpath;
        public string XMLPath
        {
            get { return this._xmlpath; }
        }

        /// <summary>
        /// 导入XML文件
        /// </summary>
        /// <param name="XMLPath">XML文件路径</param>
        private XmlDocument XMLLoad()
        {
            string XMLFile = XMLPath;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + XMLFile;
                if (File.Exists(filename)) xmldoc.Load(filename);
            }
            catch (Exception e)
            { }
            return xmldoc;
        }
        public static string ReadXml(string path)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(path);
            return xd.InnerXml;
        }

        /// <summary>
        /// 获取指定文件夹下的xml文件物理地址
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>物理地址数组</returns>
        public static string[] GetFiles()
        {
            try
            {
                string pathname = JobFactory.Instance.GetPath();
                if (Directory.Exists(pathname))
                    return Directory.GetFiles(pathname, "*.xml");
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 把xml字符串转为xml文件
        /// </summary>
        /// <param name="strxml">xml字符串</param>
        /// <param name="filename">文件名称xml格式</param>
        ///<param name="path">保存的物理地址【默认：\bin\..\..\jobs\】</param>
        public static void Write(string strxml, string filefullname)
        {
            try
            {

                var path = JobFactory.Instance.GetPath();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //if (File.Exists(pathname + filename) && filename.ToLower().EndsWith(".xml")) return;

                using (StreamWriter sw = new StreamWriter(filefullname, false))
                {
                    sw.Write(strxml);
                    sw.Close();
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filefullpath"></param>
        public static void Delete(string filefullpath)
        {
            System.IO.File.Delete(filefullpath);
        }
        /// <summary> 
        /// 反序列化 
        /// </summary> 
        /// <param name="type">类型</param> 
        /// <param name="xml">XML字符串</param> 
        /// <returns></returns> 
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary> 
        /// 反序列化 
        /// </summary> 
        /// <param name="type">类型</param> 
        /// <param name="xml">XML字符串</param> 
        /// <returns></returns> 
        public static T Deserialize<T>(string xml) where T : class,new()
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return xmldes.Deserialize(sr) as T;
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            string XMLSTR = string.Empty;
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
                XMLSTR = Encoding.UTF8.GetString(Stream.ToArray());
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Dispose();
            return XMLSTR;
        }

    }
}