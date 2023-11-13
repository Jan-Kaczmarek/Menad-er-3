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

        private void btnendregister_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt";
            string password = boxregisterpassword.Password;
            if (DoesFileExist(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (line == "hasło: " + password)
                    {
                        MessageBox.Show("Hasło jest już używane");
                        return;
                    }
                    if (line == "Login: " + boxregisterusername.Text)
                    {
                        MessageBox.Show("Taka nazwa użytkownika jest już zajęta :(");
                        return;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(boxregisterusername.Text) ||
                string.IsNullOrWhiteSpace(boxregisterpassword.Password))
            {
                MessageBox.Show("No tak to nie dziala");
            }
            else
            {
                using (StreamWriter writer = File.AppendText("C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt"))
                {
                    writer.Write("Dane Logowania");
                    writer.WriteLine("zapisuje plik");
                    writer.WriteLine("Login: " + boxregisterusername.Text);
                    writer.WriteLine("hasło: " + boxregisterpassword.Password);
                }
                MessageBox.Show("Zadziałało");
                MainWindow MainWindow = new MainWindow();
                this.Visibility = Visibility.Hidden;
                MainWindow.Show();
                EncryptFile("C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\konta.txt", "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Akonta\\Niepowiem.txt");
            }
        }

        private void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"ZAQ!2wsx"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }
    }
}