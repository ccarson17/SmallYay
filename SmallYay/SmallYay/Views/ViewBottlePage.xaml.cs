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
    public partial class ViewBottlePage : ContentPage
    {
        private readonly WineAPIService wineApiService = new WineAPIService();
        string userbottle_guid = "";

        public ViewBottlePage(UserBottleForDisplay userBottle)
        {
            if (userBottle.Year == "0") userBottle.Year = "Non-Vintage";

            this.BindingContext = userBottle;
            userbottle_guid = userBottle.guid;
            this.Appearing += ViewBottlePage_OnAppearing;
            InitializeComponent();

            DrinkButton.IsVisible = userBottle.drink_date == null;
            DeleteButton.IsVisible = userBottle.drink_date != null;
            ReviewButton.IsVisible = userBottle.drink_date != null;
            LocateButton.IsVisible = userBottle.drink_date == null;


            //CancelButton.IsVisible = !DeleteButton.IsVisible;

            BuildReviewStars(userBottle.user_rating ?? 0);
            starGroup.Opacity = userBottle.user_rating == null ? 0.1 : 1;
            //if(DeleteButton.IsVisible)
            //{

            //    Button newCancelButton = new Button()
            //    {
            //        Style = (Style)Application.Current.Resources["DefaultButtonStyle"],
            //        ImageSource = ImageSource.FromFile("baseline_cancel_white_48dp.png"),
            //        Text = "Close",
            //        HorizontalOptions = LayoutOptions.FillAndExpand
            //    };
            //    newCancelButton.Clicked += Cancel_Button_Clicked;
            //    newCancelButton.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 0);

            //    ButtonGrid.Children.Add(newCancelButton, 1, 2);
            //    Grid.SetColumnSpan(newCancelButton, 2);
            //}
            //this.BindingContext = userBottle;
        }

        private async void Drink_Button_Clicked(object sender, EventArgs e)
        {
            UserBottleForDisplay userBottle = this.BindingContext as UserBottleForDisplay;
            string drinkBottleResponse = wineApiService.DrinkUserBottle(userBottle);
            await Navigation.PushModalAsync(new PopupMessage("Cheers!", "Enjoy! You can use the History page to add a review and score for this bottle.", "OK", "default", "message", ""));
            if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {
                Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                await Application.Current.SavePropertiesAsync();
            }
            //await Navigation.PopModalAsync();
        }

        private async void Copy_Button_Clicked(object sender, EventArgs e)
        {
            UserBottleForDisplay userBottle = this.BindingContext as UserBottleForDisplay;
            var userBottleToCreate = new UserBottle()
            {
                owner_guid = userBottle.owner_guid,
                rack_guid = null,
                rack_name = null,
                row = 0,
                col = 0,
                bottle_guid = userBottle.bottle_guid,
                Year = userBottle.Year,
                Vintner = userBottle.Vintner,
                WineName = userBottle.WineName,
                Category = userBottle.Category,
                Varietal = userBottle.Varietal,
                City_Town = userBottle.City_Town,
                Region = userBottle.Region,
                State_Province = userBottle.State_Province,
                Country = userBottle.Country,
                ExpertRatings = userBottle.ExpertRatings,
                Size = userBottle.Size,
                ABV = userBottle.ABV,
                WinemakerNotes = userBottle.WinemakerNotes,
                where_bought = userBottle.where_bought,
                price_paid = userBottle.price_paid,
                user_rating = userBottle.user_rating,
                drink_date = null,
                user_notes = userBottle.user_notes
            };
            userBottleToCreate = wineApiService.CreateNewUserBottle(userBottleToCreate);
            await Navigation.PushModalAsync(new PopupMessage("Success!", "A new copy of this bottle has been added to your collection. It can be edited on the Bottles page.", "OK", "default", "message", ""));
            if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {
                Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                await Application.Current.SavePropertiesAsync();
            }
            //await Navigation.PopModalAsync();
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Expander_Tapped(object sender, EventArgs e)
        {
            var expander = detailExpander;
            if (!expander.IsExpanded)
            {
                var animate = new Animation {
                    { 0, 1, new Animation(d => expand_Label.FadeTo(1)) },
                    { 0, 1, new Animation(d => contract_Label.FadeTo(0)) }
                };
                animate.Commit(this, "a", length: 200); //animation takes 500 msec
            }
            else
            {
                var animate = new Animation {
                    { 0, 1, new Animation(d => expand_Label.FadeTo(0)) },
                    { 0, 1, new Animation(d => contract_Label.FadeTo(1)) }
                };
                animate.Commit(this, "a", length: 200); //animation takes 500 msec
            }
        }

        private void Expanded_Content_Tapped(object sender, EventArgs e)
        {
            detailExpander.IsExpanded = !detailExpander.IsExpanded;
            Expander_Tapped(detailExpander, new EventArgs());
        }

        private async void Review_Button_Clicked(object sender, EventArgs e)
        {
            UserBottleForDisplay userBottle = this.BindingContext as UserBottleForDisplay;
            await Navigation.PushModalAsync(new ReviewView(userBottle));
        }

        private async void Locate_Button_Clicked(object sender, EventArgs e)
        {
            UserBottleForDisplay userBottle = this.BindingContext as UserBottleForDisplay;
            await Navigation.PushModalAsync(new LocateView(userBottle));
        }

        private async void Delete_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PopupMessage("Are you sure?", "Deleting a bottle is permanent. Are you sure?", "Cancel", "warning", "delete", userbottle_guid));
            if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {
                Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                await Application.Current.SavePropertiesAsync();
            }
            //await Navigation.PopModalAsync();
        }

        private async void ViewBottlePage_OnAppearing(object sender, EventArgs e)
        {
            var userBottle = this.BindingContext as UserBottleForDisplay;
            var myBottlesApi = wineApiService.GetUserBottle(wineApiService.GetApiGuid(), userBottle.guid, false);
            var bindingBottle = new UserBottleForDisplay(myBottlesApi);
            if (String.IsNullOrEmpty(bindingBottle.guid)) await Navigation.PopModalAsync();
            else
            {
                if (bindingBottle.drink_date != null)
                    bindingBottle.drink_date = TimeZoneInfo.ConvertTimeFromUtc(bindingBottle.drink_date ?? DateTime.Now, TimeZoneInfo.Local);
                if (bindingBottle.created_date != null)
                    bindingBottle.created_date = TimeZoneInfo.ConvertTimeFromUtc(bindingBottle.created_date ?? DateTime.Now, TimeZoneInfo.Local);
                this.BindingContext = bindingBottle;
            }
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            var userBottle = this.BindingContext as UserBottleForDisplay;
            await Navigation.PushModalAsync(new AddBottlePage(new UserBottle(userBottle)));
        }

        private void BuildReviewStars(int review)
        {
            foreach (var item in starGroup.Children)
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