import { render } from '@testing-library/react';
import * as React from "react";
import { SensorsMenu } from '../../components/SensorsMenu';

describe("SensorMenu", () => {
    it("Should render a button for each sensor", () => {
        const testHardwareName = "TestHardware";
        const sensors: [string, string, string, boolean][] = [
            [testHardwareName, "core1", "clock", true],
            [testHardwareName, "core2", "clock", true],
            [testHardwareName, "core3", "clock", true],
            [testHardwareName, "core4", "clock", true],
        ];

        const handleSensorClick = function (hardwareSensorName: string) {
        };

        const fullSensorName = function (sensor: [string, string, string, boolean]) {
            return sensor[0] + "_" + sensor[1] + "_" + sensor[2];
        };

        const colors = ["#00FFFF", "#00FF00", "#FF00FF", "#FFFF00"];

        const { container } = render(<SensorsMenu
            hardwareName={testHardwareName}
            sensors={sensors}
            sensorClickHandler={handleSensorClick}
            fullSensorName={fullSensorName}
            colors={colors}
        />);

        const buttonCount = container.getElementsByTagName('button').length;

        expect(buttonCount).toEqual(sensors.length);
    });
});
