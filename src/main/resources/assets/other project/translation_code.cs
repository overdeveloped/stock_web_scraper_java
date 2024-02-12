using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SeldonStockScannerAPI.models;

namespace SeldonStockScannerAPI.FinvizScan
{
    public class FinvizService : IFinvizFilter
    {
        private readonly WebScraper _scraper = new WebScraper();
        private string RootUrl;
        private string FullUrl;
        private Dictionary<string, string> filterNames;

        //private FinvizFilterSave filterSave;
        public Dictionary<string, Dictionary<string, string>> translationsMaster;

        // Translations
        // Column 1
        public Dictionary<string, string> translationExchange { get; set; }
        public Dictionary<string, string> translationMarketCap { get; set; }
        public Dictionary<string, string> translationPB { get; set; }
        public Dictionary<string, string> translationEPSGrowthPast5Years { get; set; }
        public Dictionary<string, string> translationDividendYield { get; set; }
        public Dictionary<string, string> translationQuickRatio { get; set; }
        public Dictionary<string, string> translationNetProfitMargin { get; set; }
        public Dictionary<string, string> translationInstitutionalTransactions { get; set; }
        public Dictionary<string, string> translationPerformance { get; set; }
        public Dictionary<string, string> translation20SMA { get; set; }
        public Dictionary<string, string> translation20DayHighLow { get; set; }
        public Dictionary<string, string> translationBeta { get; set; }
        public Dictionary<string, string> translationPrice { get; set; }
        public Dictionary<string, string> translationAfterHoursClose { get; set; }

        // Column 2
        public Dictionary<string, string> translationIndex { get; set; }
        public Dictionary<string, string> translationPE { get; set; }
        public Dictionary<string, string> translationPriceCash { get; set; }
        public Dictionary<string, string> translationEPSGrowthNext5Years { get; set; }
        public Dictionary<string, string> translationReturnOnAssets { get; set; }
        public Dictionary<string, string> translationLTDebtEquity { get; set; }
        public Dictionary<string, string> translationPayoutRatio { get; set; }
        public Dictionary<string, string> translationFloatShort { get; set; }
        public Dictionary<string, string> translationPerformance2 { get; set; }
        public Dictionary<string, string> translation50SMA { get; set; }
        public Dictionary<string, string> translation50DayHighLow { get; set; }
        public Dictionary<string, string> translationAverageTrueRange { get; set; }
        public Dictionary<string, string> translationTargetPrice { get; set; }
        public Dictionary<string, string> translationAfterHoursChange { get; set; }

        // Column 3
        public Dictionary<string, string> translationSector { get; set; }
        public Dictionary<string, string> translationForwardPE { get; set; }
        public Dictionary<string, string> translationPriceFreeCashFlow { get; set; }
        public Dictionary<string, string> translationSalesGrowthPast5Years { get; set; }
        public Dictionary<string, string> translationReturnOnEquity { get; set; }
        public Dictionary<string, string> translationDebtEquity { get; set; }
        public Dictionary<string, string> translationInsiderOwnership { get; set; }
        public Dictionary<string, string> translationAnalystRecomends { get; set; }
        public Dictionary<string, string> translationVolatility { get; set; }
        public Dictionary<string, string> translation200SMA { get; set; }
        public Dictionary<string, string> translation52WeekHighLow { get; set; }
        public Dictionary<string, string> translationAverageVolume { get; set; }
        public Dictionary<string, string> translationIPODate { get; set; }

        // Column 4
        public Dictionary<string, string> translationIndustry { get; set; }
        public Dictionary<string, string> translationPEG { get; set; }
        public Dictionary<string, string> translationEPSGrowthThisYears { get; set; }
        public Dictionary<string, string> translationEPSGrowthQuarterOverQuarter{ get; set; }
        public Dictionary<string, string> translationReturnOnInvestment { get; set; }
        public Dictionary<string, string> translationGrossMargin { get; set; }
        public Dictionary<string, string> translationInsiderTransactions { get; set; }
        public Dictionary<string, string> translationOptionShort { get; set; }
        public Dictionary<string, string> translationRSI14 { get; set; }
        public Dictionary<string, string> translationChange { get; set; }
        public Dictionary<string, string> translationPattern { get; set; }
        public Dictionary<string, string> translationRelativeVolume { get; set; }
        public Dictionary<string, string> translationSharesOutstanding { get; set; }

        // Column 5
        public Dictionary<string, string> translationCountry { get; set; }
        public Dictionary<string, string> translationPS { get; set; }
        public Dictionary<string, string> translationEPSGrowthNextYear { get; set; }
        public Dictionary<string, string> translationSalesGrowthQuarterOverQuarter { get; set; }
        public Dictionary<string, string> translationCurrentRatio { get; set; }
        public Dictionary<string, string> translationOperatingMargin { get; set; }
        public Dictionary<string, string> translationInstitutionalOwnership { get; set; }
        public Dictionary<string, string> translationEarningsDate { get; set; }
        public Dictionary<string, string> translationGap { get; set; }
        public Dictionary<string, string> translationChangeFromOpen { get; set; }
        public Dictionary<string, string> translationCandlestick { get; set; }
        public Dictionary<string, string> translationCurrentVolume { get; set; }
        public Dictionary<string, string> translationFloat { get; set; }

        // Plus500 Filter
        private List<string> plus500tickers = new List<string>();

        public FinvizService()
        {
            translationsMaster = new Dictionary<string, Dictionary<string, string>>();
            translationExchange = new Dictionary<string, string>();
            SetupTranslations();
        }

        public FinvizService(Dictionary<string, string> filterNames)
        {
            translationsMaster = new Dictionary<string, Dictionary<string, string>>();
            this.filterNames = filterNames;
            SetupTranslations();
            this.FullUrl = BuildUrl();
        }

        private void ResetFilterNames()
        {
            this.filterNames = new Dictionary<string, string>();
            //this.filterNames.Add(EnumFilterType.Exchange.ToString(), "NYSE");
        }

        public string GetFullUrl()
        {
            return this.FullUrl;
        }

