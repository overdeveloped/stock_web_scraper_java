package com.seldon.stockscanner.TradingPlatform;

import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.ListCrudRepository;
import org.springframework.data.repository.query.Param;

public interface TradingPlatformRepo extends ListCrudRepository<TradingPlatformEntity, Long>
{
    @Query(
        """
            SELECT EXISTS(SELECT t.platformName FROM TradingPlatformEntity t WHERE t.platformName = :platformName)
        """
    )
    public boolean existsByName(@Param("platformName") String platformName);
}
