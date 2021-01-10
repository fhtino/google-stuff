using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthCommonLib;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace AuthWebApp.Pages
{


    [Authorize]
    [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveReadonly,
                           CalendarService.ScopeConstants.CalendarReadonly)]
    public class PersonalDataModel : PageModel
    {

        private IGoogleAuthProvider _auth;

        public List<Google.Apis.Drive.v3.Data.File> DriveFiles;
        public List<Google.Apis.Calendar.v3.Data.CalendarListEntry> Calendars;


        public PersonalDataModel(IGoogleAuthProvider auth)
        {
            _auth = auth;
        }


        public async Task OnGet()
        {
            GoogleCredential googleCred = await _auth.GetCredentialAsync();

            var google = new GoogleWrapper(googleCred);
            DriveFiles = await google.GetDriveFiles();
            Calendars = await google.GetCalendars();            
        }

    }

}
