using EVP.WebToPay.ClientAPI;
using System.Threading.Tasks;
using Barkodai.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Barkodai.Core
{
    public class PayseraAPI
    {
        const int PROJECT_ID = 176610;
        const string PROJECT_PASSWORD = "6281642964070c8fc6df23720ee81281";

        private static Client client = new Client(PROJECT_ID, PROJECT_PASSWORD);
        
        public static async Task<string> pay(Cart cart)
        {
            MacroRequest request = client.NewMacroRequest();
            request.OrderId = "Užsakymas " + cart.id.ToString(); 
            request.Amount = (int)(cart.items.Select(x => x.price).Sum() * 100);
            request.Currency = "EUR";
            request.Country = "LT";
            request.AcceptUrl = "http://localhost:8000/Cart/OrderSuccess/" + cart.id.ToString();
            request.CancelUrl = "http://localhost:8000/Cart/OrderFailed/" + cart.id.ToString();
            request.CallbackUrl = "http://localhost:8000/Cart/OrderSuccess/" + cart.id.ToString();
            request.Test = true;
            return client.BuildRequestUrl(request);
        }
    }
}