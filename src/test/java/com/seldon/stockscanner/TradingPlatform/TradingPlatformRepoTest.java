package com.seldon.stockscanner.TradingPlatform;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase.Replace;
import org.springframework.boot.test.autoconfigure.orm.jpa.TestEntityManager;

import com.seldon.stockscanner.Stocks.StockEntity;
import com.seldon.stockscanner.Stocks.StockRepo;

import org.springframework.boot.test.autoconfigure.orm.jpa.DataJpaTest;

@DataJpaTest
// @Import(JpaConfig.class)
@AutoConfigureTestDatabase(replace = Replace.NONE)
public class TradingPlatformRepoTest
{
    @Autowired
    private TestEntityManager em;

    @Autowired
    private TradingPlatformRepo tradingPlatformRepo;

    @Autowired
    private StockRepo stockRepo;

    @Test
    public void contextLoads() {
      Assertions.assertNotNull(em);
    }

    @Test
    void savePlatform()
    {
        TradingPlatformEntity platform = new TradingPlatformEntity("TestPlatform");
        tradingPlatformRepo.save(platform);
        assertEquals(1, tradingPlatformRepo.findAll().size());

        tradingPlatformRepo.deleteAll();

        assertEquals(0, tradingPlatformRepo.findAll().size());
    }

    @Test
    void savePlatformsWithStocks()
    {
        // Setup
        TradingPlatformEntity platform = new TradingPlatformEntity("TestPlatform");
        StockEntity stock1 = new StockEntity("TEST", "Test Stock");
        StockEntity stock2 = new StockEntity("TEST2", "Test Stock 2");

        platform.getListedStocks().add(stock1);
        platform.getListedStocks().add(stock2);

        tradingPlatformRepo.save(platform);

        // Assert
        assertEquals(1, tradingPlatformRepo.findAll().size());
        assertEquals(2, stockRepo.findAll().size());


        // Reset
        platform.removeStock(stock1);
        platform.removeStock(stock2);

        em.persist(platform);
        em.remove(stock1);
        em.remove(stock2);

        tradingPlatformRepo.delete(platform);

        // Assert
        assertEquals(0, tradingPlatformRepo.findAll().size());
        assertEquals(0, stockRepo.findAll().size());
        
    }
}
