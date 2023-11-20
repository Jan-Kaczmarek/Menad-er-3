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

        public void Handle(RefreshMessage message)
        {
            var data = message.Data;

            dataGrid_Loaded(null, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
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
                        loginData.UserName = row[1].Trim();
                    else if (row[0].Trim() == "Hasło")
                    {
                        loginData.Password = row[1].Trim();
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
            // Szukajka
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
            // Pobierz wybrany wiersz
            LoginData loginData = (LoginData)dataGrid.SelectedItem;

            // Jeśli żaden wiersz nie jest wybrany, wyświetl komunikat
            if (loginData == null)
            {
                MessageBox.Show("Proszę wybrać wiersz do edycji.");
                return;
            }

            // Ustaw wybrany wiersz jako edytowany
            dataGrid.BeginEdit();
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