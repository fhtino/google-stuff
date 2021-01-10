using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthWebApp
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            string clientSecretFilename = Environment.GetEnvironmentVariable("CLIENT_SECRET_FILENAME_PATH");
            var secrets = JObject.Parse(System.IO.File.ReadAllText(clientSecretFilename))["web"];
            var clientId = secrets["client_id"].Value<string>();
            var clientSecret = secrets["client_secret"].Value<string>();

            services
                .AddAuthentication(o =>
                {
                    // This forces challenge results to be handled by Google OpenID Handler, so there's no
                    // need to add an AccountController that emits challenges for Login.
                    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                    // This forces forbid results to be handled by Google OpenID Handler, which checks if
                    // extra scopes are required and does automatic incremental auth.
                    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                    // Default scheme that will handle everything else.
                    // Once a user is authenticated, the OAuth2 token info is stored in cookies.
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogleOpenIdConnect(options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    options.Events.OnTokenValidated += OnTokenValidated;
                });

        }


        static Task OnTokenValidated(TokenValidatedContext ctx)
        {
            string accessToken = ctx.TokenEndpointResponse.AccessToken;
            string refreshToken = ctx.TokenEndpointResponse.RefreshToken;
            string scopes = ctx.TokenEndpointResponse.Scope;

            ClaimsPrincipal principal = ctx.Principal;

            // Store the tokens so the backend service can access them.
            if (scopes.Contains(DriveService.Scope.DriveReadonly) &&
                scopes.Contains(CalendarService.Scope.CalendarReadonly))
            {
                string googleID = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                string email = principal.FindFirst(ClaimTypes.Email)?.Value;

                string clientSecretFilename = Environment.GetEnvironmentVariable("DEMOSTORAGE_FILENAME");
                new AuthCommonLib.DemoStorage(clientSecretFilename).Store(googleID, refreshToken);
            }

            return Task.CompletedTask;
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();   // <<==
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

    }
}
