package com.seldon.stockscanner.WebScraper;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;
import org.springframework.stereotype.Service;

import com.seldon.stockscanner.Pluss500.StockEntity;

@Service
public class WebScraperPlus500Impl implements WebScraperPlus500
{
    private int longestSymbol = 0;
	private int longestName = 0;

    @Override
    public List<StockEntity> getSupportedSymbols()
    {
        String url = "https://www.plus500.com/en/instruments#indicesf";

        List<StockEntity> results = new ArrayList<StockEntity>();
        try
        {
            Document doc = Jsoup.connect(url).get();
            Element accordionElement = doc.getElementsByClass("accordion").first();
            Elements tableElements = accordionElement.getElementsByTag("table");
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
