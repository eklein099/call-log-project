using System;
using RingCentral;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        const string recipient = "8324289995";
        public const string clientId = "OucjCzM7Q4qIl4ZGEmAG8w";
        public const string clientSecret = "oE9-srnDRYySyoXE8d0LNwzA6mpmPZRcmEKQ_bVat3Qw";
        public const string clientUsername = "+12055178461";
        public const string clientExtension = "101";
        public const string clientPassword = "Turtle100#RIN";

        public const string sn_username = "eklein099@gmail.com";
        public const string sn_pasword = "Turtle100#SAL";
        public const string sn_recipient = "832-627-6621"; //should be pulled from rc call log


        static RestClient restClient;

        static void Main(string[] args)
        {
            //get rc call log data
            //mine is an empty array right now.
            //not sure why because I have made calls
            Console.WriteLine(RingCentral.get_log(clientUsername, clientExtension, clientPassword)); 

            //make a note for the contact with the given phone number in your contact list
            //right now it just says tets note because my log data is empty
            Salesnexus.create_log_note(sn_username, sn_pasword, sn_recipient);
        }
    }
}
