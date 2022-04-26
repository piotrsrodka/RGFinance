import { Rate } from "./rate";
import { ValueObject } from "./valueObject";

export interface Expense extends ValueObject {
    rate: Rate;
}