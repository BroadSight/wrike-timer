using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace wrike_timer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string userSettingsFilename = "settings";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Restore application-scope property from isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(userSettingsFilename, FileMode.Open, storage))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Restore each application-scope property individually
                    while (!reader.EndOfStream)
                    {
                        string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                        this.Properties[keyValue[0]] = keyValue[1];
                    }
                }
            }
            catch (IsolatedStorageException)
            {
                // Handle when file is not found in isolated storage:
                // * When the first application session
                // * When file has been deleted
            }

            if (string.IsNullOrEmpty((string)Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey]))
            {
                var login = new LoginWindow();
                login.Show();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Persist application-scope property to isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(userSettingsFilename, FileMode.Create, storage))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Persist each application-scope property individually
                foreach (string key in this.Properties.Keys)
                {
                    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                }
            }
        }
    }
}
