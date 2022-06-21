from os import getenv

import requests
import dotenv


BASE_URL = "https://www.alphavantage.co/query"


def download_prices_for_company(ticker: str, api_key: str):
    params = {
        "function": "TIME_SERIES_WEEKLY",
        "symbol": ticker,
        "apikey": api_key,
        "datatype": "csv"
    }

    response = requests.get(
        url=BASE_URL,
        params=params
    ).url

    return response





