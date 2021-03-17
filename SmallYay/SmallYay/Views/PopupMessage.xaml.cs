using SmallYay.Models;
using SmallYay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace SmallYay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupMessage : ContentPage
    {
        string parameter = "";
        public PopupMessage(string Title, string Body, string DismissText, string MessageType, string popupType, string popupParameter)
        {
            InitializeComponent();
            parameter = popupParameter;
            if (popupType != "message") Confirm_Change_Button.IsVisible = true;
            if (popupType == "delete")
            {
                Confirm_Change_Button.Text = "Confirm";
                Confirm_Change_Button.Clicked += Delete_Button_Clicked;
            }
            Popup_Title.Text = Title;
            Popup_Text.Text = Body;
            Popup_Close_Button.Text = DismissText;
            if(MessageType.ToLower() == "error")
            {
                HeaderBG.BackgroundColor = Color.FromHex("#f53b3b");
                Popup_Title.TextColor = Color.Default;
                Popup_Close_Button.BackgroundColor = Color.FromHex("#f53b3b");
                Confirm_Change_Button.BackgroundColor = Color.FromHex("#f53b3b");
            }
            else if(MessageType.ToLower() == "warning")
            {
                HeaderBG.BackgroundColor = Color.FromHex("#f7c465");
                Popup_Close_Button.BackgroundColor = Color.FromHex("#f7c465");
                Confirm_Change_Button.BackgroundColor = Color.FromHex("#f7c465");
            }
            else
            {

            }
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PopModalAsync();
            int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
            for (int currModal = 0; currModal < numModals; currModal++)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        private async void Delete_Button_Clicked(object sender, EventArgs e)
        {
            WineAPIService wineAPIService = new WineAPIService();
            var response = wineAPIService.DeleteUserBottle(wineAPIService.GetApiGuid(), parameter);
            await Navigation.PopModalAsync();
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}