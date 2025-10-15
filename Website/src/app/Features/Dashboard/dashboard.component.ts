import { Component, OnInit } from '@angular/core';
import { Expense } from '../../models/expense';
import { Flow } from '../../models/flow';
import { Forex } from '../../models/forex';
import { Profit } from '../../models/profit';
import { Asset } from '../../models/asset';
import Utils from '../../utils/utils';
import { DashboardService } from './dashboard.service';
import { ValueObject } from '../../models/valueObject';
import { PlotLocalService } from '../Plot/plot.local.service';
import { Currency } from '../../models/currency';
import { AssetType } from '../../models/assetType';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  graph: any = {
    data: null,
    layout: null,
  };

  pieChart: any = {
    data: [
      {
        values: [],
        labels: [],
        text: [],
        hovertemplate: [],
        type: 'pie',
        textinfo: 'text+percent',
        hoverinfo: 'none',
        domain: {
          x: [0.1, 0.7],
          y: [0.0, 1],
        },
      },
    ],
    layout: {
      title: 'Skład portfela',
      showlegend: true,
      legend: {
        orientation: 'v',
        x: 1.15,
        xanchor: 'right',
        y: 0.1,
        yanchor: 'top',
        font: { color: 'greenyellow' },
      },
      margin: {
        l: 10,
        r: 50,
        t: 60,
        b: 50,
      },
      paper_bgcolor: 'black',
      plot_bgcolor: 'black',
      font: { color: 'greenyellow' },
    },
  };

  pieChartByType: any = {
    data: [
      {
        values: [],
        labels: [],
        text: [],
        hovertemplate: [],
        type: 'pie',
        textinfo: 'text+percent',
        hoverinfo: 'none',
        domain: {
          x: [0.1, 0.7],
          y: [0, 1],
        },
      },
    ],
    layout: {
      title: 'Skład portfela',
      showlegend: true,
      legend: {
        orientation: 'v',
        x: 1.15,
        xanchor: 'right',
        y: 0.1,
        yanchor: 'top',
        font: { color: 'greenyellow' },
      },
      margin: {
        l: 10,
        r: 50,
        t: 60,
        b: 50,
      },
      paper_bgcolor: 'black',
      plot_bgcolor: 'black',
      font: { color: 'greenyellow' },
    },
  };

  months = 24;

  userId = 1;

  flow: Flow = {
    bigSum: 0,
    expenses: [],
    profits: [],
    assets: [],
  };

  isAddingAsset = false;
  isAddingProfit = false;
  isAddingExpense = false;

  isEditingAsset = false;

  sumA = () => {
    const sum = this.flow.assets.reduce(
      (sum, item) => sum + item.currentCurrencyValue,
      0
    );
    return this.isIncognito ? sum / this.INCOGNITO_DIVISOR : sum;
  };

  sumP = () => {
    const sum = this.flow.profits.reduce(
      (sum, item) => sum + item.currentCurrencyValue,
      0
    );
    return this.isIncognito ? sum / this.INCOGNITO_DIVISOR : sum;
  };

  sumE = () => {
    const sum = this.flow.expenses.reduce(
      (sum, item) => sum + item.currentCurrencyValue,
      0
    );
    return this.isIncognito ? sum / this.INCOGNITO_DIVISOR : sum;
  };

  assetToAdd = Utils.getClearAsset();
  profitToAdd: Profit = Utils.getClearProfit();
  expenseToAdd: Expense = Utils.getClearExpense();

  forex: Forex;
  isBaseCurrency = false;
  isIncognito = false;
  selectedBaseCurrency = Currency.PLN;
  Currency = Currency; // for template access
  AssetType = AssetType; // for template access

  // Incognito mode divisor - change this value to adjust how much values are divided in incognito mode
  private readonly INCOGNITO_DIVISOR = 7;

  // Filtry dla pie-chart (checkboxy)
  assetTypeFilters = {
    [AssetType.Cash]: true,
    [AssetType.Stocks]: true,
    [AssetType.Metals]: true,
    [AssetType.RealEstate]: true,
    [AssetType.Crypto]: true,
    [AssetType.Other]: true,
  };

  constructor(
    private flowService: DashboardService,
    private plotService: PlotLocalService
  ) {}

  ngOnInit(): void {
    this.userId = 1;
    this.getFlow();

    this.flowService.GetForex().subscribe((response) => {
      this.forex = response;
    });
  }

  sumInvestments() {
    return this.flow.profits
      .filter((p) => p.isInterestProfit)
      .reduce((sum, current) => sum + current.currentCurrencyValue, 0);
  }

  onProfitClicked(profit: Profit) {
    if (!profit.isInterestProfit) profit.isEditing = true;
  }

  onAssetClicked(asset: Asset) {
    // Close all other assets before opening this one
    // This prevents keeping unsaved changes in other assets
    this.flow.assets.forEach((a) => (a.isEditing = false));
    asset.isEditing = true;

    // Also close add mode
    this.isAddingAsset = false;
  }

  getValue(valueObject: ValueObject) {
    let value = this.isBaseCurrency
      ? valueObject.currentCurrencyValue
      : valueObject.value;

    if (this.isIncognito) {
      value = value / this.INCOGNITO_DIVISOR;
    }

    return value;
  }

  getCurrency(valueObject: ValueObject): string {
    if (
      !this.isBaseCurrency &&
      (valueObject as Asset) &&
      (valueObject as Asset).assetType == AssetType.Stocks &&
      (valueObject as Asset).ticker &&
      (valueObject as Asset).ticker.trim().length > 0
    ) {
      return 'Stck';
    }

    return this.isBaseCurrency
      ? this.currentBaseCurrency
      : valueObject.currency;
  }

  getFormattedValue(valueObject: ValueObject): string {
    const value = this.getValue(valueObject);
    const currency = this.getCurrency(valueObject);

    // For crypto currencies, show more decimal places
    if (currency === 'BTC' || currency === 'ETH' || currency === 'SOL') {
      return value.toFixed(4);
    }

    if (currency === 'GOZ') {
      return value.toFixed(2);
    }

    // For regular currencies, show whole numbers with thousand separators
    return value.toLocaleString('en-US', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    });
  }

  getFlow() {
    this.flowService
      .Get(this.userId, this.selectedBaseCurrency)
      .subscribe((response) => {
        this.flow = response;

        // Convert AssetType strings to numbers
        this.flow.assets.forEach((asset) => {
          if (typeof asset.assetType === 'string') {
            asset.assetType =
              (AssetType as any)[asset.assetType] || AssetType.Undefined;
          }
        });

        this.graph = this.plotService.getPlot(
          this.months,
          this.sumA(),
          this.sumP(),
          this.sumE(),
          this.selectedBaseCurrency
        );

        this.updatePieChart();
        this.updatePieChartByType();
      });
  }

  updatePieChart() {
    // Filtruj assety według zaznaczonych checkboxów
    const filteredAssets = this.flow.assets.filter(
      (a) => this.assetTypeFilters[a.assetType]
    );

    if (filteredAssets.length === 0) {
      this.pieChart.data[0].values = [];
      this.pieChart.data[0].labels = [];
      this.pieChart.data[0].text = [];
      this.pieChart.data[0].hovertemplate = [];
      return;
    }

    this.pieChart.data[0].values = filteredAssets.map((a) =>
      this.isIncognito
        ? a.currentCurrencyValue / this.INCOGNITO_DIVISOR
        : a.currentCurrencyValue
    );

    // Labels (w legendzie): nazwa + wartość
    this.pieChart.data[0].labels = filteredAssets.map((a) => {
      const rawValue = this.isIncognito
        ? a.currentCurrencyValue / this.INCOGNITO_DIVISOR
        : a.currentCurrencyValue;
      const value = rawValue.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      });
      return `${a.name}: ${value} ${this.selectedBaseCurrency}`;
    });

    // Text (na wykresie): tylko nazwa
    this.pieChart.data[0].text = filteredAssets.map((a) => a.name);

    // Hover template: nazwa + wartość + waluta
    this.pieChart.data[0].hovertemplate = filteredAssets.map((a) => {
      const rawValue = this.isIncognito
        ? a.currentCurrencyValue / this.INCOGNITO_DIVISOR
        : a.currentCurrencyValue;
      const value = rawValue.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      });
      return `${a.name}<br>${value} ${this.selectedBaseCurrency}<extra></extra>`;
    });

    this.pieChart.layout.title = 'Skład portfela';
  }

  onAssetTypeFilterChange() {
    this.updatePieChart();
    this.updatePieChartByType();
  }

  onIncognitoChange() {
    this.updatePieChart();
    this.updatePieChartByType();
  }

  updatePieChartByType() {
    // Filtruj assety według zaznaczonych checkboxów
    const filteredAssets = this.flow.assets.filter(
      (a) => this.assetTypeFilters[a.assetType]
    );

    // Grupuj assety według typu
    const typeGroups = new Map<
      number,
      { name: string; icon: string; total: number }
    >();

    const typeNames = {
      [AssetType.Cash]: 'Waluta',
      [AssetType.Stocks]: 'Akcje',
      [AssetType.Metals]: 'Kruszce',
      [AssetType.RealEstate]: 'Nieruchomości',
      [AssetType.Crypto]: 'Kryptowaluty',
      [AssetType.Other]: 'Inne',
    };

    // Zsumuj wartości dla każdego typu
    filteredAssets.forEach((asset) => {
      if (!typeGroups.has(asset.assetType)) {
        typeGroups.set(asset.assetType, {
          name: typeNames[asset.assetType] || 'Nieokreślone',
          icon: this.getAssetIcon(asset.assetType),
          total: 0,
        });
      }
      const group = typeGroups.get(asset.assetType)!;
      group.total += asset.currentCurrencyValue;
    });

    // Przygotuj dane dla wykresu
    const types = Array.from(typeGroups.values());

    if (types.length === 0) {
      this.pieChartByType.data[0].values = [];
      this.pieChartByType.data[0].labels = [];
      this.pieChartByType.data[0].text = [];
      this.pieChartByType.data[0].hovertemplate = [];
      return;
    }

    this.pieChartByType.data[0].values = types.map((t) =>
      this.isIncognito ? t.total / this.INCOGNITO_DIVISOR : t.total
    );

    // Labels (w legendzie): ikonka + nazwa + wartość
    this.pieChartByType.data[0].labels = types.map((t) => {
      const rawValue = this.isIncognito
        ? t.total / this.INCOGNITO_DIVISOR
        : t.total;
      const value = rawValue.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      });
      return `${t.name}: ${value} ${this.selectedBaseCurrency}`;
    });

    // Text (na wykresie): ikonka + nazwa
    this.pieChartByType.data[0].text = types.map((t) => `${t.name}`);

    // Hover template
    this.pieChartByType.data[0].hovertemplate = types.map((t) => {
      const rawValue = this.isIncognito
        ? t.total / this.INCOGNITO_DIVISOR
        : t.total;
      const value = rawValue.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      });
      return `${t.name}<br>${value} ${this.selectedBaseCurrency}<extra></extra>`;
    });

    this.pieChartByType.layout.title = 'Skład portfela';
  }

  // (${total.toFixed(0)} ${this.selectedBaseCurrency})

  isVisible(asset: Asset): boolean {
    return asset.isEditing;
  }

  onCancelAssetEdit(asset: Asset) {
    asset.isEditing = false;

    // If canceling while adding a new asset, reset the assetToAdd and close add mode
    if (this.isAddingAsset && asset === this.assetToAdd) {
      this.isAddingAsset = false;
      this.assetToAdd = Utils.getClearAsset();
    }
  }

  addOrUpdateAsset(asset: Asset) {
    if (this.isAddingAsset) {
      this.flow.assets.push(asset);
    }

    this.flowService.AddOrUpdateAsset(asset).subscribe(() => {
      this.getFlow();
    });

    this.isAddingAsset = false;
    asset.isEditing = false;
    this.assetToAdd = Utils.getClearAsset();
  }

  addOrUpdateProfit(profit: Profit) {
    if (this.isAddingProfit) {
      this.flow.profits.push(profit);
    }

    this.flowService.AddOrUpdateProfit(profit).subscribe(() => {
      this.getFlow();
    });

    this.isAddingProfit = false;
    profit.isEditing = false;
    this.profitToAdd = Utils.getClearProfit();
  }

  onDeleteAsset(asset: Asset) {
    if (asset.id === 0) {
      this.isAddingAsset = false;
      this.assetToAdd = Utils.getClearAsset();
      return;
    }

    if (confirm(`Czy na pewno chcesz usunąć zasób "${asset.name}"?`)) {
      this.flowService.DeleteAsset(asset).subscribe(() => {
        this.getFlow();
      });
    }
  }

  onDeleteProfit(profit) {
    if (profit.id === 0) {
      this.isAddingProfit = false;
      this.profitToAdd = Utils.getClearProfit();
      return;
    }

    if (confirm(`Czy na pewno chcesz usunąć przychód "${profit.name}"?`)) {
      this.flowService.DeleteProfit(profit).subscribe(() => {
        this.getFlow();
      });
    }
  }

  onDeleteExpense(expense) {
    if (expense.id === 0) {
      this.isAddingExpense = false;
      this.expenseToAdd = Utils.getClearExpense();
      return;
    }

    if (confirm(`Czy na pewno chcesz usunąć wydatek "${expense.name}"?`)) {
      this.flowService.DeleteExpense(expense).subscribe(() => {
        this.getFlow();
      });
    }
  }

  addOrUpdateExpense(expense: Expense) {
    if (this.isAddingExpense) {
      this.flow.expenses.push(expense);
    }

    this.flowService.AddOrUpdateExpense(expense).subscribe(() => {
      this.getFlow();
    });

    this.isAddingExpense = false;
    expense.isEditing = false;
    this.expenseToAdd = Utils.getClearExpense();
  }

  flowFunction(m) {
    return this.sumA() + m * (this.sumP() - this.sumE());
  }

  getBalanceColor() {
    return this.getBalance() > 0 ? 'green' : 'red';
  }

  getBalance() {
    return this.sumP() - this.sumE();
  }

  getBankruptcy() {
    for (let i = 0; ; i++) {
      if (this.flowFunction(i) < 0) {
        return i;
      }
    }
  }

  getMillionaireTime() {
    const millionTarget = 1000000; // 1 million in base currency

    // Check if already a millionaire
    if (this.flowFunction(0) >= millionTarget) {
      return 0; // Already a millionaire
    }

    for (let i = 1; i <= 240; i++) {
      // Max 20 years
      if (this.flowFunction(i) >= millionTarget) {
        return i;
      }
    }

    return null; // Never becomes millionaire
  }

  isAlreadyMillionaire() {
    const millionTarget = 1000000;
    return this.sumA() >= millionTarget;
  }

  onBaseCurrencyChange() {
    this.getFlow();
  }

  get currentBaseCurrency() {
    return this.selectedBaseCurrency;
  }

  getForexRateInBaseCurrency(currency: string): number {
    if (!this.forex) return 0;

    if (this.selectedBaseCurrency === Currency.PLN) {
      const rates = {
        [Currency.USD]: this.forex.usd,
        [Currency.EUR]: this.forex.eur,
        [Currency.GOLD]: this.forex.gold,
      };

      return rates[currency] || 1;
    } else if (this.selectedBaseCurrency === Currency.USD) {
      const rates = {
        [Currency.PLN]: 1 / this.forex.usd,
        [Currency.EUR]: this.forex.eur / this.forex.usd,
        [Currency.GOLD]: this.forex.gold / this.forex.usd,
      };

      return rates[currency] || 1;
    } else if (this.selectedBaseCurrency === Currency.EUR) {
      const rates = {
        [Currency.PLN]: 1 / this.forex.eur,
        [Currency.USD]: this.forex.usd / this.forex.eur,
        [Currency.GOLD]: this.forex.gold / this.forex.eur,
      };

      return rates[currency] || 1;
    }

    return 0;
  }

  shouldShowCurrency(currency: string): boolean {
    return currency !== this.selectedBaseCurrency;
  }

  getCryptoPrice(cryptoPricePln: number): number {
    if (!this.forex) return 0;

    // Crypto prices from backend are in PLN, convert to selected base currency
    if (this.selectedBaseCurrency === Currency.PLN) {
      return cryptoPricePln;
    } else if (this.selectedBaseCurrency === Currency.USD) {
      return cryptoPricePln / this.forex.usd;
    } else if (this.selectedBaseCurrency === Currency.EUR) {
      return cryptoPricePln / this.forex.eur;
    }

    return cryptoPricePln;
  }

  getAssetIcon(assetType: number): string {
    const AssetType = {
      Undefined: 0,
      Cash: 1,
      Stocks: 2,
      Metals: 3,
      RealEstate: 4,
      Crypto: 5,
      Other: 99,
    };

    switch (assetType) {
      case AssetType.Cash:
        return '💵';
      case AssetType.Stocks:
        return '📈';
      case AssetType.Metals:
        return '🟨';
      case AssetType.RealEstate:
        return '🏠';
      case AssetType.Crypto:
        return '🪙';
      case AssetType.Other:
        return '💼';
      default:
        return '❓';
    }
  }
}
