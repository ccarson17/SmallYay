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
    public partial class AddRackView : ContentPage
    {
        private readonly WineAPIService wineApiService = new WineAPIService();

        public AddRackView()
        {
            InitializeComponent();
            this.BindingContext = new Rack();
            if (rackrowsEntry.Text == "0") rackrowsEntry.Text = "";
            if (rackcolsEntry.Text == "0") rackcolsEntry.Text = "";
            RackViewSaveButton.IsEnabled = true;
            RackViewSaveButton.IsVisible = true;
            RackViewUpdateButton.IsEnabled = false;
            RackViewUpdateButton.IsVisible = false;
        }

        public AddRackView(Rack userRack)
        {
            InitializeComponent();
            this.BindingContext = userRack;
            if (rackrowsEntry.Text == "0") rackrowsEntry.Text = "";
            if (rackcolsEntry.Text == "0") rackcolsEntry.Text = "";
            ViewTitle.Text = "Edit " + userRack.rack_name;
            RackViewSaveButton.IsEnabled = false;
            RackViewSaveButton.IsVisible = false;
            RackViewUpdateButton.IsEnabled = true;
            RackViewUpdateButton.IsVisible = true;
        }

        private async void RackViewSaveButton_Clicked(object sender, EventArgs e)
        {
            Rack inputRack = this.BindingContext as Rack;
            if(inputRack == null)
            {
                //await DisplayAlert("Error", "Invalid parameters", "Dismiss");
                await Navigation.PushModalAsync(new PopupMessage("Error", "Invalid parameters", "Dismiss", "error", "message", ""));
                return;
            }
            else
            {
                string validationMessage = "";
                if (inputRack.rows == 0) validationMessage += "Rows cannot be 0 or blank\n";
                if (inputRack.cols == 0) validationMessage += "Columns cannot be 0 or blank\n";
                if (String.IsNullOrEmpty(inputRack.rack_name)) validationMessage += "Rack Name cannot be blank\n";
                if(!String.IsNullOrEmpty(validationMessage))
                {
                    //await DisplayAlert("Warning", "Invalid rack:\n" + validationMessage, "Dismiss");
                    await Navigation.PushModalAsync(new PopupMessage("Warning", "Invalid rack:\n" + validationMessage, "Dismiss", "warning", "message", ""));
                    return;
                }
            }
            Rack newRack = new Rack()
            {
                owner_guid = wineApiService.GetApiGuid(),
                rows = inputRack.rows,
                cols = inputRack.cols,
                rack_name = inputRack.rack_name
            };
            wineApiService.CreateNewRack(newRack);
            this.Unfocus();
            if (newRack.owner_guid.StartsWith("API Error: "))
            {
                //await DisplayAlert("Error", newRack.owner_guid, "Dismiss");
                await Navigation.PushModalAsync(new PopupMessage("Error", newRack.owner_guid, "Dismiss", "error", "message", ""));

            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void RackViewUpdateButton_Clicked(object sender, EventArgs e)
        {
            var userRack = this.BindingContext as Rack;
            string updateResponse = "";
            if (String.IsNullOrEmpty(userRack.rack_name) || userRack.cols <= 0 || userRack.rows <= 0)
                //await DisplayAlert("Warning", "Invalid input. Please try again.", "OK");
                await Navigation.PushModalAsync(new PopupMessage("Warning", "Invalid input. Please try again.", "OK", "warning", "message", ""));

            else
                updateResponse = wineApiService.UpdateRack(userRack);
            if(updateResponse != "Created" && updateResponse != "") 
                //await DisplayAlert("Error", "API Response: " + updateResponse, "Dismiss");
                await Navigation.PushModalAsync(new PopupMessage("Error", "API Response: " + updateResponse, "Dismiss", "error", "message", ""));
            if (updateResponse == "Created")
            {
                this.Unfocus();
                await Navigation.PopModalAsync();
            }
        }

        private async void RackViewCancelButton_Clicked(object sender, EventArgs e)
        {
            this.Unfocus();
            await Navigation.PopModalAsync();
        }

        private async void Background_Tapped(object sender, EventArgs e)
        {
            this.Unfocus();
            await Navigation.PopModalAsync();
        }
    }
}