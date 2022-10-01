using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using m.transport.Domain;
using m.transport.ServiceInterface;
using System.Linq;
using System.Net.Http.Headers;
using m.transport.Models;
using DAI;

namespace m.transport.Svc
{
    public static class XmlExtenions {

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
    }

    //[Serializable, XmlRoot("GetCodesResult")]
    [XmlRoot(ElementName = "GetCodesResult", Namespace = "http://www.mophilly.com")]
    public class GetCodesResult {
        [XmlElement(ElementName = "Code", Namespace = "http://www.mophilly.com")]
        public List<Code> Code { get; set; }
    }

    [XmlRoot(ElementName = "GetRunDetailResponse", Namespace = "http://www.mophilly.com")]
    public class GetRunDetailResponse
    {
        [XmlElement(ElementName = "GetRunDetailResult", Namespace = "http://www.mophilly.com")]
        public GetRunDetailResult GetRunDetailResult { get; set; }
    }

    //[Serializable, XmlRoot("GetRunListResponse")]
    [XmlRoot(ElementName = "GetRunListResponse", Namespace = "http://www.mophilly.com")]
    public class GetRunListResponse
    {
        [XmlElement(ElementName = "GetRunListResult", Namespace = "http://www.mophilly.com")]
        public GetRunListResult GetRunListResult { get; set; }
    }

    //[Serializable, XmlRoot("GetCodesResponse")]
    [XmlRoot(ElementName = "GetCodesResponse", Namespace = "http://www.mophilly.com")]
    public class GetCodesResponse
    {
        [XmlElement(ElementName = "GetCodesResult", Namespace = "http://www.mophilly.com")]
        public GetCodesResult GetCodesResult { get; set; }
    }

    [XmlRoot(ElementName = "Areas", Namespace = "http://www.mophilly.com")]
    public class Areas
    {
        [XmlElement(ElementName = "DamageAreaCode", Namespace = "http://www.mophilly.com")]
        public List<DamageAreaCode> DamageAreaCode { get; set; }
    }

    [XmlRoot(ElementName = "DamageSeverityCode", Namespace = "http://www.mophilly.com")]
    public class DamageSeverityCode
    {
        [XmlElement(ElementName = "Id", Namespace = "http://www.mophilly.com")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Code", Namespace = "http://www.mophilly.com")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Description", Namespace = "http://www.mophilly.com")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "Severities", Namespace = "http://www.mophilly.com")]
    public class Severities
    {
        [XmlElement(ElementName = "DamageSeverityCode", Namespace = "http://www.mophilly.com")]
        public List<DamageSeverityCode> DamageSeverityCode { get; set; }
    }

    [XmlRoot(ElementName = "DamageTypeCode", Namespace = "http://www.mophilly.com")]
    public class DamageTypeCode
    {
        [XmlElement(ElementName = "Id", Namespace = "http://www.mophilly.com")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Code", Namespace = "http://www.mophilly.com")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Description", Namespace = "http://www.mophilly.com")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "Types", Namespace = "http://www.mophilly.com")]
    public class Types
    {
        [XmlElement(ElementName = "DamageTypeCode", Namespace = "http://www.mophilly.com")]
        public List<DamageTypeCode> DamageTypeCode { get; set; }
    }

    [XmlRoot(ElementName = "GetDamageCodeListResult", Namespace = "http://www.mophilly.com")]
    public class GetDamageCodeListResult
    {
        [XmlElement(ElementName = "Areas", Namespace = "http://www.mophilly.com")]
        public Areas Areas { get; set; }
        [XmlElement(ElementName = "Severities", Namespace = "http://www.mophilly.com")]
        public Severities Severities { get; set; }
        [XmlElement(ElementName = "Types", Namespace = "http://www.mophilly.com")]
        public Types Types { get; set; }
    }

    [XmlRoot(ElementName = "GetDamageCodeListResponse", Namespace = "http://www.mophilly.com")]
    public class GetDamageCodeListResponse
    {
        [XmlElement(ElementName = "GetDamageCodeListResult", Namespace = "http://www.mophilly.com")]
        public GetDamageCodeListResult GetDamageCodeListResult { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

    public class RestServiceClient : IRestServiceClient
    {
        // TODO: error handling, hard-coded url

        private string baseUrl;

        public RestServiceClient(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.Contains("v4"))
                {
                    int index = url.IndexOf("/Service.svc");
                    url = url.Insert(index, "v4");
                }

                //if (url.Contains("/transportws/"))
                //{
                //    url = url.Replace("/transportws/", "/transportwsv2/");
                //}

                if (url.Contains("soapssl"))
                {
                    url = url.Replace("soapssl", "restssl");
                }

                if (url.Contains("soap"))
                {
                    url = url.Replace("soap", "rest");
                }

                if (!url.EndsWith("/"))
                {
                    url = url + "/";
                }
                baseUrl = url;
            }
            else
            {
                baseUrl = "https://trans1.mophilly.com/transportwsv2/Service.svc/restssl/";
            }
        }

        public RestServiceClient()
        {
            baseUrl = "https://trans1.mophilly.com/transportwsv2/Service.svc/restssl/";
        }

        public async Task<LoginResult> LoginAsync(string pDriver, string pPassword, string pTruck)
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}RestLogin?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}");

                var login = result.XmlDeserializeFromString<LoginResult>();

                return login;

            }
        }

       

