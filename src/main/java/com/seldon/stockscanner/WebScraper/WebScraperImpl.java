package com.seldon.stockscanner.WebScraper;

import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.select.Elements;
import org.springframework.stereotype.Service;

import com.seldon.stockscanner.Stocks.StockEntity;

@Service
public class WebScraperImpl implements WebScraper
{
    private int longestSymbol = 0;
	private int longestName = 0;

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
    
}
