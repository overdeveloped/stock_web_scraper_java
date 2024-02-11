package com.seldon.stockscanner.WebScraper;

import java.util.List;

import com.seldon.stockscanner.Pluss500.StockEntity;

public interface WebScraperPlus500
{
    public List<StockEntity> getSupportedSymbols();
}
