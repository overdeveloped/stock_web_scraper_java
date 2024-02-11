package com.seldon.stockscanner.Pluss500;

import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import org.springframework.web.bind.annotation.GetMapping;

@RestController
public class Pluss500Controller
{
    Plus500Service plus500Service;

    public Pluss500Controller(Plus500Service plus500Service)
    {
        this.plus500Service = plus500Service;
    }

    @GetMapping("plus500")
    public List<StockEntity> retrieveListedStocks()
    {
        return plus500Service.retrieveListedStocks();
    }
    
    
}
