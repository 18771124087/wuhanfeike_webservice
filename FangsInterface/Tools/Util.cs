using FangsInterface.Bean;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace FangsInterface.Tools
{
    public class Util
    {
        public static string getNodeValueNoErrByNodeName(XmlNode node,string nodeName)
        {
            string value = string.Empty;
            try
            {
                value = node.SelectSingleNode(nodeName).InnerText;
            }
            catch (Exception)
            {
                throw new Exception("未找到 '" + nodeName + "' 节点");
            }
            return value;
        }

        public static string errMessage(string err)
        {
            BaseModel model = new BaseModel();
            model.Code = "0";
            model.Msg = err;
            return JsonConvert.SerializeObject(model);
        }

        public static bool isDebug()
        {
            string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();
            if (isDebug == "false")
                return false;
            else
                return true;
        }

       
    }
}