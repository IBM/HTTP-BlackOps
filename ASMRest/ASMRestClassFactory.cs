using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ASMRest
{
  
    /// <summary>
    /// Converts JSON responses into .Net objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ASMRestClassFactory<T>
    {
        /// <summary>
        /// Makes an object from json
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public T[] Make(string jsonData)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
            T[] result = ser.Deserialize<T[]>(jsonData);

            return result;
        }
    }
}
