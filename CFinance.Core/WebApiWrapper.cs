using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CFinance.Core.Models;
using Newtonsoft.Json;

namespace CFinance.Core
{
    public static class PortfolioSession
    {
        private static readonly string BaseUri = "https://cfinance-api.herokuapp.com/Portfolio";
        static HttpClient client = new HttpClient();

        public static async Task<List<Portfolio>?> GetPortfoliosForUser(int uid)
        {
            List<Portfolio> userPortfolios = new List<Portfolio>();

            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/GetUserPortfolios?uid={uid}");

            HttpResponseMessage response = await client.GetAsync(builder.ToString());

            if (response.IsSuccessStatusCode)
            {
                userPortfolios = await response.Content.ReadAsAsync<List<Portfolio>>();
            }

            return userPortfolios;
        }
        public static async Task<int> CreatePortfolioForUser(int uid, string name)
        {
            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/CreatePortfolioForUser");

            var parameters = new Dictionary<string, dynamic>()
            {
                {"uid", uid},
                {"name", name}
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                builder.ToString(), parameters);

            if (response.IsSuccessStatusCode)
            {
                int portfolioId = int.Parse(await response.Content.ReadAsStringAsync());

                return portfolioId;
            }

            return -1;
        }

        public static async Task<bool> AddToPortfolio(int pid, string ticker)
        {
            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/AddToPortfolio");

            var parameters = new Dictionary<string, dynamic>()
            {
                {"pid", pid},
                {"ticker", ticker}
            }; ;

            HttpResponseMessage response = await client.PostAsJsonAsync(builder.ToString(), parameters);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> DeleteFromPortfolio(int pid, string ticker)
        {
            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/DeleteFromPortfolio?pid={pid}&ticker={ticker}");

            HttpResponseMessage response = await client.DeleteAsync(builder.ToString());

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

    }
    public static class UserSession
    {
        private static readonly string BaseUri = "https://cfinance-api.herokuapp.com/User";
        static HttpClient HttpClient = new HttpClient();
        public static async Task<User> LoginUser(string username, string password)
        {
            User user = null;
            
            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/Login?username={username}&password={password}");
            
            HttpResponseMessage response = await HttpClient.GetAsync(builder.ToString());

            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<User>();
            }

            return user;
        }

        public static async Task<bool> RegisterUser(User newUser)
        {

            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/Register");

            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
                builder.ToString(), newUser);

            return response.IsSuccessStatusCode;
        }
    }

    public static class CompanySession
    {
        private static readonly string BaseUri = "https://cfinance-api.herokuapp.com/Company";
        static HttpClient HttpClient = new HttpClient();

        public static async Task<Company> GetCompanyTask(string ticker)
        {
            Company company = null;

            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/{ticker}");

            HttpResponseMessage response = await HttpClient.GetAsync(builder.ToString());

            if (response.IsSuccessStatusCode)
            {
                company = await response.Content.ReadAsAsync<Company>();
            }

            return company;
        }

        public static async Task<List<Company>> GetAllCompanies()
        {
            List<Company> companies = null;

            StringBuilder builder = new StringBuilder(BaseUri);

            HttpResponseMessage response = await HttpClient.GetAsync(builder.ToString());

            if (response.IsSuccessStatusCode)
            {
                companies = await response.Content.ReadAsAsync<List<Company>>();
            }

            return companies;
        }
    }
}
