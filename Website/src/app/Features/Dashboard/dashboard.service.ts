import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Flow } from '../../models/flow';
import { State } from '../../models/state';
import { Profit } from '../../models/profit';
import { Expense } from '../../models/expense';
import { Forex } from '../../models/forex';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private httpClient: HttpClient) {}

  environment = environment;

  flowUrl = this.environment.apiUrl + '/flow/';

  Get(id: number): Observable<Flow> {
    return this.httpClient.get<Flow>(this.flowUrl + id);
  }

  GetForex(): Observable<Forex> {
    return this.httpClient.get<Forex>(this.flowUrl + 'forex');
  }

  AddOrUpdateState(state: State): Observable<number> {
    return this.httpClient.post<number>(this.flowUrl + 'state/', state);
  }

  AddOrUpdateProfit(profit: Profit) {
    return this.httpClient.post<number>(this.flowUrl + 'profit/', profit);
  }

  AddOrUpdateExpense(expense: Expense) {
    return this.httpClient.post<number>(this.flowUrl + 'expense/', expense);
  }

  DeleteState(state: State) {
    return this.httpClient.delete(this.flowUrl + 'state/' + state.id);
  }

  DeleteProfit(profit: Profit) {
    return this.httpClient.delete(this.flowUrl + 'profit/' + profit.id);
  }

  DeleteExpense(expense: Expense) {
    return this.httpClient.delete(this.flowUrl + 'expense/' + expense.id);
  }
}
