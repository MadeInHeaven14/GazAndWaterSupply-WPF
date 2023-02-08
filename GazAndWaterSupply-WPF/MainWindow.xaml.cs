using Microsoft.AspNetCore.SignalR.Client;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace GazAndWaterSupply_WPF
{
    public partial class MainWindow : Window
    {
        public static User currentUser = new User();
        static MongoClient client = new MongoClient();
        static IMongoDatabase database = client.GetDatabase("UsersDB");
        static IMongoCollection<User> collection = database.GetCollection<User>("Users");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAutorization_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text != string.Empty && PasswordBox.Text != string.Empty)
            {
                var User = collection.Find(x => x.Login == LoginBox.Text).FirstOrDefault();
                if (User != null && User.Role == "Заказчик")
                {
                    if (PasswordBox.Text == User.Password)
                    {
                        currentUser = User;
                        ChatWindow win = new ChatWindow();
                        win.Show();
                        this.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Пользователь не найден!");
                }
            }
        }
    }
}