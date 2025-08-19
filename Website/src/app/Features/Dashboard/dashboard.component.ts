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

  sumA = () =>
    this.flow.assets.reduce((sum, item) => sum + item.currentCurrencyValue, 0);

  sumP = () =>
    this.flow.profits.reduce((sum, item) => sum + item.currentCurrencyValue, 0);

  sumE = () =>
    this.flow.expenses.reduce(
      (sum, item) => sum + item.currentCurrencyValue,
      0
    );

  assetToAdd = Utils.getClearAsset();
  profitToAdd: Profit = Utils.getClearProfit();
  expenseToAdd: Expense = Utils.getClearExpense();

  forex: Forex;
  isBaseCurrency = false;
  selectedBaseCurrency = Currency.PLN;
  Currency = Currency; // for template access

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

  getValue(valueObject: ValueObject) {
    const value = this.isBaseCurrency
      ? valueObject.currentCurrencyValue
      : valueObject.value;
    return value;
  }

  getFlow() {
    this.flowService
      .Get(this.userId, this.selectedBaseCurrency)
      .subscribe((response) => {
        this.flow = response;

        this.graph = this.plotService.getPlot(
          this.months,
          this.sumA(),
          this.sumP(),
          this.sumE(),
          this.selectedBaseCurrency
        );
      });
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

    this.flowService.DeleteAsset(asset).subscribe(() => {
      this.getFlow();
    });
  }

  onDeleteProfit(profit) {
    if (profit.id === 0) {
      this.isAddingProfit = false;
      this.profitToAdd = Utils.getClearProfit();
      return;
    }

    this.flowService.DeleteProfit(profit).subscribe(() => {
      this.getFlow();
    });
  }

  onDeleteExpense(expense) {
    if (expense.id === 0) {
      this.isAddingExpense = false;
      this.expenseToAdd = Utils.getClearExpense();
      return;
    }

    this.flowService.DeleteExpense(expense).subscribe(() => {
      this.getFlow();
    });
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
}
