import pandas as pd
import argparse


from xgboost.sklearn import XGBRegressor
from PriceParsing import download_prices_for_company
from sklearn.metrics import mean_absolute_error

from numpy import asarray
from pandas import DataFrame
from pandas import concat


def series_to_supervised(_data, n_in=1, n_out=1, dropnan=True):
    n_vars = 1 if type(_data) is list else _data.shape[1]
    df = DataFrame(_data)
    cols, names = list(), list()

    for i in range(n_in, 0, -1):
        cols.append(df.shift(i))
        names += [('var%d(t-%d)' % (j + 1, i)) for j in range(n_vars)]

    for i in range(0, n_out):
        cols.append(df.shift(-i))
        if i == 0:
            names += [('var%d(t)' % (j + 1)) for j in range(n_vars)]
        else:
            names += [('var%d(t+%d)' % (j + 1, i)) for j in range(n_vars)]

    agg = concat(cols, axis=1)
    agg.columns = names

    if dropnan:
        agg.dropna(inplace=True)

    return agg.values


def train_test_split(_data, n_test):
    return _data[:-n_test, :], _data[-n_test:, :]


def xgboost_forecast(_train, _test_x):

    _train = asarray(_train)

    _train_x, _train_y = _train[:, :-1], _train[:, -1]

    model = XGBRegressor(objective='reg:squarederror', n_estimators=988)
    model.fit(_train_x, _train_y)

    _y = model.predict(asarray([_test_x]))

    return _y[0]


def walk_forward_validation(_data, n_test):
    predictions = list()

    _train, _test = train_test_split(_data, n_test)

    history = [x for x in _train]

    for i in range(len(_test)):

        _test_X, _test_y = _test[i, :-1], _test[i, -1]

        _y = xgboost_forecast(history, _test_X)

        predictions.append(_y)

        history.append(_test[i])

        print('>expected=%.1f, predicted=%.1f' % (_test_y, _y))

    error = mean_absolute_error(_test[:, -1], predictions)
    return error, _test[:, -1], predictions


if __name__ == "__main__":
    api_key = "TDM8L7KVPP1QD1TT";
    
    parser = argparse.ArgumentParser()
    parser.add_argument('ticker')

    args = parser.parse_args()

    ticker = args.ticker

    company_csv = download_prices_for_company(
        ticker=ticker,
        api_key=api_key
    )

    data = pd.read_csv(company_csv)

    chronologically_ordered_prices = list(reversed(data['close'].values))

    train = series_to_supervised(
        _data=chronologically_ordered_prices,
        n_in=26
    )

    print(xgboost_forecast(
        _train=train[:-1, :],
        _test_x=train[-1, 1:]
    ))
