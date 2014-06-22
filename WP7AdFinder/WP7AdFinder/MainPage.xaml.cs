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

namespace WP7AdFinder
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click_Scan (object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/scan.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Tag (object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/tag.xaml", UriKind.Relative));
        }
    }
}