import { Rate } from './rate';
import { ValueObject } from './valueObject';

export interface Profit extends ValueObject {
  rate: Rate;
  isInterestProfit: boolean;

  // View model
  isEditing: boolean;
}
