using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.ComponentModel;

using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using WP7AdFinder.CIQAffiliateServices;
using WP7AdFinder.CIQProductIdentityServices;

namespace WP7AdFinder
{
    public partial class addetail : PhoneApplicationPage
    {
        public addetail ()
        {
            InitializeComponent();

            App myApp = (App)Application.Current;
            CProductAd prodAd = myApp.CIQProductAd;

            adTitle.Text = prodAd.AdTitle;
            adDescription.Text = prodAd.AdText;

            if (prodAd.ImageUrl != null && prodAd.ImageUrl.Length > 0)
            {
                adImage.Source = new BitmapImage(new Uri(prodAd.ImageUrl, UriKind.Absolute));
            }
        }

        private void ApplicationBarIconButton_Click_Home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Tag(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/tag.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Snap(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/scan.xaml", UriKind.Relative));
        }
    }
}