package com.seldon.stockscanner.Stocks;

import org.springframework.data.repository.ListCrudRepository;

public interface StockRepo extends ListCrudRepository<StockEntity, Long>
{
    
}
