using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CFinance.Core.Models;

namespace CFinance.Core
{
    public static class UserSession
    {
        private static readonly string BaseUri = "https://cfinance-api.herokuapp.com/User";
        static HttpClient HttpClient = new HttpClient();
        public static async Task<User> LoginUser(string username, string password)
        {
            User user = null;
            
            StringBuilder builder = new StringBuilder(BaseUri);
            builder.Append($"/Login?username={username}&password={password}");
            Console.WriteLine(builder.ToString());
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