        private List<FinvizCompanyEntity> filterByPluss500(List<FinvizCompanyEntity> finvizResults)
        {
            List<FinvizCompanyEntity> result = new List<FinvizCompanyEntity>();

            using (var context = new DataContext())
            {
                List<Plus500Symbol> symbols = context.plus500Symbols.ToList();

                if (symbols.Count > 0 && _scraper.GetMegaCompanies().Count() > 0)
                {
                    foreach (Plus500Symbol symbol in symbols)
                    {
                        foreach (FinvizCompanyEntity finvizResult in finvizResults)
                        {
                            if (symbol.Symbol.Contains(finvizResult.Ticker))
                            {
                                result.Add(finvizResult);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public List<string> GetPlus500List()
        {
            this.plus500tickers = _scraper.GetCompletePlus500();

            using (var context = new DataContext())
            {
                foreach (var plus500ticker in plus500tickers)
                {
                    Plus500Symbol symbol = new Plus500Symbol();
                    symbol.Symbol = plus500ticker;

                    context.plus500Symbols.Add(symbol);

                }

                context.SaveChanges();
            }


            return this.plus500tickers;
        }

        public List<FinvizCompanyEntity> GetMegaCompanies()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MarketCap.ToString(), "Mega ($200bln and more)");
            List<FinvizCompanyEntity> finvizResults = _scraper.GetMegaCompanies();

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetLongHolds()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MarketCap.ToString(), "+Micro (over $50mln)");
            this.filterNames.Add(FinvzEnumFilterType.MA20.ToString(), "Price above SMA20");
            this.filterNames.Add(FinvzEnumFilterType.Beta.ToString(), "Over 1.5");
            this.filterNames.Add(FinvzEnumFilterType.EpsGrowthNext5Years.ToString(), "Over 10%");
            this.filterNames.Add(FinvzEnumFilterType.ReturnOnEquity.ToString(), "Over +15%");
            this.filterNames.Add(FinvzEnumFilterType.CurrentRatio.ToString(), "Over 1.5");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "longHolds");

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetOversoldBounce()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.Price.ToString(), "Over $5");
            this.filterNames.Add(FinvzEnumFilterType.RSI14.ToString(), "Oversold (30)");
            this.filterNames.Add(FinvzEnumFilterType.Change.ToString(), "Up");
            this.filterNames.Add(FinvzEnumFilterType.RelativeVolume.ToString(), "Over 2");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "oversoldBounce");

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetBreakout()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MA20.ToString(), "Price above SMA20");
            this.filterNames.Add(FinvzEnumFilterType.MA50.ToString(), "Price above SMA50");
            this.filterNames.Add(FinvzEnumFilterType.MA200.ToString(), "Price above SMA200");
            this.filterNames.Add(FinvzEnumFilterType.HighLow50Day.ToString(), "New High");
            this.filterNames.Add(FinvzEnumFilterType.ReturnOnEquity.ToString(), "Over +20%");
            this.filterNames.Add(FinvzEnumFilterType.DebtEquity.ToString(), "Under 1");
            this.filterNames.Add(FinvzEnumFilterType.AverageVolume.ToString(), "Over 100K");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "breakout");

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetBreakoutV2()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MarketCap.ToString(), "+Small (over $300mln)");
            this.filterNames.Add(FinvzEnumFilterType.RelativeVolume.ToString(), "Over 1"); // Move this up if too many results
            this.filterNames.Add(FinvzEnumFilterType.CurrentVolume.ToString(), "Over 500K"); // Move this up if too many results
            this.filterNames.Add(FinvzEnumFilterType.SalesGrowthPast5Years.ToString(), "Positive (>0%)");
            this.filterNames.Add(FinvzEnumFilterType.MA50.ToString(), "Price above SMA50");
            this.filterNames.Add(FinvzEnumFilterType.HighLow52Week.ToString(), "0-10% below High");
            this.filterNames.Add(FinvzEnumFilterType.Volatility.ToString(), "Month - Over 5%");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "breakout_v2");

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetShorts()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MarketCap.ToString(), "+Small (over $300mln)");
            this.filterNames.Add(FinvzEnumFilterType.FloatShort.ToString(), "High (>20%)");
            this.filterNames.Add(FinvzEnumFilterType.AverageVolume.ToString(), "Over 500K");
            this.filterNames.Add(FinvzEnumFilterType.RelativeVolume.ToString(), "Over 1");
            this.filterNames.Add(FinvzEnumFilterType.CurrentVolume.ToString(), "Over 2M");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "shorts");

            return filterByPluss500(finvizResults);
        }

        public List<FinvizCompanyEntity> GetBounceOffMa()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MA20.ToString(), "Price above SMA20");
            this.filterNames.Add(FinvzEnumFilterType.MA50.ToString(), "Price below SMA50");
            this.filterNames.Add(FinvzEnumFilterType.AverageVolume.ToString(), "Over 400K");
            this.filterNames.Add(FinvzEnumFilterType.RelativeVolume.ToString(), "Over 1");
            this.filterNames.Add(FinvzEnumFilterType.CurrentVolume.ToString(), "Over 2M");

            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "bouncema");

            return filterByPluss500(finvizResults);
        }


        public List<FinvizCompanyEntity> GetShorts2()
        {
            ResetFilterNames();

            this.filterNames.Add(FinvzEnumFilterType.MarketCap.ToString(), "+Micro (over $50mln)");
            this.filterNames.Add(FinvzEnumFilterType.MA20.ToString(), "Price above SMA20");
            this.filterNames.Add(FinvzEnumFilterType.Beta.ToString(), "Over 1.5");
            this.filterNames.Add(FinvzEnumFilterType.EpsGrowthNext5Years.ToString(), "Over 10%");
            this.filterNames.Add(FinvzEnumFilterType.ReturnOnEquity.ToString(), "Over +15%");
            this.filterNames.Add(FinvzEnumFilterType.CurrentRatio.ToString(), "Over 1.5");

            return _scraper.GetCustomWatchList(BuildUrl(), "shorts2");
        }


        public List<FinvizCompanyEntity> GetTech()
        {
            ResetFilterNames();

            return _scraper.GetTech();
        }

        public List<FinvizCompanyEntity> ForteCapitalDayTrading()
        {
            // From this tutorial:
            // https://www.youtube.com/watch?v=yyW8WDGjdKI
            ResetFilterNames();

            //this.filterNames.Add(FinvzEnumFilterType.Country.ToString(), "USA");
            this.filterNames.Add(FinvzEnumFilterType.SalesGrowthQuarterOverQuarter.ToString(), "Over 30%");
            this.filterNames.Add(FinvzEnumFilterType.Industry.ToString(), "Stocks only (ex-Funds)");
            this.filterNames.Add(FinvzEnumFilterType.AverageVolume.ToString(), "Over 500K");
            this.filterNames.Add(FinvzEnumFilterType.IPODate.ToString(), "In the last 3 years"); // Make the most explosive moves but might keep the list too small
            this.filterNames.Add(FinvzEnumFilterType.MA200.ToString(), "Price above SMA200");
            this.filterNames.Add(FinvzEnumFilterType.MA50.ToString(), "Price above SMA50");
            this.filterNames.Add(FinvzEnumFilterType.MA20.ToString(), "Price above SMA20");
            this.filterNames.Add(FinvzEnumFilterType.FloatShort.ToString(), "Over 5%");
            this.filterNames.Add(FinvzEnumFilterType.ChangeFromOpen.ToString(), "Up");


            List<FinvizCompanyEntity> finvizResults = _scraper.GetCustomWatchList(BuildUrl(), "forte_day_trading_v2");

            return filterByPluss500(finvizResults);
        }


        public string BuildUrl()
        {
            // Start URL
            RootUrl = "https://finviz.com/screener.ashx?v=111";
            StringBuilder argumentsSB = new StringBuilder();

            string root = this.RootUrl;
            string args = "&f=";

            foreach (var filterKey in filterNames.Keys)
            {
                if (argumentsSB.Length > 0) argumentsSB.Append(",");
                Dictionary<string, string> sma200translator = translationsMaster[filterKey];
                string filterName = filterNames[filterKey];
                string filterURL = sma200translator[filterName];
                argumentsSB.Append(translationsMaster[filterKey][filterNames[filterKey]]);
            }

            string fullUrl = "";
            if (argumentsSB.Length > 0)
            {
                fullUrl = root + args + argumentsSB.ToString();
            }
            else
            {
                fullUrl = root;
            }

            this.FullUrl = fullUrl;

            return fullUrl;
        }

        private void SetupTranslations()
        {
            // Copy URL paramerters from the website:
            // https://finviz.com/screener.ashx?v=111&f=fa_div_o10

            // Column 1
            translationExchange = new Dictionary<string, string>();
            translationExchange.Add("Any", "");
            translationExchange.Add("AMEX", "exch_amex");
            translationExchange.Add("NASDAQ", "exch_nasd");
            translationExchange.Add("NYSE", "exch_nyse");
            translationsMaster.Add(FinvzEnumFilterType.Exchange.ToString(), translationExchange);

            translationMarketCap = new Dictionary<string, string>();
            translationMarketCap.Add("Any", "");
            translationMarketCap.Add("Mega ($200bln and more)", "cap_mega");
            translationMarketCap.Add("Large ($10bln to $200bln)", "cap_large");
            translationMarketCap.Add("Mid ($2bln to $10bln)", "cap_mid");
            translationMarketCap.Add("Small ($300mln to $2bln)", "cap_small");
            translationMarketCap.Add("Micro ($50mln to $300mln)", "cap_micro");
            translationMarketCap.Add("Nano (under $50mln)", "cap_nano");
            translationMarketCap.Add("+Large (over $10bln)", "cap_largeover");
            translationMarketCap.Add("+Mid (over $2bln)", "cap_midover");
            translationMarketCap.Add("+Small (over $300mln)", "cap_smallover");
            translationMarketCap.Add("+Micro (over $50mln)", "cap_microover");
            translationMarketCap.Add("-Large (under $200bln)", "cap_largeunder");
            translationMarketCap.Add("-Mid (under $10bln)", "cap_midunder");
            translationMarketCap.Add("-Small (under $2bln)", "cap_smallunder");
            translationMarketCap.Add("-Micro (under $300mln)", "cap_microunder");
            translationMarketCap.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.MarketCap.ToString(), translationMarketCap);

            translationPB = new Dictionary<string, string>();
            translationPB.Add("Any", "");
            translationPB.Add("Low (<1)", "fa_pb_low");
            translationPB.Add("High (>5)", "fa_pb_high");
            translationPB.Add("Under 1", "fa_pb_u1");
            translationPB.Add("Under 2", "fa_pb_u2");
            translationPB.Add("Under 3", "fa_pb_u3");
            translationPB.Add("Under 4", "fa_pb_u4");
            translationPB.Add("Under 5", "fa_pb_u5");
            translationPB.Add("Under 6", "fa_pb_u6");
            translationPB.Add("Under 7", "fa_pb_u7");
            translationPB.Add("Under 8", "fa_pb_u8");
            translationPB.Add("Under 9", "fa_pb_u9");
            translationPB.Add("Under 10", "fa_pb_u10");
            translationPB.Add("Over 1", "fa_pb_o1");
            translationPB.Add("Over 2", "fa_pb_o2");
            translationPB.Add("Over 3", "fa_pb_o3");
            translationPB.Add("Over 4", "fa_pb_o4");
            translationPB.Add("Over 5", "fa_pb_o5");
            translationPB.Add("Over 6", "fa_pb_o6");
            translationPB.Add("Over 7", "fa_pb_o7");
            translationPB.Add("Over 8", "fa_pb_o8");
            translationPB.Add("Over 9", "fa_pb_o9");
            translationPB.Add("Over 10", "fa_pb_o10");
            translationPB.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PB.ToString(), translationPB);

            translationEPSGrowthPast5Years = new Dictionary<string, string>();
            translationEPSGrowthPast5Years.Add("Any", "");
            translationEPSGrowthPast5Years.Add("Negative (<0%)", "fa_eps5years_neg");
            translationEPSGrowthPast5Years.Add("Positive (>0%)", "fa_eps5years_pos");
            translationEPSGrowthPast5Years.Add("Positive Low (0-10%)", "fa_eps5years_poslow");
            translationEPSGrowthPast5Years.Add("High (>25%)", "fa_eps5years_high");
            translationEPSGrowthPast5Years.Add("Under 5%", "fa_eps5years_u5");
            translationEPSGrowthPast5Years.Add("Under 10%", "fa_eps5years_u10");
            translationEPSGrowthPast5Years.Add("Under 15%", "fa_eps5years_u15");
            translationEPSGrowthPast5Years.Add("Under 20%", "fa_eps5years_u20");
            translationEPSGrowthPast5Years.Add("Under 25%", "fa_eps5years_u25");
            translationEPSGrowthPast5Years.Add("Under 30%", "fa_eps5years_u30");
            translationEPSGrowthPast5Years.Add("Over 5%", "fa_eps5years_o5");
            translationEPSGrowthPast5Years.Add("Over 10%", "fa_eps5years_o10");
            translationEPSGrowthPast5Years.Add("Over 15%", "fa_eps5years_o15");
            translationEPSGrowthPast5Years.Add("Over 20%", "fa_eps5years_o20");
            translationEPSGrowthPast5Years.Add("Over 25%", "fa_eps5years_o25");
            translationEPSGrowthPast5Years.Add("Over 30%", "fa_eps5years_o30");
            translationEPSGrowthPast5Years.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EpsGrowthPast5Years.ToString(), translationEPSGrowthPast5Years);

            translationDividendYield = new Dictionary<string, string>();
            translationDividendYield.Add("Any", "");
            translationDividendYield.Add("None (0%)", "fa_div_none");
            translationDividendYield.Add("Positive (>0%)", "fa_div_pos");
            translationDividendYield.Add("High (>5%)", "fa_div_high");
            translationDividendYield.Add("Very High (>10%)", "fa_div_veryhigh");
            translationDividendYield.Add("Over 1%", "fa_div_o1");
            translationDividendYield.Add("Over 2%", "fa_div_o2");
            translationDividendYield.Add("Over 3%", "fa_div_o3");
            translationDividendYield.Add("Over 4%", "fa_div_o4");
            translationDividendYield.Add("Over 5%", "fa_div_o5");
            translationDividendYield.Add("Over 6%", "fa_div_o6");
            translationDividendYield.Add("Over 7%", "fa_div_o7");
            translationDividendYield.Add("Over 8%", "fa_div_o8");
            translationDividendYield.Add("Over 9%", "fa_div_o9");
            translationDividendYield.Add("Over 10%", "fa_div_o10");
            translationDividendYield.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.DividendYield.ToString(), translationDividendYield);

            translationQuickRatio = new Dictionary<string, string>();
            translationQuickRatio.Add("Any", "");
            translationQuickRatio.Add("High (>3)", "fa_quickratio_high");
            translationQuickRatio.Add("Low (<0.5)", "fa_quickratio_low");
            translationQuickRatio.Add("Under 1", "fa_quickratio_u1");
            translationQuickRatio.Add("Under 0.5", "fa_quickratio_u0.5");
            translationQuickRatio.Add("Over 0.5", "fa_quickratio_o0.5");
            translationQuickRatio.Add("Over 1", "fa_quickratio_o1");
            translationQuickRatio.Add("Over 1.5", "fa_quickratio_o1.5");
            translationQuickRatio.Add("Over 2", "fa_quickratio_o2");
            translationQuickRatio.Add("Over 3", "fa_quickratio_o3");
            translationQuickRatio.Add("Over 4", "fa_quickratio_o4");
            translationQuickRatio.Add("Over 5", "fa_quickratio_o5");
            translationQuickRatio.Add("Over 10", "fa_quickratio_o10");
            translationQuickRatio.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.QuickRatio.ToString(), translationQuickRatio);

            translationNetProfitMargin = new Dictionary<string, string>();
            translationNetProfitMargin.Add("Any", "");
            translationNetProfitMargin.Add("Positive (>0%)", "fa_netmargin_pos");
            translationNetProfitMargin.Add("Negative (<0%)", "fa_netmargin_neg");
            translationNetProfitMargin.Add("Very Negative (<-20%)", "fa_netmargin_veryneg");
            translationNetProfitMargin.Add("High (>20%)", "fa_netmargin_high");
            translationNetProfitMargin.Add("Under 90%", "fa_netmargin_u90");
            translationNetProfitMargin.Add("Under 80%", "fa_netmargin_u80");
            translationNetProfitMargin.Add("Under 70%", "fa_netmargin_u70");
            translationNetProfitMargin.Add("Under 60%", "fa_netmargin_u60");
            translationNetProfitMargin.Add("Under 50%", "fa_netmargin_u50");
            translationNetProfitMargin.Add("Under 45%", "fa_netmargin_u45");
            translationNetProfitMargin.Add("Under 40%", "fa_netmargin_u40");
            translationNetProfitMargin.Add("Under 35%", "fa_netmargin_u35");
            translationNetProfitMargin.Add("Under 30%", "fa_netmargin_u30");
            translationNetProfitMargin.Add("Under 25%", "fa_netmargin_u25");
            translationNetProfitMargin.Add("Under 20%", "fa_netmargin_u20");
            translationNetProfitMargin.Add("Under 15%", "fa_netmargin_u15");
            translationNetProfitMargin.Add("Under 10%", "fa_netmargin_u10");
            translationNetProfitMargin.Add("Under 5%", "fa_netmargin_u5");
            translationNetProfitMargin.Add("Under 0%", "fa_netmargin_u0");
            translationNetProfitMargin.Add("Under -10%", "fa_netmargin_u-10");
            translationNetProfitMargin.Add("Under -20%", "fa_netmargin_u-20");
            translationNetProfitMargin.Add("Under -30%", "fa_netmargin_u-30");
            translationNetProfitMargin.Add("Under -50%", "fa_netmargin_u-50");
            translationNetProfitMargin.Add("Under -70%", "fa_netmargin_u-70");
            translationNetProfitMargin.Add("Under -100%", "fa_netmargin_u-100");
            translationNetProfitMargin.Add("Over 0%", "fa_netmargin_o0");
            translationNetProfitMargin.Add("Over 5%", "fa_netmargin_o5");
            translationNetProfitMargin.Add("Over 10%", "fa_netmargin_o10");
            translationNetProfitMargin.Add("Over 15%", "fa_netmargin_o15");
            translationNetProfitMargin.Add("Over 20%", "fa_netmargin_o20");
            translationNetProfitMargin.Add("Over 25%", "fa_netmargin_o25");
            translationNetProfitMargin.Add("Over 30%", "fa_netmargin_o30");
            translationNetProfitMargin.Add("Over 35%", "fa_netmargin_o35");
            translationNetProfitMargin.Add("Over 40%", "fa_netmargin_o40");
            translationNetProfitMargin.Add("Over 45%", "fa_netmargin_o45");
            translationNetProfitMargin.Add("Over 50%", "fa_netmargin_o50");
            translationNetProfitMargin.Add("Over 60%", "fa_netmargin_o60");
            translationNetProfitMargin.Add("Over 70%", "fa_netmargin_o70");
            translationNetProfitMargin.Add("Over 80%", "fa_netmargin_o80");
            translationNetProfitMargin.Add("Over 90%", "fa_netmargin_o90");
            translationNetProfitMargin.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.NetProfitMargin.ToString(), translationNetProfitMargin);

            translationInstitutionalTransactions = new Dictionary<string, string>();
            translationInstitutionalTransactions.Add("Any", "");
            translationInstitutionalTransactions.Add("Very Negative (<20%)", "sh_insttrans_veryneg");
            translationInstitutionalTransactions.Add("Negative (<0%)", "sh_insttrans_neg");
            translationInstitutionalTransactions.Add("Positive (>0%)", "sh_insttrans_pos");
            translationInstitutionalTransactions.Add("Very Positive (>20%)", "sh_insttrans_verypos");
            translationInstitutionalTransactions.Add("Under -50%", "sh_insttrans_u-50");
            translationInstitutionalTransactions.Add("Under -45%", "sh_insttrans_u-45");
            translationInstitutionalTransactions.Add("Under -40%", "sh_insttrans_u-40");
            translationInstitutionalTransactions.Add("Under -35%", "sh_insttrans_u-35");
            translationInstitutionalTransactions.Add("Under -30%", "sh_insttrans_u-30");
            translationInstitutionalTransactions.Add("Under -25%", "sh_insttrans_u-25");
            translationInstitutionalTransactions.Add("Under -20%", "sh_insttrans_u-20");
            translationInstitutionalTransactions.Add("Under -15%", "sh_insttrans_u-15");
            translationInstitutionalTransactions.Add("Under -10%", "sh_insttrans_u-10");
            translationInstitutionalTransactions.Add("Under -5%", "sh_insttrans_u-5");
            translationInstitutionalTransactions.Add("Over +5%", "sh_insttrans_o5");
            translationInstitutionalTransactions.Add("Over +10%", "sh_insttrans_o10");
            translationInstitutionalTransactions.Add("Over +15%", "sh_insttrans_o15");
            translationInstitutionalTransactions.Add("Over +20%", "sh_insttrans_o20");
            translationInstitutionalTransactions.Add("Over +25%", "sh_insttrans_o25");
            translationInstitutionalTransactions.Add("Over +30%", "sh_insttrans_o30");
            translationInstitutionalTransactions.Add("Over +35%", "sh_insttrans_o35");
            translationInstitutionalTransactions.Add("Over +40%", "sh_insttrans_o40");
            translationInstitutionalTransactions.Add("Over +45%", "sh_insttrans_o45");
            translationInstitutionalTransactions.Add("Over +50%", "sh_insttrans_o50");
            translationInstitutionalTransactions.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.InstitutionalTransactions.ToString(), translationInstitutionalTransactions);

            translationPerformance = new Dictionary<string, string>();
            translationPerformance.Add("Any", "");
            translationPerformance.Add("Today Up", "ta_perf_dup");
            translationPerformance.Add("Today Down", "ta_perf_ddown");
            translationPerformance.Add("Today -15%", "ta_perf_d15u");
            translationPerformance.Add("Today -10%", "ta_perf_d10u");
            translationPerformance.Add("Today -5%", "ta_perf_d5u");
            translationPerformance.Add("Today +5%", "ta_perf_d5o");
            translationPerformance.Add("Today +10%", "ta_perf_d10o");
            translationPerformance.Add("Today +15%", "ta_perf_d15o");
            translationPerformance.Add("Week -30%", "ta_perf_w30u");
            translationPerformance.Add("Week -20%", "ta_perf_w20u");
            translationPerformance.Add("Week -10%", "ta_perf_w10u");
            translationPerformance.Add("Week Down", "ta_perf_1wdown");
            translationPerformance.Add("Week Up", "ta_perf_1wup");
            translationPerformance.Add("Week +10%", "ta_perf_1w10o");
            translationPerformance.Add("Week +20%", "ta_perf_1w20o");
            translationPerformance.Add("Week +30%", "ta_perf_1w30o");
            translationPerformance.Add("Month -50%", "ta_perf_4w50u");
            translationPerformance.Add("Month -30%", "ta_perf_4w30u");
            translationPerformance.Add("Month -20%", "ta_perf_4w20u");
            translationPerformance.Add("Month -10%", "ta_perf_4w10u");
            translationPerformance.Add("Month Down", "ta_perf_4wdown");
            translationPerformance.Add("Month Up", "ta_perf_4wup");
            translationPerformance.Add("Month +10%", "ta_perf_4w10o");
            translationPerformance.Add("Month +20%", "ta_perf_4w20o");
            translationPerformance.Add("Month +30%", "ta_perf_4w30o");
            translationPerformance.Add("Month +50%", "ta_perf_4w40o");
            translationPerformance.Add("Quarter -50%", "ta_perf_13w50u");
            translationPerformance.Add("Quarter -30%", "ta_perf_13w30u");
            translationPerformance.Add("Quarter -20%", "ta_perf_13w20u");
            translationPerformance.Add("Quarter -10%", "ta_perf_13w10u");
            translationPerformance.Add("Quarter Down", "ta_perf_13wdown");
            translationPerformance.Add("Quarter Up", "ta_perf_13wup");
            translationPerformance.Add("Quarter +10%", "ta_perf_13w10o");
            translationPerformance.Add("Quarter +20%", "ta_perf_13w20o");
            translationPerformance.Add("Quarter +30%", "ta_perf_13w30o");
            translationPerformance.Add("Quarter +50%", "ta_perf_13w50o");
            translationPerformance.Add("Half -75%", "ta_perf_26w75u");
            translationPerformance.Add("Half -50%", "ta_perf_26w50u");
            translationPerformance.Add("Half -30%", "ta_perf_26w30u");
            translationPerformance.Add("Half -20%", "ta_perf_26w20u");
            translationPerformance.Add("Half -10%", "ta_perf_26w10u");
            translationPerformance.Add("Half Down", "ta_perf_26wdown");
            translationPerformance.Add("Half Up", "ta_perf_26wup");
            translationPerformance.Add("Half +10%", "ta_perf_26w10o");
            translationPerformance.Add("Half +20%", "ta_perf_26w20o");
            translationPerformance.Add("Half +30%", "ta_perf_26w30o");
            translationPerformance.Add("Half +50%", "ta_perf_26w50o");
            translationPerformance.Add("Half +100%", "ta_perf_26w100o");
            translationPerformance.Add("Year -75%", "ta_perf_52w75u");
            translationPerformance.Add("Year -50%", "ta_perf_52w50u");
            translationPerformance.Add("Year -30%", "ta_perf_52w30u");
            translationPerformance.Add("Year -20%", "ta_perf_52w20u");
            translationPerformance.Add("Year -10%", "ta_perf_52w10u");
            translationPerformance.Add("Year Down", "ta_perf_52wdown");
            translationPerformance.Add("Year Up", "ta_perf_52wup");
            translationPerformance.Add("Year +10%", "ta_perf_52w10o");
            translationPerformance.Add("Year +20%", "ta_perf_52w20o");
            translationPerformance.Add("Year +30%", "ta_perf_52w30o");
            translationPerformance.Add("Year +50%", "ta_perf_52w50o");
            translationPerformance.Add("Year +100%", "ta_perf_52w100o");
            translationPerformance.Add("Year +200%", "ta_perf_52w200o");
            translationPerformance.Add("Year +300%", "ta_perf_52w300o");
            translationPerformance.Add("Year +500%", "ta_perf_52w500o");
            translationPerformance.Add("YTD -75%", "ta_perf_ytd75u");
            translationPerformance.Add("YTD -50%", "ta_perf_ytd50u");
            translationPerformance.Add("YTD -30%", "ta_perf_ytd30u");
            translationPerformance.Add("YTD -20%", "ta_perf_ytd20u");
            translationPerformance.Add("YTD -10%", "ta_perf_ytd10u");
            translationPerformance.Add("YTD -5%", "ta_perf_ytd5u");
            translationPerformance.Add("YTD Down", "ta_perf_ytddown");
            translationPerformance.Add("YTD Up", "ta_perf_ytdup");
            translationPerformance.Add("YTD +5%", "ta_perf_ytd5o");
            translationPerformance.Add("YTD +10%", "ta_perf_ytd10o");
            translationPerformance.Add("YTD +20%", "ta_perf_ytd20o");
            translationPerformance.Add("YTD +30%", "ta_perf_ytd30o");
            translationPerformance.Add("YTD +50%", "ta_perf_ytd50o");
            translationPerformance.Add("YTD +100%", "ta_perf_ytd100o");
            translationPerformance.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Performance.ToString(), translationPerformance);

            translation20SMA = new Dictionary<string, string>();
            translation20SMA.Add("Any", "");
            translation20SMA.Add("Price below SMA20", "ta_sma20_pb");
            translation20SMA.Add("Price 10% below SMA20", "ta_sma20_pb10");
            translation20SMA.Add("Price 20% below SMA20", "ta_sma20_pb20");
            translation20SMA.Add("Price 30% below SMA20", "ta_sma20_pb30");
            translation20SMA.Add("Price 40% below SMA20", "ta_sma20_pb40");
            translation20SMA.Add("Price 50% below SMA20", "ta_sma20_pb50");
            translation20SMA.Add("Price above SMA20", "ta_sma20_pa");
            translation20SMA.Add("Price 10% above SMA20", "ta_sma20_pa10");
            translation20SMA.Add("Price 20% above SMA20", "ta_sma20_pa20");
            translation20SMA.Add("Price 30% above SMA20", "ta_sma20_pa30");
            translation20SMA.Add("Price 40% above SMA20", "ta_sma20_pa40");
            translation20SMA.Add("Price 50% above SMA20", "ta_sma20_pa50");
            translation20SMA.Add("Price crossed SMA20", "ta_sma20_pc");
            translation20SMA.Add("Price crossed SMA20 above", "ta_sma20_pca");
            translation20SMA.Add("Price crossed SMA20 below", "ta_sma20_pcb");
            translation20SMA.Add("SMA20 crossed SMA50", "ta_sma20_cross50");
            translation20SMA.Add("SMA20 crossed SMA50 above", "ta_sma20_cross50a");
            translation20SMA.Add("SMA20 crossed SMA50 below", "ta_sma20_cross50b");
            translation20SMA.Add("SMA20 crossed SMA200", "ta_sma20_cross200");
            translation20SMA.Add("SMA20 crossed SMA200 above", "ta_sma20_cross200a");
            translation20SMA.Add("SMA20 crossed SMA200 below", "ta_sma20_cross200b");
            translation20SMA.Add("SMA20 above SMA50", "ta_sma20_sa50");
            translation20SMA.Add("SMA20 below SMA50", "ta_sma20_sb50");
            translation20SMA.Add("SMA20 above SMA200", "ta_sma20_sa200");
            translation20SMA.Add("SMA20 below SMA200", "ta_sma20_sb200");
            translation20SMA.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.MA20.ToString(), translation20SMA);

            translation20DayHighLow = new Dictionary<string, string>();
            translation20DayHighLow.Add("Any", "");
            translation20DayHighLow.Add("New High", "ta_highlow20d_nh");
            translation20DayHighLow.Add("New Low", "ta_highlow20d_nl");
            translation20DayHighLow.Add("5% or more below High", "ta_highlow20d_b5h");
            translation20DayHighLow.Add("10% or more below High", "ta_highlow20d_b10h");
            translation20DayHighLow.Add("15% or more below High", "ta_highlow20d_b15h");
            translation20DayHighLow.Add("20% or more below High", "ta_highlow20d_b20h");
            translation20DayHighLow.Add("30% or more below High", "ta_highlow20d_b30h");
            translation20DayHighLow.Add("40% or more below High", "ta_highlow20d_b40h");
            translation20DayHighLow.Add("50% or more below High", "ta_highlow20d_b50h");
            translation20DayHighLow.Add("0-3% below High", "ta_highlow20d_b0to3h");
            translation20DayHighLow.Add("0-5% below High", "ta_highlow20d_b0to5h");
            translation20DayHighLow.Add("0-10% below High", "ta_highlow20d_b0to10h");
            translation20DayHighLow.Add("5% or more above Low", "ta_highlow20d_a5h");
            translation20DayHighLow.Add("10% or more above Low", "ta_highlow20d_a10h");
            translation20DayHighLow.Add("15% or more above Low", "ta_highlow20d_a15h");
            translation20DayHighLow.Add("20% or more above Low", "ta_highlow20d_a20h");
            translation20DayHighLow.Add("30% or more above Low", "ta_highlow20d_a30h");
            translation20DayHighLow.Add("40% or more above Low", "ta_highlow20d_a40h");
            translation20DayHighLow.Add("50% or more above Low", "ta_highlow20d_a50h");
            translation20DayHighLow.Add("0-3% above Low", "ta_highlow20d_a0to3h");
            translation20DayHighLow.Add("0-5% above Low", "ta_highlow20d_a0to5h");
            translation20DayHighLow.Add("0-10% above Low", "ta_highlow20d_a0to10h");
            translation20DayHighLow.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.HighLow20Day.ToString(), translation20DayHighLow);

            translationBeta = new Dictionary<string, string>();
            translationBeta.Add("Any", "");
            translationBeta.Add("Under 0", "ta_beta_u0");
            translationBeta.Add("Under 0.5", "ta_beta_u0.5");
            translationBeta.Add("Under 1", "ta_beta_u1");
            translationBeta.Add("Under 1.5", "ta_beta_u1.5");
            translationBeta.Add("Under 2", "ta_beta_u2");
            translationBeta.Add("Over 0", "ta_beta_o0");
            translationBeta.Add("Over 0.5", "ta_beta_o0.5");
            translationBeta.Add("Over 1", "ta_beta_o1");
            translationBeta.Add("Over 1.5", "ta_beta_o1.5");
            translationBeta.Add("Over 2", "ta_beta_o2");
            translationBeta.Add("Over 2.5", "ta_beta_o2.5");
            translationBeta.Add("Over 3", "ta_beta_o3");
            translationBeta.Add("Over 4", "ta_beta_o4");
            translationBeta.Add("0 to 0.5", "ta_beta_0to0.5");
            translationBeta.Add("0 to 1", "ta_beta_0to1");
            translationBeta.Add("0.5 to 1", "ta_beta_0.5to1");
            translationBeta.Add("0.5 to 1.5", "ta_beta_0.5to1.5");
            translationBeta.Add("1 to 1.5", "ta_beta_1to1.5");
            translationBeta.Add("1 to 2", "ta_beta_1to2");
            translationBeta.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Beta.ToString(), translationBeta);

            translationPrice = new Dictionary<string, string>();
            translationPrice.Add("Any", "");
            translationPrice.Add("Under $1", "sh_price_u1");
            translationPrice.Add("Under $2", "sh_price_u2");
            translationPrice.Add("Under $3", "sh_price_u3");
            translationPrice.Add("Under $4", "sh_price_u4");
            translationPrice.Add("Under $5", "sh_price_u5");
            translationPrice.Add("Under $7", "sh_price_u7");
            translationPrice.Add("Under $10", "sh_price_u10");
            translationPrice.Add("Under $15", "sh_price_u15");
            translationPrice.Add("Under $20", "sh_price_u20");
            translationPrice.Add("Under $30", "sh_price_u30");
            translationPrice.Add("Under $40", "sh_price_u40");
            translationPrice.Add("Under $50", "sh_price_u50");
            translationPrice.Add("Over $1", "sh_price_o1");
            translationPrice.Add("Over $2", "sh_price_o2");
            translationPrice.Add("Over $3", "sh_price_o3");
            translationPrice.Add("Over $4", "sh_price_o4");
            translationPrice.Add("Over $5", "sh_price_o5");
            translationPrice.Add("Over $6", "sh_price_o6");
            translationPrice.Add("Over $7", "sh_price_o7");
            translationPrice.Add("Over $10", "sh_price_o10");
            translationPrice.Add("Over $15", "sh_price_o15");
            translationPrice.Add("Over $20", "sh_price_o20");
            translationPrice.Add("Over $30", "sh_price_o30");
            translationPrice.Add("Over $40", "sh_price_o40");
            translationPrice.Add("Over $50", "sh_price_o50");
            translationPrice.Add("Over $60", "sh_price_o60");
            translationPrice.Add("Over $70", "sh_price_o70");
            translationPrice.Add("Over $80", "sh_price_o80");
            translationPrice.Add("Over $90", "sh_price_o90");
            translationPrice.Add("Over $100", "sh_price_o100");
            translationPrice.Add("$1 to $5", "sh_price_1to5");
            translationPrice.Add("$1 to $10", "sh_price_1to10");
            translationPrice.Add("$1 to $20", "sh_price_1to20");
            translationPrice.Add("$5 to $10", "sh_price_5to10");
            translationPrice.Add("$5 to $20", "sh_price_5to20");
            translationPrice.Add("$5 to $50", "sh_price_5to50");
            translationPrice.Add("$10 to $20", "sh_price_10to20");
            translationPrice.Add("$10 to $50", "sh_price_10to50");
            translationPrice.Add("$20 to $50", "sh_price_20to50");
            translationPrice.Add("$50 to $100", "sh_price_50to100");
            translationsMaster.Add(FinvzEnumFilterType.Price.ToString(), translationPrice);

            translationAfterHoursClose = new Dictionary<string, string>();
            translationAfterHoursClose.Add("Any", "");
            translationAfterHoursClose.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.AfterHoursClose.ToString(), translationAfterHoursClose);


            // Column 2
            translationIndex = new Dictionary<string, string>();
            translationIndex.Add("Any", "");
            translationIndex.Add("S&P 500", "idx_sp500");
            translationIndex.Add("DJIA", "idx_dji");
            translationIndex.Add("RUSSELL 2000", "idx_rut");
            translationIndex.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Index.ToString(), translationIndex);

            translationPE = new Dictionary<string, string>();
            translationPE.Add("Any", "");
            translationPE.Add("Low (<15)", "fa_pe_low");
            translationPE.Add("Profitable (>0)", "fa_pe_profitable");
            translationPE.Add("High (>50)", "fa_pe_high");
            translationPE.Add("Under 5", "fa_pe_u5");
            translationPE.Add("Under 10", "fa_pe_u10");
            translationPE.Add("Under 15", "fa_pe_u15");
            translationPE.Add("Under 20", "fa_pe_u20");
            translationPE.Add("Under 25", "fa_pe_u25");
            translationPE.Add("Under 30", "fa_pe_u30");
            translationPE.Add("Under 35", "fa_pe_u35");
            translationPE.Add("Under 40", "fa_pe_u40");
            translationPE.Add("Under 45", "fa_pe_u45");
            translationPE.Add("Under 50", "fa_pe_u50");
            translationPE.Add("Over 5", "fa_pe_o5");
            translationPE.Add("Over 10", "fa_pe_o10");
            translationPE.Add("Over 15", "fa_pe_o15");
            translationPE.Add("Over 20", "fa_pe_o20");
            translationPE.Add("Over 25", "fa_pe_o25");
            translationPE.Add("Over 30", "fa_pe_o30");
            translationPE.Add("Over 35", "fa_pe_o35");
            translationPE.Add("Over 40", "fa_pe_o40");
            translationPE.Add("Over 45", "fa_pe_o45");
            translationPE.Add("Over 50", "fa_pe_o50");
            translationPE.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PE.ToString(), translationPE);

            translationPriceCash = new Dictionary<string, string>();
            translationPriceCash.Add("Any", "");
            translationPriceCash.Add("Low (<3)", "fa_pc_low");
            translationPriceCash.Add("High (>50)", "fa_pc_high");
            translationPriceCash.Add("Under 1", "fa_pc_u1");
            translationPriceCash.Add("Under 2", "fa_pc_u2");
            translationPriceCash.Add("Under 3", "fa_pc_u3");
            translationPriceCash.Add("Under 4", "fa_pc_u4");
            translationPriceCash.Add("Under 5", "fa_pc_u5");
            translationPriceCash.Add("Under 6", "fa_pc_u6");
            translationPriceCash.Add("Under 7", "fa_pc_u7");
            translationPriceCash.Add("Under 8", "fa_pc_u8");
            translationPriceCash.Add("Under 9", "fa_pc_u9");
            translationPriceCash.Add("Under 10", "fa_pc_u10");
            translationPriceCash.Add("Over 1", "fa_pc_o1");
            translationPriceCash.Add("Over 2", "fa_pc_o2");
            translationPriceCash.Add("Over 3", "fa_pc_o3");
            translationPriceCash.Add("Over 4", "fa_pc_o4");
            translationPriceCash.Add("Over 5", "fa_pc_o5");
            translationPriceCash.Add("Over 6", "fa_pc_o6");
            translationPriceCash.Add("Over 7", "fa_pc_o7");
            translationPriceCash.Add("Over 8", "fa_pc_o8");
            translationPriceCash.Add("Over 9", "fa_pc_o9");
            translationPriceCash.Add("Over 10", "fa_pc_o10");
            translationPriceCash.Add("Over 20", "fa_pc_o20");
            translationPriceCash.Add("Over 30", "fa_pc_o30");
            translationPriceCash.Add("Over 40", "fa_pc_o40");
            translationPriceCash.Add("Over 50", "fa_pc_o50");
            translationPriceCash.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PriceCash.ToString(), translationPriceCash);

            translationEPSGrowthNext5Years = new Dictionary<string, string>();
            translationEPSGrowthNext5Years.Add("Negative (<0%)", "fa_estltgrowth_neg");
            translationEPSGrowthNext5Years.Add("Positive (>0%)", "fa_estltgrowth_pos");
            translationEPSGrowthNext5Years.Add("Positive Low (0-10%)", "fa_estltgrowth_poslow");
            translationEPSGrowthNext5Years.Add("High (>25%)", "fa_estltgrowth_high");
            translationEPSGrowthNext5Years.Add("Under 5%", "fa_estltgrowth_u5");
            translationEPSGrowthNext5Years.Add("Under 10%", "fa_estltgrowth_u10");
            translationEPSGrowthNext5Years.Add("Under 15%", "fa_estltgrowth_u15");
            translationEPSGrowthNext5Years.Add("Under 20%", "fa_estltgrowth_u20");
            translationEPSGrowthNext5Years.Add("Under 25%", "fa_estltgrowth_u25");
            translationEPSGrowthNext5Years.Add("Under 30%", "fa_estltgrowth_u30");
            translationEPSGrowthNext5Years.Add("Over 5%", "fa_estltgrowth_o5");
            translationEPSGrowthNext5Years.Add("Over 10%", "fa_estltgrowth_o10");
            translationEPSGrowthNext5Years.Add("Over 15%", "fa_estltgrowth_o15");
            translationEPSGrowthNext5Years.Add("Over 20%", "fa_estltgrowth_o20");
            translationEPSGrowthNext5Years.Add("Over 25%", "fa_estltgrowth_o25");
            translationEPSGrowthNext5Years.Add("Over 30%", "fa_estltgrowth_o30");
            translationEPSGrowthNext5Years.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EpsGrowthNext5Years.ToString(), translationEPSGrowthNext5Years);

            translationReturnOnAssets = new Dictionary<string, string>();
            translationReturnOnAssets.Add("Positive (>0%)", "fa_roa_pos");
            translationReturnOnAssets.Add("Negative (<0%)", "fa_roa_neg");
            translationReturnOnAssets.Add("Very Positive (>150%)", "fa_roa_verypos");
            translationReturnOnAssets.Add("Very Negative (<15%)", "fa_roa_veryneg");
            translationReturnOnAssets.Add("Under -50%", "fa_roa_u-50");
            translationReturnOnAssets.Add("Under -45%", "fa_roa_u-45");
            translationReturnOnAssets.Add("Under -40%", "fa_roa_u-40");
            translationReturnOnAssets.Add("Under -35%", "fa_roa_u-35");
            translationReturnOnAssets.Add("Under -30%", "fa_roa_u-30");
            translationReturnOnAssets.Add("Under -25%", "fa_roa_u-25");
            translationReturnOnAssets.Add("Under -20%", "fa_roa_u-20");
            translationReturnOnAssets.Add("Under -15%", "fa_roa_u-15");
            translationReturnOnAssets.Add("Under -10%", "fa_roa_u-10");
            translationReturnOnAssets.Add("Under -5%", "fa_roa_u-5");
            translationReturnOnAssets.Add("Over +5%", "fa_roa_o5");
            translationReturnOnAssets.Add("Over +10%", "fa_roa_o10");
            translationReturnOnAssets.Add("Over +15%", "fa_roa_o15");
            translationReturnOnAssets.Add("Over +20%", "fa_roa_o20");
            translationReturnOnAssets.Add("Over +25%", "fa_roa_o25");
            translationReturnOnAssets.Add("Over +30%", "fa_roa_o30");
            translationReturnOnAssets.Add("Over +35%", "fa_roa_o35");
            translationReturnOnAssets.Add("Over +40%", "fa_roa_o40");
            translationReturnOnAssets.Add("Over +45%", "fa_roa_o45");
            translationReturnOnAssets.Add("Over +50%", "fa_roa_o50");
            translationReturnOnAssets.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.ReturnOnAssets.ToString(), translationReturnOnAssets);

            translationLTDebtEquity = new Dictionary<string, string>();
            translationLTDebtEquity.Add("Any", "");
            translationLTDebtEquity.Add("High (>0.5)", "fa_ltdebteq_high");
            translationLTDebtEquity.Add("Low (<0.1)", "fa_ltdebteq_low");
            translationLTDebtEquity.Add("Under 1", "fa_ltdebteq_u1");
            translationLTDebtEquity.Add("Under 0.9", "fa_ltdebteq_u0.9");
            translationLTDebtEquity.Add("Under 0.8", "fa_ltdebteq_u0.8");
            translationLTDebtEquity.Add("Under 0.7", "fa_ltdebteq_u0.7");
            translationLTDebtEquity.Add("Under 0.6", "fa_ltdebteq_u0.6");
            translationLTDebtEquity.Add("Under 0.5", "fa_ltdebteq_u0.5");
            translationLTDebtEquity.Add("Under 0.4", "fa_ltdebteq_u0.4");
            translationLTDebtEquity.Add("Under 0.3", "fa_ltdebteq_u0.3");
            translationLTDebtEquity.Add("Under 0.2", "fa_ltdebteq_u0.2");
            translationLTDebtEquity.Add("Under 0.1", "fa_ltdebteq_u0.1");
            translationLTDebtEquity.Add("Over 0.1", "fa_ltdebteq_o0.1");
            translationLTDebtEquity.Add("Over 0.2", "fa_ltdebteq_o0.2");
            translationLTDebtEquity.Add("Over 0.3", "fa_ltdebteq_o0.3");
            translationLTDebtEquity.Add("Over 0.4", "fa_ltdebteq_o0.4");
            translationLTDebtEquity.Add("Over 0.5", "fa_ltdebteq_o0.5");
            translationLTDebtEquity.Add("Over 0.6", "fa_ltdebteq_o0.6");
            translationLTDebtEquity.Add("Over 0.7", "fa_ltdebteq_o0.7");
            translationLTDebtEquity.Add("Over 0.8", "fa_ltdebteq_o0.8");
            translationLTDebtEquity.Add("Over 0.9", "fa_ltdebteq_o0.9");
            translationLTDebtEquity.Add("Over 1", "fa_ltdebteq_o1");
            translationLTDebtEquity.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.LTDebtEquity.ToString(), translationLTDebtEquity);

            translationPayoutRatio = new Dictionary<string, string>();
            translationPayoutRatio.Add("Any", "");
            translationPayoutRatio.Add("None (0%)", "fa_payoutratio_none");
            translationPayoutRatio.Add("Positive (>0%)", "fa_payoutratio_pos");
            translationPayoutRatio.Add("Low (<20%)", "fa_payoutratio_low");
            translationPayoutRatio.Add("High (>20%)", "fa_payoutratio_high");
            translationPayoutRatio.Add("Over 0%", "fa_payoutratio_o0");
            translationPayoutRatio.Add("Over 10%", "fa_payoutratio_o10");
            translationPayoutRatio.Add("Over 20%", "fa_payoutratio_o20");
            translationPayoutRatio.Add("Over 30%", "fa_payoutratio_o30");
            translationPayoutRatio.Add("Over 40%", "fa_payoutratio_o40");
            translationPayoutRatio.Add("Over 50%", "fa_payoutratio_o50");
            translationPayoutRatio.Add("Over 60%", "fa_payoutratio_o60");
            translationPayoutRatio.Add("Over 70%", "fa_payoutratio_o70");
            translationPayoutRatio.Add("Over 80%", "fa_payoutratio_o80");
            translationPayoutRatio.Add("Over 90%", "fa_payoutratio_o90");
            translationPayoutRatio.Add("Over 100%", "fa_payoutratio_o100");
            translationPayoutRatio.Add("Under 10%", "fa_payoutratio_u10");
            translationPayoutRatio.Add("Under 20%", "fa_payoutratio_u20");
            translationPayoutRatio.Add("Under 30%", "fa_payoutratio_u30");
            translationPayoutRatio.Add("Under 40%", "fa_payoutratio_u40");
            translationPayoutRatio.Add("Under 50%", "fa_payoutratio_u50");
            translationPayoutRatio.Add("Under 60%", "fa_payoutratio_u60");
            translationPayoutRatio.Add("Under 70%", "fa_payoutratio_u70");
            translationPayoutRatio.Add("Under 80%", "fa_payoutratio_u80");
            translationPayoutRatio.Add("Under 90%", "fa_payoutratio_u90");
            translationPayoutRatio.Add("Under 100%", "fa_payoutratio_u100");
            translationPayoutRatio.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PayoutRatio.ToString(), translationPayoutRatio);

            translationFloatShort = new Dictionary<string, string>();
            translationFloatShort.Add("Any", "");
            translationFloatShort.Add("Low (<5%)", "sh_short_low");
            translationFloatShort.Add("High (>20%)", "sh_short_high");
            translationFloatShort.Add("Under 5%", "sh_short_u5");
            translationFloatShort.Add("Under 10%", "sh_short_u10");
            translationFloatShort.Add("Under 15%", "sh_short_u15");
            translationFloatShort.Add("Under 20%", "sh_short_u20");
            translationFloatShort.Add("Under 25%", "sh_short_u25");
            translationFloatShort.Add("Under 30%", "sh_short_u30");
            translationFloatShort.Add("Over 5%", "sh_short_o5");
            translationFloatShort.Add("Over 10%", "sh_short_o10");
            translationFloatShort.Add("Over 15%", "sh_short_o15");
            translationFloatShort.Add("Over 20%", "sh_short_o20");
            translationFloatShort.Add("Over 25%", "sh_short_o25");
            translationFloatShort.Add("Over 30%", "sh_short_o30");
            translationFloatShort.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.FloatShort.ToString(), translationFloatShort);

            translationPerformance2 = new Dictionary<string, string>();
            translationPerformance2.Add("Any", "");
            translationPerformance2.Add("Today Up", "ta_perf_dup");
            translationPerformance2.Add("Today Down", "ta_perf_ddown");
            translationPerformance2.Add("Today -15%", "ta_perf_d15u");
            translationPerformance2.Add("Today -10%", "ta_perf_d10u");
            translationPerformance2.Add("Today -5%", "ta_perf_d5u");
            translationPerformance2.Add("Today +5%", "ta_perf_d5o");
            translationPerformance2.Add("Today +10%", "ta_perf_d10o");
            translationPerformance2.Add("Today +15%", "ta_perf_d15o");
            translationPerformance2.Add("Week -30%", "ta_perf_w30u");
            translationPerformance2.Add("Week -20%", "ta_perf_w20u");
            translationPerformance2.Add("Week -10%", "ta_perf_w10u");
            translationPerformance2.Add("Week Down", "ta_perf_1wdown");
            translationPerformance2.Add("Week Up", "ta_perf_1wup");
            translationPerformance2.Add("Week +10%", "ta_perf_1w10o");
            translationPerformance2.Add("Week +20%", "ta_perf_1w20o");
            translationPerformance2.Add("Week +30%", "ta_perf_1w30o");
            translationPerformance2.Add("Month -50%", "ta_perf_4w50u");
            translationPerformance2.Add("Month -30%", "ta_perf_4w30u");
            translationPerformance2.Add("Month -20%", "ta_perf_4w20u");
            translationPerformance2.Add("Month -10%", "ta_perf_4w10u");
            translationPerformance2.Add("Month Down", "ta_perf_4wdown");
            translationPerformance2.Add("Month Up", "ta_perf_4wup");
            translationPerformance2.Add("Month +10%", "ta_perf_4w10o");
            translationPerformance2.Add("Month +20%", "ta_perf_4w20o");
            translationPerformance2.Add("Month +30%", "ta_perf_4w30o");
            translationPerformance2.Add("Month +50%", "ta_perf_4w40o");
            translationPerformance2.Add("Quarter -50%", "ta_perf_13w50u");
            translationPerformance2.Add("Quarter -30%", "ta_perf_13w30u");
            translationPerformance2.Add("Quarter -20%", "ta_perf_13w20u");
            translationPerformance2.Add("Quarter -10%", "ta_perf_13w10u");
            translationPerformance2.Add("Quarter Down", "ta_perf_13wdown");
            translationPerformance2.Add("Quarter Up", "ta_perf_13wup");
            translationPerformance2.Add("Quarter +10%", "ta_perf_13w10o");
            translationPerformance2.Add("Quarter +20%", "ta_perf_13w20o");
            translationPerformance2.Add("Quarter +30%", "ta_perf_13w30o");
            translationPerformance2.Add("Quarter +50%", "ta_perf_13w50o");
            translationPerformance2.Add("Half -75%", "ta_perf_26w75u");
            translationPerformance2.Add("Half -50%", "ta_perf_26w50u");
            translationPerformance2.Add("Half -30%", "ta_perf_26w30u");
            translationPerformance2.Add("Half -20%", "ta_perf_26w20u");
            translationPerformance2.Add("Half -10%", "ta_perf_26w10u");
            translationPerformance2.Add("Half Down", "ta_perf_26wdown");
            translationPerformance2.Add("Half Up", "ta_perf_26wup");
            translationPerformance2.Add("Half +10%", "ta_perf_26w10o");
            translationPerformance2.Add("Half +20%", "ta_perf_26w20o");
            translationPerformance2.Add("Half +30%", "ta_perf_26w30o");
            translationPerformance2.Add("Half +50%", "ta_perf_26w50o");
            translationPerformance2.Add("Half +100%", "ta_perf_26w100o");
            translationPerformance2.Add("Year -75%", "ta_perf_52w75u");
            translationPerformance2.Add("Year -50%", "ta_perf_52w50u");
            translationPerformance2.Add("Year -30%", "ta_perf_52w30u");
            translationPerformance2.Add("Year -20%", "ta_perf_52w20u");
            translationPerformance2.Add("Year -10%", "ta_perf_52w10u");
            translationPerformance2.Add("Year Down", "ta_perf_52wdown");
            translationPerformance2.Add("Year Up", "ta_perf_52wup");
            translationPerformance2.Add("Year +10%", "ta_perf_52w10o");
            translationPerformance2.Add("Year +20%", "ta_perf_52w20o");
            translationPerformance2.Add("Year +30%", "ta_perf_52w30o");
            translationPerformance2.Add("Year +50%", "ta_perf_52w50o");
            translationPerformance2.Add("Year +100%", "ta_perf_52w100o");
            translationPerformance2.Add("Year +200%", "ta_perf_52w200o");
            translationPerformance2.Add("Year +300%", "ta_perf_52w300o");
            translationPerformance2.Add("Year +500%", "ta_perf_52w500o");
            translationPerformance2.Add("YTD -75%", "ta_perf_ytd75u");
            translationPerformance2.Add("YTD -50%", "ta_perf_ytd50u");
            translationPerformance2.Add("YTD -30%", "ta_perf_ytd30u");
            translationPerformance2.Add("YTD -20%", "ta_perf_ytd20u");
            translationPerformance2.Add("YTD -10%", "ta_perf_ytd10u");
            translationPerformance2.Add("YTD -5%", "ta_perf_ytd5u");
            translationPerformance2.Add("YTD Down", "ta_perf_ytddown");
            translationPerformance2.Add("YTD Up", "ta_perf_ytdup");
            translationPerformance2.Add("YTD +5%", "ta_perf_ytd5o");
            translationPerformance2.Add("YTD +10%", "ta_perf_ytd10o");
            translationPerformance2.Add("YTD +20%", "ta_perf_ytd20o");
            translationPerformance2.Add("YTD +30%", "ta_perf_ytd30o");
            translationPerformance2.Add("YTD +50%", "ta_perf_ytd50o");
            translationPerformance2.Add("YTD +100%", "ta_perf_ytd100o");
            translationPerformance2.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Performance2.ToString(), translationPerformance2);

            translation50SMA = new Dictionary<string, string>();
            translation50SMA.Add("Any", "");
            translation50SMA.Add("Price below SMA50", "ta_sma50_pb");
            translation50SMA.Add("Price 10% below SMA50", "ta_sma50_pb10");
            translation50SMA.Add("Price 20% below SMA50", "ta_sma50_pb20");
            translation50SMA.Add("Price 30% below SMA50", "ta_sma50_pb30");
            translation50SMA.Add("Price 40% below SMA50", "ta_sma50_pb40");
            translation50SMA.Add("Price 50% below SMA50", "ta_sma50_pb50");
            translation50SMA.Add("Price above SMA50", "ta_sma50_pa");
            translation50SMA.Add("Price 10% above SMA50", "ta_sma50_pa10");
            translation50SMA.Add("Price 20% above SMA50", "ta_sma50_pa20");
            translation50SMA.Add("Price 30% above SMA50", "ta_sma50_pa30");
            translation50SMA.Add("Price 40% above SMA50", "ta_sma50_pa40");
            translation50SMA.Add("Price 50% above SMA50", "ta_sma50_pa50");
            translation50SMA.Add("Price crossed SMA50", "ta_sma50_pc");
            translation50SMA.Add("Price crossed SMA50 above", "ta_sma50_pca");
            translation50SMA.Add("Price crossed SMA50 below", "ta_sma50_pcb");
            translation50SMA.Add("SMA50 crossed SMA20", "ta_sma50_cross20");
            translation50SMA.Add("SMA50 crossed SMA20 above", "ta_sma50_cross20a");
            translation50SMA.Add("SMA50 crossed SMA20 below", "ta_sma50_cross20b");
            translation50SMA.Add("SMA50 crossed SMA200", "ta_sma50_cross200");
            translation50SMA.Add("SMA50 crossed SMA200 above", "ta_sma50_cross200a");
            translation50SMA.Add("SMA50 crossed SMA200 below", "ta_sma50_cross200b");
            translation50SMA.Add("SMA50 above SMA20", "ta_sma50_sa20");
            translation50SMA.Add("SMA50 below SMA20", "ta_sma50_sb20");
            translation50SMA.Add("SMA50 above SMA200", "ta_sma50_sa200");
            translation50SMA.Add("SMA50 below SMA200", "ta_sma50_sb200");
            translation50SMA.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.MA50.ToString(), translation50SMA);

            translation50DayHighLow = new Dictionary<string, string>();
            translation50DayHighLow.Add("Any", "");
            translation50DayHighLow.Add("New High", "ta_highlow50d_nh");
            translation50DayHighLow.Add("New Low", "ta_highlow50d_nl");
            translation50DayHighLow.Add("5% or more below High", "ta_highlow50d_b5h");
            translation50DayHighLow.Add("10% or more below High", "ta_highlow50d_b10h");
            translation50DayHighLow.Add("15% or more below High", "ta_highlow50d_b15h");
            translation50DayHighLow.Add("20% or more below High", "ta_highlow50d_b20h");
            translation50DayHighLow.Add("30% or more below High", "ta_highlow50d_b30h");
            translation50DayHighLow.Add("40% or more below High", "ta_highlow50d_b40h");
            translation50DayHighLow.Add("50% or more below High", "ta_highlow50d_b50h");
            translation50DayHighLow.Add("0-3% below High", "ta_highlow50d_b0to3h");
            translation50DayHighLow.Add("0-5% below High", "ta_highlow50d_b0to5h");
            translation50DayHighLow.Add("0-10% below High", "ta_highlow50d_b0to10h");
            translation50DayHighLow.Add("5% or more above Low", "ta_highlow50d_a5h");
            translation50DayHighLow.Add("10% or more above Low", "ta_highlow50d_a10h");
            translation50DayHighLow.Add("15% or more above Low", "ta_highlow50d_a15h");
            translation50DayHighLow.Add("20% or more above Low", "ta_highlow50d_a20h");
            translation50DayHighLow.Add("30% or more above Low", "ta_highlow50d_a30h");
            translation50DayHighLow.Add("40% or more above Low", "ta_highlow50d_a40h");
            translation50DayHighLow.Add("50% or more above Low", "ta_highlow50d_a50h");
            translation50DayHighLow.Add("0-3% above Low", "ta_highlow50d_a0to3h");
            translation50DayHighLow.Add("0-5% above Low", "ta_highlow50d_a0to5h");
            translation50DayHighLow.Add("0-10% above Low", "ta_highlow50d_a0to10h");
            translation50DayHighLow.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.HighLow50Day.ToString(), translation50DayHighLow);

            translationAverageTrueRange = new Dictionary<string, string>();
            translationAverageTrueRange.Add("Any", "");
            translationAverageTrueRange.Add("Over 0.25", "ta_averagetruerange_o0.25");
            translationAverageTrueRange.Add("Over 0.5", "ta_averagetruerange_o0.5");
            translationAverageTrueRange.Add("Over 0.75", "ta_averagetruerange_o0.75");
            translationAverageTrueRange.Add("Over 1", "ta_averagetruerange_o1");
            translationAverageTrueRange.Add("Over 1.5", "ta_averagetruerange_o1.5");
            translationAverageTrueRange.Add("Over 2", "ta_averagetruerange_o2");
            translationAverageTrueRange.Add("Over 2.5", "ta_averagetruerange_o2.5");
            translationAverageTrueRange.Add("Over 3", "ta_averagetruerange_o3");
            translationAverageTrueRange.Add("Over 3.5", "ta_averagetruerange_o3.5");
            translationAverageTrueRange.Add("Over 4", "ta_averagetruerange_o4");
            translationAverageTrueRange.Add("Over 4.5", "ta_averagetruerange_o4.5");
            translationAverageTrueRange.Add("Over 5", "ta_averagetruerange_o5");
            translationAverageTrueRange.Add("Under 0.25", "ta_averagetruerange_u0.25");
            translationAverageTrueRange.Add("Under 0.5", "ta_averagetruerange_u0.5");
            translationAverageTrueRange.Add("Under 0.75", "ta_averagetruerange_u0.75");
            translationAverageTrueRange.Add("Under 1", "ta_averagetruerange_u1");
            translationAverageTrueRange.Add("Under 1.5", "ta_averagetruerange_u1.5");
            translationAverageTrueRange.Add("Under 2", "ta_averagetruerange_u2");
            translationAverageTrueRange.Add("Under 2.5", "ta_averagetruerange_u2.5");
            translationAverageTrueRange.Add("Under 3", "ta_averagetruerange_u3");
            translationAverageTrueRange.Add("Under 3.5", "ta_averagetruerange_u3.5");
            translationAverageTrueRange.Add("Under 4", "ta_averagetruerange_u4");
            translationAverageTrueRange.Add("Under 4.5", "ta_averagetruerange_u4.5");
            translationAverageTrueRange.Add("Under 5", "ta_averagetruerange_u5");
            translationAverageTrueRange.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.AverageTrueRange.ToString(), translationAverageTrueRange);

            translationTargetPrice = new Dictionary<string, string>();
            translationTargetPrice.Add("Any", "");
            translationTargetPrice.Add("50% Above Price", "targetprice_a50");
            translationTargetPrice.Add("40% Above Price", "targetprice_a40");
            translationTargetPrice.Add("30% Above Price", "targetprice_a30");
            translationTargetPrice.Add("20% Above Price", "targetprice_a20");
            translationTargetPrice.Add("10% Above Price", "targetprice_a10");
            translationTargetPrice.Add("5% Above Price", "targetprice_a5");
            translationTargetPrice.Add("Above Price", "targetprice_above");
            translationTargetPrice.Add("Below Price", "targetprice_below");
            translationTargetPrice.Add("5% Below Price", "targetprice_b5");
            translationTargetPrice.Add("10% Below Price", "targetprice_b10");
            translationTargetPrice.Add("20% Below Price", "targetprice_b20");
            translationTargetPrice.Add("30% Below Price", "targetprice_b30");
            translationTargetPrice.Add("40% Below Price", "targetprice_b40");
            translationTargetPrice.Add("50% Below Price", "targetprice_b50");
            translationTargetPrice.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.TargetPrice.ToString(), translationTargetPrice);

            translationAfterHoursChange = new Dictionary<string, string>();
            translationAfterHoursChange.Add("Any", "");
            translationAfterHoursChange.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.AfterHoursChange.ToString(), translationAfterHoursChange);


            // Column 3
            translationSector = new Dictionary<string, string>();
            translationSector.Add("Any", "");
            translationSector.Add("Basic Materials", "sec_basicmaterials");
            translationSector.Add("Communication Services", "sec_communicationservices");
            translationSector.Add("Consumer Cyclical", "sec_consumercyclical");
            translationSector.Add("Consumer Defensive", "sec_consumerdefensive");
            translationSector.Add("Energy", "sec_energy");
            translationSector.Add("Financial", "sec_financial");
            translationSector.Add("Healthcare", "sec_healthcare");
            translationSector.Add("Industrials", "sec_industrials");
            translationSector.Add("Real Estate", "sec_realestate");
            translationSector.Add("Technology", "sec_technology");
            translationSector.Add("Utilities", "sec_utilities");
            translationSector.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Sector.ToString(), translationSector);

            translationForwardPE = new Dictionary<string, string>();
            translationForwardPE.Add("Any", "");
            translationForwardPE.Add("Low (<15)", "fa_fpe_low");
            translationForwardPE.Add("Profitable (>0)", "fa_fpe_profitable");
            translationForwardPE.Add("High (>50)", "fa_fpe_high");
            translationForwardPE.Add("Under 5", "fa_fpe_u5");
            translationForwardPE.Add("Under 10", "fa_fpe_u10");
            translationForwardPE.Add("Under 15", "fa_fpe_u15");
            translationForwardPE.Add("Under 20", "fa_fpe_u20");
            translationForwardPE.Add("Under 25", "fa_fpe_u25");
            translationForwardPE.Add("Under 30", "fa_fpe_u30");
            translationForwardPE.Add("Under 35", "fa_fpe_u35");
            translationForwardPE.Add("Under 40", "fa_fpe_u40");
            translationForwardPE.Add("Under 45", "fa_fpe_u45");
            translationForwardPE.Add("Under 50", "fa_fpe_u50");
            translationForwardPE.Add("Over 5", "fa_fpe_o5");
            translationForwardPE.Add("Over 10", "fa_fpe_o10");
            translationForwardPE.Add("Over 15", "fa_fpe_o15");
            translationForwardPE.Add("Over 20", "fa_fpe_o20");
            translationForwardPE.Add("Over 25", "fa_fpe_o25");
            translationForwardPE.Add("Over 30", "fa_fpe_o30");
            translationForwardPE.Add("Over 35", "fa_fpe_o35");
            translationForwardPE.Add("Over 40", "fa_fpe_o40");
            translationForwardPE.Add("Over 45", "fa_fpe_o45");
            translationForwardPE.Add("Over 50", "fa_fpe_o50");
            translationForwardPE.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.ForwardPE.ToString(), translationForwardPE);

            translationPriceFreeCashFlow = new Dictionary<string, string>();
            translationPriceFreeCashFlow.Add("Any", "");
            translationPriceFreeCashFlow.Add("Low (<15)", "fa_pfcf_low");
            translationPriceFreeCashFlow.Add("High (>50)", "fa_pfcf_high");
            translationPriceFreeCashFlow.Add("Under 5", "fa_pfcf_u5");
            translationPriceFreeCashFlow.Add("Under 10", "fa_pfcf_u10");
            translationPriceFreeCashFlow.Add("Under 15", "fa_pfcf_u15");
            translationPriceFreeCashFlow.Add("Under 20", "fa_pfcf_u20");
            translationPriceFreeCashFlow.Add("Under 25", "fa_pfcf_u25");
            translationPriceFreeCashFlow.Add("Under 30", "fa_pfcf_u30");
            translationPriceFreeCashFlow.Add("Under 35", "fa_pfcf_u35");
            translationPriceFreeCashFlow.Add("Under 40", "fa_pfcf_u40");
            translationPriceFreeCashFlow.Add("Under 45", "fa_pfcf_u45");
            translationPriceFreeCashFlow.Add("Under 50", "fa_pfcf_u50");
            translationPriceFreeCashFlow.Add("Under 60", "fa_pfcf_u60");
            translationPriceFreeCashFlow.Add("Under 70", "fa_pfcf_u70");
            translationPriceFreeCashFlow.Add("Under 80", "fa_pfcf_u80");
            translationPriceFreeCashFlow.Add("Under 90", "fa_pfcf_u90");
            translationPriceFreeCashFlow.Add("Under 100", "fa_pfcf_u100");
            translationPriceFreeCashFlow.Add("Over 5", "fa_pfcf_o5");
            translationPriceFreeCashFlow.Add("Over 10", "fa_pfcf_o10");
            translationPriceFreeCashFlow.Add("Over 15", "fa_pfcf_o15");
            translationPriceFreeCashFlow.Add("Over 20", "fa_pfcf_o20");
            translationPriceFreeCashFlow.Add("Over 25", "fa_pfcf_o25");
            translationPriceFreeCashFlow.Add("Over 30", "fa_pfcf_o30");
            translationPriceFreeCashFlow.Add("Over 35", "fa_pfcf_o35");
            translationPriceFreeCashFlow.Add("Over 40", "fa_pfcf_o40");
            translationPriceFreeCashFlow.Add("Over 45", "fa_pfcf_o45");
            translationPriceFreeCashFlow.Add("Over 50", "fa_pfcf_o60");
            translationPriceFreeCashFlow.Add("Over 60", "fa_pfcf_o70");
            translationPriceFreeCashFlow.Add("Over 70", "fa_pfcf_o80");
            translationPriceFreeCashFlow.Add("Over 80", "fa_pfcf_o90");
            translationPriceFreeCashFlow.Add("Over 90", "fa_pfcf_o100");
            translationPriceFreeCashFlow.Add("Over 100", "fa_pfcf_o50");
            translationPriceFreeCashFlow.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PriceFreeCashFlow.ToString(), translationPriceFreeCashFlow);

            translationSalesGrowthPast5Years = new Dictionary<string, string>();
            translationSalesGrowthPast5Years.Add("Any", "");
            translationSalesGrowthPast5Years.Add("Negative (<0%)", "fa_sales5years_neg");
            translationSalesGrowthPast5Years.Add("Positive (>0%)", "fa_sales5years_pos");
            translationSalesGrowthPast5Years.Add("Positive Low (0-10%)", "fa_sales5years_poslow");
            translationSalesGrowthPast5Years.Add("High (>25%)", "fa_sales5years_high");
            translationSalesGrowthPast5Years.Add("Under 5%", "fa_sales5years_u5");
            translationSalesGrowthPast5Years.Add("Under 10%", "fa_sales5years_u10");
            translationSalesGrowthPast5Years.Add("Under 15%", "fa_sales5years_u15");
            translationSalesGrowthPast5Years.Add("Under 20%", "fa_sales5years_u20");
            translationSalesGrowthPast5Years.Add("Under 25%", "fa_sales5years_u25");
            translationSalesGrowthPast5Years.Add("Under 30%", "fa_sales5years_u30");
            translationSalesGrowthPast5Years.Add("Over 5%", "fa_sales5years_o5");
            translationSalesGrowthPast5Years.Add("Over 10%", "fa_sales5years_o10");
            translationSalesGrowthPast5Years.Add("Over 15%", "fa_sales5years_o15");
            translationSalesGrowthPast5Years.Add("Over 20%", "fa_sales5years_o20");
            translationSalesGrowthPast5Years.Add("Over 25%", "fa_sales5years_o25");
            translationSalesGrowthPast5Years.Add("Over 30%", "fa_sales5years_o30");
            translationSalesGrowthPast5Years.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.SalesGrowthPast5Years.ToString(), translationSalesGrowthPast5Years);

            translationReturnOnEquity = new Dictionary<string, string>();
            translationReturnOnEquity.Add("Any", "");
            translationReturnOnEquity.Add("Positive (>0%)", "fa_roe_pos");
            translationReturnOnEquity.Add("Negative (<0%)", "fa_roe_neg");
            translationReturnOnEquity.Add("Under -50%", "fa_roe_u-50");
            translationReturnOnEquity.Add("Under -45%", "fa_roe_u-45");
            translationReturnOnEquity.Add("Under -40%", "fa_roe_u-40");
            translationReturnOnEquity.Add("Under -35%", "fa_roe_u-35");
            translationReturnOnEquity.Add("Under -30%", "fa_roe_u-30");
            translationReturnOnEquity.Add("Under -25%", "fa_roe_u-25");
            translationReturnOnEquity.Add("Under -20%", "fa_roe_u-20");
            translationReturnOnEquity.Add("Under -15%", "fa_roe_u-15");
            translationReturnOnEquity.Add("Under -10%", "fa_roe_u-10");
            translationReturnOnEquity.Add("Under -5%", "fa_roe_u-5");
            translationReturnOnEquity.Add("Over +5%", "fa_roe_o5");
            translationReturnOnEquity.Add("Over +10%", "fa_roe_o10");
            translationReturnOnEquity.Add("Over +15%", "fa_roe_o15");
            translationReturnOnEquity.Add("Over +20%", "fa_roe_o20");
            translationReturnOnEquity.Add("Over +25%", "fa_roe_o25");
            translationReturnOnEquity.Add("Over +30%", "fa_roe_o30");
            translationReturnOnEquity.Add("Over +35%", "fa_roe_o35");
            translationReturnOnEquity.Add("Over +40%", "fa_roe_o40");
            translationReturnOnEquity.Add("Over +45%", "fa_roe_o45");
            translationReturnOnEquity.Add("Over +50%", "fa_roe_o50");
            translationReturnOnEquity.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.ReturnOnEquity.ToString(), translationReturnOnEquity);

            translationDebtEquity = new Dictionary<string, string>();
            translationDebtEquity.Add("Any", "");
            translationDebtEquity.Add("High (>0.5)", "fa_debteq_high");
            translationDebtEquity.Add("Low (<0.1)", "fa_debteq_low");
            translationDebtEquity.Add("Under 1", "fa_debteq_u1");
            translationDebtEquity.Add("Under 0.9", "fa_debteq_u0.9");
            translationDebtEquity.Add("Under 0.8", "fa_debteq_u0.8");
            translationDebtEquity.Add("Under 0.7", "fa_debteq_u0.7");
            translationDebtEquity.Add("Under 0.6", "fa_debteq_u0.6");
            translationDebtEquity.Add("Under 0.5", "fa_debteq_u0.5");
            translationDebtEquity.Add("Under 0.4", "fa_debteq_u0.4");
            translationDebtEquity.Add("Under 0.3", "fa_debteq_u0.3");
            translationDebtEquity.Add("Under 0.2", "fa_debteq_u0.2");
            translationDebtEquity.Add("Under 0.1", "fa_debteq_u0.1");
            translationDebtEquity.Add("Over 0.1", "fa_debteq_o0.1");
            translationDebtEquity.Add("Over 0.2", "fa_debteq_o0.2");
            translationDebtEquity.Add("Over 0.3", "fa_debteq_o0.3");
            translationDebtEquity.Add("Over 0.4", "fa_debteq_o0.4");
            translationDebtEquity.Add("Over 0.5", "fa_debteq_o0.5");
            translationDebtEquity.Add("Over 0.6", "fa_debteq_o0.6");
            translationDebtEquity.Add("Over 0.7", "fa_debteq_o0.7");
            translationDebtEquity.Add("Over 0.8", "fa_debteq_o0.8");
            translationDebtEquity.Add("Over 0.9", "fa_debteq_o0.9");
            translationDebtEquity.Add("Over 1", "fa_debteq_o1");
            translationDebtEquity.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.DebtEquity.ToString(), translationDebtEquity);

            translationInsiderOwnership = new Dictionary<string, string>();
            translationInsiderOwnership.Add("Any", "");
            translationInsiderOwnership.Add("Low (<5%)", "sh_insiderown_low");
            translationInsiderOwnership.Add("High (>30%)", "sh_insiderown_high");
            translationInsiderOwnership.Add("Very High (>50%)", "sh_insiderown_veryhigh");
            translationInsiderOwnership.Add("Over 10%", "sh_insiderown_o10");
            translationInsiderOwnership.Add("Over 20%", "sh_insiderown_o20");
            translationInsiderOwnership.Add("Over 30%", "sh_insiderown_o30");
            translationInsiderOwnership.Add("Over 40%", "sh_insiderown_o40");
            translationInsiderOwnership.Add("Over 50%", "sh_insiderown_o50");
            translationInsiderOwnership.Add("Over 60%", "sh_insiderown_o60");
            translationInsiderOwnership.Add("Over 70%", "sh_insiderown_o70");
            translationInsiderOwnership.Add("Over 80%", "sh_insiderown_o80");
            translationInsiderOwnership.Add("Over 90%", "sh_insiderown_o90");
            translationInsiderOwnership.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.InsiderOwnership.ToString(), translationInsiderOwnership);

            translationAnalystRecomends = new Dictionary<string, string>();
            translationAnalystRecomends.Add("Any", "");
            translationAnalystRecomends.Add("Strong Buy (1)", "an_recom_strongbuy");
            translationAnalystRecomends.Add("Buy or better", "an_recom_buybetter");
            translationAnalystRecomends.Add("Buy", "an_recom_buy");
            translationAnalystRecomends.Add("Hold or better", "an_recom_holdbetter");
            translationAnalystRecomends.Add("Hold", "an_recom_hold");
            translationAnalystRecomends.Add("Hold or worse", "an_recom_holdworse");
            translationAnalystRecomends.Add("Sell", "an_recom_sell");
            translationAnalystRecomends.Add("Sell or worse", "an_recom_sellworse");
            translationAnalystRecomends.Add("Strong Sell (5)", "an_recom_strongsell");
            translationAnalystRecomends.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.AnalystRecom.ToString(), translationAnalystRecomends);

            translationVolatility = new Dictionary<string, string>();
            translationVolatility.Add("Any", "");
            translationVolatility.Add("Week - Over 3%", "ta_volatility_wo3");
            translationVolatility.Add("Week - Over 4%", "ta_volatility_wo4");
            translationVolatility.Add("Week - Over 5%", "ta_volatility_wo5");
            translationVolatility.Add("Week - Over 6%", "ta_volatility_wo6");
            translationVolatility.Add("Week - Over 7%", "ta_volatility_wo7");
            translationVolatility.Add("Week - Over 8%", "ta_volatility_wo8");
            translationVolatility.Add("Week - Over 9%", "ta_volatility_wo9");
            translationVolatility.Add("Week - Over 10%", "ta_volatility_wo10");
            translationVolatility.Add("Week - Over 12%", "ta_volatility_wo12");
            translationVolatility.Add("Week - Over 15%", "ta_volatility_wo15");
            translationVolatility.Add("Month - Over 2%", "ta_volatility_mo2");
            translationVolatility.Add("Month - Over 3%", "ta_volatility_mo3");
            translationVolatility.Add("Month - Over 4%", "ta_volatility_mo4");
            translationVolatility.Add("Month - Over 5%", "ta_volatility_mo5");
            translationVolatility.Add("Month - Over 6%", "ta_volatility_mo6");
            translationVolatility.Add("Month - Over 7%", "ta_volatility_mo7");
            translationVolatility.Add("Month - Over 8%", "ta_volatility_mo8");
            translationVolatility.Add("Month - Over 9%", "ta_volatility_mo9");
            translationVolatility.Add("Month - Over 10%", "ta_volatility_mo10");
            translationVolatility.Add("Month - Over 12%", "ta_volatility_mo12");
            translationVolatility.Add("Month - Over 15%", "ta_volatility_mo15");
            translationVolatility.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Volatility.ToString(), translationVolatility);
            
            translation200SMA = new Dictionary<string, string>();
            translation200SMA.Add("Any", "");
            translation200SMA.Add("Price below SMA200", "ta_sma200_pb");
            translation200SMA.Add("Price 10% below SMA200", "ta_sma200_pb10");
            translation200SMA.Add("Price 20% below SMA200", "ta_sma200_pb20");
            translation200SMA.Add("Price 30% below SMA200", "ta_sma200_pb30");
            translation200SMA.Add("Price 40% below SMA200", "ta_sma200_pb40");
            translation200SMA.Add("Price 50% below SMA200", "ta_sma200_pb50");
            translation200SMA.Add("Price 60% below SMA200", "ta_sma200_pb60");
            translation200SMA.Add("Price 70% below SMA200", "ta_sma200_pb70");
            translation200SMA.Add("Price 80% below SMA200", "ta_sma200_pb80");
            translation200SMA.Add("Price 90% below SMA200", "ta_sma200_pb90");
            translation200SMA.Add("Price above SMA200", "ta_sma200_pa");
            translation200SMA.Add("Price 10% above SMA200", "ta_sma200_pa10");
            translation200SMA.Add("Price 20% above SMA200", "ta_sma200_pa20");
            translation200SMA.Add("Price 30% above SMA200", "ta_sma200_pa30");
            translation200SMA.Add("Price 40% above SMA200", "ta_sma200_pa40");
            translation200SMA.Add("Price 50% above SMA200", "ta_sma200_pa50");
            translation200SMA.Add("Price 60% above SMA200", "ta_sma200_pa60");
            translation200SMA.Add("Price 70% above SMA200", "ta_sma200_pa70");
            translation200SMA.Add("Price 80% above SMA200", "ta_sma200_pa80");
            translation200SMA.Add("Price 90% above SMA200", "ta_sma200_pa90");
            translation200SMA.Add("Price 100% above SMA200", "ta_sma200_pa100");
            translation200SMA.Add("Price crossed SMA200", "ta_sma200_pc");
            translation200SMA.Add("Price crossed SMA200 above", "ta_sma200_pca");
            translation200SMA.Add("Price crossed SMA200 below", "ta_sma200_pcb");
            translation200SMA.Add("SMA200 crossed SMA20", "ta_sma200_cross20");
            translation200SMA.Add("SMA200 crossed SMA20 above", "ta_sma200_cross20a");
            translation200SMA.Add("SMA200 crossed SMA20 below", "ta_sma200_cross20b");
            translation200SMA.Add("SMA200 crossed SMA50", "ta_sma200_cross50");
            translation200SMA.Add("SMA200 crossed SMA50 above", "ta_sma200_cross50a");
            translation200SMA.Add("SMA200 crossed SMA50 below", "ta_sma200_cross50b");
            translation200SMA.Add("SMA200 above SMA20", "ta_sma200_sa20");
            translation200SMA.Add("SMA200 below SMA20", "ta_sma200_sb20");
            translation200SMA.Add("SMA200 above SMA50", "ta_sma200_sa50");
            translation200SMA.Add("SMA200 below SMA50", "ta_sma200_sb50");
            translation200SMA.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.MA200.ToString(), translation200SMA);

            translation52WeekHighLow = new Dictionary<string, string>();
            translation52WeekHighLow.Add("Any", "");
            translation52WeekHighLow.Add("New High", "ta_highlow52w_nh");
            translation52WeekHighLow.Add("New Low", "ta_highlow52w_nl");
            translation52WeekHighLow.Add("5% or more below High", "ta_highlow52w_b5h");
            translation52WeekHighLow.Add("10% or more below High", "ta_highlow52w_b10h");
            translation52WeekHighLow.Add("15% or more below High", "ta_highlow52w_b15h");
            translation52WeekHighLow.Add("20% or more below High", "ta_highlow52w_b205h");
            translation52WeekHighLow.Add("30% or more below High", "ta_highlow52w_b30h");
            translation52WeekHighLow.Add("40% or more below High", "ta_highlow52w_b40h");
            translation52WeekHighLow.Add("50% or more below High", "ta_highlow52w_b50h");
            translation52WeekHighLow.Add("60% or more below High", "ta_highlow52w_b60h");
            translation52WeekHighLow.Add("70% or more below High", "ta_highlow52w_b70h");
            translation52WeekHighLow.Add("80% or more below High", "ta_highlow52w_b80h");
            translation52WeekHighLow.Add("0-3% below High", "ta_highlow52w_b0to3h");
            translation52WeekHighLow.Add("0-5% below High", "ta_highlow52w_b0to5h");
            translation52WeekHighLow.Add("0-10% below High", "ta_highlow52w_b0to10h");
            translation52WeekHighLow.Add("5% or more above Low", "ta_highlow52w_a5h");
            translation52WeekHighLow.Add("10% or more above Low", "ta_highlow52w_a10h");
            translation52WeekHighLow.Add("15% or more above Low", "ta_highlow52w_a15h");
            translation52WeekHighLow.Add("20% or more above Low", "ta_highlow52w_a205h");
            translation52WeekHighLow.Add("30% or more above Low", "ta_highlow52w_a30h");
            translation52WeekHighLow.Add("40% or more above Low", "ta_highlow52w_a40h");
            translation52WeekHighLow.Add("50% or more above Low", "ta_highlow52w_a50h");
            translation52WeekHighLow.Add("60% or more above Low", "ta_highlow52w_a60h");
            translation52WeekHighLow.Add("70% or more above Low", "ta_highlow52w_a70h");
            translation52WeekHighLow.Add("80% or more above Low", "ta_highlow52w_a80h");
            translation52WeekHighLow.Add("90% or more above Low", "ta_highlow52w_a90h");
            translation52WeekHighLow.Add("100% or more above Low", "ta_highlow52w_a100h");
            translation52WeekHighLow.Add("120% or more above Low", "ta_highlow52w_a120h");
            translation52WeekHighLow.Add("150% or more above Low", "ta_highlow52w_a150h");
            translation52WeekHighLow.Add("200% or more above Low", "ta_highlow52w_a200h");
            translation52WeekHighLow.Add("300% or more above Low", "ta_highlow52w_a300h");
            translation52WeekHighLow.Add("500% or more above Low", "ta_highlow52w_a500h");
            translation52WeekHighLow.Add("0-3% above Low", "ta_highlow52w_a0to3h");
            translation52WeekHighLow.Add("0-5% above Low", "ta_highlow52w_a0to5h");
            translation52WeekHighLow.Add("0-10% above Low", "ta_highlow52w_a0to10h");
            translation52WeekHighLow.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.HighLow52Week.ToString(), translation52WeekHighLow);

            translationAverageVolume = new Dictionary<string, string>();
            translationAverageVolume.Add("Any", "");
            translationAverageVolume.Add("Under 50K", "sh_avgvol_u50");
            translationAverageVolume.Add("Under 100K", "sh_avgvol_u100");
            translationAverageVolume.Add("Under 500K", "sh_avgvol_u500");
            translationAverageVolume.Add("Under 750K", "sh_avgvol_u750");
            translationAverageVolume.Add("Under 1M", "sh_avgvol_u1000");
            translationAverageVolume.Add("Over 50K", "sh_avgvol_o50");
            translationAverageVolume.Add("Over 100K", "sh_avgvol_o100");
            translationAverageVolume.Add("Over 200K", "sh_avgvol_o200");
            translationAverageVolume.Add("Over 300K", "sh_avgvol_o300");
            translationAverageVolume.Add("Over 400K", "sh_avgvol_o400");
            translationAverageVolume.Add("Over 500K", "sh_avgvol_o500");
            translationAverageVolume.Add("Over 750K", "sh_avgvol_o750");
            translationAverageVolume.Add("Over 1M", "sh_avgvol_o1000");
            translationAverageVolume.Add("Over 2M", "sh_avgvol_o2000");
            translationAverageVolume.Add("100K to 500K", "sh_avgvol_100to500");
            translationAverageVolume.Add("100K to 1M", "sh_avgvol_100to1000");
            translationAverageVolume.Add("500K to 1M", "sh_avgvol_500to1000");
            translationAverageVolume.Add("500K to 10M", "sh_avgvol_500to10000");
            translationAverageVolume.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.AverageVolume.ToString(), translationAverageVolume);

            translationIPODate = new Dictionary<string, string>();
            translationIPODate.Add("Any", "");
            translationIPODate.Add("Today", "ipodate_today");
            translationIPODate.Add("Yesterday", "ipodate_yesterday");
            translationIPODate.Add("In the last week", "ipodate_prevweek");
            translationIPODate.Add("In the last month", "ipodate_prevmonth");
            translationIPODate.Add("In the last quarter", "ipodate_prevquarter");
            translationIPODate.Add("In the last year", "ipodate_prevyear");
            translationIPODate.Add("In the last 2 years", "ipodate_prev2yrs");
            translationIPODate.Add("In the last 3 years", "ipodate_prev3yrs");
            translationIPODate.Add("In the last 5 years", "ipodate_prev5yrs");
            translationIPODate.Add("More than a year ago", "ipodate_more1");
            translationIPODate.Add("More than 5 years ago", "ipodate_more5");
            translationIPODate.Add("More than 10 years ago", "ipodate_more10");
            translationIPODate.Add("More than 15 years ago", "ipodate_more15");
            translationIPODate.Add("More than 20 years ago", "ipodate_more20");
            translationIPODate.Add("More than 25 years ago", "ipodate_more25");
            translationIPODate.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.IPODate.ToString(), translationIPODate);


            // Column 4
            translationIndustry = new Dictionary<string, string>();
            translationIndustry.Add("Any", "");
            translationIndustry.Add("Stocks only (ex-Funds)", "ind_stocksonly");
            translationIndustry.Add("Advertising Agencies", "ind_advertisingagencies");
            translationIndustry.Add("Aerospace &amp; Defense", "ind_aerospacedefense");
            translationIndustry.Add("Agricultural Inputs", "ind_agriculturalinputs");
            translationIndustry.Add("Airlines", "ind_airlines");
            translationIndustry.Add("Airports &amp; Air Services", "ind_airportsairservices");
            translationIndustry.Add("Aluminum", "ind_aluminum");
            translationIndustry.Add("Apparel Manufacturing", "ind_apparelmanufacturing");
            translationIndustry.Add("Apparel Retail", "ind_apparelretail");
            translationIndustry.Add("Asset Management", "ind_assetmanagement");
            translationIndustry.Add("Auto Manufacturers", "ind_automanufacturers");
            translationIndustry.Add("Auto Parts", "ind_autoparts");
            translationIndustry.Add("Auto &amp; Truck Dealerships", "ind_autotruckdealerships");
            translationIndustry.Add("Banks - Diversified", "ind_banksdiversified");
            translationIndustry.Add("Banks - Regional", "ind_banksregional");
            translationIndustry.Add("Beverages - Brewers", "ind_beveragesbrewers");
            translationIndustry.Add("Beverages - Non-Alcoholic", "ind_beveragesnonalcoholic");
            translationIndustry.Add("Beverages - Wineries &amp; Distilleries", "ind_beverageswineriesdistilleries");
            translationIndustry.Add("Biotechnology", "ind_biotechnology");
            translationIndustry.Add("Broadcasting", "ind_broadcasting");
            translationIndustry.Add("Building Materials", "ind_buildingmaterials");
            translationIndustry.Add("Building Products &amp; Equipment", "ind_buildingproductsequipment");
            translationIndustry.Add("Business Equipment &amp; Supplies", "ind_businessequipmentsupplies");
            translationIndustry.Add("Capital Markets", "ind_capitalmarkets");
            translationIndustry.Add("Chemicals", "ind_chemicals");
            translationIndustry.Add("Closed-End Fund - Debt", "ind_closedendfunddebt");
            translationIndustry.Add("Closed-End Fund - Equity", "ind_closedendfundequity");
            translationIndustry.Add("Closed-End Fund - Foreign", "ind_closedendfundforeign");
            translationIndustry.Add("Coking Coal", "ind_cokingcoal");
            translationIndustry.Add("Communication Equipment", "ind_communicationequipment");
            translationIndustry.Add("Computer Hardware", "ind_computerhardware");
            translationIndustry.Add("Confectioners", "ind_confectioners");
            translationIndustry.Add("Conglomerates", "ind_conglomerates");
            translationIndustry.Add("Consulting Services", "ind_consultingservices");
            translationIndustry.Add("Consumer Electronics", "ind_consumerelectronics");
            translationIndustry.Add("Copper", "ind_copper");
            translationIndustry.Add("Credit Services", "ind_creditservices");
            translationIndustry.Add("Department Stores", "ind_departmentstores");
            translationIndustry.Add("Diagnostics &amp; Research", "ind_diagnosticsresearch");
            translationIndustry.Add("Discount Stores", "ind_discountstores");
            translationIndustry.Add("Drug Manufacturers - General", "ind_drugmanufacturersgeneral");
            translationIndustry.Add("Drug Manufacturers - Specialty &amp; Generic", "ind_drugmanufacturersspecialtygeneric");
            translationIndustry.Add("Education &amp; Training Services", "ind_educationtrainingservices");
            translationIndustry.Add("Electrical Equipment &amp; Parts", "ind_electricalequipmentparts");
            translationIndustry.Add("Electronic Components", "ind_electroniccomponents");
            translationIndustry.Add("Electronic Gaming &amp; Multimedia", "ind_electronicgamingmultimedia");
            translationIndustry.Add("Electronics &amp; Computer Distribution", "ind_electronicscomputerdistribution");
            translationIndustry.Add("Engineering &amp; Construction", "ind_engineeringconstruction");
            translationIndustry.Add("Entertainment", "ind_entertainment");
            translationIndustry.Add("Exchange Traded Fund", "ind_exchangetradedfund");
            translationIndustry.Add("Farm &amp; Heavy Construction Machinery", "ind_farmheavyconstructionmachinery");
            translationIndustry.Add("Farm Products", "ind_farmproducts");
            translationIndustry.Add("Financial Conglomerates", "ind_financialconglomerates");
            translationIndustry.Add("Financial Data &amp; Stock Exchanges", "ind_financialdatastockexchanges");
            translationIndustry.Add("Food Distribution", "ind_fooddistribution");
            translationIndustry.Add("Footwear &amp; Accessories", "ind_footwearaccessories");
            translationIndustry.Add("Furnishings, Fixtures &amp; Appliances", "ind_furnishingsfixturesappliances");
            translationIndustry.Add("Gambling", "ind_gambling");
            translationIndustry.Add("Gold", "ind_gold");
            translationIndustry.Add("Grocery Stores", "ind_grocerystores");
            translationIndustry.Add("Healthcare Plans", "ind_healthcareplans");
            translationIndustry.Add("Health Information Services", "ind_healthinformationservices");
            translationIndustry.Add("Home Improvement Retail", "ind_homeimprovementretail");
            translationIndustry.Add("Household &amp; Personal Products", "ind_householdpersonalproducts");
            translationIndustry.Add("Industrial Distribution", "ind_industrialdistribution");
            translationIndustry.Add("Information Technology Services", "ind_informationtechnologyservices");
            translationIndustry.Add("Infrastructure Operations", "ind_infrastructureoperations");
            translationIndustry.Add("Insurance Brokers", "ind_insurancebrokers");
            translationIndustry.Add("Insurance - Diversified", "ind_insurancediversified");
            translationIndustry.Add("Insurance - Life", "ind_insurancelife");
            translationIndustry.Add("Insurance - Property &amp; Casualty", "ind_insurancepropertycasualty");
            translationIndustry.Add("Insurance - Reinsurance", "ind_insurancereinsurance");
            translationIndustry.Add("Insurance - Specialty", "ind_insurancespecialty");
            translationIndustry.Add("Integrated Freight &amp; Logistics", "ind_integratedfreightlogistics");
            translationIndustry.Add("Internet Content &amp; Information", "ind_internetcontentinformation");
            translationIndustry.Add("Internet Retail", "ind_internetretail");
            translationIndustry.Add("Leisure", "ind_leisure");
            translationIndustry.Add("Lodging", "ind_lodging");
            translationIndustry.Add("Lumber &amp; Wood Production", "ind_lumberwoodproduction");
            translationIndustry.Add("Luxury Goods", "ind_luxurygoods");
            translationIndustry.Add("Marine Shipping", "ind_marineshipping");
            translationIndustry.Add("Medical Care Facilities", "ind_medicalcarefacilities");
            translationIndustry.Add("Medical Devices", "ind_medicaldevices");
            translationIndustry.Add("Medical Distribution", "ind_medicaldistribution");
            translationIndustry.Add("Medical Instruments &amp; Supplies", "ind_medicalinstrumentssupplies");
            translationIndustry.Add("Metal Fabrication", "ind_metalfabrication");
            translationIndustry.Add("Mortgage Finance", "ind_mortgagefinance");
            translationIndustry.Add("Oil &amp; Gas Drilling", "ind_oilgasdrilling");
            translationIndustry.Add("Oil &amp; Gas E&amp;P", "ind_oilgasep");
            translationIndustry.Add("Oil &amp; Gas Equipment &amp; Services", "ind_oilgasequipmentservices");
            translationIndustry.Add("Oil &amp; Gas Integrated", "ind_oilgasintegrated");
            translationIndustry.Add("Oil &amp; Gas Midstream", "ind_oilgasmidstream");
            translationIndustry.Add("Oil &amp; Gas Refining &amp; Marketing", "ind_oilgasrefiningmarketing");
            translationIndustry.Add("Other Industrial Metals &amp; Mining", "ind_otherindustrialmetalsmining");
            translationIndustry.Add("Other Precious Metals &amp; Mining", "ind_otherpreciousmetalsmining");
            translationIndustry.Add("Packaged Foods", "ind_packagedfoods");
            translationIndustry.Add("Packaging &amp; Containers", "ind_packagingcontainers");
            translationIndustry.Add("Paper &amp; Paper Products", "ind_paperpaperproducts");
            translationIndustry.Add("Personal Services", "ind_personalservices");
            translationIndustry.Add("Pharmaceutical Retailers", "ind_pharmaceuticalretailers");
            translationIndustry.Add("Pollution &amp; Treatment Controls", "ind_pollutiontreatmentcontrols");
            translationIndustry.Add("Publishing", "ind_publishing");
            translationIndustry.Add("Railroads", "ind_railroads");
            translationIndustry.Add("Real Estate - Development", "ind_realestatedevelopment");
            translationIndustry.Add("Real Estate - Diversified", "ind_realestatediversified");
            translationIndustry.Add("Real Estate Services", "ind_realestateservices");
            translationIndustry.Add("Recreational Vehicles", "ind_recreationalvehicles");
            translationIndustry.Add("REIT - Diversified", "ind_reitdiversified");
            translationIndustry.Add("REIT - Healthcare Facilities", "ind_reithealthcarefacilities");
            translationIndustry.Add("REIT - Hotel &amp; Motel", "ind_reithotelmotel");
            translationIndustry.Add("REIT - Industrial", "ind_reitindustrial");
            translationIndustry.Add("REIT - Mortgage", "ind_reitmortgage");
            translationIndustry.Add("REIT - Office", "ind_reitoffice");
            translationIndustry.Add("REIT - Residential", "ind_reitresidential");
            translationIndustry.Add("REIT - Retail", "ind_reitretail");
            translationIndustry.Add("REIT - Specialty", "ind_reitspecialty");
            translationIndustry.Add("Rental &amp; Leasing Services", "ind_rentalleasingservices");
            translationIndustry.Add("Residential Construction", "ind_residentialconstruction");
            translationIndustry.Add("Resorts &amp; Casinos", "ind_resortscasinos");
            translationIndustry.Add("Restaurants", "ind_restaurants");
            translationIndustry.Add("Scientific &amp; Technical Instruments", "ind_scientifictechnicalinstruments");
            translationIndustry.Add("Security &amp; Protection Services", "ind_securityprotectionservices");
            translationIndustry.Add("Semiconductor Equipment &amp; Materials", "ind_semiconductorequipmentmaterials");
            translationIndustry.Add("Semiconductors", "ind_semiconductors");
            translationIndustry.Add("Shell Companies", "ind_shellcompanies");
            translationIndustry.Add("Silver", "ind_silver");
            translationIndustry.Add("Software - Application", "ind_softwareapplication");
            translationIndustry.Add("Software - Infrastructure", "ind_softwareinfrastructure");
            translationIndustry.Add("Solar", "ind_solar");
            translationIndustry.Add("Specialty Business Services", "ind_specialtybusinessservices");
            translationIndustry.Add("Specialty Chemicals", "ind_specialtychemicals");
            translationIndustry.Add("Specialty Industrial Machinery", "ind_specialtyindustrialmachinery");
            translationIndustry.Add("Specialty Retail", "ind_specialtyretail");
            translationIndustry.Add("Staffing &amp; Employment Services", "ind_staffingemploymentservices");
            translationIndustry.Add("Steel", "ind_steel");
            translationIndustry.Add("Telecom Services", "ind_telecomservices");
            translationIndustry.Add("Textile Manufacturing", "ind_textilemanufacturing");
            translationIndustry.Add("Thermal Coal", "ind_thermalcoal");
            translationIndustry.Add("Tobacco", "ind_tobacco");
            translationIndustry.Add("Tools &amp; Accessories", "ind_toolsaccessories");
            translationIndustry.Add("Travel Services", "ind_travelservices");
            translationIndustry.Add("Trucking", "ind_trucking");
            translationIndustry.Add("Uranium", "ind_uranium");
            translationIndustry.Add("Utilities - Diversified", "ind_utilitiesdiversified");
            translationIndustry.Add("Utilities - Independent Power Producers", "ind_utilitiesindependentpowerproducers");
            translationIndustry.Add("Utilities - Regulated Electric", "ind_utilitiesregulatedelectric");
            translationIndustry.Add("Utilities - Regulated Gas", "ind_utilitiesregulatedgas");
            translationIndustry.Add("Utilities - Regulated Water", "ind_utilitiesregulatedwater");
            translationIndustry.Add("Utilities - Renewable", "ind_utilitiesrenewable");
            translationIndustry.Add("Waste Management", "ind_wastemanagement");
            translationIndustry.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Industry.ToString(), translationIndustry);

            translationPEG = new Dictionary<string, string>();
            translationPEG.Add("Any", "");
            translationPEG.Add("Low (<1)", "fa_peg_low");
            translationPEG.Add("High (>2)", "fa_peg_high");
            translationPEG.Add("Under 1", "fa_peg_u1");
            translationPEG.Add("Under 2", "fa_peg_u2");
            translationPEG.Add("Under 3", "fa_peg_u3");
            translationPEG.Add("Over 1", "fa_peg_o1");
            translationPEG.Add("Over 2", "fa_peg_o2");
            translationPEG.Add("Over 3", "fa_peg_o3");
            translationPEG.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PEG.ToString(), translationPEG);

            translationEPSGrowthThisYears = new Dictionary<string, string>();
            translationEPSGrowthThisYears.Add("Any", "");
            translationEPSGrowthThisYears.Add("Negative (<0%)", "fa_epsyoy_neg");
            translationEPSGrowthThisYears.Add("Positive (>0%)", "fa_epsyoy_pos");
            translationEPSGrowthThisYears.Add("Positive Low (0-10%)", "fa_epsyoy_poslow");
            translationEPSGrowthThisYears.Add("High (>25%)", "fa_epsyoy_high");
            translationEPSGrowthThisYears.Add("Under 5%", "fa_epsyoy_u5");
            translationEPSGrowthThisYears.Add("Under 10%", "fa_epsyoy_u10");
            translationEPSGrowthThisYears.Add("Under 15%", "fa_epsyoy_u15");
            translationEPSGrowthThisYears.Add("Under 20%", "fa_epsyoy_u20");
            translationEPSGrowthThisYears.Add("Under 25%", "fa_epsyoy_u25");
            translationEPSGrowthThisYears.Add("Under 30%", "fa_epsyoy_u30");
            translationEPSGrowthThisYears.Add("Over 5%", "fa_epsyoy_o5");
            translationEPSGrowthThisYears.Add("Over 10%", "fa_epsyoy_o10");
            translationEPSGrowthThisYears.Add("Over 15%", "fa_epsyoy_o15");
            translationEPSGrowthThisYears.Add("Over 20%", "fa_epsyoy_o20");
            translationEPSGrowthThisYears.Add("Over 25%", "fa_epsyoy_o25");
            translationEPSGrowthThisYears.Add("Over 30%", "fa_epsyoy_o30");
            translationEPSGrowthThisYears.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EPSGrowthThisYear.ToString(), translationEPSGrowthThisYears);

            translationEPSGrowthQuarterOverQuarter = new Dictionary<string, string>();
            translationEPSGrowthQuarterOverQuarter.Add("Any", "");
            translationEPSGrowthQuarterOverQuarter.Add("Negative (<0%)", "fa_epsqoq_neg");
            translationEPSGrowthQuarterOverQuarter.Add("Positive (>0%)", "fa_epsqoq_pos");
            translationEPSGrowthQuarterOverQuarter.Add("Positive Low (0-10%)", "fa_epsqoq_poslow");
            translationEPSGrowthQuarterOverQuarter.Add("High (>25%)", "fa_epsqoq_high");
            translationEPSGrowthQuarterOverQuarter.Add("Under 5%", "fa_epsqoq_u5");
            translationEPSGrowthQuarterOverQuarter.Add("Under 10%", "fa_epsqoq_u10");
            translationEPSGrowthQuarterOverQuarter.Add("Under 15%", "fa_epsqoq_u15");
            translationEPSGrowthQuarterOverQuarter.Add("Under 20%", "fa_epsqoq_u20");
            translationEPSGrowthQuarterOverQuarter.Add("Under 25%", "fa_epsqoq_u25");
            translationEPSGrowthQuarterOverQuarter.Add("Under 30%", "fa_epsqoq_u30");
            translationEPSGrowthQuarterOverQuarter.Add("Over 5%", "fa_epsqoq_o5");
            translationEPSGrowthQuarterOverQuarter.Add("Over 10%", "fa_epsqoq_o10");
            translationEPSGrowthQuarterOverQuarter.Add("Over 15%", "fa_epsqoq_o15");
            translationEPSGrowthQuarterOverQuarter.Add("Over 20%", "fa_epsqoq_o20");
            translationEPSGrowthQuarterOverQuarter.Add("Over 25%", "fa_epsqoq_o25");
            translationEPSGrowthQuarterOverQuarter.Add("Over 30%", "fa_epsqoq_o30");
            translationEPSGrowthQuarterOverQuarter.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EPSGrowthQuarterOverQuarter.ToString(), translationEPSGrowthQuarterOverQuarter);

            translationReturnOnInvestment = new Dictionary<string, string>();
            translationReturnOnInvestment.Add("Any", "");
            translationReturnOnInvestment.Add("Positive (>0%)", "fa_roi_pos");
            translationReturnOnInvestment.Add("Negative (<0%)", "fa_roi_neg");
            translationReturnOnInvestment.Add("Very Positive (>25%)", "fa_roi_verypos");
            translationReturnOnInvestment.Add("Very Negative (<-10%)", "fa_roi_veryneg");
            translationReturnOnInvestment.Add("Under -50%", "fa_roi_u-50");
            translationReturnOnInvestment.Add("Under -45%", "fa_roi_u-45");
            translationReturnOnInvestment.Add("Under -40%", "fa_roi_u-40");
            translationReturnOnInvestment.Add("Under -35%", "fa_roi_u-45");
            translationReturnOnInvestment.Add("Under -30%", "fa_roi_u-30");
            translationReturnOnInvestment.Add("Under -25%", "fa_roi_u-25");
            translationReturnOnInvestment.Add("Under -20%", "fa_roi_u-20");
            translationReturnOnInvestment.Add("Under -15%", "fa_roi_u-15");
            translationReturnOnInvestment.Add("Under -10%", "fa_roi_u-10");
            translationReturnOnInvestment.Add("Under -5%", "fa_roi_u-5");
            translationReturnOnInvestment.Add("Over +5%", "fa_roi_o5");
            translationReturnOnInvestment.Add("Over +10%", "fa_roi_o10");
            translationReturnOnInvestment.Add("Over +15%", "fa_roi_o15");
            translationReturnOnInvestment.Add("Over +20%", "fa_roi_o20");
            translationReturnOnInvestment.Add("Over +25%", "fa_roi_o25");
            translationReturnOnInvestment.Add("Over +30%", "fa_roi_o30");
            translationReturnOnInvestment.Add("Over +35%", "fa_roi_o35");
            translationReturnOnInvestment.Add("Over +40%", "fa_roi_o40");
            translationReturnOnInvestment.Add("Over +45%", "fa_roi_o45");
            translationReturnOnInvestment.Add("Over +50%", "fa_roi_o50");
            translationReturnOnInvestment.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.ReturnOnInvestment.ToString(), translationReturnOnInvestment);

            translationGrossMargin = new Dictionary<string, string>();
            translationGrossMargin.Add("Any", "");
            translationGrossMargin.Add("Positive (>0%)", "fa_grossmargin_pos");
            translationGrossMargin.Add("Negative (<0%)", "fa_grossmargin_neg");
            translationGrossMargin.Add("High (>50%)", "fa_grossmargin_high");
            translationGrossMargin.Add("Under 90%", "fa_grossmargin_u90");
            translationGrossMargin.Add("Under 80%", "fa_grossmargin_u80");
            translationGrossMargin.Add("Under 70%", "fa_grossmargin_u70");
            translationGrossMargin.Add("Under 60%", "fa_grossmargin_u60");
            translationGrossMargin.Add("Under 50%", "fa_grossmargin_u50");
            translationGrossMargin.Add("Under 45%", "fa_grossmargin_u45");
            translationGrossMargin.Add("Under 40%", "fa_grossmargin_u40");
            translationGrossMargin.Add("Under 35%", "fa_grossmargin_u35");
            translationGrossMargin.Add("Under 30%", "fa_grossmargin_u30");
            translationGrossMargin.Add("Under 25%", "fa_grossmargin_u25");
            translationGrossMargin.Add("Under 20%", "fa_grossmargin_u20");
            translationGrossMargin.Add("Under 15%", "fa_grossmargin_u15");
            translationGrossMargin.Add("Under 10%", "fa_grossmargin_u10");
            translationGrossMargin.Add("Under 5%", "fa_grossmargin_u5");
            translationGrossMargin.Add("Under 0%", "fa_grossmargin_u0");
            translationGrossMargin.Add("Under -10%", "fa_grossmargin_u-10");
            translationGrossMargin.Add("Under -20%", "fa_grossmargin_u-20");
            translationGrossMargin.Add("Under -30%", "fa_grossmargin_u-30");
            translationGrossMargin.Add("Under -50%", "fa_grossmargin_u-50");
            translationGrossMargin.Add("Under -70%", "fa_grossmargin_u-70");
            translationGrossMargin.Add("Under -100%", "fa_grossmargin_u-100");
            translationGrossMargin.Add("Over 0%", "fa_grossmargin_o0");
            translationGrossMargin.Add("Over 5%", "fa_grossmargin_o5");
            translationGrossMargin.Add("Over 10%", "fa_grossmargin_o10");
            translationGrossMargin.Add("Over 15%", "fa_grossmargin_o15");
            translationGrossMargin.Add("Over 20%", "fa_grossmargin_o20");
            translationGrossMargin.Add("Over 25%", "fa_grossmargin_o25");
            translationGrossMargin.Add("Over 30%", "fa_grossmargin_o30");
            translationGrossMargin.Add("Over 35%", "fa_grossmargin_o35");
            translationGrossMargin.Add("Over 40%", "fa_grossmargin_o40");
            translationGrossMargin.Add("Over 45%", "fa_grossmargin_o45");
            translationGrossMargin.Add("Over 50%", "fa_grossmargin_o50");
            translationGrossMargin.Add("Over 60%", "fa_grossmargin_o60");
            translationGrossMargin.Add("Over 70%", "fa_grossmargin_o70");
            translationGrossMargin.Add("Over 80%", "fa_grossmargin_o80");
            translationGrossMargin.Add("Over 90%", "fa_grossmargin_o90");
            translationGrossMargin.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.GrossMargin.ToString(), translationGrossMargin);

            translationInsiderTransactions = new Dictionary<string, string>();
            translationInsiderTransactions.Add("Any", "");
            translationInsiderTransactions.Add("Very Negative (<20%)", "sh_insidertrans_veryneg");
            translationInsiderTransactions.Add("Negative (<0%)", "sh_insidertrans_neg");
            translationInsiderTransactions.Add("Positive (>0%)", "sh_insidertrans_pos");
            translationInsiderTransactions.Add("Very Positive (>20%)", "sh_insidertrans_verypos");
            translationInsiderTransactions.Add("Under -90%", "sh_insidertrans_u-90");
            translationInsiderTransactions.Add("Under -80%", "sh_insidertrans_u-80");
            translationInsiderTransactions.Add("Under -70%", "sh_insidertrans_u-70");
            translationInsiderTransactions.Add("Under -60%", "sh_insidertrans_u-60");
            translationInsiderTransactions.Add("Under -50%", "sh_insidertrans_u-50");
            translationInsiderTransactions.Add("Under -45%", "sh_insidertrans_u-45");
            translationInsiderTransactions.Add("Under -40%", "sh_insidertrans_u-40");
            translationInsiderTransactions.Add("Under -35%", "sh_insidertrans_u-35");
            translationInsiderTransactions.Add("Under -30%", "sh_insidertrans_u-30");
            translationInsiderTransactions.Add("Under -25%", "sh_insidertrans_u-25");
            translationInsiderTransactions.Add("Under -20%", "sh_insidertrans_u-20");
            translationInsiderTransactions.Add("Under -15%", "sh_insidertrans_u-15");
            translationInsiderTransactions.Add("Under -10%", "sh_insidertrans_u-10");
            translationInsiderTransactions.Add("Under -5%", "sh_insidertrans_u-5");
            translationInsiderTransactions.Add("Over +5%", "fa_insidertrans_o5");
            translationInsiderTransactions.Add("Over +10%", "fa_insidertrans_o10");
            translationInsiderTransactions.Add("Over +15%", "fa_insidertrans_o15");
            translationInsiderTransactions.Add("Over +20%", "fa_insidertrans_o20");
            translationInsiderTransactions.Add("Over +25%", "fa_insidertrans_o25");
            translationInsiderTransactions.Add("Over +30%", "fa_insidertrans_o30");
            translationInsiderTransactions.Add("Over +35%", "fa_insidertrans_o35");
            translationInsiderTransactions.Add("Over +40%", "fa_insidertrans_o40");
            translationInsiderTransactions.Add("Over +45%", "sh_insidertrans_o45");
            translationInsiderTransactions.Add("Over +50%", "fa_insidertrans_o50");
            translationInsiderTransactions.Add("Over +60%", "fa_insidertrans_o60");
            translationInsiderTransactions.Add("Over +70%", "fa_insidertrans_o70");
            translationInsiderTransactions.Add("Over +80%", "fa_insidertrans_o80");
            translationInsiderTransactions.Add("Over +90%", "fa_insidertrans_o90");
            translationInsiderTransactions.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.InsiderTransactions.ToString(), translationInsiderTransactions);

            translationOptionShort = new Dictionary<string, string>();
            translationOptionShort.Add("Any", "");
            translationOptionShort.Add("Optionable", "sh_opt_option");
            translationOptionShort.Add("Shortable", "sh_opt_short");
            translationOptionShort.Add("Optionable and shortable", "sh_opt_optionshort");
            translationOptionShort.Add("Optionable and not shortable", "sh_opt_optionnotshort");
            translationOptionShort.Add("Not optionable and shortable", "sh_opt_notoptionshort");
            translationOptionShort.Add("Not optionable and not shortable", "sh_opt_notoptionnotshort");
            translationsMaster.Add(FinvzEnumFilterType.OptionShort.ToString(), translationOptionShort);

            translationRSI14 = new Dictionary<string, string>();
            translationRSI14.Add("Any", "");
            translationRSI14.Add("Overbought (90)", "ta_rsi_ob90");
            translationRSI14.Add("Overbought (80)", "ta_rsi_ob80");
            translationRSI14.Add("Overbought (70)", "ta_rsi_ob70");
            translationRSI14.Add("Overbought (60)", "ta_rsi_ob60");
            translationRSI14.Add("Oversold (40)", "ta_rsi_os40");
            translationRSI14.Add("Oversold (30)", "ta_rsi_os30");
            translationRSI14.Add("Oversold (20)", "ta_rsi_os20");
            translationRSI14.Add("Oversold (10)", "ta_rsi_os10");
            translationRSI14.Add("Not Overbought (<60)", "ta_rsi_nob60");
            translationRSI14.Add("Not Overbought (<50)", "ta_rsi_nob50");
            translationRSI14.Add("Not Oversold (>50)", "ta_rsi_nos50");
            translationRSI14.Add("Not Oversold (>40)", "ta_rsi_nos40");
            translationRSI14.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.RSI14.ToString(), translationRSI14);

            translationChange = new Dictionary<string, string>();
            translationChange.Add("Any", "");
            translationChange.Add("Up", "ta_change_u");
            translationChange.Add("Up 1%", "ta_change_u1");
            translationChange.Add("Up 2%", "ta_change_u2");
            translationChange.Add("Up 3%", "ta_change_u3");
            translationChange.Add("Up 4%", "ta_change_u4");
            translationChange.Add("Up 5%", "ta_change_u5");
            translationChange.Add("Up 6%", "ta_change_u6");
            translationChange.Add("Up 7%", "ta_change_u7");
            translationChange.Add("Up 8%", "ta_change_u8");
            translationChange.Add("Up 9%", "ta_change_u9");
            translationChange.Add("Up 10%", "ta_change_u10");
            translationChange.Add("Up 15%", "ta_change_u15");
            translationChange.Add("Up 20%", "ta_change_u20");
            translationChange.Add("Down", "ta_change_d");
            translationChange.Add("Down 1%", "ta_change_d1");
            translationChange.Add("Down 2%", "ta_change_d2");
            translationChange.Add("Down 3%", "ta_change_d3");
            translationChange.Add("Down 4%", "ta_change_d4");
            translationChange.Add("Down 5%", "ta_change_d5");
            translationChange.Add("Down 6%", "ta_change_d6");
            translationChange.Add("Down 7%", "ta_change_d7");
            translationChange.Add("Down 8%", "ta_change_d8");
            translationChange.Add("Down 9%", "ta_change_d9");
            translationChange.Add("Down 10%", "ta_change_d10");
            translationChange.Add("Down 15%", "ta_change_d15");
            translationChange.Add("Down 20%", "ta_change_d20");
            translationChange.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Change.ToString(), translationChange);

            translationPattern = new Dictionary<string, string>();
            translationPattern.Add("Any", "");
            translationPattern.Add("Horizontal S/R", "ta_pattern_horizontal");
            translationPattern.Add("Horizontal S/R (Strong)", "ta_pattern_horizontal2");
            translationPattern.Add("TL Resistance", "ta_pattern_tlresistance");
            translationPattern.Add("TL Resistance (Strong)", "ta_pattern_tlresistance2");
            translationPattern.Add("TL Support", "ta_pattern_tlsupport");
            translationPattern.Add("TL Support (Strong)", "ta_pattern_tlsupport2");
            translationPattern.Add("Wedge Up", "ta_pattern_wedgeup");
            translationPattern.Add("Wedge Up (Strong)", "ta_pattern_wedgeup2");
            translationPattern.Add("Wedge Down", "ta_pattern_wedgedown");
            translationPattern.Add("Wedge Down (Strong)", "ta_pattern_wedgedown2");
            translationPattern.Add("Triangle Ascending", "ta_pattern_wedgeresistance");
            translationPattern.Add("Triangle Ascending (Strong)", "ta_pattern_wedgeresistance2");
            translationPattern.Add("Triangle Descending", "ta_pattern_wedgesupport");
            translationPattern.Add("Triangle Descending (Strong)", "ta_pattern_wedgesupport2");
            translationPattern.Add("Wedge", "ta_pattern_wedgesupport");
            translationPattern.Add("Wedge (Strong)", "ta_pattern_wedgesupport2");
            translationPattern.Add("Channel Up", "ta_pattern_channelup");
            translationPattern.Add("Channel Up (Strong)", "ta_pattern_channelup2");
            translationPattern.Add("Channel Down", "ta_pattern_channeldown");
            translationPattern.Add("Channel Down (Strong)", "ta_pattern_channeldown2");
            translationPattern.Add("Channel", "ta_pattern_channel");
            translationPattern.Add("Channel (Strong)", "ta_pattern_channel2");
            translationPattern.Add("Double Top", "ta_pattern_doubletop");
            translationPattern.Add("Double Bottom", "ta_pattern_doublebottom");
            translationPattern.Add("Multiple Top", "ta_pattern_multipletop");
            translationPattern.Add("Multiple Bottom", "ta_pattern_multiplebottom");
            translationPattern.Add("Head & Shoulders", "ta_pattern_headandshoulders");
            translationPattern.Add("Head & Shoulders Inverse", "ta_pattern_headandshouldersinv");
            translationPattern.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Pattern.ToString(), translationPattern);

            translationRelativeVolume = new Dictionary<string, string>();
            translationRelativeVolume.Add("Any", "");
            translationRelativeVolume.Add("Over 10", "sh_relvol_o10");
            translationRelativeVolume.Add("Over 5", "sh_relvol_o5");
            translationRelativeVolume.Add("Over 3", "sh_relvol_o3");
            translationRelativeVolume.Add("Over 2", "sh_relvol_o2");
            translationRelativeVolume.Add("Over 1.5", "sh_relvol_o1.5");
            translationRelativeVolume.Add("Over 1", "sh_relvol_o1");
            translationRelativeVolume.Add("Over 0.75", "sh_relvol_o0.75");
            translationRelativeVolume.Add("Over 0.5", "sh_relvol_o0.5");
            translationRelativeVolume.Add("Over 0.25", "sh_relvol_o0.25");
            translationRelativeVolume.Add("Under 2", "sh_relvol_u2");
            translationRelativeVolume.Add("Under 1.5", "sh_relvol_u1.5");
            translationRelativeVolume.Add("Under 1", "sh_relvol_u1");
            translationRelativeVolume.Add("Under 0.75", "sh_relvol_u0.75");
            translationRelativeVolume.Add("Under 0.5", "sh_relvol_u0.5");
            translationRelativeVolume.Add("Under 0.25", "sh_relvol_u0.25");
            translationRelativeVolume.Add("Under 0.1", "sh_relvol_u0.1");
            translationRelativeVolume.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.RelativeVolume.ToString(), translationRelativeVolume);

            translationSharesOutstanding = new Dictionary<string, string>();
            translationSharesOutstanding.Add("Any", "");
            translationSharesOutstanding.Add("Under 1M", "sh_outstanding_u1");
            translationSharesOutstanding.Add("Under 5M", "sh_outstanding_u5");
            translationSharesOutstanding.Add("Under 10M", "sh_outstanding_u10");
            translationSharesOutstanding.Add("Under 20M", "sh_outstanding_u20");
            translationSharesOutstanding.Add("Under 50M", "sh_outstanding_u50");
            translationSharesOutstanding.Add("Under 100M", "sh_outstanding_u100");
            translationSharesOutstanding.Add("Over 1M", "sh_outstanding_o1");
            translationSharesOutstanding.Add("Over 2M", "sh_outstanding_o2");
            translationSharesOutstanding.Add("Over 5M", "sh_outstanding_o5");
            translationSharesOutstanding.Add("Over 10M", "sh_outstanding_o10");
            translationSharesOutstanding.Add("Over 20M", "sh_outstanding_o20");
            translationSharesOutstanding.Add("Over 50M", "sh_outstanding_o50");
            translationSharesOutstanding.Add("Over 100M", "sh_outstanding_o100");
            translationSharesOutstanding.Add("Over 200M", "sh_outstanding_o200");
            translationSharesOutstanding.Add("Over 500M", "sh_outstanding_o500");
            translationSharesOutstanding.Add("Over 1000M", "sh_outstanding_o1000");
            translationSharesOutstanding.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.SharesOutstanding.ToString(), translationSharesOutstanding);


            // Column 5
            translationCountry = new Dictionary<string, string>();
            translationCountry.Add("Any", "");
            translationCountry.Add("USA", "geo_usa");
            translationCountry.Add("Foreign (ex-USA)", "geo_notusa");
            translationCountry.Add("Asia", "geo_asia");
            translationCountry.Add("Europe", "geo_europe");
            translationCountry.Add("Latin America", "geo_latinamerica");
            translationCountry.Add("BRIC", "geo_bric");
            translationCountry.Add("Argentina", "geo_argentina");
            translationCountry.Add("Australia", "geo_australia");
            translationCountry.Add("Bahamas", "geo_bahamas");
            translationCountry.Add("Belgium", "geo_belgium");
            translationCountry.Add("BeNeLux", "geo_benelux");
            translationCountry.Add("Bermuda", "geo_bermuda");
            translationCountry.Add("Brazil", "geo_brazil");
            translationCountry.Add("Canada", "geo_canada");
            translationCountry.Add("Cayman Islands", "geo_caymanislands");
            translationCountry.Add("Chile", "geo_chile");
            translationCountry.Add("China", "geo_china");
            translationCountry.Add("China & Hong Kong", "geo_chinahongkong");
            translationCountry.Add("Colombia", "geo_colombia");
            translationCountry.Add("Cyprus", "geo_cyprus");
            translationCountry.Add("Denmark", "geo_denmark");
            translationCountry.Add("Finland", "geo_finland");
            translationCountry.Add("France", "geo_france");
            translationCountry.Add("Germany", "geo_germany");
            translationCountry.Add("Greece", "geo_greece");
            translationCountry.Add("Hong Kong", "geo_hongkong");
            translationCountry.Add("Hungary", "geo_hungary");
            translationCountry.Add("Iceland", "geo_iceland");
            translationCountry.Add("India", "geo_india");
            translationCountry.Add("Indonesia", "geo_indonesia");
            translationCountry.Add("Ireland", "geo_ireland");
            translationCountry.Add("Israel", "geo_israel");
            translationCountry.Add("Italy", "geo_italy");
            translationCountry.Add("Japan", "geo_japan");
            translationCountry.Add("Kazakhstan", "geo_kazakhstan");
            translationCountry.Add("Luxembourg", "geo_luxembourg");
            translationCountry.Add("Malaysia", "geo_malaysia");
            translationCountry.Add("Malta", "geo_malta");
            translationCountry.Add("Mexico", "geo_mexico");
            translationCountry.Add("Monaco", "geo_monaco");
            translationCountry.Add("Netherlands", "geo_netherlands");
            translationCountry.Add("New Zealand", "geo_newzealand");
            translationCountry.Add("Norway", "geo_norway");
            translationCountry.Add("Panama", "geo_panama");
            translationCountry.Add("Peru", "geo_peru");
            translationCountry.Add("Philippines", "geo_philippines");
            translationCountry.Add("Portugal", "geo_portugal");
            translationCountry.Add("Russia", "geo_russia");
            translationCountry.Add("Singapore", "geo_singapore");
            translationCountry.Add("South Africa", "geo_southafrica");
            translationCountry.Add("South Korea", "geo_southkorea");
            translationCountry.Add("Spain", "geo_spain");
            translationCountry.Add("Sweden", "geo_sweden");
            translationCountry.Add("Switzerland", "geo_switzerland");
            translationCountry.Add("Taiwan", "geo_taiwan");
            translationCountry.Add("Turkey", "geo_turkey");
            translationCountry.Add("United Arab Emirates", "geo_unitedarabemirates");
            translationCountry.Add("United Kingdom", "geo_unitedkingdom");
            translationCountry.Add("Uruguay", "geo_uruguay");
            translationCountry.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Country.ToString(), translationCountry);

            translationPS = new Dictionary<string, string>();
            translationPS.Add("Any", "");
            translationPS.Add("Low (<1)", "fa_ps_low");
            translationPS.Add("High (>5)", "fa_ps_high");
            translationPS.Add("Under 1", "fa_ps_u1");
            translationPS.Add("Under 2", "fa_ps_u2");
            translationPS.Add("Under 3", "fa_ps_u3");
            translationPS.Add("Under 4", "fa_ps_u4");
            translationPS.Add("Under 5", "fa_ps_u5");
            translationPS.Add("Under 6", "fa_ps_u6");
            translationPS.Add("Under 7", "fa_ps_u7");
            translationPS.Add("Under 8", "fa_ps_u8");
            translationPS.Add("Under 9", "fa_ps_u9");
            translationPS.Add("Under 10", "fa_ps_u10");
            translationPS.Add("Over 1", "fa_ps_o1");
            translationPS.Add("Over 2", "fa_ps_o2");
            translationPS.Add("Over 3", "fa_ps_o3");
            translationPS.Add("Over 4", "fa_ps_o4");
            translationPS.Add("Over 5", "fa_ps_o5");
            translationPS.Add("Over 6", "fa_ps_o6");
            translationPS.Add("Over 7", "fa_ps_o7");
            translationPS.Add("Over 8", "fa_ps_o8");
            translationPS.Add("Over 9", "fa_ps_o9");
            translationPS.Add("Over 10", "fa_ps_o10");
            translationPS.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.PS.ToString(), translationPS);

            translationEPSGrowthNextYear = new Dictionary<string, string>();
            translationEPSGrowthNextYear.Add("Any", "");
            translationEPSGrowthNextYear.Add("Negative (<0%)", "fa_epsyoy1_neg");
            translationEPSGrowthNextYear.Add("Positive (>0%)", "fa_epsyoy1_pos");
            translationEPSGrowthNextYear.Add("Positive Low (0-10%)", "fa_epsyoy1_poslow");
            translationEPSGrowthNextYear.Add("High (>25%)", "fa_epsyoy1_high");
            translationEPSGrowthNextYear.Add("Under 5%", "fa_epsyoy1_u5");
            translationEPSGrowthNextYear.Add("Under 10%", "fa_epsyoy1_u10");
            translationEPSGrowthNextYear.Add("Under 15%", "fa_epsyoy1_u15");
            translationEPSGrowthNextYear.Add("Under 20%", "fa_epsyoy1_u20");
            translationEPSGrowthNextYear.Add("Under 25%", "fa_epsyoy1_u25");
            translationEPSGrowthNextYear.Add("Under 30%", "fa_epsyoy1_u30");
            translationEPSGrowthNextYear.Add("Over 5%", "fa_epsyoy1_o5");
            translationEPSGrowthNextYear.Add("Over 10%", "fa_epsyoy1_o10");
            translationEPSGrowthNextYear.Add("Over 15%", "fa_epsyoy1_o15");
            translationEPSGrowthNextYear.Add("Over 20%", "fa_epsyoy1_o20");
            translationEPSGrowthNextYear.Add("Over 25%", "fa_epsyoy1_o25");
            translationEPSGrowthNextYear.Add("Over 30%", "fa_epsyoy1_o30");
            translationEPSGrowthNextYear.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EPSGrowthNextYear.ToString(), translationEPSGrowthNextYear);

            translationSalesGrowthQuarterOverQuarter = new Dictionary<string, string>();
            translationSalesGrowthQuarterOverQuarter.Add("Any", "");
            translationSalesGrowthQuarterOverQuarter.Add("Negative (<0%)", "fa_salesqoq_neg");
            translationSalesGrowthQuarterOverQuarter.Add("Positive (>0%)", "fa_salesqoq_pos");
            translationSalesGrowthQuarterOverQuarter.Add("Positive Low (0-10%)", "fa_salesqoq_poslow");
            translationSalesGrowthQuarterOverQuarter.Add("High (>25%)", "fa_salesqoq_high");
            translationSalesGrowthQuarterOverQuarter.Add("Under 5%", "fa_salesqoq_u5");
            translationSalesGrowthQuarterOverQuarter.Add("Under 10%", "fa_salesqoq_u10");
            translationSalesGrowthQuarterOverQuarter.Add("Under 15%", "fa_salesqoq_u15");
            translationSalesGrowthQuarterOverQuarter.Add("Under 20%", "fa_salesqoq_u20");
            translationSalesGrowthQuarterOverQuarter.Add("Under 25%", "fa_salesqoq_u25");
            translationSalesGrowthQuarterOverQuarter.Add("Under 30%", "fa_salesqoq_u30");
            translationSalesGrowthQuarterOverQuarter.Add("Over 5%", "fa_salesqoq_o5");
            translationSalesGrowthQuarterOverQuarter.Add("Over 10%", "fa_salesqoq_o10");
            translationSalesGrowthQuarterOverQuarter.Add("Over 15%", "fa_salesqoq_o15");
            translationSalesGrowthQuarterOverQuarter.Add("Over 20%", "fa_salesqoq_o20");
            translationSalesGrowthQuarterOverQuarter.Add("Over 25%", "fa_salesqoq_o25");
            translationSalesGrowthQuarterOverQuarter.Add("Over 30%", "fa_salesqoq_o30");
            translationSalesGrowthQuarterOverQuarter.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.SalesGrowthQuarterOverQuarter.ToString(), translationSalesGrowthQuarterOverQuarter);

            translationCurrentRatio = new Dictionary<string, string>();
            translationCurrentRatio.Add("Any", "");
            translationCurrentRatio.Add("High (>3)", "fa_curratio_high");
            translationCurrentRatio.Add("Low (<0.5)", "fa_curratio_low");
            translationCurrentRatio.Add("Under 1", "fa_curratio_u1");
            translationCurrentRatio.Add("Under 0.5", "fa_curratio_u0.5");
            translationCurrentRatio.Add("Over 0.5", "fa_curratio_o0.5");
            translationCurrentRatio.Add("Over 1", "fa_curratio_o1");
            translationCurrentRatio.Add("Over 1.5", "fa_curratio_o1.5");
            translationCurrentRatio.Add("Over 2", "fa_curratio_o2");
            translationCurrentRatio.Add("Over 3", "fa_curratio_o3");
            translationCurrentRatio.Add("Over 4", "fa_curratio_o4");
            translationCurrentRatio.Add("Over 5", "fa_curratio_o5");
            translationCurrentRatio.Add("Over 10", "fa_curratio_o10");
            translationCurrentRatio.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.CurrentRatio.ToString(), translationCurrentRatio);

            translationOperatingMargin = new Dictionary<string, string>();
            translationOperatingMargin.Add("Any", "");
            translationOperatingMargin.Add("Positive (>0%)", "fa_opermargin_pos");
            translationOperatingMargin.Add("Negative (<0%)", "fa_opermargin_neg");
            translationOperatingMargin.Add("Very Negative (<-20%)", "fa_opermargin_veryneg");
            translationOperatingMargin.Add("High (>20%)", "fa_opermargin_high");
            translationOperatingMargin.Add("Under 90%", "fa_opermargin_u90");
            translationOperatingMargin.Add("Under 80%", "fa_opermargin_u80");
            translationOperatingMargin.Add("Under 70%", "fa_opermargin_u70");
            translationOperatingMargin.Add("Under 60%", "fa_opermargin_u60");
            translationOperatingMargin.Add("Under 50%", "fa_opermargin_u50");
            translationOperatingMargin.Add("Under 45%", "fa_opermargin_u45");
            translationOperatingMargin.Add("Under 40%", "fa_opermargin_u40");
            translationOperatingMargin.Add("Under 35%", "fa_opermargin_u35");
            translationOperatingMargin.Add("Under 30%", "fa_opermargin_u30");
            translationOperatingMargin.Add("Under 25%", "fa_opermargin_u25");
            translationOperatingMargin.Add("Under 20%", "fa_opermargin_u20");
            translationOperatingMargin.Add("Under 15%", "fa_opermargin_u15");
            translationOperatingMargin.Add("Under 10%", "fa_opermargin_u10");
            translationOperatingMargin.Add("Under 5%", "fa_opermargin_u5");
            translationOperatingMargin.Add("Under 0%", "fa_opermargin_u0");
            translationOperatingMargin.Add("Under -10%", "fa_opermargin_u-10");
            translationOperatingMargin.Add("Under -20%", "fa_opermargin_u-20");
            translationOperatingMargin.Add("Under -30%", "fa_opermargin_u-30");
            translationOperatingMargin.Add("Under -50%", "fa_opermargin_u-50");
            translationOperatingMargin.Add("Under -70%", "fa_opermargin_u-70");
            translationOperatingMargin.Add("Under -100%", "fa_opermargin_u-100");
            translationOperatingMargin.Add("Over 0%", "fa_opermargin_o0");
            translationOperatingMargin.Add("Over 5%", "fa_opermargin_o5");
            translationOperatingMargin.Add("Over 10%", "fa_opermargin_o10");
            translationOperatingMargin.Add("Over 15%", "fa_opermargin_o15");
            translationOperatingMargin.Add("Over 20%", "fa_opermargin_o20");
            translationOperatingMargin.Add("Over 25%", "fa_opermargin_o25");
            translationOperatingMargin.Add("Over 30%", "fa_opermargin_o30");
            translationOperatingMargin.Add("Over 35%", "fa_opermargin_o35");
            translationOperatingMargin.Add("Over 40%", "fa_opermargin_o40");
            translationOperatingMargin.Add("Over 45%", "fa_opermargin_o45");
            translationOperatingMargin.Add("Over 50%", "fa_opermargin_o50");
            translationOperatingMargin.Add("Over 60%", "fa_opermargin_o60");
            translationOperatingMargin.Add("Over 70%", "fa_opermargin_o70");
            translationOperatingMargin.Add("Over 80%", "fa_opermargin_o80");
            translationOperatingMargin.Add("Over 90%", "fa_opermargin_o90");
            translationOperatingMargin.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.OperatingMargin.ToString(), translationOperatingMargin);

            translationInstitutionalOwnership = new Dictionary<string, string>();
            translationInstitutionalOwnership.Add("Any", "");
            translationInstitutionalOwnership.Add("Low (<5%)", "sh_instown_low");
            translationInstitutionalOwnership.Add("High (>90%)", "sh_instown_high");
            translationInstitutionalOwnership.Add("Under 90%", "sh_instown_u90");
            translationInstitutionalOwnership.Add("Under 80%", "sh_instown_u80");
            translationInstitutionalOwnership.Add("Under 70%", "sh_instown_u70");
            translationInstitutionalOwnership.Add("Under 60%", "sh_instown_u60");
            translationInstitutionalOwnership.Add("Under 50%", "sh_instown_u50");
            translationInstitutionalOwnership.Add("Under 40%", "sh_instown_u40");
            translationInstitutionalOwnership.Add("Under 30%", "sh_instown_u30");
            translationInstitutionalOwnership.Add("Under 20%", "sh_instown_u20");
            translationInstitutionalOwnership.Add("Under 10%", "sh_instown_u10");
            translationInstitutionalOwnership.Add("Over 10%", "sh_instown_o10");
            translationInstitutionalOwnership.Add("Over 20%", "sh_instown_o20");
            translationInstitutionalOwnership.Add("Over 30%", "sh_instown_o30");
            translationInstitutionalOwnership.Add("Over 40%", "sh_instown_o40");
            translationInstitutionalOwnership.Add("Over 50%", "sh_instown_o50");
            translationInstitutionalOwnership.Add("Over 60%", "sh_instown_o60");
            translationInstitutionalOwnership.Add("Over 70%", "sh_instown_o70");
            translationInstitutionalOwnership.Add("Over 80%", "sh_instown_o80");
            translationInstitutionalOwnership.Add("Over 90%", "sh_instown_o90");
            translationInstitutionalOwnership.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.InstitutionalOwnership.ToString(), translationInstitutionalOwnership);

            translationEarningsDate = new Dictionary<string, string>();
            translationEarningsDate.Add("Any", "");
            translationEarningsDate.Add("Today", "earningsdate_today");
            translationEarningsDate.Add("Today Before Market Open", "earningsdate_todaybefore");
            translationEarningsDate.Add("Today After Market Close", "earningsdate_todayafter");
            translationEarningsDate.Add("Tomorrow", "earningsdate_tomorrow");
            translationEarningsDate.Add("Tomorrow Before Market Open", "earningsdate_tomorrowbefore");
            translationEarningsDate.Add("Tomorrow After Market Close", "earningsdate_tomorrowafter");
            translationEarningsDate.Add("Yesterday", "earningsdate_yesterday");
            translationEarningsDate.Add("Yesterday Before Market Open", "earningsdate_yesterdaybefore");
            translationEarningsDate.Add("Yesterday After Market Close", "earningsdate_yesterdayafter");
            translationEarningsDate.Add("Next 5 Days", "earningsdate_nextdays5");
            translationEarningsDate.Add("Previous 5 Days", "earningsdate_prevdays5");
            translationEarningsDate.Add("This Week", "earningsdate_thisweek");
            translationEarningsDate.Add("Next Week", "earningsdate_nextweek");
            translationEarningsDate.Add("Previous Week", "earningsdate_prevweek");
            translationEarningsDate.Add("This Month", "earningsdate_thismonth");
            translationEarningsDate.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.EarningsDate.ToString(), translationEarningsDate);

            translationGap = new Dictionary<string, string>();
            translationGap.Add("Any", "");
            translationGap.Add("Up", "ta_gap_u");
            translationGap.Add("Up 0%", "ta_gap_u0");
            translationGap.Add("Up 1%", "ta_gap_u1");
            translationGap.Add("Up 2%", "ta_gap_u2");
            translationGap.Add("Up 3%", "ta_gap_u3");
            translationGap.Add("Up 4%", "ta_gap_u4");
            translationGap.Add("Up 5%", "ta_gap_u5");
            translationGap.Add("Up 6%", "ta_gap_u6");
            translationGap.Add("Up 7%", "ta_gap_u7");
            translationGap.Add("Up 8%", "ta_gap_u8");
            translationGap.Add("Up 9%", "ta_gap_u9");
            translationGap.Add("Up 10%", "ta_gap_u10");
            translationGap.Add("Up 15%", "ta_gap_u15");
            translationGap.Add("Up 20%", "ta_gap_u20");
            translationGap.Add("Down", "ta_gap_d");
            translationGap.Add("Down 0%", "ta_gap_d0");
            translationGap.Add("Down 1%", "ta_gap_d1");
            translationGap.Add("Down 2%", "ta_gap_d2");
            translationGap.Add("Down 3%", "ta_gap_d3");
            translationGap.Add("Down 4%", "ta_gap_d4");
            translationGap.Add("Down 5%", "ta_gap_d5");
            translationGap.Add("Down 6%", "ta_gap_d6");
            translationGap.Add("Down 7%", "ta_gap_d7");
            translationGap.Add("Down 8%", "ta_gap_d8");
            translationGap.Add("Down 9%", "ta_gap_d9");
            translationGap.Add("Down 10%", "ta_gap_d10");
            translationGap.Add("Down 15%", "ta_gap_d15");
            translationGap.Add("Down 20%", "ta_gap_d20");
            translationGap.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Gap.ToString(), translationGap);

            translationChangeFromOpen = new Dictionary<string, string>();
            translationChangeFromOpen.Add("Any", "");
            translationChangeFromOpen.Add("Up", "ta_changeopen_u");
            translationChangeFromOpen.Add("Up 0%", "ta_changeopen_u0");
            translationChangeFromOpen.Add("Up 1%", "ta_changeopen_u1");
            translationChangeFromOpen.Add("Up 2%", "ta_changeopen_u2");
            translationChangeFromOpen.Add("Up 3%", "ta_changeopen_u3");
            translationChangeFromOpen.Add("Up 4%", "ta_changeopen_u4");
            translationChangeFromOpen.Add("Up 5%", "ta_changeopen_u5");
            translationChangeFromOpen.Add("Up 6%", "ta_changeopen_u6");
            translationChangeFromOpen.Add("Up 7%", "ta_changeopen_u7");
            translationChangeFromOpen.Add("Up 8%", "ta_changeopen_u8");
            translationChangeFromOpen.Add("Up 9%", "ta_changeopen_u9");
            translationChangeFromOpen.Add("Up 10%", "ta_changeopen_u10");
            translationChangeFromOpen.Add("Up 15%", "ta_changeopen_u15");
            translationChangeFromOpen.Add("Up 20%", "ta_changeopen_u20");
            translationChangeFromOpen.Add("Down", "ta_changeopen_d");
            translationChangeFromOpen.Add("Down 0%", "ta_changeopen_d0");
            translationChangeFromOpen.Add("Down 1%", "ta_changeopen_d1");
            translationChangeFromOpen.Add("Down 2%", "ta_changeopen_d2");
            translationChangeFromOpen.Add("Down 3%", "ta_changeopen_d3");
            translationChangeFromOpen.Add("Down 4%", "ta_changeopen_d4");
            translationChangeFromOpen.Add("Down 5%", "ta_changeopen_d5");
            translationChangeFromOpen.Add("Down 6%", "ta_changeopen_d6");
            translationChangeFromOpen.Add("Down 7%", "ta_changeopen_d7");
            translationChangeFromOpen.Add("Down 8%", "ta_changeopen_d8");
            translationChangeFromOpen.Add("Down 9%", "ta_changeopen_d9");
            translationChangeFromOpen.Add("Down 10%", "ta_changeopen_d10");
            translationChangeFromOpen.Add("Down 15%", "ta_changeopen_d15");
            translationChangeFromOpen.Add("Down 20%", "ta_changeopen_d20");
            translationChangeFromOpen.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.ChangeFromOpen.ToString(), translationChangeFromOpen);

            translationCandlestick = new Dictionary<string, string>();
            translationCandlestick.Add("Any", "");
            translationCandlestick.Add("Long Lower Shadow", "ta_candlestick_lls");
            translationCandlestick.Add("Long Upper Shadow", "ta_candlestick_lus");
            translationCandlestick.Add("Hammer", "ta_candlestick_h");
            translationCandlestick.Add("Inverted Hammer", "ta_candlestick_ih");
            translationCandlestick.Add("Spinning Top White", "ta_candlestick_stw");
            translationCandlestick.Add("Spinning Top Black", "ta_candlestick_stb");
            translationCandlestick.Add("Doji", "ta_candlestick_d");
            translationCandlestick.Add("Dragonfly Doji", "ta_candlestick_dd");
            translationCandlestick.Add("Gravestone Doji", "ta_candlestick_gd");
            translationCandlestick.Add("Marubozu White", "ta_candlestick_mw");
            translationCandlestick.Add("Marubozu Black", "ta_candlestick_mb");
            translationCandlestick.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Candlestick.ToString(), translationCandlestick);

            translationCurrentVolume = new Dictionary<string, string>();
            translationCurrentVolume.Add("Any", "");
            translationCurrentVolume.Add("Under 50K", "sh_curvol_u50");
            translationCurrentVolume.Add("Under 100K", "sh_curvol_u100");
            translationCurrentVolume.Add("Under 500K", "sh_curvol_u500");
            translationCurrentVolume.Add("Under 750K", "sh_curvol_u750");
            translationCurrentVolume.Add("Under 1M", "sh_curvol_u1000");
            translationCurrentVolume.Add("Over 0", "sh_curvol_o0");
            translationCurrentVolume.Add("Over 50K", "sh_curvol_o50");
            translationCurrentVolume.Add("Over 100K", "sh_curvol_o100");
            translationCurrentVolume.Add("Over 200K", "sh_curvol_o200");
            translationCurrentVolume.Add("Over 300K", "sh_curvol_o300");
            translationCurrentVolume.Add("Over 400K", "sh_curvol_o400");
            translationCurrentVolume.Add("Over 500K", "sh_curvol_o500");
            translationCurrentVolume.Add("Over 750K", "sh_curvol_o750");
            translationCurrentVolume.Add("Over 1M", "sh_curvol_o1000");
            translationCurrentVolume.Add("Over 2M", "sh_curvol_o2000");
            translationCurrentVolume.Add("Over 5M", "sh_curvol_o5000");
            translationCurrentVolume.Add("Over 10M", "sh_curvol_o10000");
            translationCurrentVolume.Add("Over 20M", "sh_curvol_o20000");
            translationCurrentVolume.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.CurrentVolume.ToString(), translationCurrentVolume);

            translationFloat = new Dictionary<string, string>();
            translationFloat.Add("Any", "");
            translationFloat.Add("Under 1M", "sh_float_u1");
            translationFloat.Add("Under 5M", "sh_float_u5");
            translationFloat.Add("Under 10M", "sh_float_u10");
            translationFloat.Add("Under 20M", "sh_float_u20");
            translationFloat.Add("Under 50M", "sh_float_u50");
            translationFloat.Add("Under 100M", "sh_float_u100");
            translationFloat.Add("Over 1M", "sh_float_o1");
            translationFloat.Add("Over 2M", "sh_float_o2");
            translationFloat.Add("Over 5M", "sh_float_o5");
            translationFloat.Add("Over 10M", "sh_float_o10");
            translationFloat.Add("Over 20M", "sh_float_o20");
            translationFloat.Add("Over 50M", "sh_float_o50");
            translationFloat.Add("Over 100M", "sh_float_o100");
            translationFloat.Add("Over 200M", "sh_float_o200");
            translationFloat.Add("Over 500M", "sh_float_o500");
            translationFloat.Add("Over 1000M", "sh_float_o1000");
            translationFloat.Add("Custom (Elite only)", "custom doesn't work");
            translationsMaster.Add(FinvzEnumFilterType.Float.ToString(), translationFloat);


        }
    }
}
