using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using SmallYay.Config;
using SmallYay.Interfaces;

namespace SmallYay.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IThemeChanger
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            //Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(this));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void ApplyTheme(string newTheme)
        {
            if (newTheme?.ToLower() == "lighttheme")
            {
                SetTheme(Resource.Style.LightTheme);
            }
            else if (newTheme?.ToLower() == "darktheme")
            {
                SetTheme(Resource.Style.DarkTheme);
            }
            else if (newTheme?.ToLower() == "chardonnaytheme")
            {
                SetTheme(Resource.Style.ChardonnayTheme);
            }
            else if (newTheme?.ToLower() == "cabernettheme")
            {
                SetTheme(Resource.Style.CabernetTheme);
            }
            else
            {
                SetTheme(Resource.Style.MainTheme);
            }
        }
    }

    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
        DataScheme = Constants.CallbackScheme)]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            Xamarin.Essentials.Platform.OnResume();
        }
    }
}