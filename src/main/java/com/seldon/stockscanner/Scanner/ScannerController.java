package com.seldon.stockscanner.Scanner;

import org.springframework.web.bind.annotation.RestController;

import com.seldon.stockscanner.Company.CompanyDTO;
import com.seldon.stockscanner.Stocks.StockEntity;

import java.util.Set;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;

@RestController
@RequestMapping("/scanner")
public class ScannerController
{
    ScannerService scannerService;

    public ScannerController(ScannerService plus500Service)
    {
        this.scannerService = plus500Service;
    }

    @GetMapping("/dummy")
    public CompanyDTO getDummyCompany()
    {
        return scannerService.retrieveDummyCompany();
    }

    @GetMapping("/plus500")
    public Set<StockEntity> retrieveListedStocks()
    {
        return scannerService.retrieveListedStocks();
    }
    
    @GetMapping("/megacompanies")
    public Set<CompanyDTO> getMegaCompanies()
    {
        return scannerService.getMegaCompanies();
    }    
    
}
