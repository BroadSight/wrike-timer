using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wrike_timer
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            UriBuilder builder = new UriBuilder(Constants.WrikeAuth.AuthUrl);
            builder.Query = string.Format("client_id={0}&response_type=code&redirect_uri={1}&scope={2}",
               HttpUtility.UrlEncode(Constants.WrikeAuth.ApiClientId),
               HttpUtility.UrlEncode(Constants.WrikeAuth.RedirectUrl.ToString()),
               HttpUtility.UrlEncode(Constants.WrikeAuth.ApiScope)
            );
            this.browser.Source = builder.Uri;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty((string)Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey]))
            {
                MessageBoxResult result = MessageBox.Show(
                   "You have not logged in yet. Are you sure you want to close the Wrike Timer?",
                   "Wrike Timer",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var source = e.Uri;
            if (source.PartsEquals(Constants.WrikeAuth.RedirectUrl, UriComponents.HostAndPort))
            {
                var query = HttpUtility.ParseQueryString(source.Query);
                Application.Current.Resources.Add(Constants.ApiClientResourceKey, new Api.WrikeApi(query["code"]));
                this.Close();
            }
        }
    }
}
