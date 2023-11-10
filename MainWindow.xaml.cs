using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menadżer_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void boxUser_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void boxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            Window1 objwindow1 = new Window1();
            this.Visibility = Visibility.Hidden;
            objwindow1.Show();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
          







        }

    }

}
