import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Flow } from '../../models/flow';
import { State } from '../../models/state';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  
  constructor(private httpClient: HttpClient) { }

  flowUrl = environment.apiUrl + '/flow/';

  Get(id: number): Observable<Flow> {
    return this.httpClient.get<Flow>(this.flowUrl + id);
  }

  AddOrUpdateState(state: State): Observable<number> {
    return this.httpClient.post<number>(this.flowUrl + 'state/', state);
  }

  DeleteSection(id: number): Observable<any> {
    return this.httpClient.delete(this.flowUrl + id);
  }
}
