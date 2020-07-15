import { render, screen } from '@testing-library/react';
import * as React from "react";
import { ChartsMenu } from '../../components/ChartsMenu';
import '@testing-library/jest-dom/extend-expect'

describe("ChartsMenu", () => {
    it("Should render a button for each hardware", () => {
        const hardwares: [string, boolean][] = [
            ["cpu", true],
            ["ethernet 2", true],
            ["hdd", true]
        ];


        const handleClick = function (hardwareName: string) {
        };

        render(<ChartsMenu
            hardwares={hardwares}
            handleClick={handleClick}
        />);

        const buttonCount = screen.getAllByRole('button').length

        expect(buttonCount).toEqual(hardwares.length);
    });

    it("Should have buttons with all the hardwares names", () => {
        const hardwares: [string, boolean][] = [
            ["cpu", true],
            ["ethernet 2", true],
            ["hdd", true]
        ];


        const handleClick = function (hardwareName: string) {
        };

        render(<ChartsMenu
            hardwares={hardwares}
            handleClick={handleClick}
        />);

        const buttonTexts = screen.getAllByRole('button')
            .map(x => x.textContent);

        for (var i = 0; i < hardwares.length; i++) {
            expect(buttonTexts).toContain(hardwares[i][0]);
        }
    });
});
