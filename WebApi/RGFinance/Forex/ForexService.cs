using Database;
using Database.Entities;
using RGFinance.Forex;
using System.Xml;
using System.Xml.Linq;

namespace RGFinance.FlowFeature
{
    public class ForexService : IForexService
    {
        private readonly RGFContext context;

        public ForexService(RGFContext context)
        {
            this.context = context;
        }
         
        public async Task<Forex> GetForex()
        {
            // From StackOverflow - I have no idea how and why it works ;)

            XmlDocument doc = new XmlDocument();
            doc.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");

            // add another namespace alias for the *default* namespace
            nsmgr.AddNamespace("default", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");

            // *USE* the default namespace for those nodes that don't have an explicit
            // XML namespace alias in your XML document
            XmlNodeList nodes = doc.SelectNodes("gesmes:Envelope/default:Cube", nsmgr);

            // Time
            XmlNode timeNode = nodes[0].SelectSingleNode("default:Cube", nsmgr);
            string time = timeNode.Attributes["time"].Value;

            /* ALTERNATIVE */
            XDocument xdoc = XDocument.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            List<CurrencyRate>? res2 = xdoc.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null)
                .Select(x => new CurrencyRate
                {
                    Currency = (string)x.Attribute("currency"),
                    Rate = (decimal)x.Attribute("rate")
                })
                .ToList();

            var usdeur = res2.Single(cr => cr.Currency == "USD").Rate;
            var plneur = res2.Single(cr => cr.Currency == "PLN").Rate;
            var plnusd = plneur / usdeur;

            // GOLD
            var goldJsonString = await new HttpClient().GetStringAsync("https://data-asg.goldprice.org/dbXRates/USD");
            int xauPrice = goldJsonString.IndexOf("xauPrice");
            int colonIndex = goldJsonString.IndexOf(":", xauPrice);
            int comaIndex = goldJsonString.IndexOf(".", colonIndex);
            string goldPriceString = goldJsonString.Substring(colonIndex + 1, comaIndex - colonIndex - 1);
            decimal goldPriceUsd = decimal.Parse(goldPriceString, System.Globalization.NumberStyles.Number);
            decimal goldPricePln = goldPriceUsd * plnusd;

            var forex = new Forex
            {
                Time = time,
                Usd = plnusd,
                Eur = plneur,
                Gold = goldPricePln,
            };

            return forex;
        }
    }
}
