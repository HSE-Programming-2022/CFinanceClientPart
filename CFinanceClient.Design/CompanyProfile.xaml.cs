using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
using CFinance.Core.PythonModelConnector;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace CFinanceClient.Design
{
    /// <summary>
    /// Логика взаимодействия для CompanyProfile.xaml
    /// </summary>
    public partial class CompanyProfile : Page
    {
        private Company SelectedCompany;
        private List<NewsPost> companyNewsPosts;
        public bool IsSubscribed;
        public CompanyProfile(Company selectedCompany, bool isSubscribed)
        {
            InitializeComponent();
            
            SelectedCompany = selectedCompany;
            IsSubscribed = isSubscribed;

            this.SetUpCompanyProfile();
            this.YieldPlot();
            this.SetFinanceMetrica();
            this.SetUpNews();
        }

        private void SetUpCompanyProfile()
        {
            CompanyTicker.Content = SelectedCompany.Ticker;
            CompanyName.Content = SelectedCompany.Name;
            CompanyDescription.Text = SelectedCompany.Description;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private async void SetUpNews()
        {
            this.companyNewsPosts = await SelectedCompany.GetCompanyNewsAsync(10);
            this.CompanyNews.ItemsSource = companyNewsPosts;
        }

        private async void YieldPlot()
        {
            PlotModel newModel = new PlotModel()
            {
                Title = "Месячные цены на акции",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White
            };

            List<PriceHistoryData> prices = await SelectedCompany.GetHistoryPriceAsync();


            var priceSeries = new LineSeries();

            prices.ForEach(d => priceSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.Timestamp), (double) d.Open)));

            var dateAxis = new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80,
                AxislineColor = OxyColors.White,
                TicklineColor = OxyColors.White
            };
            newModel.Axes.Add(dateAxis);

            var valueAxis = new LinearAxis() 
            {
                Position=AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid, 
                MinorGridlineStyle = LineStyle.Dot,
                Title = "Цена",
                AxislineColor = OxyColors.White,
                TicklineColor = OxyColors.White,
            };

            newModel.Axes.Add(valueAxis);

            newModel.Series.Add(priceSeries);

            PricePlot.Model = newModel;
        }

        private string SumToView(decimal? sum)
        {
            if (sum is null)
            {
                return "-";
            }
            else
            {
                return $"{sum} $";
            }
        }
        private async void SetFinanceMetrica()
        {
            CurrentLiabilities.Content = SumToView(SelectedCompany.BalanceSheet.CurrentLiabilities);
            LongLiabilities.Content = SumToView(SelectedCompany.BalanceSheet.LongtermLiabilities);
            CurrentAssets.Content = SumToView(SelectedCompany.BalanceSheet.CurrentAssets);
            LongAssets.Content = SumToView(SelectedCompany.BalanceSheet.LongtermAssets);
            InvCap.Content = SumToView(SelectedCompany.BalanceSheet.InvestedCapital);
            RetEarn.Content = SumToView(SelectedCompany.BalanceSheet.RetainedEarnings);

            Investment.Content = SumToView(SelectedCompany.Cashflow.InvestmentFlow);
            Financing.Content = SumToView(SelectedCompany.Cashflow.FinancingFlow);
            Operating.Content = SumToView(SelectedCompany.Cashflow.OperatingFlow);
            Delta.Content = SumToView(SelectedCompany.Cashflow.CashFlowDelta);

            EBITDA.Content = SumToView(SelectedCompany.IncomeStatement.EBITDA);
            EBIT.Content = SumToView(SelectedCompany.IncomeStatement.EBIT);
            EBT.Content = SumToView(SelectedCompany.IncomeStatement.EBT);
            Net.Content = SumToView(SelectedCompany.IncomeStatement.NetEarnings);

            dps.Content = SelectedCompany.Metrics.dps.ToString();
            pe.Content = SelectedCompany.Metrics.pe.ToString();
            rps.Content = SelectedCompany.Metrics.rps.ToString();
            eps.Content = SelectedCompany.Metrics.eps.ToString();
            roa.Content = SelectedCompany.Metrics.roa.ToString();
            roe.Content = SelectedCompany.Metrics.roe.ToString();
            beta.Content = SelectedCompany.Metrics.beta.ToString();
            evr.Content = SelectedCompany.Metrics.evr.ToString();
            yield.Content = SelectedCompany.Metrics.dividend_yield.ToString();

            if (IsSubscribed)
            {
                string predictedPrice = await PythonRegressionBoosting.EstimateFuturePrice(SelectedCompany.Ticker);
                string priceView = $"{predictedPrice.Substring(0, predictedPrice.IndexOf('.') + 2)} USD";

                PredicttedPrice.Content = priceView;
            }
            else
            {
                PredicttedPrice.Content = ":(";
            }

        }

        private void BearishNewsSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            var bearishNews = this.companyNewsPosts.Where(x => x.SentimentLabel.Contains("Bearish")).ToList();

            CompanyNews.ItemsSource = bearishNews;
        }

        private void NeutralNewsSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            var neutralNews = this.companyNewsPosts.Where(x => x.SentimentLabel.Contains("Neutral")).ToList();

            CompanyNews.ItemsSource = neutralNews;
        }

        private void BullishNewsSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            var bullishNews = this.companyNewsPosts.Where(x => x.SentimentLabel.Contains("Bullish")).ToList();

            CompanyNews.ItemsSource = bullishNews;
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
        }

        private void PostSummary_OnInitialized(object? sender, EventArgs e)
        {
            TextBlock summary = sender as TextBlock;

            NewsPost post = summary.DataContext as NewsPost;

            summary.Text = post.Summary;
        }
    }
}

