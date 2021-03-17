using SmallYay.Config;
using SmallYay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmallYay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentView
    {
        private readonly LoginService loginService = new LoginService();
        private readonly OktaApiInterface oktaApiInterface = new OktaApiInterface();
        private readonly WineAPIService wineApiService = new WineAPIService();

        public HomeView()
        {
            InitializeComponent();
            SetBackground();
            ActivityOngoing(true);
            OktaUserProfile myUserProfile = new OktaUserProfile();
            var user_id_task = GetOktaUserId();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                myUserProfile = oktaApiInterface.GetUserProfile(user_id_task.Result);
                if (myUserProfile != null)
                {
                    SetApiGuid(myUserProfile.apiUserId);
                    SetLoggedInState(myUserProfile.given_name);
                }
                else
                {
                    SetLoggedOutState();
                }
            }
            else
            {
                SetLoggedOutState();
            }
            ActivityOngoing(false);
            //YourGuid.Text = "Your GUID is " + wineApiService.GetApiGuid();
        }
        private async void LoginClicked(object sender, EventArgs e)
        {
            ActivityOngoing(true);
            try
            {
                var callbackUrl = new Uri(Constants.RedirectUri);
                var loginUrl = new Uri(loginService.BuildAuthenticationUrl());
                try
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(10000);
                        ActivityOngoing(false);
                    });
                    var authenticationResult = await WebAuthenticator.AuthenticateAsync(loginUrl, callbackUrl);
                    var token = loginService.ParseAuthenticationResult(authenticationResult);

                    var nameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "given_name");
                    var apiGuid = token.Claims.FirstOrDefault(claim => claim.Type == "user_guid");
                    var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "preferred_username");
                    //var email = token.Claims.FirstOrDefault(claim => claim.Type == "preferred_username");
                    var name = token.Claims.FirstOrDefault(claim => claim.Type == "name");
                    string userFriendlyName = nameClaim.Value.ToString();
                    if (string.IsNullOrEmpty(apiGuid.Value.ToString())) // If user doesn't have a GUID to access the Wine API, generate one for them.
                    {
                        var userProfile = oktaApiInterface.GetUserProfile(token.Subject);
                        userProfile.apiUserId = Guid.NewGuid().ToString();
                        oktaApiInterface.UpdateUserProfile(userProfile, token.Subject);
                    }
                    await SecureStorage.SetAsync("auth_client_id", token.RawData);
                    await SecureStorage.SetAsync("auth_user_id", token.Subject);
                    await SecureStorage.SetAsync("user_guid", apiGuid.Value);
                    await SecureStorage.SetAsync("name", name.Value);
                    await SecureStorage.SetAsync("email", emailClaim.Value);
                    if (userFriendlyName != null)
                    {
                        if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
                        {
                            Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                            await Application.Current.SavePropertiesAsync();
                        }
                        SetLoggedInState(userFriendlyName);
                        ActivityOngoing(false);
                        await Navigation.PushModalAsync(new MainPage());
                    }
                }
                catch (Exception ex)
                {
                    string errorText = ex.Message;
                    ActivityOngoing(false);
                }
            }
            catch (TaskCanceledException)
            {
                ActivityOngoing(false);
            }
        }

        private async void LogoutClicked(object sender, EventArgs e)
        {
            ActivityOngoing(true);
            var client_id = await SecureStorage.GetAsync("auth_client_id");
            var logoutUri = new Uri(Constants.LogoutEndpoint + $"?id_token_hint={client_id}");
            var callbackUri = new Uri(Constants.RedirectUri);
            SetLoggedOutState();
            SecureStorage.RemoveAll();
            if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {
                Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                await Application.Current.SavePropertiesAsync();
            }
            await Navigation.PushModalAsync(new MainPageNotLoggedIn());
            try
            {
                Task.Run(async () =>
                {
                    await Task.Delay(10000);
                    ActivityOngoing(false);
                });
                await WebAuthenticator.AuthenticateAsync(logoutUri, callbackUri);
            }
            catch (TaskCanceledException)
            {
                ActivityOngoing(false);
            }
        }

        private void SetLoggedInState(string userLabel)
        {
            WelcomeLabel.Text = "Welcome, " + userLabel + "!";
            //LogoutPanel.IsVisible = true;
            LoginPanel.IsVisible = false;
        }

        private async void SetLoggedOutState()
        {
            WelcomeLabel.Text = "Welcome. Please Log In.";
            //LogoutPanel.IsVisible = false;
            LoginPanel.IsVisible = true;
            SecureStorage.Remove("auth_user_id");
        }

        private async Task<string> GetOktaUserId()
        {
            return await SecureStorage.GetAsync("auth_user_id");
        }

        private async void SetApiGuid(string guid)
        {
            await SecureStorage.SetAsync("user_guid", guid);
        }

        private void ActivityOngoing(bool state)
        {
            LoginActivityIndicator.IsRunning = state;
            LoginScreenDarken.IsVisible = state;
        }

        private void SetBackground()
        {
            if (App.Current.Properties.ContainsKey("SelectedTheme"))
            {
                string theme = App.Current.Properties["SelectedTheme"].ToString();
                switch (theme) {
                    case "Chardonnay":
                        backgroundImage.Source = ImageSource.FromFile("pexels_bruno_cantuaria_774455.jpg");
                        //backgroundImage.Source = ImageSource.FromFile("pexels_polina_tankilevitch_4110415.jpg");
                        break;
                    case "Cabernet":
                        backgroundImage.Source = ImageSource.FromFile("pexels_bruno_cantuaria_774455.jpg");
                        break;
                    case "Light":
                        backgroundImage.Source = ImageSource.FromFile("pexels_bruno_cantuaria_774455.jpg");
                        break;
                    case "Dark":
                        backgroundImage.Source = ImageSource.FromFile("pexels_bruno_cantuaria_774455.jpg");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}