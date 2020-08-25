export class ConfigurationApi {
    public async getPeriodicity(): Promise<number> {
        const response = await fetch('/configuration/periodicity');
        const data = await response.text();

        return parseInt(data);
    }

    public async setPeriodicity(value: number): Promise<boolean> {
        const requestBody = '{"periodicity":' + value + '}';

        const response = await fetch('/configuration/periodicity',
            {
                method: 'POST',
                body: requestBody,
                headers: { 'Content-Type': 'application/json' }
            });

        return response.ok;
    }
}