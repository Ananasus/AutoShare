using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace AutoShare.Engine.IO
{

    [Serializable()]
    public class SerializableList<T>:ISerializable
    {
        List<T> lst;
        public List<T> List { get { return lst; } set { lst = value; } }
        public SerializableList()
        {
            lst = new List<T>();
        }
        public SerializableList(List<T> List)
        {
            lst = List;
        }

        public SerializableList( SerializationInfo info, StreamingContext context)
        {
            lst = (List<T>)info.GetValue("list" , typeof(List<T>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("list", lst, typeof(List<T>));
        }
    }
}
