import { IHardware } from "./IHardware";
import { ISensor } from "./ISensor";


export interface ISensorReading {
    timeStamp: Date,
    hardware: IHardware,
    sensor: ISensor,
    value: number
}