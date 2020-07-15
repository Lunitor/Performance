import { render, screen } from '@testing-library/react';
import * as React from "react";
import { SensorsMenu } from '../../components/SensorsMenu';

test("Should render a button for each sensor", () => {
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

    render(<SensorsMenu
        hardwareName={testHardwareName}
        sensors={sensors}
        sensorClickHandler={handleSensorClick}
        fullSensorName={fullSensorName}
        colors={colors}
    />);

    const buttonCount = screen.getAllByRole('button').length;

    expect(buttonCount).toEqual(sensors.length);
});

test("Should show sensor names and types on buttons", () => {
    const testHardwareName = "TestHardware";

    const sensorSwitches: [string, string, string, boolean][] = [
        [testHardwareName, "core1", "clock", true],
        [testHardwareName, "core1", "temprature", true],
        [testHardwareName, "core2", "clock", true],
        [testHardwareName, "core2", "temperature", true],
        [testHardwareName, "core average", "power", true],
    ];

    const handleSensorClick = function (hardwareSensorName: string) {
    };

    const fullSensorName = function (sensor: [string, string, string, boolean]) {
        return sensor[0] + "_" + sensor[1] + "_" + sensor[2];
    };

    const colors = ["#00FFFF", "#00FF00", "#FF00FF", "#FFFF00", "#FF0000"];

    render(<SensorsMenu
        hardwareName={testHardwareName}
        sensors={sensorSwitches}
        sensorClickHandler={handleSensorClick}
        fullSensorName={fullSensorName}
        colors={colors}
    />);

    const buttonTexts = screen.getAllByRole('button')
        .map(x => x.textContent);

    for (var i = 0; i < sensorSwitches.length; i++) {
        const actualNamesTypesBySensorName = buttonTexts.filter(x => x.includes(sensorSwitches[i][1]));
        expect(actualNamesTypesBySensorName).not.toHaveLength(0);

        const expectedTypes = sensorSwitches.filter(x => x[1] == sensorSwitches[i][1]).map(x => x[2]);
        for (var expectedType in expectedTypes) {
            var containsNameTypePair = false;
            for (var actualNameType in actualNamesTypesBySensorName) {
                if (actualNameType.includes(expectedType)) {
                    containsNameTypePair = true;
                    break;
                }
            }

            expect(containsNameTypePair).toBeTruthy();
        }
    }
});

test("Should render buttons with only the given colors", () => {
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

    render(<SensorsMenu
        hardwareName={testHardwareName}
        sensors={sensors}
        sensorClickHandler={handleSensorClick}
        fullSensorName={fullSensorName}
        colors={colors}
    />);

    const buttonColors = screen.getAllByRole('button').map(x => x.style.backgroundColor);

    for (var color in buttonColors) {
        const buttonColorIsFromGivenColors = colors.find(x => x == color);

        expect(buttonColorIsFromGivenColors).toBeTruthy();
    }
});
