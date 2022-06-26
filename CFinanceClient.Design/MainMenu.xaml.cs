using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    ///
    public partial class MainMenu : Page
    {
        public User CurrentUser;
        private List<Company> companies;
        public List<Portfolio> UserPortfolios;
        public MainMenu(User currentUser)
        {
            InitializeComponent();

            WelcomeLabel.Content = $"Добро пожаловать, {currentUser.UserName}";

            CurrentUser = currentUser;
            
            Portfolios.ItemsSource = CurrentUser.Portfolios;

            this.ShowNewsFeed();
            this.SetUpCompanyList();
        }

        private async void ShowNewsFeed()
        {
            List<NewsPost> feed = await ApiTools.GetAllNewsAsync();

            NewsFeed.ItemsSource = feed;
        }

        private async void SetUpCompanyList()
        {
            List<Company> companyList = await CompanySession.GetAllCompanies();
            
            this.companies = companyList;

            CompanyList.ItemsSource = companyList;
        }
        private void PostBanner_OnInitialized(object? sender, EventArgs e)
        {
            Image postImage = sender as Image;

            NewsPost post = postImage.DataContext as NewsPost;

            string imageUri = post.ImageSource;

            if (imageUri is not "")
                postImage.Source = new BitmapImage(new Uri(post.ImageSource));
        }

        private void PostTitle_OnInitialized(object? sender, EventArgs e)
        {
            Label title = sender as Label;

            NewsPost post = title.DataContext as NewsPost;

            title.Content = post.Title;

            if (post.SentimentLabel.Contains("Bearish"))
            {
                title.Background = new SolidColorBrush(Colors.Crimson) {Opacity = 0.5};
            }
            else if (post.SentimentLabel.Contains("Bullish"))
            {
                title.Background = new SolidColorBrush(Colors.ForestGreen) { Opacity = 0.5 };
            }
        }


        private void PostSummary_OnInitialized(object? sender, EventArgs e)
        {
            TextBlock summary = sender as TextBlock;

            NewsPost post = summary.DataContext as NewsPost;

            summary.Text = post.Summary;
        }

        private void CompanyTicker_OnInitialized(object? sender, EventArgs e)
        {
            Label ticker = sender as Label;
            
            Company company = ticker.DataContext as Company;

            ticker.Content = company.Ticker;
        }

        private void CompanyName_OnInitialized(object? sender, EventArgs e)
        {
            Label name = sender as Label;
            
            Company company = name.DataContext as Company;

            name.Content = company.Name;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            Company selectedCompany = CompanyList.SelectedItem as Company;

            if (selectedCompany != null)
            {
                CompanyProfile profile = new CompanyProfile(selectedCompany, CurrentUser.SubscriptionStatus);

                NavigationService nav = NavigationService.GetNavigationService(this);

                nav.Navigate(profile);
            }
        }

        private void News_OnHandler(object sender, MouseButtonEventArgs e)
        {
            NewsPost selectedPost = NewsFeed.SelectedItem as NewsPost;
            MessageBoxResult goToBrowser = MessageBox.Show("Открыть новостной пост в браузере?", "Переход по ссылке",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (goToBrowser == MessageBoxResult.Yes)
            {
                var psi = new System.Diagnostics.ProcessStartInfo();

                psi.UseShellExecute = true;
                psi.FileName = selectedPost.PostUrl;

                if (selectedPost != null)
                {
                    System.Diagnostics.Process.Start(psi);
                }
            }
        }

        private void SearchField_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string queryString = SearchField.Text.ToLower();

            if ((!queryString.IsNullOrEmpty()) && (queryString != "Поиск"))
            {
                List<Company> queryResult =
                    companies.FindAll(x => x.Name.ToLower().Contains(queryString) || x.Ticker.ToLower().Contains(queryString));

                CompanyList.ItemsSource = queryResult;
            }
            else
            {
                CompanyList.ItemsSource = this.companies;
            }
        }

        private void SearchField_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchField.Text == "Поиск")
            {
                SearchField.Text = "";
            }
        }

        private void SearchField_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchField.Text))
            {
                SearchField.Text = "Поиск";
            }
        }

        private void ToProfile_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);

            this.NavigationService.Navigate(new ProfilePage(CurrentUser));
        }

        private async void AddPortfolio_OnClick(object sender, RoutedEventArgs e)
        {
            if (!NewPortfolioNameField.Text.IsNullOrEmpty())
            {
                int newPortfolioId = await PortfolioSession.CreatePortfolioForUser(CurrentUser.UserID, NewPortfolioNameField.Text);
                var name = NewPortfolioNameField.Text;

                if (newPortfolioId == -1)
                {
                    AddPortfolio.Background = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    NewPortfolioNameField.Text = "";

                    CurrentUser.Portfolios.Add(new Portfolio() { Name = name, PortfolioID = newPortfolioId, UserID = CurrentUser.UserID });
                }
            }
        }

        private void PortfolioView_OnHandler(object sender, MouseButtonEventArgs e)
        {
            Portfolio selectedPortfolio = Portfolios.SelectedItem as Portfolio;

            List<string> availableCompanies = (from x in companies select x.Ticker).ToList();

            PortfolioWindow newPortfolioWindow = new PortfolioWindow(selectedPortfolio, availableCompanies);

            newPortfolioWindow.Show();
        }

        private async void RefreshPortfolios_OnClick(object sender, RoutedEventArgs e)
        {
            List<Portfolio> currentUserPortfolios = await PortfolioSession.GetPortfoliosForUser(CurrentUser.UserID);

            Portfolios.ItemsSource = currentUserPortfolios;
        }
    }
}
