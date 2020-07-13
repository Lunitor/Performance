import { IGraphQLSensorReadingResult } from "../models/IGraphQLSensorReadingResult";
import { NormalizedCacheObject, ApolloClient, InMemoryCache, HttpLink, gql } from "apollo-boost";
import { ISensorReading } from "../models/ISensorReading";
import { IApi } from "./IApi";

export class GqlApi implements IApi {

    private client: ApolloClient<NormalizedCacheObject>;

    constructor() {
        this.client = new ApolloClient({
            cache: new InMemoryCache(),
            link: new HttpLink({ uri: "/graphql" })
        });
    }

    public async fetchData(): Promise<ISensorReading[]> {
        const graphqlData = await this.client.query<IGraphQLSensorReadingResult>({
            query: gql`query TestQuery {
                            sensorreadings {
                            timeStamp
                            hardware{
                                name
                                type
                            }
                            sensor{
                                hardwareName
                                name
                                type
                                maxValue
                                minValue
                            }
                            value
                            }
                        }`});

        return graphqlData.data.sensorreadings;
    }
}