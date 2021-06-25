using Newtonsoft.Json;
using SmallYay.Config;
using SmallYay.Models;
using SmallYay.Services;
using Syncfusion.SfAutoComplete.XForms;
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
    public partial class AddBottlePage : ContentPage
    {
        private readonly WineAPIService wineApiService = new WineAPIService();
        private readonly AutocompleteDataService acds = new AutocompleteDataService();
        private bool isEdit = false;
        private UserBottle ub = new UserBottle();
        private string lastSender = "";
        private ErrorLog errorLog = new ErrorLog(false);

        public AddBottlePage()
        {
            ub.Size = "750 ml";
            this.BindingContext = ub;
            InitializeComponent();
            setupAutocomplete();

            List<string> wineCategories = new List<string>() { "Red", "White", "Rosé", "Sparkling / Champagne", "Dessert / Port / Sherry" };
            List<string> wineYears = new List<string>() { "Non-Vintage" };
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
            {
                wineYears.Add(i.ToString());
            }
            CategoryPicker.ItemsSource = wineCategories;
            YearPicker.ItemsSource = wineYears;
            App.Current.Properties["current_rack"] = "";
            this.Appearing += GetRackInfo;
        }

        private void AutoCompleteFocusChanged(object sender, FocusChangedEventArgs e)
        {
            string senderText = (sender as SfAutoComplete).Text;
            if (e.HasFocus)
            {
                if (senderText == Constants.Vintner_Default ||
                    senderText == Constants.Varietal_Default ||
                    senderText == Constants.Wine_Name_Default ||
                    senderText == Constants.Category_Default ||
                    senderText == Constants.Year_Default ||
                    senderText == Constants.Region_Default ||
                    senderText == Constants.City_Town_Default ||
                    senderText == Constants.State_Province_Default ||
                    senderText == Constants.Country_Default ||
                    senderText == Constants.Where_Bought_Default ||
                    senderText == Constants.Price_Paid_Default)
                {
                    Application.Current.Properties["guid:" + (sender as SfAutoComplete).Id] = (sender as SfAutoComplete).Text;
                    Application.Current.SavePropertiesAsync();
                    (sender as SfAutoComplete).Text = "";
                    (sender as SfAutoComplete).SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
                }
            }
            else
            {
                if (senderText == "")
                {
                    (sender as SfAutoComplete).Text = Application.Current.Properties["guid:" + (sender as SfAutoComplete).Id].ToString();
                    (sender as SfAutoComplete).SetDynamicResource(SfAutoComplete.TextColorProperty, "PlaceholderColor");
                }
            }
        }

        private void setupAutocomplete()
        {
            if (!String.IsNullOrEmpty(VintnerAC.Text)) VintnerAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            VintnerAC.Text = String.IsNullOrEmpty(VintnerAC.Text) ? Constants.Vintner_Default : VintnerAC.Text;
            VintnerAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("vintner").Result ?? "") ?? new List<string>();
            VintnerAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(VarietalAC.Text)) VarietalAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            VarietalAC.Text = String.IsNullOrEmpty(VarietalAC.Text) ? Constants.Varietal_Default : VarietalAC.Text;
            VarietalAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("varietal").Result ?? "") ?? new List<string>();
            VarietalAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(WinenameAC.Text)) WinenameAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            WinenameAC.Text = String.IsNullOrEmpty(WinenameAC.Text) ? Constants.Wine_Name_Default : WinenameAC.Text;
            WinenameAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("winename").Result ?? "") ?? new List<string>();
            WinenameAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(RegionAC.Text)) RegionAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            RegionAC.Text = String.IsNullOrEmpty(RegionAC.Text) ? Constants.Region_Default : RegionAC.Text;
            RegionAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("region").Result ?? "") ?? new List<string>();
            RegionAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(CityAC.Text)) CityAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            CityAC.Text = String.IsNullOrEmpty(CityAC.Text) ? Constants.City_Town_Default : CityAC.Text;
            CityAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("city").Result ?? "") ?? new List<string>();
            CityAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(StateAC.Text)) StateAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            StateAC.Text = String.IsNullOrEmpty(StateAC.Text) ? Constants.State_Province_Default : StateAC.Text;
            StateAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("state").Result ?? "") ?? new List<string>();
            StateAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(CountryAC.Text)) CountryAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            CountryAC.Text = String.IsNullOrEmpty(CountryAC.Text) ? Constants.Country_Default : CountryAC.Text;
            CountryAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("country").Result ?? "") ?? new List<string>();
            CountryAC.FocusChanged += AutoCompleteFocusChanged;

            if (!String.IsNullOrEmpty(WhereBoughtAC.Text)) WhereBoughtAC.SetDynamicResource(SfAutoComplete.TextColorProperty, "ItemTextColor");
            WhereBoughtAC.Text = String.IsNullOrEmpty(WhereBoughtAC.Text) ? Constants.Where_Bought_Default : WhereBoughtAC.Text;
            WhereBoughtAC.AutoCompleteSource = JsonConvert.DeserializeObject<List<string>>(acds.GetAutocomplete("where_bought").Result ?? "") ?? new List<string>();
            WhereBoughtAC.FocusChanged += AutoCompleteFocusChanged;
        }

        public AddBottlePage(UserBottle userBottle)
        {
            InitializeComponent();
            ub = userBottle;
            this.BackgroundColor = Color.FromHex("#00000000");
            this.BindingContext = ub;
            List<string> wineCategories = new List<string>() { "Red", "White", "Rosé", "Sparkling / Champagne", "Dessert / Port / Sherry" };
            List<string> wineYears = new List<string>() { "Non-Vintage" };
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
            {
                wineYears.Add(i.ToString());
            }
            CategoryPicker.ItemsSource = wineCategories;
            CategoryPicker.SelectedItem = userBottle.Category;
            YearPicker.ItemsSource = wineYears;
            YearPicker.SelectedItem = userBottle.Year;
            isEdit = true;

            this.Appearing += GetRackInfoEdit;
            setupAutocomplete();
        }

        public AddBottlePage(string commandParameter)
        {
            string[] cpArray = commandParameter.Split(new string[] { "|^|"}, StringSplitOptions.None);
            var userBottle = new UserBottle()
            {
                rack_guid = cpArray[0],
                rack_name = cpArray[1],
                row = int.Parse(cpArray[2]) + 1,
                col = int.Parse(cpArray[3]) + 1
            };
            App.Current.Properties["current_rack"] = userBottle.rack_name;
            this.BindingContext = userBottle;
            InitializeComponent();
            setupAutocomplete();
            List<string> wineCategories = new List<string>() { "Red", "White", "Rosé", "Sparkling / Champagne", "Dessert / Port / Sherry" };
            List<string> wineYears = new List<string>() { "Non-Vintage" };
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
            {
                wineYears.Add(i.ToString());
            }
            CategoryPicker.ItemsSource = wineCategories;
            ub = userBottle;
            this.Appearing += GetRackInfo;
            YearPicker.ItemsSource = wineYears;
            RackPicker.SelectedItem = userBottle.rack_name;
            RackPicker.IsEnabled = false;
            RackRow.Text = userBottle.row.ToString();
            RackRow.IsEnabled = false;
            RackCol.Text = userBottle.col.ToString();
            RackCol.IsEnabled = false;
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                // run process to post form data via API
                UserBottle userBottle = this.BindingContext as UserBottle;
                userBottle.Year = (YearPicker.SelectedItem ?? -1).ToString();
                userBottle.Category = (CategoryPicker.SelectedItem ?? "Not Selected").ToString();
                string validationMessage = "";
                if (userBottle.Year == "-1") { validationMessage += "Year is Required\n"; }
                if (userBottle.Year == "Not Selected") { validationMessage += "Category is Required\n"; }
                if (userBottle.Vintner == null || userBottle.Vintner == Constants.Vintner_Default) { validationMessage += "Vintner is Required\n"; userBottle.Vintner = null; }
                if (userBottle.Varietal == null || userBottle.Varietal == Constants.Varietal_Default) { validationMessage += "Varietal is Required\n"; userBottle.Varietal = null; }
                if (userBottle.WineName == null || userBottle.WineName == Constants.Wine_Name_Default) { validationMessage += "Wine Name is Required"; userBottle.WineName = null; }
                if (userBottle.Region == Constants.Region_Default) userBottle.Region = null;
                if (userBottle.City_Town == Constants.City_Town_Default) userBottle.City_Town = null;
                if (userBottle.State_Province == Constants.State_Province_Default) userBottle.State_Province = null;
                if (userBottle.Country == Constants.Country_Default) userBottle.Country = null;
                if (userBottle.where_bought == Constants.Where_Bought_Default) userBottle.where_bought = null;
                if (userBottle.Size == "750 ml") userBottle.Size = "750";
                if(userBottle.ABV != null) userBottle.ABV = userBottle.ABV.Replace("%", "");
                if (validationMessage != "")
                {
                    //await DisplayAlert("Validation Error", validationMessage, "OK");
                    await Navigation.PushModalAsync(new PopupMessage("Validation Error", validationMessage, "OK", "error", "message", ""));
                    return;
                }
                userBottle.owner_guid = wineApiService.GetApiGuid();
                if ((userBottle.rack_name ?? "Unassigned") != "Unassigned")
                {
                    if (userBottle.row < 1 || userBottle.col < 1)
                    {
                        await Navigation.PushModalAsync(new PopupMessage("Validation Error", "Row and Column location must be chosen if a rack is chosen.", "OK", "error", "message", ""));
                        return;
                    }
                    var myRacks = wineApiService.GetMyRacks(userBottle.owner_guid, false);
                    var thisRack = myRacks.Where(x => x.rack_name == userBottle.rack_name).FirstOrDefault();
                    userBottle.rack_guid = thisRack.guid;
                }
                else
                {
                    userBottle.rack_name = null;
                    userBottle.row = 0;
                    userBottle.col = 0;
                }
                //if (userBottle.bottle_guid == null)
                //{
                //    string bottleMatch = wineApiService.GetMyBottleMatchGuid(userBottle);
                //    if (String.IsNullOrEmpty(bottleMatch))
                //    {
                //        string api_response = wineApiService.CreateNewBottle(userBottle);

                //        if (api_response.Contains("error")) // handle API errors, including validation
                //        {
                //            //await DisplayAlert("Error", api_response, "OK");
                //            await Navigation.PushModalAsync(new PopupMessage("Error", api_response, "OK", "error", "message", ""));
                //        }
                //        else
                //        {
                //            userBottle.bottle_guid = api_response;
                //            //await DisplayAlert("Success", "Bottle created. Guid is: " + api_response, "OK"); // this is for testing. Instead at this point, need to create the UserBottle object based on this bottle Guid.
                //        }
                //    }
                //    else
                //    {
                //        userBottle.bottle_guid = bottleMatch;
                //    }
                //}
                UserBottle userBottleCreated = new UserBottle();
                if (isEdit)
                {
                    //var response = wineApiService.UpdateUserBottle(userBottle); // replace with update function
                    var response = wineApiService.UpdateUserBottleEz(userBottle); // replace with update function
                    if (response.Contains("Conflict with existing bottle"))
                    {
                        await Navigation.PushModalAsync(new PopupMessage("Warning", "The rack location you have chosen is already occupied.", "OK", "error", "message", ""));
                    }
                    else
                    {
                        if (response != "Created")
                            await Navigation.PushModalAsync(new PopupMessage("Error", "API Error: Status - " + response, "OK", "error", "message", ""));
                        else
                            await Navigation.PushModalAsync(new PopupMessage("Success!", "Bottle Updated!", "OK", "default", "message", ""));
                    }
                }
                else
                {
                    //userBottleCreated = wineApiService.CreateNewUserBottle(userBottle);
                    userBottleCreated = wineApiService.CreateNewBottleEz(userBottle);
                    if (userBottleCreated.guid.StartsWith("API Error:"))
                        await Navigation.PushModalAsync(new PopupMessage("Error", userBottleCreated.guid, "OK", "error", "message", ""));
                    else
                        await Navigation.PushModalAsync(new PopupMessage("Cheers!", "New Bottle Added!", "OK", "default", "message", ""));
                }
                if (Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
                {
                    Application.Current.Properties.Remove("bottle_filter_value_timestamp");
                    await Application.Current.SavePropertiesAsync();
                }
                acds.PopulateAutocompleteLists(true);
            }
            catch(Exception ex)
            {
                errorLog.LogError(ex);
            }
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void YearPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YearPicker.SelectedIndex != -1) YearLabel.IsVisible = false;
            else YearLabel.IsVisible = true;
        }

        private void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategoryPicker.SelectedIndex != -1) CategoryLabel.IsVisible = false;
            else CategoryLabel.IsVisible = true;
        }

        private void RackPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RackPicker.SelectedIndex != -1) RackLabel.IsVisible = false;
            else RackLabel.IsVisible = true;
        }

        private void Add_Rack_Button_Clicked(object sender, EventArgs e)
        {
            FadeInAddRacks();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (tapsender, tape) =>
            {
                FadeOutAddRacks();
            };
            AddRackPopupBackground.GestureRecognizers.Add(tap);
        }

        private void RackViewSaveButton_Clicked(object sender, EventArgs e)
        {
            UserBottle userBottle = this.BindingContext as UserBottle;
            Rack newRack = new Rack()
            {
                owner_guid = wineApiService.GetApiGuid(),
                rows = userBottle.row,
                cols = userBottle.col,
                rack_name = userBottle.rack_name          
            };
            wineApiService.CreateNewRack(newRack);
            if (newRack.owner_guid.StartsWith("API Error: "))
            {
                //DisplayAlert("Error", newRack.owner_guid, "Dismiss");
                Navigation.PushModalAsync(new PopupMessage("Error", newRack.owner_guid, "Dismiss", "error", "message", ""));
            }
            else
            {
                GetRackInfo(sender, e);
                FadeOutAddRacks();
            }
        }

        private void RackViewCancelButton_Clicked(object sender, EventArgs e)
        {
            FadeOutAddRacks();
        }

        private void GetRackInfo(object sender, EventArgs e)
        {
            string myUserId = wineApiService.GetApiGuid();
            var myRacks = wineApiService.GetMyRacks(myUserId, false);

            var rackPicker = myRacks.OrderBy(x => x.rack_name).Select(x => x.rack_name).ToList();

            string currentRack = App.Current.Properties["current_rack"] as String ?? "";

            if (myRacks.Count() > 0)
            {
                rackPicker.Insert(0, "Unassigned");
                RackPicker.ItemsSource = rackPicker;
                RackPicker.SelectedIndex = 0;
                NoRacksGrid.IsVisible = false;
                RacksGrid.IsVisible = true;
                if (currentRack != "") RackPicker.SelectedItem = currentRack;
            }
            else
            {
                RackPicker.ItemsSource = rackPicker;
                NoRacksGrid.IsVisible = true;
                RacksGrid.IsVisible = false;
            }
        }

        private void GetRackInfoEdit(object sender, EventArgs e)
        {
            string myUserId = wineApiService.GetApiGuid();
            var myRacks = wineApiService.GetMyRacks(myUserId, false);
            var selectedRack = ub.rack_name;
            var rackPicker = myRacks.OrderBy(x => x.rack_name).Select(x => x.rack_name).ToList();

            if (myRacks.Count() > 0)
            {
                rackPicker.Insert(0, "Unassigned");
                RackPicker.ItemsSource = rackPicker;
                NoRacksGrid.IsVisible = false;
                RacksGrid.IsVisible = true;
                if (ub.row != 0 && ub.col != 0)
                {
                    RackPicker.SelectedItem = selectedRack;
                    RackRow.Text = ub.row.ToString();
                    RackCol.Text = ub.col.ToString();
                }
                else
                {
                    //RackPicker.SelectedIndex = 0;
                    RackRow.Text = "";
                    RackCol.Text = "";
                }
            }
            else
            {
                RackPicker.ItemsSource = rackPicker;
                NoRacksGrid.IsVisible = true;
                RacksGrid.IsVisible = false;
            }
        }

        private void FadeInAddRacks()
        {
            this.AddRackPopup.IsVisible = true;
            this.AddRackPopupBackground.IsVisible = true;
            this.AddRackPopup.AnchorX = 0.5;
            this.AddRackPopup.AnchorY = 0.5;

            Animation scaleAnimation = new Animation(
                f => this.AddRackPopup.Scale = f,
                0.1,
                1,
                Easing.SinInOut);

            Animation fadeAnimation = new Animation(
                f => this.AddRackPopup.Opacity = f,
                0.2,
                1,
                Easing.SinInOut);

            Animation bgFadeAnimation = new Animation(
                f => this.AddRackPopupBackground.Opacity = f,
                0.2,
                1,
                Easing.SinInOut);

            scaleAnimation.Commit(this.AddRackPopup, "popupScaleAnimation", 150);
            fadeAnimation.Commit(this.AddRackPopup, "popupFadeAnimation", 150);
            bgFadeAnimation.Commit(this.AddRackPopupBackground, "popupBgFadeAnimation", 150);
        }

        private async void FadeOutAddRacks()
        {
            await Task.WhenAny<bool>
              (
                this.AddRackPopup.FadeTo(0, 150, Easing.SinInOut),
                this.AddRackPopupBackground.FadeTo(0, 150, Easing.SinInOut)
              );

            this.AddRackPopup.IsVisible = false;
            this.AddRackPopupBackground.IsVisible = false;
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}