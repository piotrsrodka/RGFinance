import { Rate } from './rate';
import { ValueObject } from './valueObject';
import { AssetType } from './assetType';

export interface Asset extends ValueObject {
  interest: number;
  interestRate: Rate;
  assetType: AssetType;

  // View-model
  isEditing: boolean;
}
