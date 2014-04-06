using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Zuken.Common
{
    /// <summary>
    /// 文档的整个属性
    /// </summary>
    [XmlRoot("DocumentProperty")]
    [Serializable]
    public class DocumentProperty : List<PartProperty>, IXmlSerializable, ISerializable
    {
        XmlSerializer dataSerializer = new XmlSerializer(typeof(DocumentProperty));
        public DocumentProperty() { }
        public DocumentProperty(SerializationInfo info, StreamingContext context)
        {
            var enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Add((PartProperty)enumerator.Value);
            }
        }
        public DocumentProperty(IList<Dictionary<string,string>> list) 
        {
            foreach (var item in list)
            {
                this.Add(new PartProperty(item));
            }
        }

        #region IXmlSerializable 成员

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                var part = new PartProperty();
                var SubReader = reader.ReadSubtree();
                SubReader.Read();
                part.ReadXml(SubReader);
                Add(part);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this)
            {
                writer.WriteStartElement("PartProperty");
                key.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var i = 0;
            foreach (var item in this)
            {
                info.AddValue("__PartProperty" + (i++).ToString(), item, typeof(PartProperty));
            }
        }

        #endregion

        /// <summary>
        /// 序列化为xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            StringBuilder sb = new StringBuilder();

            using (System.IO.StringWriter Stringwriter = new System.IO.StringWriter(sb))
            {
                dataSerializer.Serialize(Stringwriter, this);
                return sb.ToString();
            }
        }
        /// <summary>
        /// 将xml 序列化为 数据
        /// </summary>
        /// <param name="xml"></param>
        public void ReadXml(string xml)
        {
            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                foreach (var item in (DocumentProperty)dataSerializer.Deserialize(sr))
                {
                    Add(item);
                }
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void SaveXml(string filePath)
        {
            File.WriteAllText(filePath, ToXml(), Encoding.UTF8);
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DocumentProperty LoadXml(string filePath)
        {
            var docprop = new DocumentProperty();
            docprop.ReadXml(File.ReadAllText(filePath, Encoding.UTF8));
            return docprop;
        }
        /// <summary>
        /// 写二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveFile(string filePath)
        {
            using (MemoryStream msdoc = new MemoryStream())
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(msdoc, this);
                msdoc.Position = 0;
                byte[] byties = new byte[msdoc.Length];
                msdoc.Read(byties, 0, (int)msdoc.Length);
                File.WriteAllBytes(filePath, byties);
            }
        }
        /// <summary>
        /// 读取二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DocumentProperty LoadFile(string filePath)
        {
            byte[] byties = File.ReadAllBytes(filePath);
            var docprop = new DocumentProperty();
            using (MemoryStream msdoc = new MemoryStream())
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                msdoc.Write(byties, 0, byties.Length);
                msdoc.Position = 0;
                foreach (var item in (DocumentProperty)bFormatter.Deserialize(msdoc))
                {
                    docprop.Add(item);
                }
            }
            return docprop;
        }

        /// <summary>
        /// 转换为List
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> ToList()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>(Count);
            foreach (var item in this)
            {
                result.Add(item.ToDictionary());
            }
            return result;
        }

        public ArrayList ConvertToBOM()
        {
            ArrayList arr = new ArrayList(Count);
            foreach (var item in this)
            {
                arr.Add(item.BOMData);
            }
            return arr;
        }
    }
}
