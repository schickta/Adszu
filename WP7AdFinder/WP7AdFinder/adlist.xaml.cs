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

using System.Collections.ObjectModel;
using WP7AdFinder.CIQAffiliateServices;
using WP7AdFinder.CIQProductIdentityServices;

namespace WP7AdFinder
{
    public partial class prodinfo : PhoneApplicationPage
    {
        public prodinfo ()
        {
            InitializeComponent();

            App myApp = (App) Application.Current;

            // Do an Asynch call to get the product identity used for the title.

            CIQProductIdentityServicesClient pid = new CIQProductIdentityServicesClient();
            pid.QueryIdentityCompleted += new EventHandler<QueryIdentityCompletedEventArgs>(pid_QueryIdentityCompleted);
            pid.QueryIdentityAsync(myApp.CIQTag);

            // Set the ad list.

            ProductAds pAds = myApp.AdList;
            this.adList.ItemsSource = pAds;
        }

        void pid_QueryIdentityCompleted(object sender, QueryIdentityCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                App myApp = (App)Application.Current;
                myApp.CIQProductIdentity = (CProductIdentity)e.Result;

                // Set the title to include the product ID and Description.

                this.prodText.Text = myApp.CIQProductIdentity.ProductIdentity.ToString() + ": " +
                    myApp.CIQProductIdentity.Description.ToString();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
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

        private void adList_Tap(object sender, GestureEventArgs e)
        {
            if (this.adList.SelectedIndex >= 0)
            {
                App myApp = (App)Application.Current;
                myApp.CIQProductAd = (CProductAd) this.adList.SelectedItem;

                NavigationService.Navigate(new Uri("/addetail.xaml", UriKind.Relative));
            }
        }
    }
}