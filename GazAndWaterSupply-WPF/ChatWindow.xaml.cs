using Microsoft.AspNetCore.SignalR.Client;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace GazAndWaterSupply_WPF
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        HubConnection connection;
        static MongoClient client = new MongoClient();
        static IMongoDatabase database = client.GetDatabase("UsersDB");
        static IMongoCollection<User> collectionUsers = database.GetCollection<User>("Users");
        static IMongoCollection<Projects> collectionProjects = database.GetCollection<Projects>("Projects");
        public ChatWindow()
        {
            InitializeComponent();
            LoginLabel.Content = MainWindow.currentUser.Login;

            // создаем подключение к хабу
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7077/chat")
                .WithAutomaticReconnect()   // автопереподключение
                .Build();


            // регистрируем функцию Receive для получения данных
            connection.On<string, string, string>("Receive", (senderName, receiverName, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (message != string.Empty && message != null)
                    {
                        var newMessage = $"{senderName}: {message}";
                        ChatPanel.Items.Add(newMessage);
                    }    
                });
            });

            // перед подключением
            connection.Reconnecting += error =>
            {
                // обработка события

                return Task.CompletedTask;
            };

            // после подключения
            connection.Reconnected += connectionId =>
            {
                // обработка события

                return Task.CompletedTask;
            };
            LoadUser();
        }

        void LoadUser()
        {
            foreach (var item in collectionProjects.AsQueryable().ToList())
            {
                if (LoginLabel.Content.ToString() == item.Customer)
                {
                    if (UsersList.Items.Contains(item.Developer) == false)
                    {
                        UsersList.Items.Add(item.Developer);
                    }

                    if (UsersList.Items.Contains(item.Designer) == false)
                    {
                        UsersList.Items.Add(item.Designer);
                    }
                    //UsersList.Items.Add(item.Designer);
                }

                //if (item.Role != "Заказчик")
                //{
                //    UsersList.Items.Add(item.Login);
                //}
            }
        }

        // обработчик загрузки окна
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // подключемся к хабу
                await connection.StartAsync();
                ChatPanel.Items.Add("Вы вошли в чат");
                BtnSend.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ChatPanel.Items.Add(ex.Message);
            }
        }
        

        // обработчик закрытия окна
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await connection.InvokeAsync("Send", "", "", $"Пользователь {LoginLabel.Content} выходит из чата");
            await connection.StopAsync();   // отключение от хаба
        }

        // обработчик нажатия на кнопку
        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // отправка сообщения
                await connection.InvokeAsync("Send", LoginLabel.Content, UsersList.SelectedItem.ToString(), Message_Box.Text);
                Message_Box.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ChatPanel.Items.Add(ex.Message);
            }
        }
    }
}
