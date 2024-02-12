package com.seldon.stockscanner.Scanner;

import org.springframework.web.bind.annotation.RestController;

import com.seldon.stockscanner.Stocks.StockEntity;

import java.util.Set;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;


@RestController
@RequestMapping("/scanner")
public class ScannerController
{
    ScannerService plus500Service;

    public ScannerController(ScannerService plus500Service)
    {
        this.plus500Service = plus500Service;
    }

    @GetMapping("/plus500")
    public Set<StockEntity> retrieveListedStocks()
    {
        return plus500Service.retrieveListedStocks();
    }
    
    
    
}
