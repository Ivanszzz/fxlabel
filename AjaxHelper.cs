using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AHLabelPrint
{
	public class AjaxHelper
	{
        /// <summary>
        /// 客户端请求数据
        /// </summary>
        public static string ClientRequest(string actionUrl, string jsonBody)
        {
            string returnMsg = string.Empty;
            ClientResponseMsg respMsg = new ClientResponseMsg();
            returnMsg = JsonConvert.SerializeObject(respMsg);

            //Dictionary<string, object> reqParams = (Dictionary<string, object>)reqData;
            string url = ConfigurationManager.AppSettings["MESServerUrl"] + actionUrl;
            //string body = JsonConvert.SerializeObject(reqParams["data"]);
            if (!string.IsNullOrEmpty(jsonBody))
            {
                returnMsg = HttpMethods.HttpPost(url, jsonBody.ToString(), "application/json", "POST", "");
            }
            return returnMsg;
        }
    }
}
