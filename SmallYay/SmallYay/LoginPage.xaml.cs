using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SmallYay.Config;
using SmallYay.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmallYay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginService loginService = new LoginService();
        private readonly OktaApiInterface oktaApiInterface = new OktaApiInterface();

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            try
            {
                var callbackUrl = new Uri(Constants.RedirectUri);
                var loginUrl = new Uri(loginService.BuildAuthenticationUrl());
                var authenticationResult = await WebAuthenticator.AuthenticateAsync(loginUrl, callbackUrl);
                
                try
                {
                    var token = loginService.ParseAuthenticationResult(authenticationResult);
                    
                    var nameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "given_name");
                    var apiGuid = token.Claims.FirstOrDefault(claim => claim.Type == "user_guid");
                    if(string.IsNullOrEmpty(apiGuid.Value.ToString())) // If user doesn't have a GUID to access the Wine API, generate one for them.
                    {
                        var userProfile = oktaApiInterface.GetUserProfile(token.Subject);
                        userProfile.apiUserId = Guid.NewGuid().ToString();
                        oktaApiInterface.UpdateUserProfile(userProfile, token.Subject);
                    }
                    await SecureStorage.SetAsync("auth_client_id", token.RawData);
                    await SecureStorage.SetAsync("auth_user_id", token.Subject);
                    if (nameClaim != null)
                    {
                        WelcomeLabel.Text = $"Welcome to Xamarin.Forms {nameClaim.Value}!";
                        LogoutPanel.IsVisible = !(LoginPanel.IsVisible = false);
                    }
                }
                catch(Exception ex)
                {
                    string errorText = ex.Message;
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async void LogoutClicked(object sender, EventArgs e)
        {
            var client_id = await SecureStorage.GetAsync("auth_client_id");
            var logoutUri = new Uri(Constants.LogoutEndpoint + $"?id_token_hint={client_id}");
            var callbackUri = new Uri(Constants.RedirectUri);
            WelcomeLabel.Text = "Welcome to Xamarin.Forms!";
            LogoutPanel.IsVisible = !(LoginPanel.IsVisible = true);
            SecureStorage.RemoveAll();
            try
            {
                await WebAuthenticator.AuthenticateAsync(logoutUri, callbackUri);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}