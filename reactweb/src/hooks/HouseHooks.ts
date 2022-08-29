import {useEffect, useState} from "react";
import {House} from "../types/house";
import config from "../config";

const useFetchHouses = (): House[] =>{
    const [houses, setHouses] = useState<House[]>([]);

    useEffect(() => {
        const fetchHouses = async () => {
            const rsp = await fetch(`${config.baseApiUrl}/houses`);
            const houses = await rsp.json();
            setHouses(houses)
        };
        fetchHouses();
    }, [])

    return houses;
};

export default useFetchHouses;