import { Expense } from './expense';
import { Profit } from './profit';
import { Asset } from './asset';

export interface Flow {
  bigSum: number;
  assets: Asset[];
  profits: Profit[];
  expenses: Expense[];
}
