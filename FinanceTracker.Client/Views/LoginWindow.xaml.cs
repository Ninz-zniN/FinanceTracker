using FinanceTracker.Client.Services;
using FinanceTracker.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceTracker.Client.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dto = new LoginDto
                {
                    Username = UsernameBox.Text,
                    Password = PasswordBox.Password
                };

                var response = await ApiClient.PostAsync<LoginDto, AuthResponseDto>("auth/login", dto);

                if (response != null)
                {
                    ApiClient.SetToken(response.Token);
                    new MainWindow().Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка входа: " + ex.Message);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dto = new RegisterDto
                {
                    Username = UsernameBox.Text,
                    Password = PasswordBox.Password
                };

                var response = await ApiClient.PostAsync<RegisterDto, AuthResponseDto>("auth/register", dto);

                if (response != null)
                {
                    ApiClient.SetToken(response.Token);

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка регистрации: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
