package com.seldon.stockscanner.Pluss500;

import org.springframework.data.repository.ListCrudRepository;


public interface Plus500Repo extends ListCrudRepository<StockEntity, Long>
{
    
}
