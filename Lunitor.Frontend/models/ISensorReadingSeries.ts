import { ISensor } from "./Isensor";
import { TimeSeries } from "pondjs";

export interface ISensorReadingSeries {
    hardwareName: string,
    sensor: ISensor,
    readings: TimeSeries
}