package com.seldon.stockscanner.Company;

public class CompanyDTO
{
    public String ticker;
    public String company;
    public String sector;
    public String industry;
    public String country;
    public String marketCap;
    public String PE;
    public String price;
    public String change;
    public String volume;

    public CompanyDTO()
    {
    }


    public CompanyDTO(String ticker, String company, String sector, String industry, String country, String marketCap,
            String PE, String price, String change, String volume)
    {
        this.ticker = ticker;
        this.company = company;
        this.sector = sector;
        this.industry = industry;
        this.country = country;
        this.marketCap = marketCap;
        this.PE = PE;
        this.price = price;
        this.change = change;
        this.volume = volume;
    }


    public CompanyDTO(CompanyBuilder builder)
    {
        this.ticker = builder.ticker;
        this.company = builder.company;
        this.sector = builder.sector;
        this.industry = builder.industry;
        this.country = builder.country;
        this.marketCap = builder.marketCap;
        this.PE = builder.PE;
        this.price = builder.price;
        this.change = builder.change;
        this.volume = builder.volume;        
    }

    public static class CompanyBuilder
    {
        private String ticker;
        private String company;
        private String sector;
        private String industry;
        private String country;
        private String marketCap;
        private String PE;
        private String price;
        private String change;
        private String volume;

        public CompanyBuilder ticker(String ticker)
        {
            this.ticker = ticker;
            return this;
        }

        public CompanyBuilder company(String company)
        {
            this.company = company;
            return this;
            
        }

        public CompanyBuilder sector(String sector)
        {
            this.sector = sector;
            return this;

        }

        public CompanyBuilder industry(String industry)
        {
            this.industry = industry;
            return this;
        }

        public CompanyBuilder country(String country)
        {
            this.country = country;
            return this;
        }

        public CompanyBuilder marketCap(String marketCap)
        {
            this.marketCap = marketCap;
            return this;
        }

        public CompanyBuilder PE(String PE)
        {
            this.PE = PE;
            return this;
        }

        public CompanyBuilder price(String price)
        {
            this.price = price;
            return this;
        }

        public CompanyBuilder change(String change)
        {
            this.change = change;
            return this;
        }

        public CompanyBuilder volume(String volume)
        {
            this.volume = volume;
            return this;
        }
        
        public CompanyDTO build()
        {
            CompanyDTO company = new CompanyDTO(this);
            return company;
        }
        
    }
}