        public async Task<MobileSettingsResult> GetMobileSettingsAsync()
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetMobileSettings");

                var settings = result.XmlDeserializeFromString<MobileSettingsResult>();

                return settings;

            }
        }

        public async Task<Code[]> GetCodesAsync(string pDriver, string pPassword, string pTruck, string codeTypes)
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetCodes?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}&codeTypes={codeTypes}");

                //result = result.Replace("xmlns=\"http://www.mophilly.com\"", string.Empty);

                var codes = result.XmlDeserializeFromString<GetCodesResponse>();

                return codes.GetCodesResult.Code.ToArray();

            }
        }

        // TODO: this deserialization should be simplified
        public async Task<DamageCodes> GetDamageCodeListAsync(string pDriver, string pPassword, string pTruck, string pHandheldID) 
        {
            var severity = new List<m.transport.Domain.DamageSeverityCode>();
            var area = new List<m.transport.Domain.DamageAreaCode>();
            var types = new List<m.transport.Domain.DamageTypeCode>();
            var codes = new DamageCodes();


            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetDamageCodeList?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}&pHandheldID={pHandheldID}");
               
                //result = result.Replace("xmlns=\"http://www.mophilly.com\"", string.Empty);

                var response = result.XmlDeserializeFromString<GetDamageCodeListResponse>();

                foreach (var s in response.GetDamageCodeListResult.Severities.DamageSeverityCode) {
                    severity.Add(new m.transport.Domain.DamageSeverityCode() { Code = s.Code, Description = s.Description, Id = s.Id });
                }

                codes.Severities = severity.ToArray();

                foreach (var a in response.GetDamageCodeListResult.Areas.DamageAreaCode)
                {
                    area.Add(new m.transport.Domain.DamageAreaCode() { Location = a.Location, Code = a.Code, Description = a.Description, Id = a.Id });
                }

                codes.Areas = area.ToArray();

                foreach (var t in response.GetDamageCodeListResult.Types.DamageTypeCode)
                {
                    types.Add(new m.transport.Domain.DamageTypeCode() { Code = t.Code, Description = t.Description, Id = t.Id });
                }

                codes.Types = types.ToArray();

                return codes;

            }

        }

        public async Task<CurrentLoadResultV2> GetCurrentLoadAsync(string pDriver, string pPassword, string pTruck, bool clearExceptions, string returnLoad)
        {
            using (var http = GetHttpClient())
            {
                var result = await http.GetStringAsync($"{baseUrl}GetCurrentLoadV2?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}&clearExceptions={clearExceptions}&returnLoad={returnLoad}");

                var load = result.XmlDeserializeFromString<CurrentLoadResultV2>();

                return load;
            }
        }

        public async Task<List<XElement>> GetUnpaidExpenseListAsync(string pDriver, string pPassword, string pTruck)
        {
            using (var http = GetHttpClient())
            {
                //http.DefaultRequestHeaders.Add("accept", "application/json");


                var result = await http.GetStringAsync($"{baseUrl}GetUnpaidExpenseList?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}");

                XDocument doc = XDocument.Parse(result);

                

                var exp = doc.Descendants("UnpaidExpenses").ToList<XElement>();

                //var exp = result.XmlDeserializeFromString<ArrayOfXElement>();

                return exp;
            }

        }

        public async Task LogMobileDeviceAsync(MobileDevice device)
        {

            using (var http = GetHttpClient())
            {

                var xml = device.Serialize<Domain.MobileDevice>();

                var wrapped = "<LogMobileDevice xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    xml.Replace("<MobileDevice xmlns=\"http://schemas.datacontract.org/2004/07/m.transport.Domain\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", "<device>").Replace("</MobileDevice>", "</device>") +
                       "</LogMobileDevice>";
          
                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}LogMobileDevice", body);
            }
        }

        public async Task UpdateOdometerAsync(string pDriver, string pPassword, string pTruck, string pOdometer)
        {

            using (var http = GetHttpClient())
            {

                var wrapped = "<UpdateOdometer xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    $"<pOdometer>{pOdometer}</pOdometer>" +
                    "</UpdateOdometer>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}UpdateOdometer", body);


            }
        }

        public async Task UpdateGPSLocationAsync(string pDriver, string pPassword, string pTruck, string longitude, string lattitude)
        {

            using (var http = GetHttpClient())
            {

                var wrapped = "<UpdateLocation xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    $"<pLongitude>{longitude}</pLongitude>" +
                    $"<pLattitude>{lattitude}</pLattitude>" +
                    "</UpdateLocation>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}UpdateLocation", body);


            }
        }

        public async Task UpdateTruckAsync(string pDriver, string pPassword, string pTruck, string pNewTruck)
        {
            using (var http = GetHttpClient())
            {
                var wrapped = "<UpdateTruck xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   $"<pDriver>{pDriver}</pDriver>" +
                   $"<pPassword>{pPassword}</pPassword>" +
                   $"<pTruck>{pTruck}</pTruck>" +
                   $"<pNewTruck>{pNewTruck}</pNewTruck>" +
                   "</UpdateTruck>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}UpdateTruck", body);
                Console.WriteLine(result.ToString());
            }

        }

        public async Task<List<XElement>> SendExpenseAsync(string pDriver, string pPassword, string pTruck, Domain.Expense expense)
        {
            using (var http = GetHttpClient())
            {

                var xml = expense.Serialize<Domain.Expense>();

                var wrapped = "<SendExpense xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    xml.Replace("<Expense xmlns=\"http://www.mophilly.com/\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", "<expense>").Replace("</Expense>", "</expense>") +
                       "</SendExpense>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}SendExpense", body);

                var resp = await result.Content.ReadAsStringAsync();

                XDocument doc = XDocument.Parse(resp);

                var ret = new List<XElement>();
                ret.Add(doc.Root);

                return ret;
            }
        }

        public async Task UploadCurrentLoadAsync(string pDriver, string pPassword, string pTruck, CurrentLoadUpdateV2 pCurrentLoad)
        {
            using (var http = GetHttpClient())
            {

                var xml = pCurrentLoad.Serialize<CurrentLoadUpdateV2>();

                var wrapped = "<UploadCurrentLoadV2 xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    xml.Replace("<CurrentLoadUpdateV2 xmlns=\"http://www.mophilly.com/\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", "<pCurrentLoad>").Replace("</CurrentLoadUpdateV2>", "</pCurrentLoad>") +
                       "</UploadCurrentLoadV2>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}UploadCurrentLoadV2", body);

                var resp = await result.Content.ReadAsStringAsync();

                XDocument doc = XDocument.Parse(resp);
            }
        }

        public async Task SendDamagePhotosAsync(string pDriver, string pPassword, string pTruck, DamagePhoto[] photos) {
            using (var http = GetHttpClient())
            {

                var xml = photos.Serialize<DamagePhoto[]>();

                var wrapped = "<SendDamagePhotos xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    xml.Replace("<ArrayOfDamagePhoto xmlns=\"http://www.mophilly.com/\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", "<photos>").Replace("</ArrayOfDamagePhoto>", "</photos>") +
                       "</SendDamagePhotos>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}SendDamagePhotos", body);

                var resp = await result.Content.ReadAsStringAsync();

                XDocument doc = XDocument.Parse(resp);
            }
        }

        public async Task<byte[]> GetDriverSignatureAsync(string pDriver, string pPassword, string pTruck)
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetDriverSignature?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}");

                XDocument doc = XDocument.Parse(result);

                var raw = doc.Descendants("base64Binary").First().Value;

                byte[] data = Convert.FromBase64String(raw);

                return data;
            }
        }

        public async Task SendDriverSignatureAsync(string pDriver, string pPassword, string pTruck, DeliverySignature signature)
        {
            using (var http = GetHttpClient())
            {

                var xml = signature.Serialize<DeliverySignature>();

                var wrapped = "<SendDriverSignature xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    xml.Replace("<DeliverySignature xmlns=\"http://www.mophilly.com/\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", "<signature>").Replace("</DeliverySignature>", "</signature>") +
                       "</SendDriverSignature>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}SendDriverSignature", body);

            }
        }

        public async Task<GetRunListResult> GetRunListAsync(string pDriver, string pPassword, string pTruck)
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetRunList?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}");

                var data = result.XmlDeserializeFromString<GetRunListResponse>();

                return data.GetRunListResult;
            }
        }

        public async Task<GetRunDetailResult> GetRunDetailAsync(string pDriver, string pPassword, string pTruck, string pRunID)
        {
            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}GetRunDetail?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}&pRunID={pRunID}");

                var data = result.XmlDeserializeFromString<GetRunDetailResponse>();

                return data.GetRunDetailResult;
            }
        }

        public async Task<string> SendGPSEmailNotification(string pDriver, string pPassword, string pTruck, string pFullName)
        {
            using (var http = GetHttpClient())
            {

                var wrapped = "<SendGPSEmail xmlns=\"http://www.mophilly.com\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    $"<pDriver>{pDriver}</pDriver>" +
                    $"<pPassword>{pPassword}</pPassword>" +
                    $"<pTruck>{pTruck}</pTruck>" +
                    $"<pFullName>{pFullName}</pFullName>" +
                    "</SendGPSEmail>";

                HttpContent body = new StringContent(wrapped, System.Text.Encoding.UTF8, "text/xml");

                var result = await http.PostAsync($"{baseUrl}SendGPSEmail", body);


            }



            using (var http = GetHttpClient())
            {

                var result = await http.GetStringAsync($"{baseUrl}SendEmailNotification?pDriver={pDriver}&pPassword={pPassword}&pTruck={pTruck}");
                return result;
            }
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            return client;
        }
    }
}
