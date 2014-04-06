using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Zuken.Common
{
    /// <summary>
    /// 元器件的属性
    /// </summary>
    [XmlRoot("PartProperty")]
    [Serializable]
    public class PartProperty : IDictionary<string, string>, IXmlSerializable, ISerializable
    {
        Dictionary<string, string> data;
        List<Dictionary<string, string>> mChildRelation = new List<Dictionary<string, string>>(10);
        List<Dictionary<string, string>> mObjectRelation = new List<Dictionary<string, string>>(10);
        XmlSerializer keySerializer = new XmlSerializer(typeof(string));
        XmlSerializer dataSerializer = new XmlSerializer(typeof(PartProperty));
        /// <summary>
        /// 父子关系属性
        /// </summary>
        const string __ChildRelation = "__ChildRelation";
        /// <summary>
        /// 相关关系属性
        /// </summary>
        const string __ObjectRelation = "__ObjectRelation";

        #region 构造函数
        public PartProperty()
        {
            data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public PartProperty(int capacity)
        {
            data = new Dictionary<string, string>(capacity, StringComparer.OrdinalIgnoreCase);
        }
        public PartProperty(IDictionary<string, string> dic)
        {
            data = new Dictionary<string, string>(dic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public PartProperty(SerializationInfo info, StreamingContext context)
            : this()
        {
            var enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Name == __ChildRelation)
                {
                    mChildRelation = ((DocumentProperty)enumerator.Value).ToList();
                }
                else if (enumerator.Name == __ObjectRelation)
                {
                    mObjectRelation = ((DocumentProperty)enumerator.Value).ToList();
                }
                else
                {
                    Add(enumerator.Name, enumerator.Value.ToString());
                }

            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 父子关系
        /// </summary>
        public List<Dictionary<string, string>> ChildRelation
        {
            get { return mChildRelation; }
        }
        /// <summary>
        /// 相关关系
        /// </summary>
        public List<Dictionary<string, string>> ObjectRelation
        {
            get { return mObjectRelation; }
        }
        /// <summary>
        /// 转换问为BOM数据
        /// </summary>
        public Dictionary<string, object> BOMData
        {
            get
            {
                var bom = new Dictionary<string, object>(Count + 2);
                foreach (var item in Keys)
                {
                    bom[item] = this[item];
                }
                if (ChildRelation != null && ChildRelation.Count > 0)
                    bom["child"] = ConvertToDicObj(ChildRelation);
                if (ObjectRelation != null && ObjectRelation.Count > 0)
                    bom["relationobject"] = ConvertToDicObj(ObjectRelation);
                return bom;
            }
        }

        /// <summary>
        /// 将数据转换为字典
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        System.Collections.ArrayList ConvertToDicObj(List<Dictionary<string,string>> dic)
        {
            System.Collections.ArrayList result = new System.Collections.ArrayList(dic.Count);
            Dictionary<string, object> i = null;
            foreach (var item in dic)
            {
                i = new Dictionary<string, object>(item.Count);
                foreach (var ci in item)
                {
                    i.Add(ci.Key, ci.Value);
                }

                result.Add(i);
            }
            return result;
        }
        #endregion
        
        #region IDictionary<string,string> 成员

        public void Add(string key, string value)
        {
            data.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return data.Keys; }
        }

        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return data.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return data.Values; }
        }

        public string this[string key]
        {
            get
            {
                var val = string.Empty;
                if (!data.TryGetValue(key, out val))
                {
                    val = string.Empty;
                }
                return val;
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> 成员

        public void Add(KeyValuePair<string, string> item)
        {
            data.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            var val = string.Empty;
            if (data.TryGetValue(item.Key, out val))
            {
                return val == item.Value;
            }
            return false;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            foreach (var item in data)
            {
                array[arrayIndex++] = item;
            }
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            if (Contains(item))
            {
                return data.Remove(item.Key);
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> 成员

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IXmlSerializable 成员

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("item"))
                {
                    ReadProp(reader);
                }
                else if (reader.IsStartElement(__ChildRelation))
                {
                    ReadSubProp(reader, p => mChildRelation = p.ToList());
                }
                else if (reader.IsStartElement(__ObjectRelation))
                {
                    ReadSubProp(reader, p => mObjectRelation = p.ToList());
                }
            }
            reader.ReadEndElement();
        }
        /// <summary>
        /// 读取子属性
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="func"></param>
        private void ReadSubProp(System.Xml.XmlReader reader, Action<DocumentProperty> func)
        {
            var part = new DocumentProperty();
            var SubReader = reader.ReadSubtree();
            SubReader.Read();
            part.ReadXml(SubReader);
            func(part);
            reader.ReadEndElement();
            reader.MoveToContent();
        }
        private void ReadProp(XmlReader reader)
        {
            reader.ReadStartElement("item");

            reader.ReadStartElement("key");
            var key = (string)keySerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            var value = (string)keySerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadEndElement();
            reader.MoveToContent();
            this.Add(key, value);
        }
        /// <summary>
        /// 写子属性
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="key"></param>
        /// <param name="prop"></param>
        private void WriteSubProp(XmlWriter writer, string key, List<Dictionary<string, string>> prop)
        {
            if (prop != null && prop.Count > 0)
            {
                writer.WriteStartElement(key);
                new DocumentProperty(prop).WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        private void WriteSubProp(SerializationInfo info, StreamingContext context, string key, List<Dictionary<string, string>> prop)
        {
            if (prop != null && prop.Count > 0)
            {
                info.AddValue(key, new DocumentProperty(prop), typeof(DocumentProperty));
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                keySerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            WriteSubProp(writer, __ChildRelation, ChildRelation);
            WriteSubProp(writer, __ObjectRelation, ObjectRelation);
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in Keys)
            {
                info.AddValue(item, this[item]);
            }
            WriteSubProp(info, context, __ChildRelation, ChildRelation);
            WriteSubProp(info, context, __ObjectRelation, ObjectRelation);
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
                data = ((PartProperty)dataSerializer.Deserialize(sr)).data;
            }
        }

        /// <summary>
        /// 转换为字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            return data;
        }
    }
}
