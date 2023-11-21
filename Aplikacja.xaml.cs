using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using LightMessageBus.Interfaces;
using LightMessageBus;
using System.Reflection;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Menadżer_3
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Aplikacja : Window, IMessageHandler<RefreshMessage>
    {
        public string username { get; set; }

        public List<LoginData> SearchData = new List<LoginData>();

        public Aplikacja()
        {
            InitializeComponent();
            MessageBus.Default.FromAny().Where<RefreshMessage>().Notify(this);
        }

        public static string DecryptString(string cipherText)
        {
            string key = "1234567890123456"; // Your Key Here
            byte[] iv = new byte[16];
            byte[] buffer;

            try
            {
                buffer = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                // Niepoprawny format Base64, zwróć oryginalny tekst
                return cipherText;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                try
                {
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
                catch (CryptographicException)
                {
                    // Błąd podczas deszyfrowania, zwróć oryginalny tekst
                    return cipherText;
                }
            }
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

        public void Handle(RefreshMessage message)
        {
            var data = message.Data;

            dataGrid_Loaded(null, null);
        }

        public bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }

        // Zapisywanie zmian w datagrid
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Zatwierdź zmiany w DataGrid
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            view.Refresh();

            // Zapisz zmiany do pliku tekstowego
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\" + username + ".txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (LoginData item in dataGrid.ItemsSource)
                {
                    sw.WriteLine($"Nazwa Strony: {item.SiteName}");
                    sw.WriteLine($"email: {item.Email}");
                    sw.WriteLine($"Nazwa Użytkownika: {EncryptString(item.UserName)}");
                    sw.WriteLine($"Hasło: {EncryptString(item.Password)}");
                }
            }
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\" + username + ".txt";

            // Utwórz listę, która będzie przechowywać dane
            List<LoginData> data = new List<LoginData>();

            // Otwórz teczuszkę i wczytaj dane
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                LoginData loginData = new LoginData();
                while ((line = reader.ReadLine()) != null)
                {
                    // Rozdziel linie na podstawie dwukropka
                    string[] row = line.Split(':');
                    if (row[0].Trim() == "Nazwa Strony")
                        loginData.SiteName = row[1].Trim();
                    else if (row[0].Trim() == "email")
                        loginData.Email = row[1].Trim();
                    else if (row[0].Trim() == "Nazwa Użytkownika")
                    {
                        // Sprawdź, czy nazwa użytkownika jest zakodowana w Base64
                        if (IsBase64String(row[1].Trim()))
                        {
                            // Odszyfruj nazwę użytkownika
                            loginData.UserName = DecryptString(row[1].Trim());
                        }
                        else
                        {
                            loginData.UserName = row[1].Trim();
                        }
                    }
                    else if (row[0].Trim() == "Hasło")
                    {
                        // Sprawdź, czy hasło jest zakodowane w Base64
                        if (IsBase64String(row[1].Trim()))
                        {
                            // Odszyfruj hasło
                            loginData.Password = DecryptString(row[1].Trim());
                        }
                        else
                        {
                            loginData.Password = row[1].Trim();
                        }

                        data.Add(loginData);
                        loginData = new LoginData();
                    }
                }
            }

            // coś
            SearchData = data;
            dataGrid.ItemsSource = data;

            dataGrid.Columns.Clear();

            dataGrid.AutoGenerateColumns = false;

            dataGrid.IsReadOnly = false;

            // Piękna kolejność
            DataGridTextColumn siteNameColumn = new DataGridTextColumn();
            siteNameColumn.Header = "SiteName";
            siteNameColumn.Binding = new Binding("SiteName");
            dataGrid.Columns.Add(siteNameColumn);

            DataGridTextColumn emailColumn = new DataGridTextColumn();
            emailColumn.Header = "Email";
            emailColumn.Binding = new Binding("Email");
            dataGrid.Columns.Add(emailColumn);

            DataGridTextColumn userNameColumn = new DataGridTextColumn();
            userNameColumn.Header = "UserName";
            userNameColumn.Binding = new Binding("UserName");
            dataGrid.Columns.Add(userNameColumn);

            DataGridTextColumn passwordColumn = new DataGridTextColumn();
            passwordColumn.Header = "Password";
            passwordColumn.Binding = new Binding("Password");
            dataGrid.Columns.Add(passwordColumn);

            // Niszczarka
            DataGridTemplateColumn deleteColumn = new DataGridTemplateColumn();
            deleteColumn.Header = "Usuń";
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Button));
            factory.SetValue(ContentControl.ContentProperty, "Usuń");
            factory.AddHandler(Button.ClickEvent, new RoutedEventHandler(Delete_Click));
            template.VisualTree = factory;
            deleteColumn.CellTemplate = template;
            dataGrid.Columns.Add(deleteColumn);
            // Dodajemy przycisk zapisywania
            saveButton.Content = "Zapisz";
            saveButton.Click += Button_Click_1;
        }

        // To nic nie robi :)
        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz wybrany wiersz
            Button button = (Button)sender;
            LoginData loginData = (LoginData)button.DataContext;

            // Zapytaj się jełopa bo coś może popsuć
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć to informacje ?", "Potwierdzenie", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Usuń wybrany wiersz z listy
                List<LoginData> data = (List<LoginData>)dataGrid.ItemsSource;
                data.Remove(loginData);

                // Odśwież DataGrid
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = data;

                // Usuń dane z pliku, Towarzysz jest pewny
                string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\" + username + ".txt";
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (LoginData item in data)
                    {
                        writer.WriteLine("Nazwa Strony: " + item.SiteName);
                        writer.WriteLine("email: " + item.Email);
                        writer.WriteLine("Nazwa Użytkownika: " + item.UserName);
                        writer.WriteLine("Hasło: " + item.Password);
                    }
                }
            }
        }

        private void EditRow(object sender, RoutedEventArgs e)
        {
        }

        private void Nteczka_Click(object sender, RoutedEventArgs e)
        {
            Nowateczka Nowateczka = new Nowateczka();
            Nowateczka.username = username;
            Nowateczka.Show();
        }

        private void Szukajka_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = (sender as TextBox).Text.ToLower().Trim();
            var searchCollection = SearchData;
            dataGrid.ItemsSource = SearchData;
            var searchedCollection = new List<LoginData>();

            if (searchCollection != null)
            {
                foreach (var item in searchCollection)
                {
                    if (((Menadżer_3.LoginData)item).SiteName.ToLower().Contains(filter))
                    {
                        searchedCollection.Add(item);
                    }
                }
                dataGrid.ItemsSource = searchedCollection;
            }

            if (filter.Trim().Length == 0)
            {
                dataGrid.ItemsSource = SearchData;
            }
        }
    }

    public class LoginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SiteName { get; set; }
        public string UserName { get; set; }
    }
}