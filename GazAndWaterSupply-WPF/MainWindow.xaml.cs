using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace GazAndWaterSupply_WPF
{
    public partial class MainWindow : Window
    {
        HubConnection connection;  // подключение для взаимодействия с хабом
        public MainWindow()
        {
            InitializeComponent();


            // создаем подключение к хабу
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7077/chat")
                .WithAutomaticReconnect()   // автопереподключение
                .Build();


            // регистрируем функцию Receive для получения данных
            connection.On<string, string>("Receive", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    chatbox.Items.Insert(0, newMessage);
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
        }

        // обработчик загрузки окна
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // подключемся к хабу
                await connection.StartAsync();
                chatbox.Items.Add("Вы вошли в чат");
                sendBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }
        // обработчик нажатия на кнопку
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // отправка сообщения
                await connection.InvokeAsync("Send", userTextBox.Text, messageTextBox.Text);
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }

        // обработчик закрытия окна
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await connection.InvokeAsync("Send", "", $"Пользователь {userTextBox.Text} выходит из чата");
            await connection.StopAsync();   // отключение от хаба
        }
    }
}