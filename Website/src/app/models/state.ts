import { Rate } from "./rate";
import { ValueObject } from "./valueObject";

export interface State extends ValueObject {
    interest: number;
    interestRate: Rate;

    // View-model
    isEditing: boolean;
}