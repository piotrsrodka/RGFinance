using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using RGFinance.StocksFeature;

namespace RGFinance.FlowFeature
{
    public class FlowService : IFlowService
    {
        private readonly RGFContext context;
        private readonly IForexService forexService;
        private readonly IStockPriceServiceFactory stockPriceServiceFactory;

        public FlowService(RGFContext context, IForexService forexService, IStockPriceServiceFactory stockPriceServiceFactory)
        {
            this.context = context;
            this.forexService = forexService;
            this.stockPriceServiceFactory = stockPriceServiceFactory;
        }

        public async Task<Flow> GetFlowAsync(BaseCurrency baseCurrency = BaseCurrency.PLN)
        {
            Forex? forex = await this.forexService.GetForex();

            List<Asset> assets = await this.context.Assets
                .ToListAsync();

            // Update stock prices for assets with tickers
            await UpdateStockPrices(assets, forex);

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
            decimal valueInPLN = 0;

            if (valueObject is Asset asset && asset.HasTicker)
            {                
                if (asset.CurrentCurrencyValue > 0)
                {
                    valueInPLN = GetValueInPLN(asset.CurrentCurrencyValue, asset.Currency, forex);
                    valueObject.CurrentCurrencyValue = ConvertFromPLN(valueInPLN, forex, baseCurrency);
                    return;
                }
            }

            valueInPLN = GetValueInPLN(valueObject.Value, valueObject.Currency, forex);
            valueObject.CurrentCurrencyValue = ConvertFromPLN(valueInPLN, forex, baseCurrency);
        }

        private static decimal GetValueInPLN(decimal value, CurrencyType currencyType, Forex forex)
        {
            return currencyType switch
            {
                CurrencyType.PLN => value,
                CurrencyType.EUR => value * forex.Eur,
                CurrencyType.USD => value * forex.Usd,
                CurrencyType.GOZ => value * forex.Gold,
                CurrencyType.BTC => value * forex.Btc,
                CurrencyType.ETH => value * forex.Eth,
                CurrencyType.SOL => value * forex.Sol,
                CurrencyType.DOGE => value * forex.Doge,
                _ => value
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

        private async Task UpdateStockPrices(List<Asset> assets, Forex forex)
        {
            foreach (var asset in assets)
            {
                // Only update stock assets that have a ticker
                if (asset.AssetType == AssetType.Stocks && !string.IsNullOrEmpty(asset.Ticker))
                {
                    try
                    {
                        decimal pricePerShare;

                        // Check cache first
                        var cachedPrice = await this.context.StockPriceCache
                            .FirstOrDefaultAsync(c => c.Ticker == asset.Ticker);

                        if (cachedPrice != null && (DateTime.UtcNow - cachedPrice.LastUpdated).TotalHours < 24)
                        {
                            // Use cached price (less than 24 hours old)
                            pricePerShare = cachedPrice.Price;
                        }
                        else
                        {
                            // Fetch from API
                            var stockPriceService = this.stockPriceServiceFactory.GetService(asset.Ticker);
                            pricePerShare = await stockPriceService.GetStockPrice(asset.Ticker);

                            // Update or create cache entry
                            if (cachedPrice != null)
                            {
                                cachedPrice.Price = pricePerShare;
                                cachedPrice.LastUpdated = DateTime.UtcNow;
                                this.context.StockPriceCache.Update(cachedPrice);
                            }
                            else
                            {
                                this.context.StockPriceCache.Add(new StockPriceCache
                                {
                                    Ticker = asset.Ticker,
                                    Price = pricePerShare,
                                    LastUpdated = DateTime.UtcNow
                                });
                            }
                            await this.context.SaveChangesAsync();
                        }

                        // asset.Value contains quantity of shares
                        // Calculate total value: quantity * price per share
                        var totalValue = asset.Value * pricePerShare;

                        // Store the calculated value in CurrentCurrencyValue
                        if (asset.Ticker.Contains('.'))
                        { // European stock
                            asset.Currency = CurrencyType.EUR;
                            asset.CurrentCurrencyValue = totalValue;
                        }
                        else
                        {
                            asset.Currency = CurrencyType.USD;
                            asset.CurrentCurrencyValue = totalValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue processing other assets
                        Console.WriteLine($"Failed to update stock price for {asset.Ticker}: {ex.Message}");
                    }
                }
            }
        }
    }
}
