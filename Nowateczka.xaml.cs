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
    /// Logika interakcji dla klasy Nowateczka.xaml
    /// </summary>
    public partial class Nowateczka : Window
    {
        public Nowateczka()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\DaneLogowania.txt";
            string webname = WebsiteName.Text;
            string email = Email.Text;
            string username = UserName.Text;
            string password = Password.Text;

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("Dane Logowania");
                writer.WriteLine("Nazwa Strony: " + webname);
                writer.WriteLine("email: " + email);
                writer.WriteLine("Nazwa Użytkownika: " + username);
                writer.WriteLine("Hasło: " + password);
            }
            this.Close();
        }
    }
}