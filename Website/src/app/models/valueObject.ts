export interface ValueObject {
  id: number;
  name: string;
  value: number;
  currency: string;
  tags: string;

  // not db mapped
  currentCurrencyValue: number;
}
