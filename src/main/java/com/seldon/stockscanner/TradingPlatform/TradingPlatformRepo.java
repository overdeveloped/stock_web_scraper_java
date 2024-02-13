package com.seldon.stockscanner.TradingPlatform;

import java.util.Set;

import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.ListCrudRepository;
import org.springframework.data.repository.query.Param;

import com.seldon.stockscanner.Stocks.StockEntity;

public interface TradingPlatformRepo extends ListCrudRepository<TradingPlatformEntity, Long>
{
    @Query(
        """
            SELECT EXISTS(SELECT t.platformName FROM TradingPlatformEntity t WHERE t.platformName = :platformName)
        """
    )
    public boolean existsByName(@Param("platformName") String platformName);



    @Query(
        // """
        //     SELECT s.stockSymbol, s.stockName
        //     FROM StockEntity s
        //     LEFT JOIN stock_listed_on_platform slop ON s.id = slop.stock_id
        //     INNER JOIN TradingPlatformEntity tp ON tp.id = slop.platform_id
        //     WHERE tp.platform_name = :platformName

        //     """
        """
            SELECT s.stockSymbol, s.stockName
            FROM StockEntity s
        """
    )
    // TODO: IS THIS IN THE WRONG REPO?
    public Set<StockEntity> findByPlatformName();
    // public Set<StockEntity> findByPlatformName(@Param("platformName") String platformName);
}

