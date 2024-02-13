package com.seldon.stockscanner.Scanner;

import java.util.List;
import java.util.Set;

import org.apache.commons.lang3.NotImplementedException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.graphql.GraphQlProperties.Websocket;
import org.springframework.stereotype.Service;

import com.seldon.stockscanner.Company.CompanyDTO;
import com.seldon.stockscanner.Stocks.StockEntity;
import com.seldon.stockscanner.Stocks.StockRepo;
import com.seldon.stockscanner.TradingPlatform.TradingPlatformEntity;
import com.seldon.stockscanner.TradingPlatform.TradingPlatformRepo;
import com.seldon.stockscanner.WebScraper.WebScraper;

import jakarta.persistence.EntityManager;
import jakarta.transaction.Transactional;

@Service
public class ScannerServiceImpl implements ScannerService
{
    Logger logger = LoggerFactory.getLogger(ScannerServiceImpl.class);
    
    private EntityManager em;
    private WebScraper webScraper;
    private StockRepo stockRepo;
    private TradingPlatformRepo tradingPlatformRepo;

    public ScannerServiceImpl(EntityManager em, WebScraper webScraperPlus500, StockRepo stockRepo, TradingPlatformRepo tradingPlatformRepo)
    {
        this.em = em;
        this.webScraper = webScraperPlus500;
        this.stockRepo = stockRepo;
        this.tradingPlatformRepo = tradingPlatformRepo;
    }

    @Override
    @Transactional
    public Set<StockEntity> retrieveListedStocks()
    {
        Set<StockEntity> results = this.webScraper.getPluss500SupportedSymbols();
        
        resetPlatformsAndStocks();

        // Check that the Plus500 entity exists in the database
        if(!this.tradingPlatformRepo.existsByName("Plus500"))
        {
            System.out.println("CREATE PLUS500 ENTRY");

            // DOTO: ADD MANY TO MANY LINK BETWEEN PLATFORM AND STOCKS
            // saveStocksWithPlatform(results);





        }
        else
        {
            System.out.println("PLUS500 ENTRY EXISTS");
        }


        // plus500Repo.saveAll(results);

        return results;
    }
    
    @Transactional
    public void saveStocksWithPlatform(Set<StockEntity> stocks)
    {
        TradingPlatformEntity tradingPlatform = new TradingPlatformEntity("Plus500");

        int count = 0;
        for (StockEntity stock : stocks)
        {
            // System.out.println("LOOP");
            // tradingPlatform.getListedStocks().add(stock);
            stock.getListedOn().add(tradingPlatform);
            
            if (count == 10)
            {
                break;
            }
            count ++;
        }

        tradingPlatform.setListedStocks(stocks);
        // stocks.forEach(stock ->
        // {
        // });

        this.tradingPlatformRepo.save(tradingPlatform);

    }

    @Transactional
    public void resetPlatformsAndStocks()
    {
        TradingPlatformEntity tradingPlatform = new TradingPlatformEntity("Plus500");
        List<StockEntity> stocks = this.stockRepo.findAll();

        for (StockEntity stock : stocks)
        {
            tradingPlatform.removeStock(stock);
            stock.getListedOn().remove(tradingPlatform);
        }

        em.persist(tradingPlatform);

        tradingPlatformRepo.save(tradingPlatform);
        tradingPlatformRepo.deleteAll();

        stockRepo.deleteAll();
    }

    @Override
    public CompanyDTO retrieveDummyCompany()
    {
        return webScraper.getDummyCompany();
    }

    @Override
    public Set<CompanyDTO> getMegaCompanies()
    {
        return webScraper.getMegaCompanies();
    }

}
