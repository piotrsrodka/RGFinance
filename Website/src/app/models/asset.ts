import { Rate } from './rate';
import { ValueObject } from './valueObject';

export interface Asset extends ValueObject {
  interest: number;
  interestRate: Rate;

  // View-model
  isEditing: boolean;
}
