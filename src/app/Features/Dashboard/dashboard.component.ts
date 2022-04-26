import { Component, OnInit } from '@angular/core';
import { Flow } from '../../models/flow';
import { Rate } from '../../models/rate';
import { State } from '../../models/state';
import { DashboardService } from './dashboard.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: []
})
export class DashboardComponent implements OnInit {

    userId = 1;

    flow: Flow = {
        expenses: [],
        profits: [],
        states: [],
    };

    isAddingState = false;
    isAddingProfit = false;
    isAddingExpense = false;

    getClearState(): State {
        return {
            id: 0,
            currency: "PLN",
            interest: 0,
            interestRate: Rate.Yearly,
            name: "",
            tags: "",
            value: 0,
        }
    }

    stateToAdd = this.getClearState();

    constructor(private flowService: DashboardService) { }

    ngOnInit(): void {
        this.userId = 1;
        this.getFlow();
    }

    getFlow() {
        this.flowService.Get(this.userId).subscribe(response => {
            this.flow = response;
        })
    }

    addState(stateToAdd: State) {
        this.flow.states.push(stateToAdd);
        this.flowService.AddOrUpdateState(stateToAdd).subscribe(() => {
            this.getFlow();
        });
        this.isAddingState = false;
        this.stateToAdd = this.getClearState();
    }
}
