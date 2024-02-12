package com.seldon.stockscanner.WebScraper;

import java.util.Set;

import com.seldon.stockscanner.Stocks.StockEntity;

public interface WebScraper
{
    public Set<StockEntity> getPluss500SupportedSymbols();
}
