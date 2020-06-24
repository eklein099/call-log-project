using RingCentral;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class AuthFlow
    {
        public static async Task authorize()
        {
            string password = Program.clientPassword;
            string phone_number = Program.clientUsername;
            string extension = Program.clientExtension;
            string url = "https://platform.devtest.ringcentral.com/restapi/oauth/token";
            string email = "eklein099%40gmail.com";
            string id_secret = "1Fc-YQzESNuDNEEQv289YQ:16y8qLwNSaW9JLphyJkGqQKe37-YGUSDCJ4jdjZolFMg";


            HttpClient client = new HttpClient();

            //encode authorization thing
            var encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(id_secret));
            Console.WriteLine("encoded auth thingy: " + encoded);

            var values = new Dictionary<string, string>
            {
                {"Accept", "application/json"},
                { "Content-Type", "application/x-www-form-urlencoded"},
                { "Authorization",encoded},
                { "grant_type", "password"},
                {"username", "eklein099@gmail.com" },
                { "password", "Turtle100#RIN"}
            };

            var content = new FormUrlEncodedContent(values);
            string cstring = await content.ReadAsStringAsync();
            Console.WriteLine("content: "+cstring) ;
            var response = await client.PostAsync(url,content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
        }

        public static async Task authorize2(string username,string extension, string password)
        {
            string clientId = "1Fc-YQzESNuDNEEQv289YQ";
            string clientSecret = "16y8qLwNSaW9JLphyJkGqQKe37-YGUSDCJ4jdjZolFMg";
            RestClient rc = new RestClient(clientId, clientSecret, false);
            await rc.Authorize(username, extension, password);

            int daysBack = -7;

            var fromDate = DateTime.UtcNow.AddDays(daysBack).ToString("yyyy-MM-ddTHH:mm:ssZ");
            Console.WriteLine("date: " + fromDate);

            ReadUserCallLogParameters logParameters = new ReadUserCallLogParameters
            {
                extensionNumber = Program.clientExtension,
                showBlocked = true,
                phoneNumber = Program.clientUsername,
                direction = new[] { "Inbound", "Outbound" },
                //sessionId = "<ENTER VALUE>",
                //type = new[] { "Voice", "Fax" },
                //transport = new[] { "PSTN", "VoIP" },
                //view = 'Simple',
                //withRecording = true,
                //recordingType = 'Automatic',
                //dateTo = "<ENTER VALUE>",
                dateFrom = fromDate,
                page = 1,
                perPage = 100,
                showDeleted = true
            };
            Console.WriteLine("created log parameters");

            var r = await rc.Restapi().Account("~").Extension("~").CallLog().List(logParameters);
            //process response
            Console.WriteLine(r.records.Length);
        }
    }
}
