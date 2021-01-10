using AuthCommonLib;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AuthBackground
{

    class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("This is DEMO application");

            string clientSecretFileName = Environment.GetEnvironmentVariable("CLIENT_SECRET_FILENAME_PATH");
            string demoStorageFileName = Environment.GetEnvironmentVariable("DEMOSTORAGE_FILENAME");

            if (String.IsNullOrEmpty(clientSecretFileName)) throw new ApplicationException("missing client secret filename");
            if (String.IsNullOrEmpty(demoStorageFileName)) throw new ApplicationException("missing demostorage filename");
            if (!File.Exists(clientSecretFileName)) throw new ApplicationException("Cannot find client_secret json file");

            byte[] clientSecret = File.ReadAllBytes(clientSecretFileName);
            var storage = new DemoStorage(demoStorageFileName);

            while (true)
            {
                string[] googleIDs = storage.GetGoogleIDs();

                foreach (var googleID in googleIDs)
                {
                    Console.WriteLine($"\n---> Processing user {googleID}");
                    string refreshToken = storage.GetRefreshToken(googleID);
                    Console.WriteLine($"refresh_token: {refreshToken.Substring(0, 50)}...");

                    var google = new GoogleWrapper(refreshToken, clientSecret);

                    (await google.GetDriveFiles()).Take(5).ToList().ForEach(x => Console.WriteLine($"File: {x.Name}"));
                    (await google.GetCalendars()).Take(5).ToList().ForEach(x => Console.WriteLine($"Calendar: {x.Id}"));
                }

                Console.WriteLine("\n\nPress a key to continue");
                Console.ReadKey(true);                
                //Thread.Sleep(10 * 1000);
            }
        }

    }

}
