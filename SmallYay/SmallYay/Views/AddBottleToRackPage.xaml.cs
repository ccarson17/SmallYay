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
    public partial class AddBottleToRackPage : ContentPage
    {
        public List<UserBottleForDisplay> myBottles { get; private set; }
        public List<UserBottle> myBottlesApi { get; private set; }
        private readonly WineAPIService wineApiService = new WineAPIService();
        private string cp = "";
        public AddBottleToRackPage(string commandParameter)
        {
            InitializeComponent();
            cp = commandParameter;
            myBottles = new List<UserBottleForDisplay>();
            myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "current", false, null).Where(x => x.rack_guid == null).ToList();
            string[] commands = commandParameter.Split(new string[] { "|^|" }, StringSplitOptions.None);
            NoUnassignedBottleLabel.IsVisible = myBottlesApi.Count == 0;
            UnassignedBottleList.IsVisible = myBottlesApi.Count != 0;
            foreach (var item in myBottlesApi)
            {
                item.rack_guid = commands[0];
                item.rack_name = commands[1];
                item.row = int.Parse(commands[2]) + 1;
                item.col = int.Parse(commands[3]) + 1;
                myBottles.Add(new UserBottleForDisplay(item));
            }
            BindingContext = this;
            this.Disappearing += AddBottleToRackPage_Disappearing;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var userBottle = e.Item as UserBottleForDisplay;
            wineApiService.AddUserBottleToRack(userBottle);
            await Navigation.PopModalAsync();
        }

        private async void Add_Bottle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddBottlePage(cp));
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void AddBottleToRackPage_Disappearing(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
        }
    }
}