using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace wrike_timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Api.WrikeApi _client;
        private Api.Model.Contact _contact;

        private CustomStopwatch _activeTimer { get { return (CustomStopwatch)CollectionViewSource.GetDefaultView(ActiveTimers).CurrentItem; } }

        public static readonly DependencyProperty ActiveTimersProperty =
           DependencyProperty.Register("ActiveTimer", typeof(ObservableCollection<CustomStopwatch>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<CustomStopwatch>()));
        public ObservableCollection<CustomStopwatch> ActiveTimers
        {
            get { return (ObservableCollection<CustomStopwatch>)this.GetValue(ActiveTimersProperty); }
            set { this.SetValue(ActiveTimersProperty, value); }
        }

        public MainWindow(Api.WrikeApi client)
        {
            _client = client;
            InitializeComponent();

            _contact = _client.GetUser();
            //ActiveTimers.Add(new CustomStopwatch(new Api.Model.Task() { Title = "General 2019-06-01" }));
            //ActiveTimers.Add(new CustomStopwatch(new Api.Model.Task() { Title = "Wrike Admin Overhead" }));
        }

        private void dragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void timerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (this._activeTimer.IsRunning)
            {
                this._activeTimer.Stop();
            }
            else
            {
                this._activeTimer.Start();
            }
        }

        private void taskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}