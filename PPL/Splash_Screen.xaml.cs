using System.Windows;

namespace PPL
{
    public partial class Splash_Screen : Window
    {
        public Splash_Screen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Show();
        }
    }
}
