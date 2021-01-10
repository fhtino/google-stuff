using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;


namespace AuthWebApp.Pages
{

    [Authorize]
    public class ProtectedModel : PageModel
    {

        private IGoogleAuthProvider _auth;

        public List<string> CurrentUserScopes = new List<string>();
        public string CurrentTokensJson = null;
        public string CurrentAccessToken = null;
        public string CurrentRefreshToken = null;


        public ProtectedModel(IGoogleAuthProvider auth)
        {
            _auth = auth;
        }


        public async Task OnGet()
        {
            // Get scopes
            IReadOnlyList<string> currentScopes = await _auth.GetCurrentScopesAsync();
            CurrentUserScopes = currentScopes.ToList();
            
            // Get tokens
            AuthenticateResult authResult = await HttpContext.AuthenticateAsync();
            CurrentAccessToken = authResult.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken).Substring(0, 50) + "...";
            CurrentRefreshToken = authResult.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken).Substring(0, 50) + "...";
        }

    }

}
