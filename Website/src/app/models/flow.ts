import { Expense } from "./expense";
import { Profit } from "./profit";
import { State } from "./state";

export interface Flow {
    bigSum: number;
    states: State[],
    profits: Profit[],
    expenses: Expense[],
}