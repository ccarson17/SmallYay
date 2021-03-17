using SmallYay.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmallYay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageNotLoggedIn : ContentPage
    {
        public MainPageNotLoggedIn()
        {
            InitializeComponent();
            this.Content = new HomeView();
        }
    }
}