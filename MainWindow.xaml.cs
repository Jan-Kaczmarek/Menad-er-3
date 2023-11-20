using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string DecryptString(string cipherText)
        {
            string key = "1234567890123456"; // Your Key Here
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public bool DoesFileExist(string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Jeśli plik został pomyślnie otwarty, zwróć prawdę
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                // Jeśli plik nie istnieje, zwróć fałsz
                return false;
            }
            catch (Exception)
            {
                // Jeśli wystąpił inny błąd, zwróć fałsz
                return false;
            }
        }

        public bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        private void boxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void boxUser_TextChanged(object sender, TextChangedEventArgs e)
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
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt";
            string username = boxUser.Text;
            string password = boxPassword.Password;

            if (DoesFileExist(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                var logins = lines.Where(L => L.StartsWith("Login: "));
                var passwords = lines.Where(P => P.StartsWith("hasło: "));

                int counter = -1;

                foreach (var login in logins)
                {
                    counter += 1;
                    string encryptedUsername = login.Substring(7); // Usuń "Login: " z linii

                    if (IsBase64String(encryptedUsername))
                    {
                        string decryptedUsername = DecryptString(encryptedUsername);

                        if (decryptedUsername == username)
                        {
                            string passwordFromCollection = passwords.ElementAtOrDefault(counter).ToString().Substring(7);
                            string encryptedPassword = passwordFromCollection; // Usuń "hasło: " z linii

                            if (IsBase64String(encryptedPassword))
                            {
                                string decryptedPassword = DecryptString(encryptedPassword);

                                if (decryptedPassword == password)
                                {
                                    MessageBox.Show("Zalogowano pomyślnie!");
                                    Aplikacja Aplikacja = new Aplikacja();
                                    this.Visibility = Visibility.Hidden;
                                    Aplikacja.username = decryptedUsername;
                                    Aplikacja.Show();
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show("Niepoprawne hasło!");
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Nie znaleziono użytkownika!");
        }
    }
}