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
    public partial class AccountView : ContentView
    {
        private readonly LoginService loginService = new LoginService();
        private readonly OktaApiInterface oktaApiInterface = new OktaApiInterface();
        private readonly WineAPIService wineApiService = new WineAPIService();

        public AccountView()
        {
            InitializeComponent();
            ActivityOngoing(true);
            OktaUserProfile myUserProfile = new OktaUserProfile();
            var user_id_task = GetOktaUserId();
            var name_task = GetName();
            var email_task = GetEmail();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                myUserProfile = oktaApiInterface.GetUserProfile(user_id_task.Result);
                nameLabel.Text = "Name: " + name_task.Result;
                emailLabel.Text = "Username: " + email_task.Result;
                SetApiGuid(myUserProfile.apiUserId);
                SetLoggedInState(myUserProfile.given_name);
            }
            else
            {
                SetLoggedOutState();
            }
            ActivityOngoing(false);
            //YourGuid.Text = "Your GUID is " + wineApiService.GetApiGuid();
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
            //await Navigation.PushModalAsync(new MainPageNotLoggedIn());
            try
            {
                await Task.Run(async () =>
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
            LogoutPanel.IsVisible = true;
            LoginPanel.IsVisible = false;
        }

        private void SetLoggedOutState()
        {
            LogoutPanel.IsVisible = false;
            LoginPanel.IsVisible = true;
        }

        private async Task<string> GetOktaUserId()
        {
            return await SecureStorage.GetAsync("auth_user_id");
        }

        private async Task<string> GetName()
        {
            return await SecureStorage.GetAsync("name");
        }

        private async Task<string> GetEmail()
        {
            return await SecureStorage.GetAsync("email");
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

        private void Cabernet_Button_Clicked(object sender, EventArgs e)
        {
            ((App.Current) as App).ThemeChanger.ApplyTheme("CabernetTheme");
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(new CabernetTheme());
                saveTheme("Cabernet");
            }
        }

        private void Dark_Button_Clicked(object sender, EventArgs e)
        {
            ((App.Current) as App).ThemeChanger.ApplyTheme("DarkTheme");
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(new DarkTheme());
                saveTheme("Dark");
            }
        }

        private void Light_Button_Clicked(object sender, EventArgs e)
        {
            ((App.Current) as App).ThemeChanger.ApplyTheme("LightTheme");
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(new LightTheme());
                saveTheme("Light");
            }
        }

        private void Chardonnay_Button_Clicked(object sender, EventArgs e)
        {
            ((App.Current) as App).ThemeChanger.ApplyTheme("ChardonnayTheme");
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(new ChardonnayTheme());
                saveTheme("Chardonnay");
            }
        }

        private async void saveTheme(string theme)
        {
            if (App.Current.Properties.ContainsKey("SelectedTheme"))
                App.Current.Properties["SelectedTheme"] = theme;
            else
                App.Current.Properties.Add("SelectedTheme", theme);
            await App.Current.SavePropertiesAsync();
        }
    }
}