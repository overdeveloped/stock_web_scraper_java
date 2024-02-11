package com.seldon.stockscanner.TradingPlatform;

import java.util.Set;

import com.seldon.stockscanner.Pluss500.StockEntity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
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

    @ManyToMany
    @JoinTable(name = "stock_listed_on_platform", joinColumns = @JoinColumn(name = "platform_id"), inverseJoinColumns = @JoinColumn(name = "stock_id"))
    Set<StockEntity> listedStocks;

    @Column(name = "platform_name")
    String platformName;
}
