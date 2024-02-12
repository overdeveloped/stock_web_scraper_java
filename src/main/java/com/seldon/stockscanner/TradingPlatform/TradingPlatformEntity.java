package com.seldon.stockscanner.TradingPlatform;

import java.util.HashSet;
import java.util.Set;

import com.seldon.stockscanner.Stocks.StockEntity;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.JoinTable;
import jakarta.persistence.ManyToMany;
import jakarta.persistence.Table;

@Entity
@Table(name = "trading_platform_entity")
public class TradingPlatformEntity
{
    public TradingPlatformEntity()
    {
    }

    public TradingPlatformEntity(String platformName)
    {
        this.platformName = platformName;
    }

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    Long id;

    @ManyToMany(fetch = FetchType.LAZY, cascade = {CascadeType.PERSIST, CascadeType.MERGE, CascadeType.REFRESH})
    @JoinTable(
        name = "stock_listed_on_platform",
        joinColumns = @JoinColumn(name = "platform_id"),
        inverseJoinColumns = @JoinColumn(name = "stock_id")
    )
    Set<StockEntity> listedStocks = new HashSet<>();

    @Column(name = "platform_name")
    String platformName;
    
    public Long getId() {
        return id;
    }
    
    public Set<StockEntity> getListedStocks()
    {
        return listedStocks;
    }

    public void setListedStocks(Set<StockEntity> listedStocks)
    {
        this.listedStocks = listedStocks;
    }

    public String getPlatformName()
    {
        return platformName;
    }

    public void setPlatformName(String platformName)
    {
        this.platformName = platformName;
    }

    public void removeStock(StockEntity stock)
    {
        this.listedStocks.remove(stock);
        stock.getListedOn().remove(this);
    }
}
