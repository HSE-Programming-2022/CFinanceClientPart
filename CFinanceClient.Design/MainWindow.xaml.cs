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


namespace CFinanceClient.Design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private async void Login_OnClick(object sender, RoutedEventArgs e)
        {
            string username = LoginField.Text;
            string password = PasswordField.Text;
            var user = await UserSession.LoginUser(password: password, username: username);

            if (user != null)
                Test.Content = user.UserID;
            else
                Test.Content = "Not found";
        }

        private async void SearchCompany_OnClick(object sender, RoutedEventArgs e)
        {
            string ticker = CompanyTicker.Text;

            var company = await CompanySession.GetCompanyTask(ticker);

            if (company != null)
            {
                List<PriceHistoryData> prices = await company.GetHistoryPriceAsync();
                Test.Content = prices[0].High;
            }
            else
                Test.Content = "Not found";
        }
    }
}
