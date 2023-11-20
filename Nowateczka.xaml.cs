using LightMessageBus;
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
        public string username;

        public Nowateczka()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\" + username + ".txt";
            string webname = WebsiteName.Text;
            string email = Email.Text;
            string userName = UserName.Text;
            string password = Password.Text;

            if (string.IsNullOrWhiteSpace(webname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Wszystko musi być wypełnione cymbale");
                return;
            }

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("Dane Logowania");
                writer.WriteLine("Nazwa Strony: " + webname);
                writer.WriteLine("email: " + email);
                writer.WriteLine("Nazwa Użytkownika: " + userName);
                writer.WriteLine("Hasło: " + password);
            }
            MessageBus.Default.Publish(new RefreshMessage(this, "Refresh"));
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Generowanie losowego hasła o losowej długości od 5 do 15
            Random rnd = new Random();
            int passwordLength = rnd.Next(5, 16);
            string password = GeneratePassword(passwordLength);

            // Przypisanie wygenerowanego hasła do kontrolki
            Password.Text = password;
        }

        private string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }
    }
}