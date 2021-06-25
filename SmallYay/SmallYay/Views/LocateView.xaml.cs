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
    public partial class LocateView : ContentPage
    {
        private readonly WineAPIService wineApiService = new WineAPIService();
        private readonly AutocompleteDataService acds = new AutocompleteDataService();
        private int BottleSize = 30;
        private int RackSlotSize = 32;
        private int GridSize = 34;
        private int cornerRadius = 3;
        private int frameCornerRadius = 15;
        private int gridPadding = 5;
        private int gridSpacing = 1;
        private bool cacheRequest = false;
        private Dictionary<int, Guid> imageDict = new Dictionary<int, Guid>();
        private List<Rack> myRacksApi = new List<Rack>();
        private BoxView selectedBoxView = new BoxView();
        private Color selectedBottleColor = Color.White;

        public LocateView(UserBottleForDisplay userBottle)
        {
            InitializeComponent();
            this.BindingContext = userBottle;
            GridDisplaySV.Content = Build_Grid_Content(userBottle);
            StartAnimation();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartAnimation();
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void StartAnimation()
        {
            await Task.WhenAll(Task.Delay(1200), selectedBoxView.FadeTo(0, 1100), selectedBoxView.ScaleTo(2, 700));
            await Task.WhenAll(Task.Delay(150), selectedBoxView.ScaleTo(0.7, 10));
            await Task.WhenAll(Task.Delay(150), selectedBoxView.FadeTo(1, 10));
            StartAnimation();
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private ScrollView Build_Grid_Content(UserBottleForDisplay userBottle)
        {
            //setSizes(3);
            myRacksApi = wineApiService.GetMyRacks(wineApiService.GetApiGuid(), cacheRequest);
            var rack = myRacksApi.Where(x => x.guid == userBottle.rack_guid).FirstOrDefault();
            var myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", cacheRequest, null);
            //acds.PopulateAutocompleteLists(false);
            cacheRequest = true; // Allow 30s crequest caching after initial load
            int mainLayoutRow = 0;

            var outerScrollView = new ScrollView() { HorizontalOptions = LayoutOptions.Fill, Orientation = ScrollOrientation.Vertical };

            var mainLayout = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                RowSpacing = 0
            };

            if (rack != null)
            {
                var rackLayout = new Grid()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    RowSpacing = 0
                };
                var thisRacksBottles = myBottlesApi.Where(x => x.rack_guid == rack.guid).ToList();
                Label rackTitle = new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    Text = rack.rack_name,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    Style = App.Current.Resources["RackItemStyle"] as Style
                };
                rackLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                rackLayout.Children.Add(rackTitle, 0, 0);

                Label rackBottles = new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    Text = thisRacksBottles.Count() + " Bottles",
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Center,
                    Style = App.Current.Resources["RackItemStyle"] as Style
                };
                rackLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                rackLayout.Children.Add(rackBottles, 0, 1);

                var rackGrid = new Grid()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.Black,
                    Padding = gridPadding,
                    Margin = new Thickness(0, 0, 0, 20),
                    ColumnSpacing = gridSpacing,
                    RowSpacing = gridSpacing
                };
                // Set up grid
                for (int buildRow = 0; buildRow < rack.rows; buildRow++)
                {
                    rackGrid.RowDefinitions.Add(new RowDefinition() { Height = GridSize });
                }
                for (int buildCol = 0; buildCol < rack.cols; buildCol++)
                {
                    rackGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridSize });
                }
                for (int row = 0; row < rack.rows; row++)
                {
                    for (int col = 0; col < rack.cols; col++)
                    {
                        var thisSlotsBottle = thisRacksBottles.Where(x => x.row - 1 == row && x.col - 1 == col).FirstOrDefault();
                        var boxColor = Color.White;
                        var bgBoxView = new BoxView() { Style = App.Current.Resources["RackBottleSlotStyle"] as Style, WidthRequest = RackSlotSize, HeightRequest = RackSlotSize, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, CornerRadius = cornerRadius };
                        if (thisSlotsBottle != null)
                        {
                            if (thisSlotsBottle.Category == "Red") boxColor = Color.FromHex("#6e0000");
                            else if (thisSlotsBottle.Category == "White") boxColor = Color.FromHex("#e6dd85");
                            else if (thisSlotsBottle.Category == "Rosé") boxColor = Color.FromHex("#e68585");
                            else if (thisSlotsBottle.Category == "Sparkling / Champagne") boxColor = Color.FromHex("#004501");
                            else if (thisSlotsBottle.Category == "Dessert / Port / Sherry") boxColor = Color.FromHex("#552270");
                            else boxColor = Color.FromHex("#616161");
                            var bottleBoxView = new BoxView() { Color = boxColor, WidthRequest = BottleSize, HeightRequest = BottleSize, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, CornerRadius = BottleSize / 2 };
                            
                            //var tapGestureRecognizer = new TapGestureRecognizer();
                            //tapGestureRecognizer.Tapped += async (s, e) => {
                            //    await Navigation.PushModalAsync(new ViewBottlePage(new UserBottleForDisplay(thisSlotsBottle)));
                            //};
                            //bgBoxView.GestureRecognizers.Add(tapGestureRecognizer);
                            //bottleBoxView.GestureRecognizers.Add(tapGestureRecognizer);

                            rackGrid.Children.Add(bgBoxView, col, row);
                            if (userBottle.row == row + 1 && userBottle.col == col + 1)
                            {
                                selectedBoxView = new BoxView() { Style = App.Current.Resources["RackBottleSlotStyleHighlight"] as Style, WidthRequest = RackSlotSize, HeightRequest = RackSlotSize, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, CornerRadius = BottleSize / 2 };
                                //rackGrid.Children.Add(selectedBoxView, col, row);
                                selectedBottleColor = boxColor;
                            }
                            rackGrid.Children.Add(bottleBoxView, col, row);
                        }
                        else
                        {
                            //var tapGestureRecognizer = new TapGestureRecognizer();
                            //tapGestureRecognizer.CommandParameter = rack.guid + "|^|" + rack.rack_name + "|^|" + row + "|^|" + col;
                            //tapGestureRecognizer.Tapped += async (s, e) => {
                            //    var thisItem = (BoxView)s;
                            //    var gr = (TapGestureRecognizer)thisItem.GestureRecognizers[0];
                            //    var cp = (string)gr.CommandParameter;
                            //    //await Navigation.PushModalAsync(new AddBottlePage(cp));
                            //    await Navigation.PushModalAsync(new AddBottleToRackPage(cp));
                            //};
                            //bgBoxView.GestureRecognizers.Add(tapGestureRecognizer);
                            rackGrid.Children.Add(bgBoxView, col, row);
                        }
                    }
                }
                rackGrid.Children.Add(selectedBoxView, (int)userBottle.col - 1, (int)userBottle.row - 1);
                rackGrid.Children.Add(new BoxView() { Color = selectedBottleColor, WidthRequest = BottleSize, HeightRequest = BottleSize, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, CornerRadius = BottleSize / 2 }, (int)userBottle.col - 1, (int)userBottle.row - 1);

                rackLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var scrollGrid = new ScrollView() { Orientation = ScrollOrientation.Both, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, Margin = new Thickness(10, 0, 10, 0) };
                AbsoluteLayout rackAbso = new AbsoluteLayout();
                rackAbso.Children.Add(new Frame() { BackgroundColor = Color.FromHex("#000000"), CornerRadius = frameCornerRadius, Margin = new Thickness(0, 0, 0, 20) }, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                rackAbso.Children.Add(rackGrid, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                scrollGrid.Content = rackAbso;
                //scrollGrid.Content = rackGrid;
                rackLayout.Children.Add(scrollGrid, 0, 2);

                var screenWidth = Application.Current.MainPage.Width;
                int frameMargin = Convert.ToInt32(0.05 * screenWidth);
                Frame rackFrame = new Frame() { };
                rackFrame.Style = App.Current.Resources["RackOuterFrameStyle"] as Style;
                rackFrame.Margin = new Thickness(frameMargin, 5, frameMargin, 5);

                AbsoluteLayout frameAbso = new AbsoluteLayout();
                frameAbso.Children.Add(rackLayout, new Rectangle() { X = 0.5, Y = 0.5, Width = 1, Height = 1 }, AbsoluteLayoutFlags.All);
                //frameAbso.Children.Add(new Label() { Text = "TEST" }, new Rectangle() { X = 1, Y = 0, Width = 60, Height = 60 }, AbsoluteLayoutFlags.PositionProportional);
                rackFrame.Content = frameAbso;

                var screenHeight = Application.Current.MainPage.Height;
                int scrollHeight = Convert.ToInt32(0.71 * screenHeight);
                if (rack.rows * GridSize > scrollHeight)
                    mainLayout.RowDefinitions.Add(new RowDefinition() { Height = scrollHeight });
                else
                    mainLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                mainLayout.Children.Add(rackFrame, 0, mainLayoutRow++);

            }
            else
            {
                var rackLayout = new Grid()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    RowSpacing = 0
                };
                Label rackTitle = new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    Text = "This bottle is unassigned",
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    Style = App.Current.Resources["RackItemStyle"] as Style
                };
                rackLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                rackLayout.Children.Add(rackTitle, 0, 0);
                AbsoluteLayout frameAbso = new AbsoluteLayout();
                frameAbso.Children.Add(rackLayout, new Rectangle() { X = 0.5, Y = 0.5, Width = 1, Height = 1 }, AbsoluteLayoutFlags.All);
                Frame rackFrame = new Frame() { };
                rackFrame.Style = App.Current.Resources["RackOuterFrameStyle"] as Style;
                var screenWidth = Application.Current.MainPage.Width;
                int frameMargin = Convert.ToInt32(0.05 * screenWidth);
                rackFrame.Margin = new Thickness(frameMargin, 5, frameMargin, 5);
                rackFrame.Content = frameAbso;
                mainLayout.Children.Add(rackFrame, 0, mainLayoutRow++);
            }
            outerScrollView.Content = mainLayout;
            return outerScrollView;
        }
    }
}