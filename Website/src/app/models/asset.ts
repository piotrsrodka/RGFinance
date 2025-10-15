import { Rate } from './rate';
import { ValueObject } from './valueObject';
import { AssetType } from './assetType';

export interface Asset extends ValueObject {
  interest: number;
  interestRate: Rate;
  assetType: AssetType;
  ticker?: string; // Stock ticker symbol (e.g., "EOSE" for EOS Energy)

  // View-model
  isEditing: boolean;
}
