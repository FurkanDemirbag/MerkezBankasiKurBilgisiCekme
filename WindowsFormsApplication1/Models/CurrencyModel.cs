using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApplication1.Models
{
    public class CurrencyModel
    {
        public class Tarih_Date
        {
            public DateTime Tarih { get; set; }
            public DateTime Date { get; set; }
            public String Bulten_No { get; set; }
            public List<Currency> Currency { get; set; }
        }

        public class Currency
        {
            public int CrossOrder { get; set; }
            public String Kod { get; set; }
            public String CurrencyCode { get; set; }
            public int Unit { get; set; }
            public String Isim { get; set; }
            public String CurrencyName { get; set; }
            public String ForexBuying { get; set; }
            public String ForexSelling { get; set; }
            public String BanknoteBuying { get; set; }
            public String BanknoteSelling { get; set; }
            public String CrossRateUSD { get; set; }
            public String CrossRateOther { get; set; }
        }

        public void Save(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(CurrencyModel));
                XML.Serialize(stream, this);
            }
        }

        public static CurrencyModel LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(CurrencyModel));

                return (CurrencyModel) XML.Deserialize(stream);
            }
        }
    }
}
