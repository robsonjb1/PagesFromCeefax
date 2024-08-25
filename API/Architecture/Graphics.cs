using System.Text;

namespace API.Architecture;

public class Graphics
{
    // Page headers
    public readonly static StringBuilder HeaderHome = new();
    public readonly static StringBuilder HeaderSussex = new();
    public readonly static StringBuilder HeaderWorld = new();
    public readonly static StringBuilder HeaderPolitics = new();
    public readonly static StringBuilder HeaderSciTech = new();
    public readonly static StringBuilder HeaderBusiness = new();
    public readonly static StringBuilder HeaderFootball = new();
    public readonly static StringBuilder HeaderRugby = new();
    public readonly static StringBuilder HeaderCricket = new();
    public readonly static StringBuilder HeaderGolf = new();
    public readonly static StringBuilder HeaderFormula1 = new();
    public readonly static StringBuilder HeaderTennis = new();
    public readonly static StringBuilder HeaderEntertainment = new();
    public readonly static StringBuilder HeaderTV = new();
    public readonly static StringBuilder HeaderMarkets = new();
    public readonly static StringBuilder HeaderWeather = new();
    public readonly static StringBuilder HeaderLinks = new();
    
    // Transition screens
    public readonly static StringBuilder PromoMap = new();
    public readonly static StringBuilder PromoWeather = new();
    public readonly static StringBuilder PromoBusiness = new();
    public readonly static StringBuilder PromoSport = new();
    public readonly static StringBuilder PromoTV = new();
    public readonly static StringBuilder PromoNews = new();
    public readonly static StringBuilder PromoTeletext = new();
    public readonly static StringBuilder PromoLinks = new();
    public readonly static StringBuilder PromoChristmas = new();
    public readonly static StringBuilder PromoLoading = new();


