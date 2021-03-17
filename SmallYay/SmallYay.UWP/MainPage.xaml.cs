using SmallYay.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmallYay.UWP
{
    public sealed partial class MainPage : IThemeChanger
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new SmallYay.App(this));
        }

        public void ApplyTheme(string newTheme)
        {

        }
    }
}
