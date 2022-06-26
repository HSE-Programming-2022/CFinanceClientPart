using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using CFinance.Core;
using CFinance.Core.Models;
using ServiceStack;

namespace CFinanceClient.Design
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }
        private void LoginMode_OnClick(object sender, RoutedEventArgs e)
        {
            RegisterMode.BorderThickness = new Thickness(0, 0, 0, 0);
            LoginMode.BorderThickness = new Thickness(0, 0, 0, 2);

            RegLoginField.Text = "";
            RegEmailField.Text = "";
            RegPasswordField.Password = "";
            RepeatPasswordField.Password = "";

            LoginPanel.Visibility = Visibility.Visible;
            RegistrationPanel.Visibility = Visibility.Collapsed;

            LoginButton.Visibility = Visibility.Visible;
            RegistrationButton.Visibility = Visibility.Collapsed;

            Error.Visibility = Visibility.Hidden;
        }

        private void RegisterMode_OnClick(object sender, RoutedEventArgs e)
        {
            LoginMode.BorderThickness = new Thickness(0, 0, 0, 0);
            RegisterMode.BorderThickness = new Thickness(0, 0, 0, 2);

            LoginField.Text = "";
            PasswordField.Password = "";

            LoginPanel.Visibility = Visibility.Collapsed;
            RegistrationPanel.Visibility = Visibility.Visible;

            LoginButton.Visibility = Visibility.Collapsed;
            RegistrationButton.Visibility = Visibility.Visible;

            Error.Visibility = Visibility.Hidden;
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            string userLogin = LoginField.Text;
            string userPassword = PasswordField.Password;

            if (userLogin.IsEmpty() || userPassword.IsEmpty())
            {
                Error.Content = "Заполните все поля";
                Error.Visibility = Visibility.Visible;
            }
            else
            {
                User user = await UserSession.LoginUser(
                    username: userLogin,
                    password: userPassword
                );

                if (user == null)
                {
                    Error.Content = "Пользователь не найден, попробуйте снова!";
                    Error.Visibility = Visibility.Visible;
                }
                else
                {
                    Error.Visibility = Visibility.Hidden;
                    LoginButton.Content = user.UserID;

                    MainMenu menu = new MainMenu(user);

                    _mainFrame.Navigate(menu);
                }
            }
        }

        private async void RegistrationButton_OnClick(object sender, RoutedEventArgs e)
        {
            string userLogin = RegLoginField.Text;
            string userPassword = RegPasswordField.Password;
            string userEmail = RegEmailField.Text;

            if (userLogin.IsEmpty() || userPassword.IsEmpty() || userEmail.IsEmpty())
            {
                Error.Content = "Заполните все поля";
                Error.Visibility = Visibility.Visible;
                return;

            }

            User newUser = new User()
            {
                Email = userEmail,
                Password = userPassword,
                UserName = userLogin
            };

            bool response = await UserSession.RegisterUser(newUser);

            if (response)
            {
                var registeredUser = await UserSession.LoginUser(
                    username: RegLoginField.Text,
                    password: RegPasswordField.Password
                    );

                MainMenu menu = new MainMenu(registeredUser);

                _mainFrame.Navigate(menu);
            }
            else
            {
                Error.Visibility = Visibility.Visible;
                Error.Content = "Возможно, такой пользователь уже существует. Попробуйте снова.";
            }
        }

        private void PasswordField_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RepeatPasswordField.Password != RegPasswordField.Password)
            {
                Error.Visibility = Visibility.Visible;
                Error.Content = "Пароли не совпадают";

                RegistrationButton.IsEnabled = false;
            }
            else
            {
                Error.Visibility = Visibility.Hidden;
                RegistrationButton.IsEnabled = true;
            }
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
               if (RegistrationButton.Visibility == Visibility.Visible)
               {
                    RegistrationButton_OnClick(null, null);
               }
               else
               {
                    LoginButton_OnClick(null, null);
               }
            }
        }
    }
}
