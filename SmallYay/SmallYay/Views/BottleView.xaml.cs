using Newtonsoft.Json;
using SmallYay.Models;
using SmallYay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmallYay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottleView : ContentView
    {
        public List<UserBottleForDisplay> myBottles { get; private set; }
        public List<UserBottle> myBottlesApi { get; private set; }
        private readonly WineAPIService wineApiService = new WineAPIService();
        private readonly AutocompleteDataService acds = new AutocompleteDataService();
        public List<string> SortOrder = new List<string>();
        public UserBottleFilter bottleFilter = new UserBottleFilter() { orderBy = "vintner, winename" };

        public BottleView()
        {
            //Application.Current.Properties.Clear();

            InitializeComponent();
            initializeFilterSettings();

            //sortFilterFrame.SizeChanged += SortFilterFrame_SizeChanged;

            //GenerateFilterValues();

            if (!Application.Current.Properties.ContainsKey("bottle_filter_value_timestamp"))
            {    
                GenerateFilterValues();
            }
            else
            {
                DateTime timeStamp = DateTime.Parse(Application.Current.Properties["bottle_filter_value_timestamp"].ToString());
                if (DateTime.UtcNow > timeStamp.AddHours(24))
                {
                    GenerateFilterValues();
                }
            }

            SortOrder = new List<string>() { "Wine Name", "Year: Old to New", "Year: New to Old", "Price: High to Low", "Price: Low to High", "Your Rating", "Category", "Vintner", "Varietal" };
            SortOrderPicker.ItemsSource = SortOrder;

            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, getBottleFilter());
            myBottles = convertBottlesToDisplay(myBottlesApi);

            acds.PopulateAutocompleteLists(false);

            List<WineLocation> locationDetailList = !Application.Current.Properties.ContainsKey("bottle_Filter_Location_Detail") ? new List<WineLocation>() : JsonConvert.DeserializeObject<List<WineLocation>>(Application.Current.Properties["bottle_Filter_Location_Detail"].ToString());
            SortOrder = !Application.Current.Properties.ContainsKey("bottle_Filter_SortOrder") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_SortOrder"].ToString());
            List<string> yearList = !Application.Current.Properties.ContainsKey("bottle_Filter_Year") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_Year"].ToString());
            List<string> categoryList = !Application.Current.Properties.ContainsKey("bottle_Filter_Category") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_Category"].ToString());
            List<string> varietalList = !Application.Current.Properties.ContainsKey("bottle_Filter_Varietal") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_Varietal"].ToString());
            List<string> vintnerList = !Application.Current.Properties.ContainsKey("bottle_Filter_Vintner") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_Vintner"].ToString());
            List<string> ratingList = !Application.Current.Properties.ContainsKey("bottle_Filter_Rating") ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Application.Current.Properties["bottle_Filter_Rating"].ToString());
            List<string> locationList = locationDetailList.Select(x => x.Location).ToList();

            MinYearFilterPicker.ItemsSource = yearList;
            MaxYearFilterPicker.ItemsSource = yearList;
            CategoryFilterPicker.ItemsSource = categoryList;
            VarietalFilterPicker.ItemsSource = varietalList;
            VintnerFilterPicker.ItemsSource = vintnerList;
            LocationFilterPicker.ItemsSource = locationList;
            MinRatingFilterPicker.ItemsSource = ratingList;
            MaxRatingFilterPicker.ItemsSource = ratingList;
            MinRatingFilterPicker.SelectedIndex = 0;
            MaxRatingFilterPicker.SelectedIndex = 0;
            MaxPriceEntry.Text = "";
            MinPriceEntry.Text = "";
            getFilterSettings();
            BindingContext = this;
        }

        private void SortFilterFrame_SizeChanged(object sender, EventArgs e)
        {
            //HeaderHeightLabel.HeightRequest = sortFilterFrame.Height + 10;
        }

        async void GenerateFilterValues()
        {
            Application.Current.Properties["bottle_filter_value_timestamp"] = DateTime.UtcNow.ToString();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, null);
            myBottles = convertBottlesToDisplay(myBottlesApi);

            SortOrder = new List<string>() { "Wine Name", "Year: Old to New", "Year: New to Old", "Price: High to Low", "Price: Low to High", "Your Rating", "Category", "Vintner", "Varietal" };
            List<string> yearList = myBottles.OrderByDescending(x => x.Year).Select(x => x.Year).Distinct().ToList();
            List<string> categoryList = myBottles.OrderBy(x => x.Category).Select(x => x.Category).Distinct().ToList();
            List<string> varietalList = myBottles.OrderBy(x => x.Varietal).Select(x => x.Varietal).Distinct().ToList();
            List<string> vintnerList = myBottles.OrderBy(x => x.Vintner).Select(x => x.Vintner).Distinct().ToList();
            List<string> locationList = myBottles.OrderBy(x => x.location_display).Select(x => x.location_display).Distinct().ToList();
            List<string> ratingList = new List<string>() { "Any", "0.5 Stars", "1 Star", "1.5 Stars", "2 Stars", "2.5 Stars", "3 Stars", "3.5 Stars", "4 Stars", "4.5 Stars", "5 Stars" };
            List<WineLocation> locationDetailList = new List<WineLocation>();
            locationDetailList.Add(new WineLocation()
            {
                Location = "Any",
                City_Town = null,
                Region = null,
                State_Province = null,
                Country = null
            });
            foreach (var item in locationList)
            {
                WineLocation wl = new WineLocation()
                {
                    Location = item,
                    City_Town = myBottles.Where(x => x.location_display == item).Select(x => x.City_Town).FirstOrDefault(),
                    Region = myBottles.Where(x => x.location_display == item).Select(x => x.Region).FirstOrDefault(),
                    State_Province = myBottles.Where(x => x.location_display == item).Select(x => x.State_Province).FirstOrDefault(),
                    Country = myBottles.Where(x => x.location_display == item).Select(x => x.Country).FirstOrDefault()
                };
                locationDetailList.Add(wl);
            }

            yearList.Insert(0, "Any");
            categoryList.Insert(0, "Any");
            varietalList.Insert(0, "Any");
            vintnerList.Insert(0, "Any");
            locationList.Insert(0, "Any");

            string SortOrderJSON = JsonConvert.SerializeObject(SortOrder);
            string yearListJSON = JsonConvert.SerializeObject(yearList);
            string categoryListJSON = JsonConvert.SerializeObject(categoryList);
            string varietalListJSON = JsonConvert.SerializeObject(varietalList);
            string vintnerListJSON = JsonConvert.SerializeObject(vintnerList);
            string ratingListJSON = JsonConvert.SerializeObject(ratingList);
            string locationDetailListJSON = JsonConvert.SerializeObject(locationDetailList);
            Application.Current.Properties["bottle_Filter_Year"] = yearListJSON;
            Application.Current.Properties["bottle_Filter_Category"] = categoryListJSON;
            Application.Current.Properties["bottle_Filter_Varietal"] = varietalListJSON;
            Application.Current.Properties["bottle_Filter_Vintner"] = vintnerListJSON;
            Application.Current.Properties["bottle_Filter_Location_Detail"] = locationDetailListJSON;
            Application.Current.Properties["bottle_Filter_Rating"] = ratingListJSON;
            Application.Current.Properties["bottle_Filter_SortOrder"] = SortOrderJSON;
            await Application.Current.SavePropertiesAsync();
        }

        private List<UserBottleForDisplay> convertBottlesToDisplay(List<UserBottle> inputBottles)
        {
            var outputBottles = new List<UserBottleForDisplay>();
            foreach (var item in myBottlesApi)
            {
                var bottleColorNull = Application.Current.Resources["Default_Wine_Color"] as Color?;
                switch (item.Category)
                {
                    case "Red":
                        bottleColorNull = Application.Current.Resources["Red_Wine_Color"] as Color?;
                        break;
                    case "White":
                        bottleColorNull = Application.Current.Resources["White_Wine_Color"] as Color?;
                        break;
                    case "Rosé":
                        bottleColorNull = Application.Current.Resources["Rose_Wine_Color"] as Color?;
                        break;
                    case "Sparkling / Champagne":
                        bottleColorNull = Application.Current.Resources["Sparkling_Wine_Color"] as Color?;
                        break;
                    case "Dessert / Port / Sherry":
                        bottleColorNull = Application.Current.Resources["Dessert_Wine_Color"] as Color?;
                        break;
                    default:
                        break;
                }
                Color defaultColor = Color.FromHex("#FFFFFF");
                Color bottleColor = bottleColorNull ?? defaultColor;
                string bottle_color = bottleColor.ToHex();
                outputBottles.Add(new UserBottleForDisplay(item, bottle_color));
            }
            return outputBottles;
        }

        private async void Add_Bottle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddBottlePage());
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var userBottle = e.Item as UserBottleForDisplay;
            await Navigation.PushModalAsync(new ViewBottlePage(userBottle));
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            if(SearchGrid.IsVisible)
            {
                var bounds = sortFilterFrame.Bounds;
                bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
                await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
                SearchGrid.IsVisible = false;
                SortGrid.IsVisible = false;
                //HeaderHeightLabel.HeightRequest = 70;
            }
            else
            {
                SearchGrid.IsVisible = true;
                SortGrid.IsVisible = false;
                var bounds = sortFilterFrame.Bounds;
                bounds.Height = this.Height * 0.32;
                await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
                //HeaderHeightLabel.HeightRequest = 70;
            }
        }

        private async void SortFilter_Clicked(object sender, EventArgs e)
        {
            if (SortGrid.IsVisible)
            {
                var bounds = sortFilterFrame.Bounds;
                bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
                await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
                SearchGrid.IsVisible = false;
                SortGrid.IsVisible = false;
                //HeaderHeightLabel.HeightRequest = 70;
            }
            else
            {
                SearchGrid.IsVisible = false;
                SortGrid.IsVisible = true;
                var bounds = sortFilterFrame.Bounds;
                bounds.Height = this.Height * 0.82; // new proportional Height of .82
                await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
                //HeaderHeightLabel.HeightRequest = 70;
            }
        }

        private async void SortFilterApply_Clicked(object sender, EventArgs e)
        {
            saveFilterSettings();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, getBottleFilter());
            myBottles = convertBottlesToDisplay(myBottlesApi);
            bottleList.ItemsSource = myBottles;
            var bounds = sortFilterFrame.Bounds;
            bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
            await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
            SearchGrid.IsVisible = false;
            SortGrid.IsVisible = false;
            //HeaderHeightLabel.HeightRequest = 70;
        }

        private async void SortFilterClear_Clicked(object sender, EventArgs e)
        {
            // to do: filter clear
            MinYearFilterPicker.SelectedIndex = 0;
            MaxYearFilterPicker.SelectedIndex = 0;
            CategoryFilterPicker.SelectedIndex = 0;
            VarietalFilterPicker.SelectedIndex = 0;
            VintnerFilterPicker.SelectedIndex = 0;
            LocationFilterPicker.SelectedIndex = 0;
            MinRatingFilterPicker.SelectedIndex = 0;
            MaxRatingFilterPicker.SelectedIndex = 0;
            LocationFilterPicker.SelectedIndex = 0;
            MaxPriceEntry.Text = "";
            MinPriceEntry.Text = "";
            SortOrderPicker.SelectedItem = "Vintner";
            saveFilterSettings();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, getBottleFilter());
            myBottles = convertBottlesToDisplay(myBottlesApi);
            bottleList.ItemsSource = myBottles;
            var bounds = sortFilterFrame.Bounds;
            bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
            await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
            SearchGrid.IsVisible = false;
            SortGrid.IsVisible = false;
            //HeaderHeightLabel.HeightRequest = 70;
        }

        private async void SearchApply_Clicked(object sender, EventArgs e)
        {
            saveFilterSettings();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, getBottleFilter());
            myBottles = convertBottlesToDisplay(myBottlesApi);
            bottleList.ItemsSource = myBottles;
            var bounds = sortFilterFrame.Bounds;
            bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
            await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
            SearchGrid.IsVisible = false;
            SortGrid.IsVisible = false;
            await Task.Run(async delegate
            {
                await Task.Delay(3000);
                //HeaderHeightLabel.HeightRequest = 70;
            });
        }

        private async void SearchClear_Clicked(object sender, EventArgs e)
        {
            var bounds = sortFilterFrame.Bounds;
            bounds.Height = this.Height * 0.08; // reset proportional Height to 0.08
            //HeaderHeightLabel.HeightRequest = 70;
            //SearchEntry.Unfocus();
            await sortFilterFrame.LayoutTo(bounds, 300, Easing.SinInOut);
            SearchGrid.IsVisible = false;
            SortGrid.IsVisible = false;
            SearchEntry.Text = "";
            saveFilterSettings();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, getBottleFilter());
            myBottles = convertBottlesToDisplay(myBottlesApi);
            bottleList.ItemsSource = myBottles;
        }

        async void saveFilterSettings()
        {
            Application.Current.Properties["bottle_MinYear"] = MinYearFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_MaxYear"] = MaxYearFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_Category"] = CategoryFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_Varietal"] = VarietalFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_Vintner"] = VintnerFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_Location"] = LocationFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_MinPrice"] = MinPriceEntry.Text;
            Application.Current.Properties["bottle_MaxPrice"] = MaxPriceEntry.Text;
            Application.Current.Properties["bottle_MinRating"] = MinRatingFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_MaxRating"] = MaxRatingFilterPicker.SelectedItem;
            Application.Current.Properties["bottle_SortOrder"] = SortOrderPicker.SelectedItem;
            Application.Current.Properties["bottle_SearchQuery"] = SearchEntry.Text;
            await Application.Current.SavePropertiesAsync();
        }

        void getFilterSettings()
        {
            MinYearFilterPicker.SelectedItem = Application.Current.Properties["bottle_MinYear"];
            MaxYearFilterPicker.SelectedItem = Application.Current.Properties["bottle_MaxYear"];
            CategoryFilterPicker.SelectedItem = Application.Current.Properties["bottle_Category"];
            VarietalFilterPicker.SelectedItem = Application.Current.Properties["bottle_Varietal"];
            VintnerFilterPicker.SelectedItem = Application.Current.Properties["bottle_Vintner"];
            LocationFilterPicker.SelectedItem = Application.Current.Properties["bottle_Location"];
            MinRatingFilterPicker.SelectedItem = Application.Current.Properties["bottle_MinRating"];
            MaxRatingFilterPicker.SelectedItem = Application.Current.Properties["bottle_MaxRating"];
            SortOrderPicker.SelectedItem = Application.Current.Properties["bottle_SortOrder"];
            MaxPriceEntry.Text = Application.Current.Properties["bottle_MaxPrice"].ToString();
            MinPriceEntry.Text = Application.Current.Properties["bottle_MinPrice"].ToString();
            SearchEntry.Text = (Application.Current.Properties["bottle_SearchQuery"] ?? "").ToString();
        }

        async void initializeFilterSettings()
        {
            if (!Application.Current.Properties.ContainsKey("bottle_MinYear")) Application.Current.Properties["bottle_MinYear"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_MaxYear")) Application.Current.Properties["bottle_MaxYear"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_Category")) Application.Current.Properties["bottle_Category"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_Varietal")) Application.Current.Properties["bottle_Varietal"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_Vintner")) Application.Current.Properties["bottle_Vintner"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_Location")) Application.Current.Properties["bottle_Location"] = "Any";
            if (!Application.Current.Properties.ContainsKey("bottle_MinRating")) Application.Current.Properties["bottle_MinRating"] = "0";
            if (!Application.Current.Properties.ContainsKey("bottle_MaxRating")) Application.Current.Properties["bottle_MaxRating"] = "0";
            if (!Application.Current.Properties.ContainsKey("bottle_MaxPrice")) Application.Current.Properties["bottle_MaxPrice"] = "";
            if (!Application.Current.Properties.ContainsKey("bottle_MinPrice")) Application.Current.Properties["bottle_MinPrice"] = "";
            if (!Application.Current.Properties.ContainsKey("bottle_SortOrder")) Application.Current.Properties["bottle_SortOrder"] = "Vintner";
            if (!Application.Current.Properties.ContainsKey("bottle_SearchQuery")) Application.Current.Properties["bottle_SearchQuery"] = "";
            await Application.Current.SavePropertiesAsync();
        }

        UserBottleFilter getBottleFilter()
        {
            UserBottleFilter output = new UserBottleFilter();
            string sortField = (Application.Current.Properties["bottle_SortOrder"] ?? "Vintner").ToString();
            string sortOrder = "vintner, winename";
            switch (sortField)
            {
                case "Wine Name":
                    sortOrder = "winename";
                    break;
                case "Year: Old to New":
                    sortOrder = "year asc, vintner, winename";
                    break;
                case "Year: New to Old":
                    sortOrder = "year desc, vintner, winename";
                    break;
                case "Price: High to Low":
                    sortOrder = "price_paid desc, vintner, winename";
                    break;
                case "Price: Low to High":
                    sortOrder = "price_paid asc, vintner, winename";
                    break;
                case "Your Rating":
                    sortOrder = "user_rating desc, vintner, winename";
                    break;
                case "Category":
                    sortOrder = "category, vintner, winename";
                    break;
                case "Vintner":
                    sortOrder = "vintner, winename";
                    break;
                case "Varietal":
                    sortOrder = "varietal, winename";
                    break;
                default:
                    break;
            }

            string minYear = (string)Application.Current.Properties["bottle_MinYear"];
            string maxYear = (string)Application.Current.Properties["bottle_MaxYear"];
            string category = (string)Application.Current.Properties["bottle_Category"];
            string varietal = (string)Application.Current.Properties["bottle_Varietal"];
            string vintner = (string)Application.Current.Properties["bottle_Vintner"];
            string location = (string)Application.Current.Properties["bottle_Location"];
            string minPrice = (string)Application.Current.Properties["bottle_MinPrice"];
            string maxPrice = (string)Application.Current.Properties["bottle_MaxPrice"];
            string minRating = ((string)Application.Current.Properties["bottle_MinRating"] ?? "0").Replace(" Stars", "");
            string maxRating = ((string)Application.Current.Properties["bottle_MaxRating"] ?? "0").Replace(" Stars", "");
            string searchQuery = (string)Application.Current.Properties["bottle_SearchQuery"];
            minRating = minRating == "Any" ? "0" : minRating;
            maxRating = maxRating == "Any" ? "0" : maxRating;
            string sortOrderStr = (string)Application.Current.Properties["bottle_SortOrder"];

            string minYearFilter = minYear == "Any" ? null : minYear;
            string maxYearFilter = maxYear == "Any" ? null : maxYear;
            string categoryFilter = category == "Any" ? null : category;
            string varietalFilter = varietal == "Any" ? null : varietal;
            string vintnerFilter = vintner == "Any" ? null : vintner;
            string locationFilter = location == "Any" ? null : location;
            string minPriceFilter = minPrice == "" ? null : minPrice;
            string maxPriceFilter = maxPrice == "" ? null : maxPrice;
            int minRatingFilter = MinRatingFilterPicker.SelectedIndex == 0 ? 0 : (int)(Double.Parse(minRating) * 2);
            int maxRatingFilter = MaxRatingFilterPicker.SelectedIndex == 0 ? 0 : (int)(Double.Parse(maxRating) * 2);

            List<WineLocation> locationDetailList = !Application.Current.Properties.ContainsKey("bottle_Filter_Location_Detail") ? new List<WineLocation>() : JsonConvert.DeserializeObject<List<WineLocation>>(Application.Current.Properties["bottle_Filter_Location_Detail"].ToString());
            var thisLocation = locationDetailList.Where(x => x.Location == location).FirstOrDefault();
            string regionFilter = null;
            string countryFilter = null;
            string city_townFilter = null;
            string state_provinceFilter = null;

            if (thisLocation != null)
            {
                regionFilter = thisLocation.Region;
                countryFilter = thisLocation.Country;
                city_townFilter = thisLocation.City_Town;
                state_provinceFilter = thisLocation.State_Province;
            }

            output = new UserBottleFilter()
            {
                minYear = minYearFilter,
                maxYear = maxYearFilter,
                Category = categoryFilter,
                Varietal = varietalFilter,
                Vintner = vintnerFilter,
                minPrice = minPriceFilter,
                maxPrice = maxPriceFilter,
                minRating = minRatingFilter,
                maxRating = maxRatingFilter,
                orderBy = sortOrder,
                Region = regionFilter,
                Country = countryFilter,
                City_Town = city_townFilter,
                State_Province = state_provinceFilter,
                searchQuery = searchQuery
            };
            return output;
        }

        class WineLocation
        {
            public string Location { get; set; }
            public string City_Town { get; set; }
            public string Region { get; set; }
            public string State_Province { get; set; }
            public string Country { get; set; }
        }

        private void SearchEntry_Completed(object sender, EventArgs e)
        {
            SearchApply_Clicked(sender, e);
        }


        //private void HeaderHeightLabel_SizeChanged(object sender, EventArgs e)
        //{
        //    HeaderHeightLabel.HeightRequest = 70;
        //}

        //private void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        //{
        //    HeaderHeightLabel.HeightRequest = 70;
        //}
    }
}