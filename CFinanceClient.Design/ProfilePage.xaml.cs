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
using CFinance.Core.Models;
using CFinance.Core.PaymentProcedure;

namespace CFinanceClient.Design
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private User CurrentUser { get; set; }

        public ProfilePage(User currentUser)
        {
            InitializeComponent();

            CurrentUser = currentUser;
            Username.Content = currentUser.UserName;
            Email.Content = currentUser.Email;

            if (currentUser.SubscriptionStatus)
            {
                SubStatus.Content = "Да";
                SubStatus.Foreground = new SolidColorBrush(Colors.ForestGreen);
                
                PurchaseButton.Visibility = Visibility.Collapsed;
                PhoneNumber.Visibility = Visibility.Collapsed;

                SubBanner.Text = "Твоя подписка действительна -- попробуй посмотреть наши прогнозы :)";
            }
            else
            {
                SubStatus.Content = "Нет";
                SubStatus.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.GoBack();
        }

        private void PurchaseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PhoneNumber.Text))
            {
                Payments.InitiatePurchase(CurrentUser, PhoneNumber.Text);
            }
        }
    }
}
