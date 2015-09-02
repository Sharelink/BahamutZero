using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon
{
    public class DocumentModel
    {
        public string ToDocument()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static string ToDocument(dynamic obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T ToDocumentObject<T>(string Document) where T : DocumentModel
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Document);
        }

        public static dynamic ToDocumentObject(string Document)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(Document);
        }

    }


}
