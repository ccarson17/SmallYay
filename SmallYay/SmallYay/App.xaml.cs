using SmallYay.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.Generic;
using SmallYay.Interfaces;

namespace SmallYay
{
    public partial class App : Application
    {
        public readonly IThemeChanger ThemeChanger;
        public App(IThemeChanger themeChanger)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDEyNTU1QDMxMzgyZTM0MmUzMEZnV0hzcktPU1EyZ1VuaUU2MnEzRTlyVjh5NjkxWHd2czM5bGEzOFV4Q1k9");
            InitializeComponent();
            this.ThemeChanger = themeChanger;
            Device.SetFlags(new string[] { "Expander_Experimental" });
            var user_id_task = GetOktaUserId();
            if (App.Current.Properties.ContainsKey("SelectedTheme"))
            {
                string theme = App.Current.Properties["SelectedTheme"] as string;
                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries != null)
                {
                    mergedDictionaries.Clear();
                    switch (theme) {
                        case "Light":
                            ((App.Current) as App).ThemeChanger.ApplyTheme("LightTheme");
                            mergedDictionaries.Add(new LightTheme());
                            break;
                        case "Dark":
                            ((App.Current) as App).ThemeChanger.ApplyTheme("DarkTheme");
                            mergedDictionaries.Add(new DarkTheme());
                            break;
                        case "Cabernet":
                            ((App.Current) as App).ThemeChanger.ApplyTheme("CabernetTheme");
                            mergedDictionaries.Add(new CabernetTheme());
                            break;
                        case "Chardonnay":
                            ((App.Current) as App).ThemeChanger.ApplyTheme("ChardonnayTheme");
                            mergedDictionaries.Add(new ChardonnayTheme());
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new NavigationPage(new MainPageNotLoggedIn());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async Task<string> GetOktaUserId()
        {
            return await SecureStorage.GetAsync("auth_user_id");
        }
    }
}
