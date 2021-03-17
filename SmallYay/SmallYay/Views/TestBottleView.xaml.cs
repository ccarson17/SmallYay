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
    public partial class TestBottleView : ContentView
    {
        private readonly WineAPIService wineApiService = new WineAPIService();

        public TestBottleView()
        {
            InitializeComponent();
            List<Bottle> bottles = wineApiService.GetAllBottles();
            List<testListViewItem> items = new List<testListViewItem>();
            foreach(var item in bottles)
            {
                items.Add(new testListViewItem() { MainText = item.Vintner, SubText = item.WineName });
            }
            TestListView.ItemsSource = items;
            ActivityOngoing(false);
        }

        public class testListViewItem
        {
            public string MainText { get; set; }
            public string SubText { get; set; }
        }

        private void ActivityOngoing(bool state)
        {
            TestBottleActivityIndicator.IsRunning = state;
            TestBottleScreenDarken.IsVisible = state;
        }
    }
}