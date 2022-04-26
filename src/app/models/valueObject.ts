import { Rate } from "./rate";

export interface ValueObject {
    id: number;
    name: string;
    value: number;
    currency: string;
    
    tags: string;
}