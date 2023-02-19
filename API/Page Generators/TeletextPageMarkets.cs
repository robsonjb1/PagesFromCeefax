using System.Text;
using API.Architecture;
using API.DataTransferObjects;
using API.Magazine;
using API.Services;
using static System.Collections.Specialized.BitVector32;

namespace API.PageGenerators
{
    public interface ITeletextPageMarkets
    {
        public StringBuilder CreateMarketsPage();
        public StringBuilder DiskCreateMarketsPage();
    }

    public class TeletextPageMarkets : ITeletextPageMarkets
    {
        private readonly MarketData _md;
        
        public TeletextPageMarkets(IMarketService ms)
        {
            _md = ms.GetMarketData();
        }

        #region Public Methods
        public StringBuilder CreateMarketsPage()
        {
            StringBuilder sb = new();
            sb.Append(Graphics.HeaderMarkets);

            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">UK MARKETS</span></p>");
            sb.Append(OutputMarket("FTSE 100"));
            sb.Append(OutputMarket("FTSE 250"));
            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">EUROPE MARKETS</span></p>");
            sb.Append(OutputMarket("AEX"));
            sb.Append(OutputMarket("DAX"));
            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">US MARKETS</span></p>");
            sb.Append(OutputMarket("Dow Jones"));
            sb.Append(OutputMarket("Nasdaq"));
            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">ASIA MARKETS</span></p>");
            sb.Append(OutputMarket("Hang Seng"));
            sb.Append(OutputMarket("Nikkei 225"));
            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

            sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">CURRENCIES</span></p>");
            sb.Append(OutputCurrency("Euro"));
            sb.Append(OutputCurrency("USD"));
           
            sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Red} ink{(int)Mode7Colour.White}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");

            return sb;
        }

        public StringBuilder DiskCreateMarketsPage()
        {
            StringBuilder sb = new();

            sb.AppendLine(String.Concat(
                 (char)Utility.ConvertToAsciiColour(Mode7Colour.Yellow),
                 "UK MARKETS"));
            sb.Append(DiskOutputMarket("FTSE 100"));
            sb.Append(DiskOutputMarket("FTSE 250"));
            sb.AppendLine(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.Red),
                string.Join("", Enumerable.Repeat((char)224, 39))));

            sb.AppendLine(String.Concat(
                 (char)Utility.ConvertToAsciiColour(Mode7Colour.Yellow),
                 "EUROPE MARKETS"));
            sb.Append(DiskOutputMarket("AEX"));
            sb.Append(DiskOutputMarket("DAX"));
            sb.AppendLine(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.Red),
                string.Join("", Enumerable.Repeat((char)224, 39))));

            sb.AppendLine(String.Concat(
                 (char)Utility.ConvertToAsciiColour(Mode7Colour.Yellow),
                 "US MARKETS"));
            sb.Append(DiskOutputMarket("Dow Jones"));
            sb.Append(DiskOutputMarket("Nasdaq"));
            sb.AppendLine(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.Red),
                string.Join("", Enumerable.Repeat((char)224, 39))));

            sb.AppendLine(String.Concat(
                 (char)Utility.ConvertToAsciiColour(Mode7Colour.Yellow),
                 "ASIA MARKETS"));
            sb.Append(DiskOutputMarket("Hang Seng"));
            sb.Append(DiskOutputMarket("Nikkei 225"));
            sb.AppendLine(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.Red),
                string.Join("", Enumerable.Repeat((char)224, 39))));

            sb.AppendLine(String.Concat(
                 (char)Utility.ConvertToAsciiColour(Mode7Colour.Yellow),
                 "CURRENCIES"));
            sb.Append(DiskOutputCurrency("Euro"));
            sb.Append(DiskOutputCurrency("USD"));

            return sb;
        }
        #endregion

        #region Private Methods

        private StringBuilder OutputMarket(string marketName)
        {
            StringBuilder sb = new();

            MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

            if(mr != null)
            {
                sb.Append($"<p><span class=\"indent ink{(int)Mode7Colour.White}\">");
                sb.Append(mr.Name.PadRight(14, ' ').Replace(" ", "&nbsp;"));
                sb.Append(mr.Value.PadLeft(9, ' ').Replace(" ", "&nbsp;"));
                if (mr.Movement.StartsWith("-"))
                {
                    sb.Append($"<span class=\"ink{(int)Mode7Colour.Red}\">");
                }
                else
                {
                    sb.Append($"<span class=\"ink{(int)Mode7Colour.Green}\">");
                }
                sb.Append($"&nbsp;&nbsp;{mr.Movement}");
                sb.Append($"</span><span class=\"ink{(int)Mode7Colour.Cyan}\">");
                sb.Append(mr.Closed ? "&nbsp;&nbsp;Closed" : "");
                sb.Append("</span></p>");
            }

            return sb;
        }

        private StringBuilder DiskOutputMarket(string marketName)
        {
            StringBuilder sb = new();

            MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

            if (mr != null)
            {
                sb.Append(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.White),
                mr.Name.PadRight(14),
                mr.Value.PadLeft(9)));

                if (mr.Movement.StartsWith("-"))
                {
                    sb.Append((char)Utility.ConvertToAsciiColour(Mode7Colour.Red));
                }
                else
                {
                    sb.Append((char)Utility.ConvertToAsciiColour(Mode7Colour.Green));
                }

                sb.Append($" {mr.Movement}");
                sb.AppendLine(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.Cyan),
                mr.Closed ? " Closed" : ""));
            }

            return sb;
        }

        private StringBuilder OutputCurrency(string currency)
        {
            StringBuilder sb = new();

            MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name.StartsWith("GBP") && z.Name.EndsWith(currency));

            if (mr != null)
            {
                sb.Append($"<p><span class=\"indent ink{(int)Mode7Colour.White}\">{currency.PadRight(16, ' ').Replace(" ", "&nbsp")}");
                sb.Append(mr.Value.PadRight(9, ' ').Replace(" ", "&nbsp;"));

                if (mr.Movement.StartsWith("-"))
                {
                    sb.Append($"<span class=\"ink{(int)Mode7Colour.Red}\">");
                }
                else
                {
                    sb.Append($"<span class=\"ink{(int)Mode7Colour.Green}\">");
                }
                sb.Append(mr.Movement);
                sb.Append("</span></p>");
            }

            return sb;
        }

        private StringBuilder DiskOutputCurrency(string currency)
        {
            StringBuilder sb = new();

            MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name.StartsWith("GBP") && z.Name.EndsWith(currency));

            if (mr != null)
            {
                sb.Append(String.Concat(
                (char)Utility.ConvertToAsciiColour(Mode7Colour.White),
                currency.PadRight(16),
                mr.Value.Replace("€", "E").PadRight(8)));

                if (mr.Movement.StartsWith("-"))
                {
                    sb.Append((char)Utility.ConvertToAsciiColour(Mode7Colour.Red));
                }
                else
                {
                    sb.Append((char)Utility.ConvertToAsciiColour(Mode7Colour.Green));
                }
                sb.AppendLine(mr.Movement);
            }

            return sb;
        }

        #endregion
    }
}