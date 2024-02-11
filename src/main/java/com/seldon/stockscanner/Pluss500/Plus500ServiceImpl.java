package com.seldon.stockscanner.Pluss500;

import java.util.List;

import org.springframework.stereotype.Service;

import com.seldon.stockscanner.TradingPlatform.TradingPlatformEntity;
import com.seldon.stockscanner.TradingPlatform.TradingPlatformRepo;
import com.seldon.stockscanner.WebScraper.WebScraperPlus500;

@Service
public class Plus500ServiceImpl implements Plus500Service
{
    private WebScraperPlus500 webScraperPlus500;
    private Plus500Repo plus500Repo;
    private TradingPlatformRepo tradingPlatformRepo;

    public Plus500ServiceImpl(WebScraperPlus500 webScraperPlus500, Plus500Repo plus500Repo, TradingPlatformRepo tradingPlatformRepo)
    {
        this.webScraperPlus500 = webScraperPlus500;
        this.plus500Repo = plus500Repo;
        this.tradingPlatformRepo = tradingPlatformRepo;
    }

    @Override
    public List<StockEntity> retrieveListedStocks()
    {
        List<StockEntity> results = this.webScraperPlus500.getSupportedSymbols();

        // Check that the Plus500 entity exists in the database
        if(!this.tradingPlatformRepo.existsByName("Plus500"))
        {
            System.out.println("CREATE PLUS500 ENTRY");
            TradingPlatformEntity tradingPlatform = new TradingPlatformEntity("Plus500");

            // DOTO: ADD MANY TO MANY LINK BETWEEN PLATFORM AND STOCKS


            this.tradingPlatformRepo.save(new TradingPlatformEntity("Plus500"));





        }
        else
        {
            System.out.println("PLUS500 ENTRY EXISTS");
        }


        // plus500Repo.deleteAll();
        // plus500Repo.saveAll(results);

        return results;
    }
    
}
