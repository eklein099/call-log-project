using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections;

namespace ConsoleApp
{
    public class Salesnexus
    {

        static string salesnexus_url = "https://logon.salesnexus.com/api/call-v1";

        private static readonly HttpClient client = new HttpClient();

        //string json = "[{\"function\":\"greet\"," +
        //                "\"request-id\":\"simple hello world request\"}]";
        public static string call_api(String url, String json)
        {
            Console.WriteLine("request: \n\n" + json + "\n");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

            
        }

        public static string get_token(String username, String password)
        {
            string json = "[{\"function\":\"get-login-token\"," +
                          "\"parameters\":{" +
                          "\"email-address\":\""+username+"\"," +
                          "\"password\":\""+password+"\""+
                          "}," +
                          "\"request-id\":\"token request\""+
                          "}]";

            string response = call_api(salesnexus_url,json);
            return getJsonProperty(response,"login-token");

        }

        public static string get_users(string username, string password)
        {
            string token = get_token(username, password);

            string json = "["+
    "{" +
        "\"function\": \"get-users\"," +
        "\"parameters\": {" +
                   "\"login-token\": \""+token+"\"" +
       "}," +
        "\"request-id\": \"request to get all users in the database\"" +
    "}" +
"]";
            string response = call_api(salesnexus_url, json);
            return response;
        }

        public static string get_contacts(string authtoken)
        {
            int start = 0;
            int page_size = 999999999;

                string json = "[" +
    "{" +
            "\"function\": \"get-contacts\"," +
            "\"parameters\": {" +
                "\"login-token\": \"" + authtoken + "\"," +
                "\"filter-value\": \"\"," +
                "\"start-after\": \"" + start + "\"," +
                "\"page-size\": \"" + page_size + "\"" +
            "}," +
            "\"request-id\": \"Get a list of contacts\"" +
    "}" +
"]";
                string response = call_api(salesnexus_url, json);
                response = response.Substring(response.IndexOf("contact-info"));
                response = response.Substring(0, response.IndexOf("request-id"));
                response = response.Substring(response.IndexOf("\n"));
                start += page_size;

            return response;
        }

        public static string find_contact_by_number(string phone_number, string authCode)
        {
            string body = get_contacts(authCode);

            while (body.IndexOf('{') != -1)
            {
                string contactInfo = body.Substring(0, body.IndexOf('}'));

                if(contactInfo.IndexOf(phone_number) != -1)
                {
                    contactInfo = contactInfo.Substring(contactInfo.IndexOf("\"")+1);
                    contactInfo = contactInfo.Substring(0, contactInfo.IndexOf("\""));
                    return contactInfo;
                }

                body = body.Substring(body.IndexOf('}') + 1);
            }
            return "none";
        }

        public static void create_log_note(string username, string password, string number)
        {
            string auth_token = get_token(username, password);
            string contact_id = find_contact_by_number(number, auth_token);
            string json = "[" +
    "{" +
        "\"function\": \"create-note\"," +
        "\"parameters\": {" +
             "\"login-token\": \""+auth_token+"\"," +
            "\"details\": \"test note\"," +
            "\"contact-id\": \""+contact_id+"\"," +
            "\"type\": \"1\"" +
        "}," +
        "\"request-id\": \"Create a note\"" +
    "}" +
"]";
            string result = call_api(salesnexus_url, json);
            Console.WriteLine(result);
        }

        public static string getJsonProperty(string json, string key)
        {
            string bottom = json.Substring(json.IndexOf(key) + key.Length + 4);
            return bottom.Substring(0, bottom.IndexOf("\""));
        }
    }
}
