package com.seldon.stockscanner.Pluss500;

import java.util.Set;

import com.seldon.stockscanner.TradingPlatform.TradingPlatformEntity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.ManyToMany;
import jakarta.persistence.Table;

@Entity
@Table(name = "stock_entity")
public class StockEntity
{
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    Long id;

    @ManyToMany(mappedBy = "stock_listed_on_platform")
    Set<TradingPlatformEntity> listedOn;

    @Column(name = "stock_symbol", length = 50)
    String stockSymbol;

    @Column(name = "stock_name", length = 50)
    String stockName;

    public StockEntity()
    {}
    
    public StockEntity(String stockSymbol, String stockName)
    {
        this.stockSymbol = stockSymbol;
        this.stockName = stockName;
    }

    public Long getId()
    {
        return this.id;
    }

    public String getStockSymbol()
    {
        return this.stockSymbol;
    }

    public void setStockSymbol(String stockSymbol)
    {
        this.stockSymbol = stockSymbol;
    }
}
