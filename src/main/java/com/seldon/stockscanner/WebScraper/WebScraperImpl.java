package com.seldon.stockscanner.WebScraper;

import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;
import org.springframework.stereotype.Service;

import com.seldon.stockscanner.Company.CompanyDTO;
import com.seldon.stockscanner.Stocks.StockEntity;

@Service
public class WebScraperImpl implements WebScraper
{
    private int longestSymbol = 0;
	private int longestName = 0;
    private String rootUrl = "https://finviz.com/screener.ashx?v=111";

    // Plus500
    // The idea being that there may be more platforms added in the future
    @Override
    public Set<StockEntity> getPluss500SupportedSymbols()
    {
        String url = "https://www.plus500.com/en/instruments#indicesf";

        Set<StockEntity> results = new HashSet<StockEntity>();
        try
        {
            Document doc = Jsoup.connect(url).get();
            Elements tableRows = doc.getElementsByTag("tr");

            tableRows.forEach(row ->
            {
                Elements spans = row.getElementsByTag("span");
                
                if (spans.size() > 1)
                {
                    int sybLength = spans.get(0).text().length();
                    int namLength = spans.get(1).text().length();
                    if (sybLength > longestSymbol) { longestSymbol = sybLength; }
                    if (namLength > longestName) { longestName = namLength; }

                    results.add(new StockEntity(spans.get(0).text(), spans.get(1).text()));
                }

            });

            // tableElements.forEach(table ->
            // {
            //     Elements tableData = table.getElementsByTag("span");

            //     if (tableData.size() > 1)
            //     {
            //         int sybLength = tableData.get(0).text().length();
            //         int namLength = tableData.get(1).text().length();
        
            //         if (sybLength > longestSymbol) { longestSymbol = sybLength; }
            //         if (namLength > longestName) { longestName = namLength; }

            //         results.add(new StockEntity(tableData.get(0).text(), tableData.get(1).text()));
            //     }

            //     tableData.forEach(datum ->
            //     {
            //         System.out.println(datum.text());
            //     });

            //     System.out.println();
            // });
            // System.out.println(tableElements.size());
            // System.out.println(tableElements.text());
            
            System.out.println(this.longestSymbol);
            System.out.println(this.longestName);
            

        }
        catch (IOException ex)
        {
            ex.printStackTrace();
        }

        return results;
    }

    @Override
    public CompanyDTO getDummyCompany()
    {
        CompanyDTO dummyComp = new CompanyDTO(
            "ticker",
            "company",
            "sector",
            "industry",
            "country",
            "marketCap",
            "pe",
            "price",
            "change",
            "volume"
        );

        return dummyComp;
    }

    @Override
    public Set<CompanyDTO> getMegaCompanies()
    {
        Set<CompanyDTO> results = new HashSet<CompanyDTO>();
        StringBuilder sb = new StringBuilder();
        
        String fullUrl = sb.append(rootUrl).append("&f=cap_mega").toString();

        try
        {
            Document doc = Jsoup.connect(fullUrl).get();
            Element paginator = doc.getElementById("screener_pagination");
            Elements pages = paginator.getElementsByTag("a");

            int pageScaler = 1;

            // Save the number of pages so that we can iterate through them all
            if (pages.size() > 1)
            {
                pageScaler = pages.size() - 1;
            }

            for (int index = 1; index < 20 * pageScaler; index+=20)
            {
                String pageUrl = String.format("%s&r=%s", fullUrl, Integer.toString(index));
                System.out.println(pageUrl);
                doc = Jsoup.connect(pageUrl).get();

                Element resultsTable = doc.getElementsByClass("styled-table-new").getFirst();
                Elements tableRows = resultsTable.getElementsByTag("tr");
                
                for (Element row : tableRows)
                {
                    Elements rowData = row.getElementsByTag("td");

                    if (rowData.size() > 0)
                    {
                        CompanyDTO company = new CompanyDTO.CompanyBuilder()
                        .ticker(rowData.get(1).text())
                        .company(rowData.get(2).text())
                        .sector(rowData.get(3).text())
                        .industry(rowData.get(4).text())
                        .country(rowData.get(5).text())
                        .marketCap(rowData.get(6).text())
                        .PE(rowData.get(7).text())
                        .price(rowData.get(8).text())
                        .change(rowData.get(9).text())
                        .volume(rowData.get(10).text())
                        .build();
                        
                        System.out.println(row.text());
                        System.out.println();
                        
                        results.add(company);
                    }
                }
            }
        }
        catch (IOException ex)
        {
            ex.printStackTrace();
        }

        return results;
    }
}
