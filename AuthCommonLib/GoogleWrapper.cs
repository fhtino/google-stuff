using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AuthCommonLib
{

    public class GoogleWrapper
    {

        private IConfigurableHttpClientInitializer _credentials;


        public GoogleWrapper(string refresh_token, byte[] googleSecretsFileBody)
        {
            var _googleClientSecrets = GoogleClientSecrets.Load(new MemoryStream(googleSecretsFileBody)).Secrets;

            TokenResponse tokenReponse = new TokenResponse() { RefreshToken = refresh_token };

            var userCredential = new UserCredential(
                   new GoogleAuthorizationCodeFlow(
                       new GoogleAuthorizationCodeFlow.Initializer()
                       {
                           ClientSecrets = _googleClientSecrets
                       }),
                   "user",
                   tokenReponse);

            _credentials = userCredential;
        }


        public GoogleWrapper(GoogleCredential cred)
        {
            _credentials = cred;
        }


        public async Task<List<Google.Apis.Drive.v3.Data.File>> GetDriveFiles()
        {
            var driveSvc = new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = _credentials
                });

            var listRequest = driveSvc.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, ownedByMe, parents, mimeType, modifiedTime, size, md5Checksum)";
            var lisrResponse = await listRequest.ExecuteAsync();
            return lisrResponse.Files.ToList();
        }


        public async Task<List<Google.Apis.Calendar.v3.Data.CalendarListEntry>> GetCalendars()
        {
            var calendarSvc = new CalendarService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = _credentials
                });
            var calendars = await calendarSvc.CalendarList.List().ExecuteAsync();
            return calendars.Items.ToList();
        }

    }

}
