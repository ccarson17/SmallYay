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
    public partial class ReviewView : ContentPage
    {
        private readonly WineAPIService wineApiService = new WineAPIService();

        public ReviewView(UserBottleForDisplay userBottle)
        {
            InitializeComponent();
            this.BindingContext = userBottle;
            ReviewEditor.Focused += (sender, e) => {
                ReviewEditorFrame.BorderColor = Color.DarkSlateGray;
                MainFrame.VerticalOptions = LayoutOptions.StartAndExpand;
            };
            ReviewEditor.Unfocused += (sender, e) => {
                ReviewEditorFrame.BorderColor = Color.LightGray;
                MainFrame.VerticalOptions = LayoutOptions.Center;
            };
            BuildReviewStars(userBottle.user_rating ?? 0);
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            UserBottleForDisplay userBottle = this.BindingContext as UserBottleForDisplay;
            string returnCode = wineApiService.ReviewUserBottle(userBottle);
            if(returnCode != "Created")
            {
                await Navigation.PushModalAsync(new PopupMessage("Failed", "API Returned status: " + returnCode, "OK", "error", "message", ""));
            }
            else
            {
                for (int i = 0; i < Navigation.ModalStack.Count(); i++)
                    await Navigation.PopModalAsync();
            }
            if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {
                Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                await Application.Current.SavePropertiesAsync();
            }
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            var slider = sender as Slider;
            int sliderInt = (int)Math.Round(slider.Value);
            slider.Value = sliderInt;
            BuildReviewStars(sliderInt);
        }

        private void BuildReviewStars(int review)
        {
            foreach(var item in starGroup.Children)
            {
                var thisImage = item as Image;
                int imageNum = 0;
                if (thisImage == star1) imageNum = 1;
                else if (thisImage == star2) imageNum = 2;
                else if (thisImage == star3) imageNum = 3;
                else if (thisImage == star4) imageNum = 4;
                else if (thisImage == star5) imageNum = 5;
                if (review >= 2 * imageNum) thisImage.Source = ImageSource.FromFile("baseline_star_border_black_48dp_full.png");
                else if (review + 1 == 2 * imageNum) thisImage.Source = ImageSource.FromFile("baseline_star_border_black_48dp_half.png");
                else thisImage.Source = ImageSource.FromFile("baseline_star_border_black_48dp_empty.png");
            }
        }
    }
}