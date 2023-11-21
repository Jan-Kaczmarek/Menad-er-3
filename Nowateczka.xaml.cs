using LightMessageBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public static string EncryptString(string plainText)
        {
            string key = "1234567890123456"; // Your Key Here
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\" + username + ".txt";
            string webname = WebsiteName.Text;
            string email = Email.Text;
            string userName = UserName.Text;
            string password = Password.Text;
            string encryptedPassword = EncryptString(password);
            string encryptedUserName = EncryptString(userName);

            if (string.IsNullOrWhiteSpace(webname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Wszystko musi być wypełnione cymbale");
                return;
            }
            if (File.Exists(filePath))
            {
                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.WriteLine("Dane Logowania");
                    writer.WriteLine("Nazwa Strony: " + webname);
                    writer.WriteLine("email: " + email);
                    writer.WriteLine("Nazwa Użytkownika: " + encryptedUserName);
                    writer.WriteLine("Hasło: " + encryptedPassword);
                }
                MessageBus.Default.Publish(new RefreshMessage(this, "Refresh"));
                this.Close();
            }
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