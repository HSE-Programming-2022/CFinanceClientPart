using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CFinance.Core.Models;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;

namespace CFinance.Core.PaymentProcedure
{
    public static class Payments
    {
        private static readonly string secret_key = "eyJ2ZXJzaW9uIjoiUDJQIiwiZGF0YSI6eyJwYXlpbl9tZXJjaGFudF9zaXRlX3VpZCI6InY2NTk4OC0wMCIsInVzZXJfaWQiOiI3OTA1NTExMzkyMiIsInNlY3JldCI6IjRhMWNhYzJlNGFhNjU4ZTc0ODYxNDNmYjg4NDhkY2Y0MWI5ZDlkMTNlODE1Yjg2NDdlZTdjNjk2ZjViYjMwYjgifX0=";
        private static readonly string public_key = "48e7qUxn9T7RyYE1MVZswX1FRSbE6iyCj2gCRwwF3Dnh5XrasNTx3BGPiMsyXQFNKQhvukniQG8RTVhYm3iPy1jDiwp1ZMaMf7yMa6sdhpvihekWMY6jGwdjRyNr1Ttp5n9yuPirmAuMpyU8uodUU46GJh179dhmiedyxmFNx715Y8qGFVZex8o6WEPkB";

        public static async void InitiatePurchase(User user, string phoneNumber)
        {
            var client = BillPaymentsClientFactory.Create(
                secretKey: secret_key
            );

            var paymentForm = client.CreatePaymentForm(
                    paymentInfo: new PaymentInfo
                    {
                        PublicKey = public_key,
                        Amount = new MoneyAmount
                        {
                            ValueDecimal = 99.9m,
                            CurrencyEnum = CurrencyEnum.Rub
                        },
                        BillId = Guid.NewGuid().ToString(),
                    }
            );

            var psi = new System.Diagnostics.ProcessStartInfo();

            psi.UseShellExecute = true;
            psi.FileName = paymentForm.ToString();

            if (paymentForm != null)
            {
                System.Diagnostics.Process.Start(psi);
            }
        }
    }
}
