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

using WP7AdFinder.CIQProductIdentityServices;
using WP7AdFinder.CIQAffiliateServices;

namespace WP7AdFinder
{
    public partial class Tag : PhoneApplicationPage
    {
        public Tag()
        {
            InitializeComponent();

            App myApp = (App)Application.Current;
            this.scancode.Text = myApp.CIQTag;
        }

        private void ApplicationBarIconButton_Click_Scan(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/scan.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Home (object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void searchTag_Click(object sender, RoutedEventArgs e)
        {
            App myApp = (App)Application.Current;
            myApp.CIQTag = this.scancode.Text;

            if (myApp.CIQTag.Length > 0)
            {
                CIQPlatformClient pcl = new CIQPlatformClient();
                pcl.GetAdListCompleted += new EventHandler<GetAdListCompletedEventArgs>(pcl_GetAdListCompleted);
                pcl.GetAdListAsync(Guid.NewGuid(), myApp.CIQTag, myApp.Longitude, myApp.Latitude, 5);
            }
            else
            {
                MessageBox.Show("No Dude. Try again with a valid code value.");
            }
        }

        void pcl_GetAdListCompleted(object sender, GetAdListCompletedEventArgs e)
        {
            ProductAds pAds = (ProductAds)e.Result;

            if (pAds != null && pAds.Count > 0)
            {
                App myApp = (App)Application.Current;
                myApp.AdList = pAds;

                NavigationService.Navigate(new Uri("/adlist.xaml", UriKind.Relative));
            }
            else
            {
               // No Ads Found Page: NavigationService.Navigate(new Uri("/adlist.xaml", UriKind.Relative));
            }
        }
    }
}