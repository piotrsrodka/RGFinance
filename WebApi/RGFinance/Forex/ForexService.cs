using Database;
using System.Xml.Linq;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace RGFinance.FlowFeature
{
    public class ForexService : IForexService
    {
        private readonly RGFContext context;

        public ForexService(RGFContext context)
        {
            this.context = context;
        }

        public async Task<Forex> GetForex()
        {
            Forex result = null!;

            try
            {
                result = await this.GetForexFromApi();
                result.Online = true;
            }
            catch (Exception ex)
            {
                result = await this.context.Forexes
                    .OrderByDescending(f => f.Time)
                    .FirstOrDefaultAsync()!;
            }

            if (result == null)
            {
                throw new InvalidOperationException("No forex data available.");
            }

            return result;
        }

        public async Task<Forex> GetForexFromApi()
        {         
            XDocument xdoc = XDocument.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            List<CurrencyRate>? ecb = xdoc.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null)
                .Select(x => new CurrencyRate
                {
                    Currency = (string)x.Attribute("currency"),
                    Rate = (decimal)x.Attribute("rate")
                })
                .ToList();

            var times = xdoc.DescendantNodes().OfType<XElement>()
                .Where(e => e.Name.LocalName == "Cube" && e.Attribute("time") != null)
                .ToList();

            string time2 = times.First().Attribute("time")!.Value;

            decimal usdeur = ecb.Single(cr => cr.Currency == "USD").Rate;
            decimal plneur = ecb.Single(cr => cr.Currency == "PLN").Rate;
            decimal plnusd = plneur / usdeur;

            // GOLD
            var goldJsonString = await new HttpClient().GetStringAsync("https://data-asg.goldprice.org/dbXRates/USD");
            int xauPrice = goldJsonString.IndexOf("xauPrice");
            int colonIndex = goldJsonString.IndexOf(":", xauPrice);
            int comaIndex = goldJsonString.IndexOf(".", colonIndex);
            string goldPriceString = goldJsonString.Substring(colonIndex + 1, comaIndex - colonIndex - 1);
            decimal goldPriceUsd = decimal.Parse(goldPriceString, System.Globalization.NumberStyles.Number);
            decimal goldPricePln = goldPriceUsd * plnusd;

            // BTC & ETH
            var btcPriceUsd = await this.GetCryptoPrice("90");
            var ethPriceUsd = await this.GetCryptoPrice("80");            
            var solPriceUsd = await this.GetCryptoPrice("48543");
            var dogePriceUsd = await this.GetCryptoPrice("2");

            var forex = new Forex
            {
                Time = time2,
                Usd = plnusd,
                Eur = plneur,
                Gold = goldPricePln,
                Btc = btcPriceUsd * plnusd,
                Eth = ethPriceUsd * plnusd,
                Sol = solPriceUsd * plnusd,
                Doge = dogePriceUsd * plnusd
            };

            var dbForexWithCurrentTime = await this.context.Forexes
                    .FirstOrDefaultAsync(f => f.Time == forex.Time);

            if (dbForexWithCurrentTime == null)
            {
                this.context.Forexes.Add(forex);
                await this.context.SaveChangesAsync();
            }

            return forex;
        }

        private async Task<decimal> GetCryptoPrice(string id)
        {
            var client = new HttpClient();
            string response = await client.GetStringAsync($"https://api.coinlore.net/api/ticker/?id={id}");
            response = response.TrimStart('[').TrimEnd(']');
            var json = System.Text.Json.JsonDocument.Parse(response);
            var string_result = json.RootElement.GetProperty("price_usd").GetString();
            var formatProvider = new System.Globalization.CultureInfo("en-US");
            var result = decimal.Parse(string_result!, System.Globalization.NumberStyles.Any, formatProvider);

            return result;
        }
    }
}
