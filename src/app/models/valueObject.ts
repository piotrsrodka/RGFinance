import { Rate } from './rate';

export interface ValueObject {
  id: number;
  name: string;
  value: number;
  valuePLN: number;
  currency: string;

  tags: string;
}
