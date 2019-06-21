using System.Windows;
using System.Windows.Input;

namespace wrike_timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PUNCT_SPACE = " ";

        private bool _timerRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void dragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void timerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (this._timerRunning)
            {
                this.timerAmount.Content = $"0{PUNCT_SPACE}00";
                this.timerIcon.Style = (Style)this.Resources["pausePathStyle"];
            }
            else
            {
                this.timerAmount.Content = "0:00";
                this.timerIcon.Style = (Style)this.Resources["playPathStyle"];
            }
            this._timerRunning = !this._timerRunning;
        }
    }
}