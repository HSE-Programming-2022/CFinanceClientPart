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
        private static string EnvPath = "C:\\Users\\Adedal\\AppData\\Local\\Programs\\Python\\Python310\\python.exe"
        private static string scriptName = "model.py";

        public static float EstimateFuturePrice(string ticker)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            start.Arguments = $"\"{scriptName}\" \"{ticker}\"";
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd();
                    string result = reader.ReadToEnd();
                    
                    return float.Parse(result);
                }
            }
        }

    }
}
