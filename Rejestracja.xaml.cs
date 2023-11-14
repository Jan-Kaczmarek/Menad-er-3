using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        // szyfriwanie
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

        // Czy plik istnieje xD
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

        // Rejestracja
        private void btnendregister_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt";
            string password = boxregisterpassword.Password;
            string encryptedPassword = EncryptString(password);
            string User = boxregisterusername.Text;
            string username = EncryptString(User);

            if (DoesFileExist(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (line == "hasło: " + encryptedPassword)
                    {
                        MessageBox.Show("Hasło jest już używane");
                        return;
                    }
                    if (line == "Login: " + username)
                    {
                        MessageBox.Show("Taka nazwa użytkownika jest już zajęta :(");
                        return;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(User) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("No tak to nie dziala");
            }
            else
            {
                using (StreamWriter writer = File.AppendText("C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt"))
                {
                    writer.Write("Dane Logowania ");
                    writer.WriteLine("zapisuje plik");
                    writer.WriteLine("Login: " + username);
                    writer.WriteLine("hasło: " + encryptedPassword); // Zapisanie zaszyfrowanego hasła
                }
                MessageBox.Show("Zadziałało");
                MainWindow MainWindow = new MainWindow();
                this.Visibility = Visibility.Hidden;
                MainWindow.Show();
            }
        }
    }
}