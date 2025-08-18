import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Flow } from '../../models/flow';
import { Asset } from '../../models/asset';
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

  Get(id: number, baseCurrency?: string): Observable<Flow> {
    const params = baseCurrency ? `?baseCurrency=${baseCurrency}` : '';
    return this.httpClient.get<Flow>(this.flowUrl + id + params);
  }

  GetForex(): Observable<Forex> {
    return this.httpClient.get<Forex>(this.flowUrl + 'forex');
  }

  AddOrUpdateAsset(asset: Asset): Observable<number> {
    return this.httpClient.post<number>(this.flowUrl + 'asset/', asset);
  }

  AddOrUpdateProfit(profit: Profit) {
    return this.httpClient.post<number>(this.flowUrl + 'profit/', profit);
  }

  AddOrUpdateExpense(expense: Expense) {
    return this.httpClient.post<number>(this.flowUrl + 'expense/', expense);
  }

  DeleteAsset(asset: Asset) {
    return this.httpClient.delete(this.flowUrl + 'asset/' + asset.id);
  }

  DeleteProfit(profit: Profit) {
    return this.httpClient.delete(this.flowUrl + 'profit/' + profit.id);
  }

  DeleteExpense(expense: Expense) {
    return this.httpClient.delete(this.flowUrl + 'expense/' + expense.id);
  }
}
