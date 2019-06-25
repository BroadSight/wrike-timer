using System.Windows;
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

        public static readonly DependencyProperty ActiveTimerProperty =
           DependencyProperty.Register("ActiveTimer", typeof(CustomStopwatch), typeof(MainWindow), new UIPropertyMetadata(new CustomStopwatch()));
        public CustomStopwatch ActiveTimer
        {
            get { return (CustomStopwatch)this.GetValue(ActiveTimerProperty); }
            set { this.SetValue(ActiveTimerProperty, value); }
        }

        public MainWindow(Api.WrikeApi client)
        {
            _client = client;
            InitializeComponent();

            _contact = _client.GetUser();
        }

        private void dragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void timerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActiveTimer.IsRunning)
            {
                this.ActiveTimer.Stop();
            }
            else
            {
                this.ActiveTimer.Start();
            }
        }
    }
}