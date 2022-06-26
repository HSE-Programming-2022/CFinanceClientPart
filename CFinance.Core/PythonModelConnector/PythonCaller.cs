using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;


namespace CFinance.Core.PythonModelConnector
{
    public static class PythonRegressionBoosting
    {
        private static string EnvPath = "C:\\Users\\Adedal\\AppData\\Local\\Programs\\Python\\Python310\\python.exe";
        private static string scriptName = "PythonModelConnector\\BoostingRegression.py";

        public static async Task<string> EstimateFuturePrice(string ticker)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            start.FileName = EnvPath;
            start.Arguments = $"\"{scriptName}\" {ticker}";
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd();
                    string result = await reader.ReadToEndAsync();
                    

                    return result;
                }
            }
        }

    }
}
