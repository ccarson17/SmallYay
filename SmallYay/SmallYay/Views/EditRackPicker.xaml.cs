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
    public partial class EditRackPicker : ContentPage
    {
        public List<Rack> myRacks { get; private set; }
        private readonly WineAPIService wineApiService = new WineAPIService();
        private string cp = "";
        public EditRackPicker(List<Rack> racks)
        {
            InitializeComponent();
            myRacks = racks;
            BindingContext = this;
            this.Disappearing += AddBottleToRackPage_Disappearing;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var editRack = e.Item as Rack;
            //wineApiService.UpdateRack(editRack);
            await Navigation.PushModalAsync(new AddRackView(editRack));
            if(Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void AddBottleToRackPage_Disappearing(object sender, EventArgs e)
        {
            if(Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
        }
    }
}