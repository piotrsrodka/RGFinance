import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// import * as PlotlyJS from 'plotly.min.js';
import { PlotlyViaWindowModule  } from 'angular-plotly.js';

// PlotlyModule.plotlyjs = PlotlyJS;

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './Features/Dashboard/dashboard.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { EditStateComponent } from '../app/Features/Dashboard/editState/edit-state.component';
import { EditExpenseComponent } from './Features/Dashboard/editExpense/edit-expense.component';
import { EditProfitComponent } from './Features/Dashboard/editProfit/edit-profit.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    EditStateComponent,
    EditExpenseComponent,
    EditProfitComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    PlotlyViaWindowModule,
  ],
  providers: [
    
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
