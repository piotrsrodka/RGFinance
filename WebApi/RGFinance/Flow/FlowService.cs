using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace RGFinance.FlowFeature
{
    public class FlowService : IFlowService
    {
        private readonly RGFContext context;
        private readonly IForexService forexService;

        public FlowService(RGFContext context, IForexService forexService)
        {
            this.context = context;
            this.forexService = forexService;
        }

        public async Task<Flow> GetFlowAsync(BaseCurrency baseCurrency = BaseCurrency.PLN)
        {
            Forex? forex = await this.forexService.GetForex();

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
                this.ExchangeToCurrency(profit, forex, baseCurrency);
            }

            foreach (var asset in assets)
            {
                this.ExchangeToCurrency(asset, forex, baseCurrency);
            }

            foreach (var expense in expenses)
            {
                this.ExchangeToCurrency(expense, forex, baseCurrency);
            }

            return new Flow
            {
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

        private void ExchangeToCurrency(ValueObject valueObject, Forex forex, BaseCurrency baseCurrency)
        {
            decimal valueInPLN = GetValueInPLN(valueObject, forex);
            valueObject.CurrentCurrencyValue = ConvertFromPLN(valueInPLN, forex, baseCurrency);
        }

        private static decimal GetValueInPLN(ValueObject valueObject, Forex forex)
        {
            var currencyType = valueObject.Currency;

            return currencyType switch
            {
                CurrencyType.PLN => valueObject.Value,
                CurrencyType.EUR => valueObject.Value * forex.Eur,
                CurrencyType.USD => valueObject.Value * forex.Usd,
                CurrencyType.GOZ => valueObject.Value * forex.Gold,
                CurrencyType.BTC => valueObject.Value * forex.Btc,
                CurrencyType.ETH => valueObject.Value * forex.Eth,
                CurrencyType.SOL => valueObject.Value * forex.Sol,
                CurrencyType.DOGE => valueObject.Value * forex.Doge,
                _ => valueObject.Value
            };
        }

        private static decimal ConvertFromPLN(decimal valueInPLN, Forex forex, BaseCurrency baseCurrency)
        {
            return baseCurrency switch
            {
                BaseCurrency.PLN => valueInPLN,
                BaseCurrency.EUR => valueInPLN / forex.Eur,
                BaseCurrency.USD => valueInPLN / forex.Usd,
                _ => valueInPLN
            };
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
                        Value = Math.Round(grossValue - (0.19m * grossValue), 8),
                        IsInterestProfit = true,
                    };

                    interestProfits.Add(interestProfit);
                }
            }

            return interestProfits;
        }
    }
}
