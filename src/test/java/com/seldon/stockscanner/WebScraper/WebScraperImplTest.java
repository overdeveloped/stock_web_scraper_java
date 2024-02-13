package com.seldon.stockscanner.WebScraper;

import static org.mockito.Mockito.when;

import java.util.HashSet;

import org.hibernate.mapping.Set;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.beans.factory.annotation.Autowired;

import com.seldon.stockscanner.Company.CompanyDTO;

@ExtendWith(MockitoExtension.class) 
public class WebScraperImplTest
{
    // @Mock
    // private WebScraperImpl webScraperImpl;
    
    @InjectMocks
    private WebScraperImpl webScraper;
    

    @Test
    public void testGetMegaCompanies()
    {
        when(webScraper.getMegaCompanies()).thenReturn(new HashSet<CompanyDTO>());
    }
}
