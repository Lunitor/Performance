import { IHardware } from "./IHardware";

export interface ISensor {
    hardware: IHardware
    type: string
    name: string
    minValue: number
    maxValue: number
}