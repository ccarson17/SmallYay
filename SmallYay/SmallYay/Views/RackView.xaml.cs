using Newtonsoft.Json;
using SmallYay.Models;
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
    public partial class RackView : ContentView
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
        private int sizeValue = 3;
        private bool cacheRequest = false;
        private Dictionary<int, Guid> imageDict = new Dictionary<int, Guid>();
        private List<Rack> myRacksApi = new List<Rack>();

        public RackView()
        {
            InitializeComponent();

            var zoomString = (GetZoomLevelStorage().Result ?? "3").ToString();
            int zoomLevel = 3;
            Int32.TryParse(zoomString, out zoomLevel);
            setSizes(zoomLevel);
            SetZoomLevel(zoomLevel);

            GridDisplaySV.Content = Build_Grid_Content();
            ZoomControlsSL = Build_Zoom_Controls(ZoomControlsSL);
            FooterControlsSL = Build_Footer_Controls(FooterControlsSL);
        }

        private ScrollView Build_Grid_Content()
        {
            setSizes(GetZoomLevel() ?? 3);
            myRacksApi = wineApiService.GetMyRacks(wineApiService.GetApiGuid(), cacheRequest);
            var myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", cacheRequest, null);
            acds.PopulateAutocompleteLists(false);
            cacheRequest = true; // Allow 30s crequest caching after initial load
            int mainLayoutRow = 0;

            var outerScrollView = new ScrollView() { HorizontalOptions = LayoutOptions.Fill, Orientation = ScrollOrientation.Vertical };

            var mainLayout = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                RowSpacing = 0
            };

            foreach (var rack in myRacksApi)
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
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += async (s, e) => {
                                await Navigation.PushModalAsync(new ViewBottlePage(new UserBottleForDisplay(thisSlotsBottle)));
                            };
                            bgBoxView.GestureRecognizers.Add(tapGestureRecognizer);
                            bottleBoxView.GestureRecognizers.Add(tapGestureRecognizer);
                            rackGrid.Children.Add(bgBoxView, col, row);
                            rackGrid.Children.Add(bottleBoxView, col, row);
                        }
                        else
                        {
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.CommandParameter = rack.guid + "|^|" + rack.rack_name + "|^|" + row + "|^|" + col;
                            tapGestureRecognizer.Tapped += async (s, e) => {
                                var thisItem = (BoxView)s;
                                var gr = (TapGestureRecognizer)thisItem.GestureRecognizers[0];
                                var cp = (string)gr.CommandParameter;
                                //await Navigation.PushModalAsync(new AddBottlePage(cp));
                                await Navigation.PushModalAsync(new AddBottleToRackPage(cp));
                            };
                            bgBoxView.GestureRecognizers.Add(tapGestureRecognizer);
                            rackGrid.Children.Add(bgBoxView, col, row);
                        }
                    }
                }
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
                if(rack.rows * GridSize > scrollHeight)
                    mainLayout.RowDefinitions.Add(new RowDefinition() { Height = scrollHeight });
                else
                    mainLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                mainLayout.Children.Add(rackFrame, 0, mainLayoutRow++);

            }
            outerScrollView.Content = mainLayout;
            return outerScrollView;
        }



        private StackLayout Build_Zoom_Controls(StackLayout inputSL)
        {
            int zoomNum = GetZoomLevel() ?? 3;
            int childCount = inputSL.Children.Count();
            for (int i = 0; i < childCount; i++)
            {
                inputSL.Children.RemoveAt(0);
            }
            Button GridSizeMinus = new Button()
            {
                Text = "-",
                IsEnabled = zoomNum > 1,
                FontSize = 36, //Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Padding = 0,
                FontAttributes = FontAttributes.Bold,
                Style = (Style)Application.Current.Resources["DefaultButtonStyle"]
            };
            Button GridSizePlus = new Button()
            {
                Text = "+",
                IsEnabled = zoomNum < 5,
                FontSize = 36, //Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Padding = 0,
                FontAttributes = FontAttributes.Bold,
                Style = (Style)Application.Current.Resources["DefaultButtonStyle"]
            };
            Image size1image = new Image() { Opacity = 0.3, Margin = new Thickness(0,0), Source = ImageSource.FromFile("baseline_grid_on_black_48.png"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            Image size2image = new Image() { Opacity = 0.3, Margin = new Thickness(0, 0), Source = ImageSource.FromFile("baseline_grid_on_black_48.png"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            Image size3image = new Image() { Opacity = 0.3, Margin = new Thickness(0, 0), Source = ImageSource.FromFile("baseline_grid_on_black_48.png"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            Image size4image = new Image() { Opacity = 0.3, Margin = new Thickness(0, 0), Source = ImageSource.FromFile("baseline_grid_on_black_48.png"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            Image size5image = new Image() { Opacity = 0.3, Margin = new Thickness(0, 0), Source = ImageSource.FromFile("baseline_grid_on_black_48.png"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            if (imageDict.Count > 0) {
                //foreach (var item in imageDict)
                //    imageDict.Remove(item.Key);
                imageDict = new Dictionary<int, Guid>();
            }
            imageDict.Add(1, size1image.Id);
            imageDict.Add(2, size2image.Id);
            imageDict.Add(3, size3image.Id);
            imageDict.Add(4, size4image.Id);
            imageDict.Add(5, size5image.Id);
            GridSizeMinus.Clicked += GridSizeMinus_Clicked;
            GridSizePlus.Clicked += GridSizePlus_Clicked;
            TapGestureRecognizer imageTap = new TapGestureRecognizer();
            imageTap.Tapped += GridResize_Clicked;
            size1image.GestureRecognizers.Add(imageTap);
            size2image.GestureRecognizers.Add(imageTap);
            size3image.GestureRecognizers.Add(imageTap);
            size4image.GestureRecognizers.Add(imageTap);
            size5image.GestureRecognizers.Add(imageTap);

            if (zoomNum == 1) size1image.Opacity = 1;
            else if (zoomNum == 2) size2image.Opacity = 1;
            else if (zoomNum == 3) size3image.Opacity = 1;
            else if (zoomNum == 4) size4image.Opacity = 1;
            else if (zoomNum == 5) size5image.Opacity = 1;
            Grid imageGrid = new Grid()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 24 });
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 30 });
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 36 });
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 40 });
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 48 });
            imageGrid.RowDefinitions.Add(new RowDefinition() { Height = 48 });
            imageGrid.Padding = 0;
            imageGrid.Margin = 0;

            imageGrid.Children.Add(new Label() { Text = "Zoom", VerticalOptions = LayoutOptions.CenterAndExpand, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), FontAttributes = FontAttributes.Bold, Margin = new Thickness(10, 0, 10, 0), Style = App.Current.Resources["RackItemStyle"] as Style }, 0, 0);
            imageGrid.Children.Add(size1image, 1, 0);
            imageGrid.Children.Add(size2image, 2, 0);
            imageGrid.Children.Add(size3image, 3, 0);
            imageGrid.Children.Add(size4image, 4, 0);
            imageGrid.Children.Add(size5image, 5, 0);

            //inputSL.Spacing = 2;
            //inputSL.Children.Add(GridSizeMinus);
            inputSL.Children.Add(imageGrid);
            //inputSL.Children.Add(size1image);
            //inputSL.Children.Add(size2image);
            //inputSL.Children.Add(size3image);
            //inputSL.Children.Add(size4image);
            //inputSL.Children.Add(size5image);
            //inputSL.Children.Add(GridSizePlus);
            return inputSL;
        }

        private StackLayout Build_Footer_Controls(StackLayout inputSL)
        {
            int childCount = inputSL.Children.Count();
            for (int i = 0; i < childCount; i++)
            {
                inputSL.Children.RemoveAt(0);
            }
            inputSL.BackgroundColor = Color.FromHex("#00000000");
            Button AddRack = new Button()
            {
                //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = "Add Rack",
                //FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.EndAndExpand,
                //CornerRadius = 12,
                ImageSource = ImageSource.FromFile("baseline_grid_on_white_48dp.png"),
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 0),
                Style = (Style)Application.Current.Resources["DefaultButtonStyle"]
            };
            AddRack.Clicked += async (sender, e) =>
            {
                await Navigation.PushModalAsync(new AddRackView());
            };
            Button EditRack = new Button()
            {
                //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = "Edit Racks",
                //FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.EndAndExpand,
                //CornerRadius = 12,
                ImageSource = ImageSource.FromFile("baseline_edit_white_48dp.png"),
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 0),
                Style = (Style)Application.Current.Resources["DefaultButtonStyle"]
            };
            EditRack.IsEnabled = myRacksApi.Count() != 0;
            EditRack.Clicked += async (sender, e) =>
            {
                if(myRacksApi.Count == 1)
                    await Navigation.PushModalAsync(new AddRackView(myRacksApi.FirstOrDefault()));
                else
                    await Navigation.PushModalAsync(new EditRackPicker(myRacksApi));
            };
            inputSL.Children.Add(AddRack);
            inputSL.Children.Add(EditRack);
            return inputSL;
        }

        //private async Task RackFooter_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new AddRackView());
        //}

        //private async Task RackFooter_Edit_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new AddRackView());
        //}

        private void GridSizeMinus_Clicked(object sender, EventArgs e)
        {
            int zoomLevel = GetZoomLevel() ?? 2;
            zoomLevel--;
            if (zoomLevel < 1) zoomLevel = 1;
            SetZoomLevel(zoomLevel);
            ZoomControlsSL = Build_Zoom_Controls(ZoomControlsSL);
            GridDisplaySV.Content = Build_Grid_Content();
            FooterControlsSL = Build_Footer_Controls(FooterControlsSL);
        }

        private void GridSizePlus_Clicked(object sender, EventArgs e)
        {
            int zoomLevel = GetZoomLevel() ?? 2;
            zoomLevel++;
            if (zoomLevel > 5) zoomLevel = 5;
            SetZoomLevel(zoomLevel);
            ZoomControlsSL = Build_Zoom_Controls(ZoomControlsSL);
            GridDisplaySV.Content = Build_Grid_Content();
            FooterControlsSL = Build_Footer_Controls(FooterControlsSL);
        }

        private void GridResize_Clicked(object sender, EventArgs e)
        {
            int zoomLevel = 3;
            var clickedId = (sender as Image).Id;
            foreach(var item in imageDict)
            {
                if (item.Value == clickedId) zoomLevel = item.Key;
            }
            if (zoomLevel > 5) zoomLevel = 5;
            if (zoomLevel < 1) zoomLevel = 1;
            SetZoomLevel(zoomLevel);
            ZoomControlsSL = Build_Zoom_Controls(ZoomControlsSL);
            GridDisplaySV.Content = Build_Grid_Content();
            FooterControlsSL = Build_Footer_Controls(FooterControlsSL);
        }

        private int? GetZoomLevel()
        {
            if (!Application.Current.Properties.ContainsKey("rack_zoom_level")) SetZoomLevel(3);
            return Application.Current.Properties["rack_zoom_level"] as int?;
        }

        private void SetZoomLevel(int zoom)
        {
            if (Application.Current.Properties.ContainsKey("rack_zoom_level")) Application.Current.Properties["rack_zoom_level"] = zoom;
            else Application.Current.Properties.Add("rack_zoom_level", zoom);
            SetZoomLevelStorage(zoom);
        }

        private async void SetZoomLevelStorage(int zoomLevel)
        {
            await SecureStorage.SetAsync("rack_zoom_level", zoomLevel.ToString());
        }

        private async Task<string> GetZoomLevelStorage()
        {
            return await SecureStorage.GetAsync("rack_zoom_level");
        }

        private void setSizes(int sizeValue)
        {
            switch (sizeValue)
            {
                case 1:
                    BottleSize = 10;
                    RackSlotSize = 12;
                    GridSize = 14;
                    cornerRadius = 0;
                    gridPadding = 2;
                    gridSpacing = 2;
                    frameCornerRadius = 5;
                    break;
                case 2:
                    BottleSize = 15;
                    RackSlotSize = 17;
                    GridSize = 19;
                    cornerRadius = 2;
                    gridPadding = 3;
                    gridSpacing = 3;
                    frameCornerRadius = 7;
                    break;
                case 3:
                    BottleSize = 30;
                    RackSlotSize = 32;
                    GridSize = 34;
                    cornerRadius = 4;
                    gridPadding = 5;
                    gridSpacing = 5;
                    frameCornerRadius = 10;
                    break;
                case 4:
                    BottleSize = 50;
                    RackSlotSize = 54;
                    GridSize = 58;
                    cornerRadius = 6;
                    gridPadding = 7;
                    gridSpacing = 7;
                    frameCornerRadius = 15;
                    break;
                case 5:
                    BottleSize = 70;
                    RackSlotSize = 74;
                    GridSize = 78;
                    cornerRadius = 8;
                    gridPadding = 9;
                    gridSpacing = 9;
                    frameCornerRadius = 15;
                    break;
                default:
                    BottleSize = 30;
                    RackSlotSize = 32;
                    GridSize = 34;
                    cornerRadius = 4;
                    gridPadding = 5;
                    gridSpacing = 5;
                    break;
            }
        }
    }
}