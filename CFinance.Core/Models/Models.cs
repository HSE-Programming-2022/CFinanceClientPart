﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using ServiceStack;

namespace CFinance.Core.Models
{
    public static class ApiTools
    {
        private static readonly string apiKey = "TDM8L7KVPP1QD1TT";

        public static async Task<List<NewsPost>> GetAllNewsAsync()
        {
            List<NewsPost> resultFeed = new List<NewsPost>();

            var GetNewsFeed = await $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&apikey={apiKey}"
                .GetStringFromUrlAsync();
            dynamic newsFeed = JsonConvert.DeserializeObject(GetNewsFeed);

            foreach (var post in newsFeed.feed)
            {
                resultFeed.Add(new NewsPost()
                {
                    Title = post.title,
                    ImageSource = post.banner_image,
                    PostUrl = post.url,
                    TimePublished = post.time_published,
                    Summary = post.summary,
                    SentimentLabel = post.overall_sentiment_label
                });
            }

            return resultFeed;
        }
    }
    public class NewsPost
    {
        public string Title;
        public string PostUrl;
        public string? TimePublished;
        public string? Summary;
        public string? ImageSource;
        public string SentimentLabel;
    }

    public class PriceHistoryData
    {
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }

    }
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
        public string Email { get; set; }
        public List<Portfolio> Portfolios { get; set; }

        public bool SubscriptionStatus { get; set; }

    }

    public class Company
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public int IndusrtyID { get; set; }
        public int SectorID { get; set; }

        public Cashflows Cashflow { get; set; }
        public Metrics Metrics { get; set; }
        public IncomeStatement IncomeStatement { get; set; }
        public BalanceSheet BalanceSheet { get; set; }

        private readonly string apiKey = "TDM8L7KVPP1QD1TT";

        public async Task<List<PriceHistoryData>> GetHistoryPriceAsync()
        {
            
            var GetPriceTask = await $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={this.Ticker}&apikey={apiKey}&datatype=csv"
                .GetStringFromUrlAsync();

            var monthlyPrices = GetPriceTask.FromCsv<List<PriceHistoryData>>();
            
            return monthlyPrices;
        }

        public async Task<List<NewsPost>> GetCompanyNewsAsync(int amount)
        {
            List<NewsPost> resultFeed = new List<NewsPost>();

            var GetNewsFeed = await $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&tickers={this.Ticker}&limit={amount}&apikey={this.apiKey}"
                        .GetStringFromUrlAsync();
            dynamic newsFeed = JsonConvert.DeserializeObject(GetNewsFeed);

            foreach (var post in newsFeed.feed)
            {
                resultFeed.Add(new NewsPost()
                {
                    Title = post.title,
                    ImageSource = post.banner_image,
                    PostUrl = post.url,
                    TimePublished = post.time_published,
                    Summary = post.summary,
                    SentimentLabel = post.overall_sentiment_label
                });
            }
            return resultFeed;
        }
    }

    public class Cashflows
    {
        public string Ticker { get; set; }
        public int CurrencyID { get; set; }
        public decimal? OperatingFlow { get; set; }
        public decimal? FinancingFlow { get; set; }
        public decimal? InvestmentFlow { get; set; }
        public decimal? CashFlowDelta { get; set; }
        [JsonIgnore] public Company Company { get; set; }

    }

    public class Metrics
    {
        public string? Ticker { get; set; }
        public double? pe { get; set; }
        public double? dps { get; set; }
        public decimal? dividend_yield { get; set; }
        public double? eps { get; set; }
        public double? rps { get; set; }
        public double? roe { get; set; }
        public double? beta { get; set; }
        public double? evr { get; set; }
        public double? roa { get; set; }

        public Company Company { get; set; }
    }

    public class IncomeStatement
    {
        public string Ticker { get; set; }
        public decimal? EBITDA { get; set; }
        public decimal? EBIT { get; set; }
        public decimal? EBT { get; set; }
        public decimal? NetEarnings { get; set; }

        [JsonIgnore] public Company Company;
    }

    public class BalanceSheet
    {
        public string Ticker { get; set; }
        public decimal? CurrentLiabilities { get; set; }
        public decimal? LongtermLiabilities { get; set; }
        public decimal? CurrentAssets { get; set; }
        public decimal? LongtermAssets { get; set; }
        public decimal? InvestedCapital { get; set; }
        public decimal? RetainedEarnings { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        [JsonIgnore] public Company Company { get; set; }
    }

    public class Portfolio :INotifyPropertyChanged
    {
        public int UserID { get; set; }
        public int PortfolioID { get; set; }
        public string Name { get; set; }
        public List<PortfolioCompany> Companies { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public List<PortfolioCompany> CompaniesView
        {
            get { return Companies; }
            set
            {
                Companies = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class PortfolioCompany : INotifyPropertyChanged
    {
        public string Ticker { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        
        [JsonIgnore]
        public string TickerView
        {
            get { return Ticker; }
            set
            {
                Ticker = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string ticker = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ticker));
        }
    }

}
