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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebRequest request = WebRequest.CreateHttp(Constants.WrikeAuth.TokenUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var requestBody = string.Format("client_id={0}&client_secret={1}&grant_type=authorization_code&code={2}&redirect_uri={3}",
                   HttpUtility.UrlEncode(Constants.WrikeAuth.ApiClientId),
                   HttpUtility.UrlEncode(Constants.WrikeAuth.ApiClientSecret),
                   HttpUtility.UrlEncode(query["code"]),
                   HttpUtility.UrlEncode(Constants.WrikeAuth.RedirectUrl.ToString())
                );
                using (var requestStream = new StreamWriter(request.GetRequestStream()))
                {
                    requestStream.Write(requestBody);
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var responseStream = new StreamReader(response.GetResponseStream()))
                    {
                        JToken responseBody = JToken.Parse(responseStream.ReadToEnd());
                        Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey] = responseBody.Value<string>("refresh_token");
                        // TODO: initialize API client with access token and host
                    }
                    this.Close();
                }
                catch (WebException ex)
                {
                    string responseBody = "";
                    try
                    {
                        using (var stream = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            responseBody = stream.ReadToEnd();
                        }
                    }
                    catch (Exception) { }

                    // TODO: make this message user-friendly
                    MessageBox.Show($"{responseBody} {ex.ToString()}");
                }
            }
        }
    }
}
