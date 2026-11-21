using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;


public class TeletextPageTravel(CeefaxContent mc, TravelData td)
{
    private readonly CeefaxContent _cc = mc;
    private readonly TravelData _td = td;

    #region Public Methods

    public StringBuilder CreateTravelTrainsPage()
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.TravelTrains);
        StringBuilder sb = new();

        if(_td.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(section.Header);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]STONEGATE DEPARTURES");
            OutputTrainRecord(sb, _td.SOGDepartures.Take(5).ToList());
            
            sb.AppendLine($"[{TeletextControl.AlphaGreen}]Updated at " + Utility.ConvertToUKTime(_td.LastRefreshUTC).ToString("HH:mm") + " (UK time)");
            sb.AppendLine("");
            sb.FooterText(section, true);
        }

        return sb;
    }

    public void OutputTrainRecord(StringBuilder sb, List<TravelRecord> records)
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
                sb.LineBreak(TeletextControl.AlphaBlue);
                TeletextControl alertCol = t.Status.StartsWith("On time") ? 
                    TeletextControl.AlphaGreen : t.Status.StartsWith("Exp.") ? TeletextControl.AlphaYellow : TeletextControl.AlphaRed;
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