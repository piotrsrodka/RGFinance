using Database;
using System.Xml;
using System.Xml.Linq;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace RGFinance.FlowFeature
{
    public class FlowService : IFlowService
    {
        private readonly RGFContext context;

        public FlowService(RGFContext context)
        {
            this.context = context;
        }

        public async Task<Flow> GetFlowAsync(int id)
        {
            Forex? forex = await this.GetForex();

            List<Asset> assets = await this.context.Assets
                .ToListAsync();

            List<Profit> profits = await this.context.Profits
                .ToListAsync();

            List<Expense> expenses = await this.context.Expenses
                .ToListAsync();

            // Profits for assets with interest
            List<Profit> interestProfits = GetInterestProfits(assets);

            // Combine
            profits = profits.Concat(interestProfits).ToList();

            foreach (var profit in profits)
            {
                this.ExchangeFromPLN(profit, forex);
            }

            foreach (var asset in assets)
            {
                this.ExchangeFromPLN(asset, forex);
            }

            foreach (var expense in expenses)
            {
                this.ExchangeFromPLN(expense, forex);
            }


            decimal bigSum = 0;

            decimal plns = assets.Where(s => s.Currency.ToUpper() == "PLN").Sum(s => s.Value);
            decimal eurs = assets.Where(s => s.Currency.ToUpper() == "EUR").Sum(s => s.Value * forex.Eur);
            decimal usds = assets.Where(s => s.Currency.ToUpper() == "USD").Sum(s => s.Value * forex.Usd);
            decimal goldie = assets.Where(s => s.Currency.ToUpper() == "GOZ").Sum(s => s.Value * forex.Gold);

            bigSum = plns + eurs + usds + goldie;

            return new Flow
            {
                BigSum = bigSum,
                Expenses = expenses.OrderByDescending(e => e.CurrentCurrencyValue).ToList(),
                Assets = assets.OrderByDescending(s => s.CurrentCurrencyValue).ToList(),
                Profits = profits.OrderByDescending(p => p.CurrentCurrencyValue).ToList(),
            };
        }

        public async Task<int> AddOrUpdateAsset(Asset asset)
        {
            this.context.Assets.Update(asset);
            return await this.context.SaveChangesAsync();
        }

        public async Task<int> AddOrUpdateProfit(Profit profit)
        {
            this.context.Profits.Update(profit);
            return await this.context.SaveChangesAsync();
        }

        public async Task<int> AddOrUpdateExpense(Expense expense)
        {
            this.context.Expenses.Update(expense);
            return await this.context.SaveChangesAsync();
        }

        public async Task<Forex> GetForex()
        {
            // From StackOverflow - I have no idea how and why it works ;)

            XmlDocument doc = new XmlDocument();
            doc.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");

            // add another namespace alias for the *default* namespace
            nsmgr.AddNamespace("default", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");

            // *USE* the default namespace for those nodes that don't have an explicit
            // XML namespace alias in your XML document
            XmlNodeList nodes = doc.SelectNodes("gesmes:Envelope/default:Cube", nsmgr);

            // Time
            XmlNode timeNode = nodes[0].SelectSingleNode("default:Cube", nsmgr);
            string time = timeNode.Attributes["time"].Value;

            /* ALTERNATIVE */
            XDocument xdoc = XDocument.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            List<CurrencyRate>? res2 = xdoc.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null)
                .Select(x => new CurrencyRate
                {
                    Currency = (string)x.Attribute("currency"),
                    Rate = (decimal)x.Attribute("rate")
                })
                .ToList();

            var usdeur = res2.Single(cr => cr.Currency == "USD").Rate;
            var plneur = res2.Single(cr => cr.Currency == "PLN").Rate;
            var plnusd = plneur / usdeur;

            // GOLD
            var goldJsonString = await new HttpClient().GetStringAsync("https://data-asg.goldprice.org/dbXRates/USD");
            int xauPrice = goldJsonString.IndexOf("xauPrice");
            int colonIndex = goldJsonString.IndexOf(":", xauPrice);
            int comaIndex = goldJsonString.IndexOf(".", colonIndex);
            string goldPriceString = goldJsonString.Substring(colonIndex + 1, comaIndex - colonIndex - 1);
            decimal goldPriceUsd = decimal.Parse(goldPriceString, System.Globalization.NumberStyles.Number);
            decimal goldPricePln = goldPriceUsd * plnusd;

            var forex = new Forex
            {
                Time = time,
                Usd = plnusd,
                Eur = plneur,
                Gold = goldPricePln,
            };

            return forex;
        }

        public async Task DeleteProfit(int profitId)
        {
            var profit = await this.context.Profits.FirstOrDefaultAsync(p => p.Id == profitId);
            this.context.Profits.Remove(profit);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteAsset(int id)
        {
            var asset = await this.context.Assets.FirstOrDefaultAsync(p => p.Id == id);
            this.context.Assets.Remove(asset);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteExpense(int id)
        {
            var expense = await this.context.Expenses.FirstOrDefaultAsync(p => p.Id == id);
            this.context.Expenses.Remove(expense);
            await this.context.SaveChangesAsync();
        }

        private void ExchangeFromPLN(ValueObject valueObject, Forex forex)
        {
            if (valueObject.Currency.ToUpper() == "PLN")
            {
                valueObject.CurrentCurrencyValue = valueObject.Value;
            }
            else if (valueObject.Currency.ToUpper() == "EUR")
            {
                valueObject.CurrentCurrencyValue = valueObject.Value * forex.Eur;
            }
            else if (valueObject.Currency.ToUpper() == "USD")
            {
                valueObject.CurrentCurrencyValue = valueObject.Value * forex.Usd;
            }
            else if (valueObject.Currency.ToUpper() == "GOZ")
            {
                valueObject.CurrentCurrencyValue = valueObject.Value * forex.Gold;
            }
        }

        private static List<Profit> GetInterestProfits(List<Asset> assets)
        {
            List<Profit> interestProfits = new List<Profit>();

            foreach (Asset asset in assets)
            {
                if (asset.Interest > 0)
                {
                    var grossValue = asset.Value * asset.Interest / 1200.0m;

                    var interestProfit = new Profit
                    {
                        Currency = asset.Currency,
                        Id = -1,
                        Name = asset.Name + " %%",
                        Rate = Rate.Monthly,
                        Value = Math.Round(grossValue - (0.19m * grossValue), 2),
                        IsInterestProfit = true,
                    };

                    interestProfits.Add(interestProfit);
                }
            }

            return interestProfits;
        }
    }
}
