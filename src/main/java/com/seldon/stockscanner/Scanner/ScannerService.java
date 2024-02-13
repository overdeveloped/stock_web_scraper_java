package com.seldon.stockscanner.Scanner;

import java.util.Set;

import com.seldon.stockscanner.Company.CompanyDTO;
import com.seldon.stockscanner.Stocks.StockEntity;

public interface ScannerService
{
    public Set<StockEntity> retrieveListedStocks();
    
    public CompanyDTO retrieveDummyCompany();

    public Set<CompanyDTO> getMegaCompanies();
}
