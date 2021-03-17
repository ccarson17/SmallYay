using Microsoft.IdentityModel.Tokens;
using SmallYay.Config;
using SmallYay.Services;
using SmallYay.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

namespace SmallYay
{
    public partial class MainPage : TabbedPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void HomePage_Appearing(object sender, EventArgs e)
        {
            HomeContentPage.Content = new HomeView();
        }

        private async void BottlePage_Appearing(object sender, EventArgs e)
        {
            var user_id_task = GetOktaUserId();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                BottlesContentPage.Content = new BottleView();
            }
            else
            {
                BottlesContentPage.Content = new PleaseLoginView();
            }
        }

        private async void RackPage_Appearing(object sender, EventArgs e)
        {
            var user_id_task = GetOktaUserId();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                RacksContentPage.Content = new RackView();
            }
            else
            {
                RacksContentPage.Content = new PleaseLoginView();
            }
        }

        private async void HistoryPage_Appearing(object sender, EventArgs e)
        {
            var user_id_task = GetOktaUserId();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                HistoryContentPage.Content = new HistoryView();
            }
            else
            {
                HistoryContentPage.Content = new PleaseLoginView();
            }
        }

        private async void AccountPage_Appearing(object sender, EventArgs e)
        {
            var user_id_task = GetOktaUserId();
            if (!string.IsNullOrEmpty(user_id_task.Result))
            {
                AccountContentPage.Content = new AccountView();
            }
            else
            {
                AccountContentPage.Content = new PleaseLoginView();
            }
        }

        private async Task<string> GetOktaUserId()
        {
            return await SecureStorage.GetAsync("auth_user_id");
        }
    }
}
