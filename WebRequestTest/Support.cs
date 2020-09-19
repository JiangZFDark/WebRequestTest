using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebRequestTest
{
    public static class Support
    {
        public static string CallWebService(string url, string action, Params param)
        {
            try
            {
                var _url = url;             //"http://xxxxxxxxx/Service1.asmx";
                var _action = action;       //"http://xxxxxxxx/Service1.asmx?op=HelloWorld";
                string xml = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                                <soapenv:Header/>
                                <soapenv:Body>
                                    <tem:queryObject>
                                        <!--Optional:-->
                                            <tem:jkid>{0}</tem:jkid>
                                        <!--Optional:-->
                                            <tem:yhbz>{1}</tem:yhbz>
                                        <!--Optional:-->
                                            <tem:yhxm>{2}</tem:yhxm>
                                        <!--Optional:-->
                                            <tem:zdbs>{3}</tem:zdbs>
                                        <!--Optional:-->
                                            <tem:xtbs>{4}</tem:xtbs>
                                        <!--Optional:-->
                                            <tem:E1>{5}</tem:E1>
                                        <!--Optional:-->
                                            <tem:V1>{6}</tem:V1>
                                        <!--Optional:-->
                                            <tem:QueryDoc>{7}</tem:QueryDoc>
                                    </tem:queryObject>
                                </soapenv:Body>
                                </soapenv:Envelope>";
                xml = string.Format(xml, param.jkid, param.yhbz, param.yhxm, param.zdbs, param.xtbs, param.E1, param.V1, param.xml);
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope(xml);
                HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                    //Console.Write(soapResult);
                    return soapResult;
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            //webRequest.Accept = "text/xml";
            webRequest.Accept = "application/json";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string xml)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            //        soapEnvelopeDocument.LoadXml(
            //        @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
            //           xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
            //           xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
            //    <SOAP-ENV:Body>
            //        <HelloWorld xmlns=""http://tempuri.org/"" 
            //            SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
            //            <int1 xsi:type=""xsd:integer"">12</int1>
            //            <int2 xsi:type=""xsd:integer"">32</int2>
            //        </HelloWorld>
            //    </SOAP-ENV:Body>
            //</SOAP-ENV:Envelope>");
            soapEnvelopeDocument.LoadXml(xml);
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public static dynamic GetXmlNode(string xml, string xmlPath, string nodeName, Dictionary<string, string> xmlNameSpace)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xml);

                XmlElement xmlERoot = xmlDoc.DocumentElement;//获取根节点

                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                foreach(var key in xmlNameSpace.Keys)
                {
                    nsMgr.AddNamespace(key, xmlNameSpace[key]);
                }

                XmlNode queryNode = xmlERoot.SelectSingleNode(xmlPath, nsMgr);//定位到

                XmlNode resNode = queryNode.SelectSingleNode(nodeName, nsMgr);

                return resNode.InnerText;//获取指定节点的值
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
