import { Component, OnInit } from '@angular/core';
import { Axes } from '../../models/axes';
import { Expense } from '../../models/expense';
import { Flow } from '../../models/flow';
import { Forex } from '../../models/forex';
import { Profit } from '../../models/profit';
import { State } from '../../models/state';
import Utils from '../../utils/utils';
import { DashboardService } from './dashboard.service';
import { ValueObject } from '../../models/valueObject';

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
    states: [],
  };

  isAddingState = false;
  isAddingProfit = false;
  isAddingExpense = false;

  isEditingState = false;

  sumS = () =>
    this.flow.states.reduce((sum, current) => sum + current.valuePLN, 0);
  sumP = () =>
    this.flow.profits.reduce((sum, current) => sum + current.valuePLN, 0);
  sumE = () =>
    this.flow.expenses.reduce((sum, current) => sum + current.valuePLN, 0);

  // sumP: number = 1;
  // sumE: number = 1;

  stateToAdd = Utils.getClearState();
  profitToAdd: Profit = Utils.getClearProfit();
  expenseToAdd: Expense = Utils.getClearExpense();

  forex: Forex;
  isPLN = false;

  constructor(private flowService: DashboardService) {}

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
      .reduce((sum, current) => sum + current.valuePLN, 0);
  }

  onProfitClicked(profit: Profit) {
    if (!profit.isInterestProfit) profit.isEditing = true;
  }

  getValue(valueObject: ValueObject) {
    const value = this.isPLN ? valueObject.valuePLN : valueObject.value;
    return value;
  }

  getFlow() {
    this.flowService.Get(this.userId).subscribe((response) => {
      this.flow = response;

      this.graph = {
        data: [
          {
            x: this.getX(this.months),
            y: this.getY(this.months),
            type: 'scatter',
            mode: 'lines+markers+text',
            marker: { color: 'black' },
          },
        ],
        layout: {
          autosize: true,
          // width: 820,
          // height: 400,
          xaxis: {
            title: {
              text: 'Miesiąc',
            },
          },
          yaxis: {
            title: {
              text: 'Flow (kasa) PLN',
            },
          },
          showlegend: false,
          pierdola: 12,
          paper_bgcolor: 'white',
        },
      };
    });
  }

  addOrUpdateState(state: State) {
    if (this.isAddingState) {
      this.flow.states.push(state);
    }

    this.flowService.AddOrUpdateState(state).subscribe(() => {
      this.getFlow();
    });

    this.isAddingState = false;
    state.isEditing = false;
    this.stateToAdd = Utils.getClearState();
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

  onDeleteState(state: State) {
    if (state.id === 0) {
      this.isAddingState = false;
      this.stateToAdd = Utils.getClearState();
      return;
    }

    this.flowService.DeleteState(state).subscribe(() => {
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

  getX(m) {
    let result = [];

    for (let i = 0; i < m; i++) {
      result.push(i);
    }

    return result;
  }

  getY(m) {
    let result = [];

    for (let i = 0; i < m; i++) {
      result.push(this.flowFunction(i));
    }

    return result;
  }

  flowFunction(m) {
    return this.flow.bigSum + m * (this.sumP() - this.sumE());
  }

  getBalanceColor() {
    return this.getBalance() > 0 ? 'green' : 'red';
  }

  getBalance() {
    return this.sumP() - this.sumE();
  }

  getBancruptcy() {
    for (let i = 0; ; i++) {
      if (this.flowFunction(i) < 0) {
        return i;
      }
    }
  }

  plotFlow() {
    let canvas = document.getElementById('flow-canvas') as HTMLCanvasElement;

    if (null == canvas || !canvas.getContext) return;

    var axes: Axes = {
      x0: 10, // x0 pixels from left to x=0
      y0: 180, // y0 pixels from top to y=0
      xscale: 1, // pixels from x = 0 to x = 1
      yscale: 0.0005,
      doNegativeX: false,
    };

    var ctx = canvas.getContext('2d');

    this.showAxes(ctx, axes);
    this.funGraph(ctx, axes, 'rgb(11,153,11)', 1);
  }

  funGraph(ctx, axes, color, thick) {
    let xx;
    let yy;
    let dx = 20;
    let x0 = axes.x0;
    let y0 = axes.y0;
    let xscale = axes.xscale;
    let yscale = axes.yscale;

    var iMax = Math.round((ctx.canvas.width - x0) / dx);
    var iMin = 0;

    ctx.beginPath();
    ctx.lineWidth = thick;
    ctx.strokeStyle = color;

    for (var m = 0; m <= 12; m++) {
      xx = dx * m;
      yy = yscale * this.flowFunction(m);

      if (m == iMin) {
        ctx.moveTo(x0 + xx, y0 - yy);
      } else {
        ctx.lineTo(x0 + xx, y0 - yy);
      }
    }

    ctx.stroke();
  }

  showAxes(ctx, axes) {
    var x0 = axes.x0;
    var y0 = axes.y0;
    const width = ctx.canvas.width;
    const height = ctx.canvas.height;
    var xmin = 0;

    ctx.beginPath();
    ctx.strokeStyle = 'rgb(128,128,128)';
    ctx.moveTo(xmin, y0);
    ctx.lineTo(width, y0); // X axis
    ctx.moveTo(x0, 0);
    ctx.lineTo(x0, height); // Y axis
    ctx.stroke();
  }
}
