import { ISensorReading } from "../models/ISensorReading";

export interface IApi {
    fetchData(): Promise<ISensorReading[]>;
}