    static Graphics()
    {
        HeaderHome.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]    h4h4|,|h<<|h<$");
        HeaderHome.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]    j7k5¬p¬j55¬jw1");
        HeaderHome.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]//////-.-.,,,-..,-,.//////");
       
        HeaderWorld.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   |hh4|,|h<l4| h<l0");
        HeaderWorld.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   ozz%¬p¬j7k4¬pjuz%");
        HeaderWorld.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]/////-,,/,,,-.-.,,-,,/////");

        HeaderPolitics.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] h<|h<|h4 |(|$|h<$|,$");
        HeaderPolitics.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] j7£ju¬ju0¬ ¬ ¬ju0s{{5");
        HeaderPolitics.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]///-./-,,-,.,/,/,-,.,,.///");

        HeaderSciTech.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] x,$x,h4 (|$|,_<$|h4");
        HeaderSciTech.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] s{{%opj5£ ¬ ¬s*u0¬k5");
        HeaderSciTech.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]///,,/-,-.//,/,,/,.,-.////");

        HeaderSussex.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   x,$|h4x,$x,$|,h4|");
        HeaderSussex.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   s{{%oz%s{{%s{{%¬sh7}}");
        HeaderSussex.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]/////,,/-,/,,/,,/,,-.,////");

        HeaderBusiness.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}] |l4|h4|,$|_<th<h<,h<,");
        HeaderBusiness.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}] ¬{{4oz%s{{5¬j5¬jwbs¬bs¬");
        HeaderBusiness.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsRed}]///,,.-,/,,.,-.,-,-,,-,,//");

        HeaderMarkets.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}] xll0|l4|l4|h4|$l<h<,");
        HeaderMarkets.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}] ¬jj5¬k5¬k4¬k4¬1j5bs¬");
        HeaderMarkets.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsRed}]///,--.,-.,-.,-.,.-.-,,///");

        HeaderFootball.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}] h<h<|h<|(|$|l4|l4| |");
        HeaderFootball.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}] j7ju¬ju¬ ¬ ¬{{4¬k5¬0¬0");
        HeaderFootball.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]///-.-,,-,,/,/,,.,-.,.,.//");

        HeaderRugby.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]<4hhh,$<4hh hhhlhhlhl");
        HeaderRugby.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]7kjzjr5w{{b{{ jzjjjjzjj");
        HeaderRugby.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]//.--,-,.,,-,/-,----,--///");

        HeaderCricket.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]   h<$|l4|h<$|h4|$l<");
        HeaderCricket.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]   ju0¬k4¬ju0¬k4¬1j5");
        HeaderCricket.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]/////-,.,-.,-,.,-.,.-.////");

        HeaderTennis.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]   (|$|$xl0xl0|h<,");
        HeaderTennis.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]    ¬ ¬1¬j5¬j5¬bs¬");
        HeaderTennis.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]//////,/,.,-.,-.,-,,//////");

        HeaderGolf.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]      |,,h<|h4 |,       ");
        HeaderGolf.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]      ¬r¬ju¬ju0¬£       ");
        HeaderGolf.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]////////,,,-,,-,.,////////");

        HeaderFormula1.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]<<4<l(<h,4<4h,hlh,4<4l$");
        HeaderFormula1.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]555uz 5jp57kb{{j£jp57kj");
        HeaderFormula1.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]//...,,/.-,..--,-/-,..--//");

        HeaderEntertainment.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsMagenta}]|$xl0l<h<h<t(|$|l4|_<t_<<th<_<t(|$");
        HeaderEntertainment.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsMagenta}]¬1¬j5j5jwj7}} ¬ ¬k5¬j5¬j55¬jwj5¬ ¬");
        HeaderEntertainment.AppendLine($"[{TeletextControl.GraphicsYellow}]£££££££££££££££££££££££££££££££££££££££");

        HeaderTV.AppendLine($"[{TeletextControl.GraphicsCyan}]h<,,,,,,||[{TeletextControl.AlphaCyan}]____________________________");
        HeaderTV.AppendLine($"[{TeletextControl.GraphicsCyan}]j5k7j5¬ ¬n[{TeletextControl.GraphicsYellow}]¬{{%¬{{%¬+%{{ChannelTop}}[{TeletextControl.AlphaWhite}]{{DayOfWeek}}");
        HeaderTV.AppendLine($"[{TeletextControl.GraphicsCyan}]j5j5\"m' ¬¬[{TeletextControl.GraphicsYellow}]¬z5¬z5¬x4{{ChannelBottom}}[{TeletextControl.AlphaWhite}]      {{TimeSpan}}");
        HeaderTV.AppendLine($"[{TeletextControl.GraphicsCyan}]*-,,,,,,/.[{TeletextControl.AlphaCyan}]____________________________");

        HeaderWeather.AppendLine($"[{TeletextControl.GraphicsWhite}]j£3kj£3kj£3k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] h44|h<h<|(|$|h4|$|l");
        HeaderWeather.AppendLine($"[{TeletextControl.GraphicsWhite}]j $kj $kj 'k[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] *uu?jwj7¬ ¬ ¬k5¬1¬k4");
        HeaderWeather.AppendLine($"[{TeletextControl.GraphicsWhite}]\"£££\"£££\"£££[{TeletextControl.GraphicsBlue}]////,,.-,-.,/,/,-.,.,-.///");

        //HeaderTop40Singles.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  l<h<|h<| |_ h<|  |,$|_<th<,$| |$|,$  ") + "</span></p>");
        //HeaderTop40Singles.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  j5ju¬j7£ /n,ju¬  s{5¬j5¬ju{5¬0¬1s{5  ") + "</span></p>");
        //HeaderTop40Singles.AppendLine("<p><span class=\"ink6\">" + Utility.BlockGraph("£££££££££££££££££££££££££££££££££££££££") + "</span></p>");

        //HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper5 ink1\">" + Utility.BlockGraph("   (l<$|,|h<l4| h4|,h4    ") + "</span></p>");
        //HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper5 ink1\">" + Utility.BlockGraph("    j5 ¬£}j7k5\"m' ¬sju0   ") + "</span></p>");
        //HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink5\">" + Utility.BlockGraph("££££££££££££££££££££££££££") + "</span></p>");

        //HeaderShares.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("    |,$| |h<l4|,|h<$|,$   ") + "</span></p>");
        //HeaderShares.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("    s{5¬£¬j7k5¬£}jw1s{5   ") + "</span></p>");
        //HeaderShares.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink2\">" + Utility.BlockGraph("////,,.,/,-.-.,/,-,.,,.///") + "</span></p>");

        //HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("  h4|h<h<|h<th4h4xl0|$|,  ") + "</span></p>");
        //HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("  j7¬jwj7¬ju?juj5¬j5¬1s¬  ") + "</span></p>");
        //HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//-.,-,-.,-,.-,-.,-.,.,,//") + "</span></p>");


        PromoLoading.AppendLine(String.Empty);
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}] YOU ARE WATCHING ITEMS OF NEWS");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}] YOU ARE WATCHING ITEMS OF NEWS");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}]      AND INFORMATION FROM");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}]      AND INFORMATION FROM");
        PromoLoading.AppendLine(String.Empty);
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]  |<,,l4    (l||||    (l||||");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]  ¬5j}}~5?///¬j ,l¬j///¬j , ¬j£¬£¬");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]  ¬urs{{55bs{{¬jpss¬j s{{¬jp¬p¬j!|\"¬");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]  £££££q}}|||¬\"£££cz|¬¬¬\"£££cz|¬|¬");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}]THE BBC TELETEXT SERVICE 1974-2012");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}][{TeletextControl.DoubleHeight}]THE BBC TELETEXT SERVICE 1974-2012");
        PromoLoading.AppendLine(String.Empty);
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]   This site is a tribute to the");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaCyan}]in-vision[{TeletextControl.AlphaYellow}]service, often shown late");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]   at night to British TV viewers.");
        PromoLoading.AppendLine(String.Empty);
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]So enjoy life in a slower lane with");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]a little light music and some[{TeletextControl.AlphaGreen}][{TeletextControl.FlashOn}]NEW!");
        PromoLoading.AppendLine(String.Empty);
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaCyan}][{TeletextControl.DoubleHeight}]    ... PAGES FROM CEEFAX ...");
        PromoLoading.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaCyan}][{TeletextControl.DoubleHeight}]    ... PAGES FROM CEEFAX ...");
        PromoLoading.AppendLine(String.Empty);
        
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                     _4_~¬|¬}}     [{TeletextControl.AlphaWhite}]1/5");
        PromoMap.AppendLine($"[LINE1][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]_?0~[FF]%");
        PromoMap.AppendLine($"[LINE2][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]j*¬¬¬¬¬y||t");
        PromoMap.AppendLine($"[LINE3][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}] ({{¬¬¬¬¬¬¬?");
        PromoMap.AppendLine($"[LINE4][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}] !'~¬¬¬¬¬¬!");
        PromoMap.AppendLine($"[LINE5][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]  ~o¬¬¬¬w1");
        PromoMap.AppendLine($"[LINE6][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]  ~o¬¬¬¬wq");
        PromoMap.AppendLine($"[LINE7][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]   j)\"¬[DD]¬}}");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                 p|¬t  x¬¬g¬¬¬¬0");
        PromoMap.AppendLine($"[{TeletextControl.AlphaWhite}]Current[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]        n[EE]t +\" ¬¬¬¬¬¬");
        PromoMap.AppendLine($"[{TeletextControl.AlphaWhite}]temperatures[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]   +¬¬¬¬¬ _4 +¬¬¬¬¬|0");
        PromoMap.AppendLine($"[{TeletextControl.AlphaWhite}]at [TTT][{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]        \"%*o! \"   *¬¬¬¬¬u");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                           *¬[CC]¬}}");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                       k||¬¬¬¬¬¬¬¬¬");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                    4  '¬¬¬¬¬¬¬¬¬¬}}z¬t");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                   j!   j¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]               ppp<'  x|[BB]¬¬¬¬¬¬¬¬¬'");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]             \"£!      ! +/'i¬¬[AA]¬qp0");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                         t¬¬¬¬¬¬¬¬¬¬¬'");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                  [GG]¬¬¬¬¬¬¬¬?s///?!");
        PromoMap.AppendLine($"[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]                    8?' \"'     \"");

        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]¬wwwwwwwwwwwwww¬¬    j¬1*z¬2%_z}}?g   ");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]jo¬¬¬¬¬¬¬¬/????o¬    \"£.£££(, £{{co7_ ");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]$(\"¬¬¬¬¬¬¬b5555{{¬              _-6%44");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]oqh4?/?/??>-=l=.¬               ! zuu");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}] +tx5q55555555u3¬                )_~o");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]  kdm/}}|}}|}}}}}}~}}|¬      p p0       *)!");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]  j_0h/¬¬¬¬¬¬¬¬¬¬    _~¬¬¬¬¬t      _!");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]  **%  /////////     j¬¬¬¬¬¬¬4");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]¬sj5¬k5¬{{5¬k5¬£jw![{TeletextControl.GraphicsCyan}] _0£o?£sro}}");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]¬ j5¬j5¬j5¬j5¬pju0[{TeletextControl.GraphicsCyan}]    \"¬\"££o¬4");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   pp0pp_0 _0pp0[{TeletextControl.GraphicsCyan}]    9 h¬ty|~¬}}");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   ¬j5¬$j5pj5/l4[{TeletextControl.GraphicsCyan}]      z¬¬¬¬¬¬¬?0");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]   /*%/,*-/.%,.%[{TeletextControl.GraphicsCyan}]    _ ooo¬¬¬¬¬?%");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}] $p¬o/¬¬¬¬¬¬¬¬¬¬       j¬¬¬¬¬¬¬ $");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]   ?|s¬¬¬¬¬¬¬¬¬¬         £s¬¬¬¬u");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}] a¬)r¬¬¬¬¬¬¬¬¬y¬       _x¬¬¬¬¬¬¬");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}] 8£|~¬¬¬¬¬¬¬¬¬¬        j¬¬¬¬?¬¬¬");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]\"px¬¬¬¬¬¬¬¬¬¬¬£         £gsx~¬?!");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}](\"¬¬¬¬¬¬¬¬¬¬¬¬           j¬¬¬7");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}](¬¬¬¬¬¬¬¬¬¬¬¬            j¬?!");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}] l¬¬¬¬¬¬¬¬¬¬         $ ($ !      h¬¬}}");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}][{TeletextControl.DoubleHeight}]Follows in a moment...[{TeletextControl.GraphicsCyan}]4        ¬¬¬");
        PromoBusiness.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}][{TeletextControl.DoubleHeight}]Follows in a moment...[{TeletextControl.GraphicsCyan}]4        ¬¬¬");

        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬4¬46j4¬44¬¬¬¬¬¬¬¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬u+6$%'5*'bj_ *ha/7¬¬¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬7+3l+ ( !      ! &$xo¬¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬?t;d!             :a>a~¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬/mp)p     [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]ppp[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]    d':g<o?");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬$2-   [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]x¬¬¬¬¬¬}}0[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]  f a&ax");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬w;, £  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]z££££¬£££m0[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]  $p,g{{");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬?-,  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]hu&£££¬£££d}}[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]     cq");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬7+-,  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]~¬    ¬   j¬4  [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]bssx");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬//!  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]¬¬¬¬?%¬*o¬¬¬5  [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]bqrq");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬=,%  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]k¬¬¬ursqz¬¬¬!  [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}],,t¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬}}<&c0  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]o¬u£ss3a¬¬7  [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]£,ltr");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬=/s,  [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]\"o¬}}2'x¬¬'   [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]ds/l¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬|?q$9   [{TeletextControl.ContiguousGraphics}][{TeletextControl.GraphicsYellow}]+/¬¬¬/!    [{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]-rit{{");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬7y/9!_              f$2o¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬~}}>x%2  0         4e+t¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬¬wy%:xa_        'o;t/¬¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsCyan}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬>i1&6j ~ e_jj42}}¬~¬¬¬¬¬");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsBlue}] [{TeletextControl.NewBackground}]");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsBlue}] [{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]¬j5¬      n=j5        ¬k5           ");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsBlue}] [{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]¬j5¬jw¬bs¬j5j7¬jw¬j7¬ ¬j5¬{{5¬j5¬jws ");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsBlue}] [{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]¬zu¬ju|ju¬j5j5¬ju|j5  ¬j5¬x4¬zu¬ht¬ ");
        PromoWeather.AppendLine($"[{TeletextControl.GraphicsBlue}] [{TeletextControl.NewBackground}]");
        
        
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]     _r¬q0      [{TeletextControl.GraphicsYellow}]p0");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]     ,,,,,      [{TeletextControl.GraphicsBlue}]¬5");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}] 7d_~¬¬¬¬¬}}08k  [{TeletextControl.GraphicsBlue}]¬5    [{TeletextControl.GraphicsCyan}]¬sjw¬j7¬jw?\"¬!");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}] 5 i|||||||6 j  [{TeletextControl.GraphicsBlue}]¬5    [{TeletextControl.GraphicsCyan}]p¬j5 ju¬j5¬ ¬");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}] 5 j¬¬¬¬¬¬¬5 j  [{TeletextControl.GraphicsBlue}]¬5");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}] 5 j¬¬¬¬¬¬¬5 j  [{TeletextControl.GraphicsBlue}]¬5    [{TeletextControl.GraphicsCyan}]j7¬jw1¬_0¬jw1");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}] \",z¬¬¬¬¬¬¬u,!  [{TeletextControl.GraphicsBlue}]¬5    [{TeletextControl.GraphicsCyan}]j5¬ju0¬zu¬_z5");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]    k¬¬¬¬¬7    [{TeletextControl.GraphicsYellow}]x¬}}0");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]     ,l|<,    [{TeletextControl.GraphicsYellow}]j}};y¬[{TeletextControl.AlphaCyan}]      follows...");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]      \"¬!     [{TeletextControl.GraphicsYellow}]j¬5¬¬");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsWhite}]    _x~¬}}t0   [{TeletextControl.GraphicsYellow}]j¬5¬¬");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬       [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]p|¬¬|p");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬      [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]~¬¬¬¬¬¬}}");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬     [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]z¬¬¬¬¬¬¬¬u");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsRed}] _pppppp      [{TeletextControl.GraphicsYellow}]j¬5¬¬    [{TeletextControl.SeparatedGraphics}][{TeletextControl.GraphicsGreen}]h¬¬¬¬¬¬¬¬¬¬4");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsRed}][{TeletextControl.HoldGraphics}]j¬¬¬¬¬¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬[{TeletextControl.GraphicsYellow}]j¬5¬[{TeletextControl.GraphicsCyan}]¬¬¬¬[{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]¬¬¬¬¬¬¬¬¬¬¬¬ ");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsRed}] \"££££££      [{TeletextControl.GraphicsYellow}]j¬5¬¬    [{TeletextControl.SeparatedGraphics}][{TeletextControl.GraphicsGreen}]*¬¬¬¬¬¬¬¬¬¬%");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬     [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]k¬¬¬¬¬¬¬¬7");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               j¬5¬¬      [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]o¬¬¬¬¬¬?");
        PromoSport.AppendLine($"[{TeletextControl.GraphicsYellow}]               *¬}}¬?       [{TeletextControl.GraphicsGreen}][{TeletextControl.SeparatedGraphics}]£/¬¬/£");
        PromoSport.AppendLine(String.Empty);
        

        PromoTV.AppendLine($"[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaRed}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsCyan}]pppp_ppp0pppp[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬9¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬j¬¬¬5¬¬¬¬[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬9¬¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsCyan}]||||h|||4||||[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬¬9¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsGreen}]¬¬¬¬j¬¬¬5¬¬¬¬[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsGreen}]¬¬¬¬j¬¬¬5¬¬¬¬[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬[{TeletextControl.GraphicsGreen}]££££\"£££!££[{TeletextControl.GraphicsCyan}]pppppppppp  [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsCyan}]?1;¬¬¬¬¬¬¬  [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsCyan}]¬5¬¬¬¬?'/¬  [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬5¬¬¬¬¬||¬  [{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬ ££££££££££   ¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬jj¬¬¬¬¬¬¬¬55¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬jj¬¬¬¬¬¬¬¬55¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.GraphicsYellow}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬z¬¬¬¬¬¬¬¬¬¬u¬¬¬¬¬¬¬¬¬¬");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.DoubleHeight}][{TeletextControl.BlackBackground}]TV AND ENTERTAINMENT IN A MOMENT [{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.DoubleHeight}][{TeletextControl.BlackBackground}]TV AND ENTERTAINMENT IN A MOMENT [{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}]");
        PromoTV.AppendLine(String.Empty);

        PromoTeletext.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]£¬£h<| ¬ h<| ¬$h<| |h4j=   sh<$");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] ¬ jws ¬0jws ¬0jws ~k4ju   ¬b{{5000");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]");
        PromoTeletext.AppendLine(String.Empty);
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬/////////////////¬¬¬¬¬[{TeletextControl.AlphaYellow}] ...pages of");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]CEEFAX ¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬~¬¬¬[{TeletextControl.AlphaYellow}] information");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬~¬¬¬[{TeletextControl.AlphaYellow}] summoned[{TeletextControl.AlphaCyan}]by");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬~¬¬¬[{TeletextControl.AlphaCyan}] you[{TeletextControl.AlphaYellow}]to your");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬[{TeletextControl.AlphaYellow}] tv screen at");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬[{TeletextControl.AlphaYellow}] the touch of");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬[{TeletextControl.AlphaYellow}] numbered keys");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬[{TeletextControl.AlphaYellow}] on a remote-");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬[{TeletextControl.AlphaYellow}] control pad.");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬[{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.GraphicsBlue}]¬¬¬¬¬");
        PromoTeletext.AppendLine($"[{TeletextControl.GraphicsBlue}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬[{TeletextControl.AlphaCyan}] You[{TeletextControl.AlphaYellow}]choose");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaWhite}]                   [{TeletextControl.UpArrow}]     [{TeletextControl.AlphaYellow}]what to read");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaYellow}]THE BBC       [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬[{TeletextControl.AlphaYellow}] and when to");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaYellow}]TELETEXT      [{TeletextControl.GraphicsRed}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]1 2 3  [{TeletextControl.BlackBackground}][{TeletextControl.AlphaYellow}]read it...");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaYellow}]SERVICE       [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaYellow}]IS CALLED     [{TeletextControl.GraphicsRed}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]4 5 6  [{TeletextControl.BlackBackground}]");
        PromoTeletext.AppendLine($"[{TeletextControl.AlphaWhite}]CEEFAX        [{TeletextControl.GraphicsRed}]¬¬¬¬¬¬¬¬¬");
        PromoTeletext.AppendLine(String.Empty);        

        PromoNews.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]");
        PromoNews.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]£¬£h<| ¬ h<| ¬$h<| |h4j=   sh<$");
        PromoNews.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] ¬ jws ¬0jws ¬0jws ~k4ju   ¬b{{5000");
        PromoNews.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}]");
        PromoNews.AppendLine(String.Empty);
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  |||||||||||||||||||||");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬ [{TeletextControl.GraphicsCyan}]||||||||||||||||");
        PromoNews.AppendLine($"[{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]UP-TO-THE-MINUTE UK AND WORLD NEWS..");
        PromoNews.AppendLine($" [{TeletextControl.AlphaCyan}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]POLITICS, SCIENCE AND BUSINESS...");
        PromoNews.AppendLine($"  [{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]THE LATEST SPORT, INCLUDING");
        PromoNews.AppendLine($"   [{TeletextControl.AlphaCyan}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]FOOTBALL, TENNIS, CRICKET, GOLF");
        PromoNews.AppendLine($"    [{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]RUGBY AND MOTOR RACING...");
        PromoNews.AppendLine($"     [{TeletextControl.AlphaCyan}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]WEATHER, TV, ENTERTAINMENT");
        PromoNews.AppendLine($"      [{TeletextControl.AlphaYellow}][{TeletextControl.NewBackground}][{TeletextControl.AlphaBlue}]AND A GREAT DEAL MORE.........");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬ [{TeletextControl.GraphicsCyan}]||||||||||||||||");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬ [{TeletextControl.GraphicsCyan}]¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬||||||||||||||||||");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
        PromoNews.AppendLine($"[{TeletextControl.GraphicsBlue}]                  ,,,,,,,,,,,,,,,,,,,,,");
        Utility.PadLines(PromoNews, 2);
        PromoNews.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]Latest news coming up next >>>");
       


        // PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("<,,4<,,4<,,4") + "</span>");
        // PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("|<,h|,$|<,h|,$x<lth| |4") + "&nbsp;</span></p>");
        // PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("5b(55b(55j|5") + "</span>");
        // PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("¬5 j¬,$¬=,j¬,$¬=n¬b¬,¬1") + "&nbsp;</span></p>");
        // PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("urp5urp5urp5") + "</span>");
        // PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("/-,*/,$/-,*/  /%*/*/ /%") + "&nbsp;</span></p>");
        // PromoCeefax.AppendLine("<br>");
        // PromoCeefax.AppendLine("<p><span class=\"ink2\">" + Utility.BlockGraph("h,,,,,,,,|4       ") + "</span><span>" + Utility.BlockGraph("j       ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("||||<||||||") + "</span></p>");
        // PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("  $,l ") + "</span><span class=\"ink2\">");
        // PromoCeefax.AppendLine(Utility.BlockGraph(" w5 ") + "</span><span>" + Utility.BlockGraph("5  (0 h  8! 0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("¬¬¬?!+7£¬¬¬") + "</span></p>");
        // PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("    \" ") + "</span><span class=\"ink2\">");
        // PromoCeefax.Append(Utility.BlockGraph(" }5 ") + "</span><span>" + Utility.BlockGraph("5 d0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|||tp "));
        // PromoCeefax.Append("</span><span>" + Utility.BlockGraph("£  ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("¬¬ ") + "</span><span>" + Utility.BlockGraph("$1 "));
        // PromoCeefax.Append("</span><span class=\"ink3\">" + Utility.BlockGraph("% ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("?o¬") + "</span></p>");
        // PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("¬¬¬¬ ") + "</span><span>" + Utility.BlockGraph(",,,,,% "));
        // PromoCeefax.Append("</span><span class=\"ink5\">" + Utility.BlockGraph("l|") + "</span><span class=\"paper5 ink6\">" + Utility.BlockGraph("  x|t   "));
        // PromoCeefax.Append("</span><span class=\"ink3\">" + Utility.BlockGraph("  ¬? ") + "</span><span>" + Utility.BlockGraph("1$   ") + "</span><span class=\"ink3\">");
        // PromoCeefax.AppendLine(Utility.BlockGraph("%j¬") + "</span></p>");
        // PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("//// ") + "</span><span class=\"ink1\">");
        // PromoCeefax.AppendLine(Utility.BlockGraph("wwww¬5 ") + "</span><span>" + Utility.BlockGraph(",$x¬¬¬¬¬t0  ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/!        /") + "</span></p>");
        // PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("\"£££££ ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/.../% ") + "</span><span>");
        // PromoCeefax.AppendLine(Utility.BlockGraph("\"££££///'£") + "</span></p>");

        //LogoSunny[0] = "<span class=\"ink6\">" + Utility.BlockGraph("     j") + "</span>";
        //LogoSunny[1] = "<span class=\"ink6\">" + Utility.BlockGraph("  (0 h  8! 0") + "</span>";
        //LogoSunny[2] = "<span class=\"ink6\">" + Utility.BlockGraph(" d0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|||tp ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("£") + "</span>";
        //LogoSunny[3] = "<span class=\"ink5\">" + Utility.BlockGraph(" l|") + "</span><span class=\"paper5 ink6\">" + Utility.BlockGraph("  x|t   ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|") + "</span>";
        //LogoSunny[4] = "<span class=\"ink6\">" + Utility.BlockGraph(" ,$") + "</span><span>" + Utility.BlockGraph("x¬¬¬¬¬t0") + "</span>";
        //LogoSunny[5] = "<span>" + Utility.BlockGraph(" \"££££///'£") + "</span>";

        //LogoNight[0] = "<span class=\"ink5\">" + Utility.BlockGraph(" ||||<||||||") + "</span>";
        //LogoNight[1] = "<span class=\"ink5\">" + Utility.BlockGraph(" ¬¬¬?!+7£¬¬¬") + "</span>";
        //LogoNight[2] = "<span class=\"ink5\">" + Utility.BlockGraph(" ¬¬ ") + "</span><span>" + Utility.BlockGraph("$1 ") + "</span><span class=\"ink3\">" + Utility.BlockGraph("% ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("?o¬") + "</span>";
        //LogoNight[3] = "<span class=\"ink3\">" + Utility.BlockGraph(" ¬? ") + "</span><span>" + Utility.BlockGraph("1$   ") + "</span><span class=\"ink3\">" + Utility.BlockGraph("%j¬") + "</span>";
        //LogoNight[4] = "<span class=\"ink1\">" + Utility.BlockGraph(" /!        /") + "</span>";

        //LogoTV[0] = "<span class=\"ink2\">" + Utility.BlockGraph("h,,,,,,,,|4") + "</span>";
        //LogoTV[1] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("  $,l ") + "</span><span class=\"ink2\">" + Utility.BlockGraph(" w5 ") + "</span><span>" + Utility.BlockGraph("0") + "</span>";
        //LogoTV[2] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("    \" ") + "</span><span class=\"ink2\">" + Utility.BlockGraph(" }5 ") + "</span><span>" + Utility.BlockGraph("5") + "</span>";
        //LogoTV[3] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("¬¬¬¬ ") + "</span><span>" + Utility.BlockGraph(",,,,,%") + "</span>";
        //LogoTV[4] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("//// ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("wwww¬5") + "</span>";
        //LogoTV[5] = "<span class=\"ink2\">" + Utility.BlockGraph("\"£££££ ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/.../%") + "</span>";

        PromoLinks.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]0p0             p0       _,");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]7 \"d 8d _,$48$_& \" 8d 8d_up 8,_  _$");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsCyan}]5  jj_:h!_5¬  j   j_:j_: 5 6 z )8!   ");
        PromoLinks.AppendLine($"[{TeletextControl.GraphicsCyan}]  5p8!\"d0*p&55  \"dp8\"dp\"dp 5 e8k_&\"d___");
        PromoLinks.AppendLine($"[{TeletextControl.GraphicsRed}]                ppppppppp0ppp[{TeletextControl.HoldGraphics}][{TeletextControl.GraphicsMagenta}]ppp[{TeletextControl.GraphicsMagenta}]pp  ");
        
        PromoLinks.Append($"[{TeletextControl.GraphicsYellow}]      [{TeletextControl.HoldGraphics}]\"+o~¬/[{TeletextControl.GraphicsWhite}]j[{TeletextControl.GraphicsRed}]");
        PromoLinks.AppendLine($"[{TeletextControl.NewBackground}][{TeletextControl.GraphicsMagenta}]$,,,[{TeletextControl.GraphicsRed}]¬[{TeletextControl.BlackBackground}]5¬¬¬¬¬¬¬¬//[{TeletextControl.GraphicsWhite}]z%");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]               [{TeletextControl.GraphicsMagenta}]£££££££[{TeletextControl.HoldGraphics}]£!£[{TeletextControl.GraphicsWhite}](,,,,,.//! ");
        
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]If you have any suggestions or feedback");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]relating to this site, please click on");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]the[{TeletextControl.AlphaYellow}]X (@pfceefax)[{TeletextControl.AlphaWhite}]link at the bottom");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]of the screen.");
        PromoLinks.AppendLine(String.Empty);
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]If you would like to learn more about");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]the history of Ceefax,[{TeletextControl.AlphaYellow}]the world's");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaYellow}]first teletext service,[{TeletextControl.AlphaWhite}]please visit");
        PromoLinks.AppendLine($"[{TeletextControl.AlphaWhite}]the website below:");
        PromoLinks.AppendLine(String.Empty);
        PromoLinks.AppendLine($"[{TeletextControl.AlphaCyan}]http://teletext.mb21.co.uk");
        Utility.PadLines(PromoLinks, 4);
        PromoLinks.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]CEEFAX: The world at your fingertips");

        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}]");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]                      px|||tp0");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]775<4<h$44         _>£££££k¬?!");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]555w15j s5         /  7  7j?$");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsWhite}]<$t0p(_ph0pp0p0p0     £($£j¬¬4");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]5 555j*lj 555<5-4    [{TeletextControl.GraphicsRed}]ow{{¬j¬¬¬");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsGreen}]£!!!!\"\"£\" !!!£!£!     [{TeletextControl.GraphicsRed}]+'h~¬¬¬4");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]FROM[{TeletextControl.GraphicsRed}]                    ///¬¬}}");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] |<,,l4    ,||||4    ,||||4o¬?!");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] ¬5j}}~5?//o55(,~5?//o55($j5j¬5j7k7k5");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] ¬urs{{55bs¬5urs{{55bs¬5uzuz5j¬%j7h4k5");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsYellow}] £££££q}}||~5££££q}}~¬¬5££££!*/ z}}~}}~5");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsRed}]                   _pp~¬¬¬¬¬}}||||$");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]AND A VERY HAPPY   [{TeletextControl.GraphicsRed}]++¬¬¬¬¬¬¬¬¬¬%");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]    NEW YEAR!     [{TeletextControl.GraphicsRed}]_|trx¬¬¬¬¬¬?'");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.GraphicsRed}]                  x/¬/£//£($£/¬/!");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}]");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}]");
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaWhite}][{TeletextControl.NewBackground}]");
        Utility.PadLines(PromoChristmas, 3);
        PromoChristmas.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]CEEFAX: The world at your fingertips");



        //PrestelIntro1.AppendLine("<p class=\"doubleHeight\"><span class=\"ink7\">&nbsp;&nbsp;Good " + Utility.Greeting(DateTime.Now) + "</span></p><br>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;If you thought online banking, </span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;e-mail, home shopping and social </span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;networking all came about because of</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;the internet...</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro1.AppendLine("<p><span>&nbsp;&nbsp;</span><span class=\"paper7 ink1\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;THEN THINK AGAIN!&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;During the 1980's, British Telecom</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;pioneered all these services through</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;a little-known system called...</span></p>");
        //PrestelIntro1.AppendLine("<br>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;P </span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;P </span><span class=\"ink2\">R </span><span class=\"ink5\">E </span>");
        //PrestelIntro1.AppendLine("<span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Available through</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;P </span><span class=\"ink2\">R </span><span class=\"ink5\">E </span><span class=\"ink1\">S </span><span class=\"ink7\">T </span>");
        //PrestelIntro1.AppendLine("<span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;specially adapted</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;P </span><span class=\"ink2\">R </span><span class=\"ink5\">E </span><span class=\"ink1\">S </span><span class=\"ink7\">T </span><span class=\"ink3\">E </span><span class=\"ink6\">L </span>");
        //PrestelIntro1.AppendLine("<span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;TV's, set top boxes</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"ink5\">E </span><span class=\"ink1\">S </span><span class=\"ink7\">T </span><span class=\"ink3\">E </span><span class=\"ink6\">L </span>");
        //PrestelIntro1.AppendLine("<span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and home computers.</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"ink7\">T </span><span class=\"ink3\">E </span><span class=\"ink6\">L </span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"ink6\">L </span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink7\">" + String.Join("", Enumerable.Repeat("&nbsp;", 32 - Utility.ToMonthName(DateTime.Now).Length)) + "</span><span class=\"paper1 ink 7\">&nbsp;&nbsp;" + DateTime.Now.Day.ToString() + " " + Utility.ToMonthName(DateTime.Now).ToUpper() + "&nbsp;&nbsp;</span></p>");
        //PrestelIntro1.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");

        //PrestelIntro2.AppendLine("<p class=\"doubleHeight\"><span class=\"ink7\">&nbsp;&nbsp;Main Index</span></p><br>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro2.AppendLine("<p><span>&nbsp;&nbsp;</span><span class=\"paper7 ink1\">&nbsp;&nbsp;1 FOCUS&nbsp;&nbsp;</span><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>");
        //PrestelIntro2.AppendLine("<span class=\"paper5 ink1\">&nbsp;&nbsp;LATEST NEWS&nbsp;&nbsp;</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;[NEWS1]</span</p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink4\">&nbsp;&nbsp;[NEWS2]</span</p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro2.AppendLine("<br>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;&nbsp;20 AGRICULTURE&nbsp;&nbsp;&nbsp;24 INSURANCE</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;&nbsp;21 BANKING&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;25 MICROCOMPUTING</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;&nbsp;22 BUSINESS&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;26 TELESHOPPING</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;&nbsp;23 EDUCATION&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;27 TRAVEL</span></p>");
        //PrestelIntro2.AppendLine("<br>");
        //PrestelIntro2.AppendLine("<br>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;5 MESSAGE SERVICES </span><span class=\"ink4\">Mailbox,Telex Link</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;6 NEWS, WEATHER, LEISURE, SPORT</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;7 A-Z INDEXES </span><span class=\"ink4\">to information & IPs</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;8 CUSTOMER GUIDE </span><span class=\"ink4\">All about Prestel</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink7\">&nbsp;&nbsp;9 WHAT'S NEW" + String.Join("", Enumerable.Repeat("&nbsp;", 18 - Utility.ToMonthName(DateTime.Now).Length)) + "</span><span class=\"paper1 ink 7\">&nbsp;&nbsp;" + DateTime.Now.Day.ToString() + " " + Utility.ToMonthName(DateTime.Now).ToUpper() + "&nbsp;&nbsp;</span></p>");
        //PrestelIntro2.AppendLine("<p><span class=\"ink1\">" + Utility.SepGraph("  ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");

        //PrestelIntro3.AppendLine("<br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink5\">From MR JASON ROBSON</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;072760776&nbsp;&nbsp;&nbsp;&nbsp;TUE 21 JUL 2015 10:34</span></p>");
        //PrestelIntro3.AppendLine("<br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink5\">To&nbsp;&nbsp;&nbsp;614271596#</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CEEFAX VISITOR</span></p>");
        //PrestelIntro3.AppendLine("<br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink6\">PRESTEL </span><span class=\"ink7\">was built by many 'information</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink7\">providers'.</span></p>");
        //PrestelIntro3.AppendLine("<br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink7\">Two popular ones catered to the growing</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink6\">home computing </span><span class=\"ink7\">industry, and could be</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink7\">quite eccentric!</span></p>");
        //PrestelIntro3.AppendLine("<br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink6\">The Gnome at Home </span><span class=\"ink7\">and </span><span class=\"ink6\">MICRONET 800</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink7\">were pages visited by computer geeks</span></p>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink7\">of the 1980's.</span></p>");
        //PrestelIntro3.AppendLine("<br><br><br><br><br>");
        //PrestelIntro3.AppendLine("<p><span class=\"ink4\">KEY 1 TO SEND, KEY 2 NOT TO SEND</span></p>");




        ////PrestelIntro1.AppendLine("<p><span class=\"ink4\">" + Utility.BlockGraph("                          P") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("<,,,,,l        ~c3}           ") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("5MICROj        8a2d    d           ") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("5     j      h£-,,.(,,,,= ") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("m,,,,,>&&&d  pyppppp   &       ") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("5)))))j    ))5MODEMj      ") + "</span></p>");
        ////PrestelIntro1.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("uqsssrz      £££££££     ") + "</span></p>");


        //PrestelDialcomLogin.AppendLine("<p>" + Utility.BlockGraph("                                       ") + "</p>");
        //PrestelDialcomLogin.AppendLine("<br>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("              WELCOME TO") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<br>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("            BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("         BT  BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("        BT    BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("       BT      BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("        BT    BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("         BT  BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("            BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<br>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("   MESSAGING AND INFORMATION SERVICES") + "</span></p>");
        //PrestelDialcomLogin.AppendLine("<br>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           ") + "Please enter your</span></p>");
        //PrestelDialcomLogin.AppendLine("<br>");
        //PrestelDialcomLogin.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           ") + "customer identity</span></p>");

        //PrestelDialcomLogoff.AppendLine("<p>" + Utility.BlockGraph("                                       ") + "</p>");
        //PrestelDialcomLogoff.AppendLine("<br>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("            BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("         BT  BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("        BT    BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("       BT      BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("        BT    BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("         BT  BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("            BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           BT  DIALCOM  GROUP") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<br>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("   MESSAGING AND INFORMATION SERVICES") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<br>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("           ROWLING COMPUTER") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<br><br>");
        //PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("         THANK YOU FOR CALLING") + "</span></p>");
        //PrestelDialcomLogoff.AppendLine("<br><br><br><br><br>");
        //PrestelDialcomLogoff.AppendLine("NO CARRIER");
        ////PrestelDialcomLogoff.AppendLine("<p><span class=\"ink7 indent\">" + Utility.BlockGraph("       CEEFAX WILL RETURN SHORTLY") + "</span></p>");

    }
}