using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRequestTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Params param = new Params()
            {
                jkid = "Q_Syntime",
                zdbs = "",
                E1 = "",
                V1 = "",
                xtbs = "",
                yhbz = "",
                yhxm = "",
                xml = "{\"token\":\"123\"}"
            };
            string url = "http://192.168.88.210:8021/Cosber.asmx";
            string action = "http://tempuri.org/queryObject";//"http://192.168.88.210:8021/Cosber.asmx?op=queryObject";
            var result1 = Support.CallWebService(url, action, param);
            Console.WriteLine(result1);
            Dictionary<string, string> xmlNameSpace = new Dictionary<string, string>();
            xmlNameSpace.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            xmlNameSpace.Add("temp", "http://tempuri.org/");
            //result1 = result1.Replace("xmlns=\"http://tempuri.org/\"", "");
            string result2 = Support.GetXmlNode(result1, "/soap:Envelope/soap:Body/temp:queryObjectResponse", "temp:queryObjectResult", xmlNameSpace);
            Console.WriteLine(result2);
            Console.ReadKey();
        }
    }
}
