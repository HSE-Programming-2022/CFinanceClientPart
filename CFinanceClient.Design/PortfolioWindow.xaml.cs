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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using CFinance.Core;
using CFinance.Core.Models;

namespace CFinanceClient.Design
{
    /// <summary>
    /// Логика взаимодействия для PortfolioWindow.xaml
    /// </summary>
    public partial class PortfolioWindow : Window
    {
        public Portfolio CurrentPortfolio;
        public List<string> CompaniesToAdd;

        public PortfolioWindow(Portfolio currentPortfolio, List<string> companyList)
        {
            InitializeComponent();

            CurrentPortfolio = currentPortfolio;
            CompaniesToAdd = companyList;

            CompaniesToChoose.ItemsSource = CompaniesToAdd;

            this.DataContext = CurrentPortfolio;
        }

        private async void DeletePosition_OnClick(object sender, RoutedEventArgs e)
        {
            Label position = (sender as FrameworkElement).Tag as Label;

            string positionToDelete = position.Content.ToString();

            bool result = await PortfolioSession.DeleteFromPortfolio(CurrentPortfolio.PortfolioID, positionToDelete);

            if (result)
            {
                CompaniesToAdd.Add(positionToDelete);

                CompaniesToChoose.ItemsSource = CompaniesToAdd;

                CurrentPortfolio.Companies.Remove(new PortfolioCompany(){Ticker=positionToDelete});

                PortfolioCompanies.Items.Refresh();
            }
        }

        private async void CompanyToAdd_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string ticker = CompaniesToChoose.SelectedItem as string;

            bool result  = await PortfolioSession.AddToPortfolio(CurrentPortfolio.PortfolioID, ticker);

            if (result)
            {
                CurrentPortfolio.Companies.Add(new PortfolioCompany() {Ticker = ticker});
                
                CompaniesToAdd.Remove(ticker);

                CompaniesToChoose.ItemsSource= CompaniesToAdd;
                PortfolioCompanies.ItemsSource = CurrentPortfolio.Companies;
            }
            else
            {
                PortfolioName.Content = "Fail";
            }

        }
    }
}
