using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NS_Serialize
{
    public class MemorySerialize
    {       
        public static bool serialize(Object objects, out byte[] bytes)
        {
            bool ret = true;         
            bytes = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objects);
                bytes = memoryStream.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ret = false;
            }
            return ret;
        }
        
        public static bool deSerialize(byte[] bytes, out Object objects)
        {
            bool ret = true;
            objects = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream(bytes);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                objects = binaryFormatter.Deserialize(memoryStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ret = false;
            }
            return ret;
        }
    }
}
