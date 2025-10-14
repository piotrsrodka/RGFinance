import { Expense } from '../models/expense';
import { Profit } from '../models/profit';
import { Rate } from '../models/rate';
import { Asset } from '../models/asset';
import { AssetType } from '../models/assetType';

export default class Utils {
  public static getClearAsset(): Asset {
    return {
      id: 0,
      currency: 'PLN',
      interest: 0,
      interestRate: Rate.Yearly,
      assetType: AssetType.Undefined,
      name: '',
      tags: '',
      value: 0,
      currentCurrencyValue: 0,
      isEditing: false,
    };
  }

  public static getClearProfit(): Profit {
    return {
      id: 0,
      currency: 'PLN',
      name: '',
      tags: '',
      value: 0,
      currentCurrencyValue: 0,
      rate: Rate.Monthly,
      isEditing: false,
      isInterestProfit: false,
    };
  }

  public static getClearExpense(): Expense {
    return {
      id: 0,
      currency: 'PLN',
      name: '',
      tags: '',
      value: 0,
      currentCurrencyValue: 0,
      rate: Rate.Monthly,
      isEditing: false,
    };
  }
}
