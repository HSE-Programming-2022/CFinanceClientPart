using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CFinance.Core.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
        public string Email { get; set; }

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

        [JsonIgnore] public Company Company { get; set; }
    }

}
