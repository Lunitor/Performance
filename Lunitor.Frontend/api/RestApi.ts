import { IApi } from "./IApi";
import { ISensorReading } from "../models/ISensorReading";

export class RestApi implements IApi {

    public async fetchData(): Promise<ISensorReading[]> {
        const response = await fetch('/sensorreadings');
        const data = await response.json();

        return data;
    }
}