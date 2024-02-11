package com.seldon.stockscanner.Pluss500;

import java.util.List;

public interface Plus500Service
{
    public List<StockEntity> retrieveListedStocks();
}
