using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;


public class TeletextPageTravel(CeefaxContent mc, TravelData td)
{
    private readonly CeefaxContent _cc = mc;
    private readonly TravelData _td = td;

    #region Public Methods

    public StringBuilder CreateTravelPage()
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Travel);
        StringBuilder sb = new();

        if(_td.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderTravel);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]STONEGATE DEPARTURES");
            OutputRecords(sb, _td.SOGDepartures.Take(5).ToList());
            
            sb.AppendLine($"[{TeletextControl.AlphaGreen}]Data accurate as at " + Utility.ConvertToUKTime(_td.LastRefreshUTC).ToString("HH:mm") + " (UK time)");
            sb.AppendLine("");
            sb.FooterText(section);
        }

        return sb;
    }

    public void OutputRecords(StringBuilder sb, List<TravelRecord> records)
    {
        if(records.Count == 0)
        {
            sb.AppendLine("");
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]No upcoming services");

            // Pad to section end
            sb.PadLines(14);
        }
        else
        {
            foreach(var t in records)
            {
                TeletextControl alertCol = t.Status.StartsWith("On time") ? TeletextControl.AlphaGreen : TeletextControl.AlphaRed;
                sb.LineBreak(TeletextControl.AlphaBlue);
                sb.AppendLine($"[{TeletextControl.AlphaWhite}]{t.Destination.ToUpper().PadRightWithTrunc(24)}[{TeletextControl.AlphaWhite}]{t.Time.PadRightWithTrunc(4)}[{alertCol}]{t.Status.PadLeftWithTrunc(9)}");
                sb.AppendLine($"[{TeletextControl.AlphaCyan}]Platform {t.Platform} {t.OperatedBy}");
            }
            sb.LineBreak(TeletextControl.AlphaBlue);

            // Pad to section end
            sb.PadLines(15 - records.Count * 3);
        }
    }

    #endregion
}