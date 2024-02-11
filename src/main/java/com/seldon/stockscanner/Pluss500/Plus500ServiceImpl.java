package com.seldon.stockscanner.Pluss500;

import java.util.List;

import org.springframework.stereotype.Service;

import com.seldon.stockscanner.WebScraper.WebScraperPlus500;

@Service
public class Plus500ServiceImpl implements Plus500Service
{
    private WebScraperPlus500 webScraperPlus500;
    private Plus500Repo plus500Repo;

    public Plus500ServiceImpl(WebScraperPlus500 webScraperPlus500, Plus500Repo plus500Repo)
    {
        this.webScraperPlus500 = webScraperPlus500;
        this.plus500Repo = plus500Repo;
    }

    @Override
    public List<StockEntity> retrieveListedStocks()
    {
        List<StockEntity> results = this.webScraperPlus500.getSupportedSymbols();
        plus500Repo.saveAll(results);

        return results;
    }
    
}
