using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Xml.Linq;

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

            List<State> states = await this.context.States
                // .OrderByDescending(p => p.Value)
                .ToListAsync();

            List<Profit> profits = await this.context.Profits
                // .OrderByDescending(p => p.Value)
                .ToListAsync();

            // profits from states that have interest rates
            List<Profit> interestProfits = new List<Profit>();

            foreach (var state in states)
            {
                if (state.Interest > 0)
                {
                    var grossValue = state.Value * state.Interest / 1200.0m;

                    var interestProfit = new Profit
                    {
                        Currency = state.Currency,
                        Id = -1,
                        Name = state.Name + " %%",
                        Rate = Rate.Monthly,
                        Value = Math.Round(grossValue - (0.19m * grossValue), 2), // deduce tax from interests
                        IsInterestProfit = true,
                    };

                    interestProfits.Add(interestProfit);
                }
            }

            profits = profits.Concat(interestProfits).ToList();

            var expenses = await this.context.Expenses
                .OrderByDescending(p => p.Value)
                .ToListAsync();

            

            foreach (var profit in profits)
            {
                if (profit.Currency.ToUpper() == "PLN")
                {
                    profit.ValuePLN = profit.Value;
                }
                if (profit.Currency.ToUpper() == "EUR")
                {
                    profit.ValuePLN = profit.Value * forex.Eur;
                }
                else if (profit.Currency.ToUpper() == "USD")
                {
                    profit.ValuePLN = profit.Value * forex.Usd;
                }
                else if (profit.Currency.ToUpper() == "GOZ")
                {
                    profit.ValuePLN = profit.Value * forex.Gold;
                }
            }

            foreach (var state in states)
            {
                if (state.Currency.ToUpper() == "PLN")
                {
                    state.ValuePLN = state.Value;
                }
                if (state.Currency.ToUpper() == "EUR")
                {
                    state.ValuePLN = state.Value * forex.Eur;
                }
                else if (state.Currency.ToUpper() == "USD")
                {
                    state.ValuePLN = state.Value * forex.Usd;
                }
                else if (state.Currency.ToUpper() == "GOZ")
                {
                    state.ValuePLN = state.Value * forex.Gold;
                }
            }


            foreach (var expense in expenses)
            {
                if (expense.Currency.ToUpper() == "PLN")
                {
                    expense.ValuePLN = expense.Value;
                }
                if (expense.Currency.ToUpper() == "EUR")
                {
                    expense.ValuePLN = expense.Value * forex.Eur;
                }
                else if (expense.Currency.ToUpper() == "USD")
                {
                    expense.ValuePLN = expense.Value * forex.Usd;
                }
                else if (expense.Currency.ToUpper() == "GOZ")
                {
                    expense.ValuePLN = expense.Value * forex.Gold;
                }
            }


            decimal bigSum = 0;

            decimal plns = states.Where(s => s.Currency.ToUpper() == "PLN").Sum(s => s.Value);
            decimal eurs = states.Where(s => s.Currency.ToUpper() == "EUR").Sum(s => s.Value * forex.Eur);
            decimal usds = states.Where(s => s.Currency.ToUpper() == "USD").Sum(s => s.Value * forex.Usd);
            decimal goldie = states.Where(s => s.Currency.ToUpper() == "GOZ").Sum(s => s.Value * forex.Gold);

            bigSum = plns + eurs + usds + goldie;

            return new Flow
            {
                BigSum = bigSum,
                Expenses = expenses.OrderByDescending(e => e.ValuePLN).ToList(),
                States = states.OrderByDescending(s => s.ValuePLN).ToList(),
                Profits = profits.OrderByDescending(p => p.ValuePLN).ToList(),
            };
        }

        public async Task<int> AddOrUpdateState(State state)
        {
            this.context.States.Update(state);
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

        public async Task DeleteState(int id)
        {
            var state = await this.context.States.FirstOrDefaultAsync(p => p.Id == id);
            this.context.States.Remove(state);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteExpense(int id)
        {
            var expense = await this.context.Expenses.FirstOrDefaultAsync(p => p.Id == id);
            this.context.Expenses.Remove(expense);
            await this.context.SaveChangesAsync();
        }
    }
}
