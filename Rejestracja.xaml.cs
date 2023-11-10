using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Menadżer_3
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btnendregister_Click(object sender, RoutedEventArgs e)
        {
           

            if (string.IsNullOrWhiteSpace(boxregisterusername.Text) || string.IsNullOrWhiteSpace(boxregisterpassword.Password))
            {       
                using (StreamWriter writer = File.AppendText("C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\A konta\\konta.txt"))
                {
                writer.Write("Dane Logowania");
                writer.WriteLine("zapisuje plik");
                writer.WriteLine("Login: " + boxregisterusername.Text);
                writer.WriteLine("hasło: " + boxregisterpassword.Password);
                }
            }
            else
            {
                MessageBox.Show("No tak to nie dziala");
            }
        }
    }
}
