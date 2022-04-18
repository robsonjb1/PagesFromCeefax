using System.Text;

namespace PagesFromCeefax
{
    public class Graphics
    {
        // Page headers
        public static StringBuilder HeaderHeadlines = new StringBuilder();
        public static StringBuilder HeaderPolitics = new StringBuilder();
        public static StringBuilder HeaderHome = new StringBuilder();
        public static StringBuilder HeaderSussex = new StringBuilder();
        public static StringBuilder HeaderWorld = new StringBuilder();
        public static StringBuilder HeaderSciTech = new StringBuilder();
        public static StringBuilder HeaderBusiness = new StringBuilder();
        public static StringBuilder HeaderFootball = new StringBuilder();
        public static StringBuilder HeaderRugby = new StringBuilder();
        public static StringBuilder HeaderCricket = new StringBuilder();
        public static StringBuilder HeaderGolf = new StringBuilder();
        public static StringBuilder HeaderFormula1 = new StringBuilder();
        public static StringBuilder HeaderTennis = new StringBuilder();
        public static StringBuilder HeaderEntertainment = new StringBuilder();
        public static StringBuilder HeaderTop40Singles = new StringBuilder();
        public static StringBuilder HeaderTV = new StringBuilder();
        public static StringBuilder HeaderMarkets = new StringBuilder();
        public static StringBuilder HeaderWeather = new StringBuilder();
        public static StringBuilder HeaderLinks = new StringBuilder();
        public static StringBuilder HeaderTravel = new StringBuilder();

        public static string[] LogoSunny = new string[6];
        public static string[] LogoNight = new string[6];
        public static string[] LogoTV = new string[6];
        
        // Transitional screens
        public static StringBuilder PromoMap = new StringBuilder();
        public static StringBuilder PromoWeather = new StringBuilder();
        public static StringBuilder PromoSport = new StringBuilder();
        public static StringBuilder PromoTV = new StringBuilder();
        public static StringBuilder PromoNews = new StringBuilder();
        public static StringBuilder PromoTeletext = new StringBuilder();
        public static StringBuilder PromoCeefax = new StringBuilder();
        public static StringBuilder PromoLinks = new StringBuilder();
        public static StringBuilder PromoChristmas = new StringBuilder();

        static Graphics()
        {
            HeaderPolitics.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   h<|h<|h4 |(|$|h<$|,$   ") + "</span></p>");
            HeaderPolitics.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   j7£ju@ju0@ @ @ju0s{5   ") + "</span></p>");
            HeaderPolitics.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("///-./-,,-,.,/,/,-,.,,.///") + "</span></p>");

            HeaderHome.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("      h4h4|,|h<<|h<$      ") + "</span></p>");
            HeaderHome.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("      j7k5@p@j55@jw1      ") + "</span></p>");
            HeaderHome.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//////-.-.,,,-..,-,.//////") + "</span></p>");

            HeaderSussex.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("     x,$|h4x,$x,$|,h4|    ") + "</span></p>");
            HeaderSussex.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("     s{%oz%s{%s{%@sh7}    ") + "</span></p>");
            HeaderSussex.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("/////,,/-,/,,/,,/,,-.,////") + "</span></p>");

            HeaderWorld.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("     |hh4|,|h<l4| h<l0    ") + "</span></p>");
            HeaderWorld.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("     ozz%@p@j7k4@pjuz%    ") + "</span></p>");
            HeaderWorld.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("/////-,,/,,,-.-.,,-,,/////") + "</span></p>");

            HeaderSciTech.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   x,$x,h4 (|$|,_<$|h4    ") + "</span></p>");
            HeaderSciTech.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   s{%opj5£ @ @s*u0@k5    ") + "</span></p>");
            HeaderSciTech.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("///,,/-,-.//,/,,/,.,-.////") + "</span></p>");

            HeaderBusiness.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("   |l4|h4|,$|_<th<h<,h<,  ") + "</span></p>");
            HeaderBusiness.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("   @{4oz%s{5@j5@jwbs@bs@  ") + "</span></p>");
            HeaderBusiness.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink2\">" + Utility.BlockGraph("///,,.-,/,,.,-.,-,-,,-,,//") + "</span></p>");

            HeaderFootball.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("   h<h<|h<|(|$|l4|l4| |   ") + "</span><p>");
            HeaderFootball.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("   j7ju@ju@ @ @{4@k5@0@0  ") + "</span><p>");
            HeaderFootball.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("///-.-,,-,,/,/,,.,-.,.,.//") + "</span><p>");
   
            HeaderRugby.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("  <4hhh,$<4hh hhhlhhlhl   ") + "</span><p>");
            HeaderRugby.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("  7kjzjr5w{b{ jzjjjjzjj   ") + "</span><p>");
            HeaderRugby.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//.--,-,.,,-,/-,----,--///") + "</span><p>");
   
            HeaderCricket.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("     h<$|l4|h<$|h4|$l<    ") + "</span><p>");
            HeaderCricket.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("     ju0@k4@ju0@k4@1j5    ") + "</span><p>");
            HeaderCricket.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("/////-,.,-.,-,.,-.,.-.////") + "</span><p>");
   
            HeaderGolf.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("        |,,h<|h4 |,       ") + "</span><p>");
            HeaderGolf.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("        @r@ju@ju0@£       ") + "</span><p>");
            HeaderGolf.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("////////,,,-,,-,.,////////") + "</span><p>");
   
            HeaderFormula1.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("  <<4<l(<h,4<4h,hlh,4<4l$ ") + "</span><p>");
            HeaderFormula1.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("  555uz 5jp57kb{j£jp57kj  ") + "</span><p>");
            HeaderFormula1.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//...,,/.-,..--,-/-,..--//") + "</span><p>");
   
            HeaderTennis.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("     (|$|$xl0xl0|h<,      ") + "</span><p>");
            HeaderTennis.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink4\">" + Utility.BlockGraph("      @ @1@j5@j5@bs@      ") + "</span><p>");
            HeaderTennis.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//////,/,.,-.,-.,-,,//////") + "</span><p>");
            
            HeaderEntertainment.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  |$xl0l<h<h<t(|$|l4|_<t_<<th<_<t(|$   ") + "</span></p>");
            HeaderEntertainment.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  @1@j5j5jwj7} @ @k5@j5@j55@jwj5@ @    ") + "</span></p>");
            HeaderEntertainment.AppendLine("<p><span class=\"ink6\">" + Utility.BlockGraph("£££££££££££££££££££££££££££££££££££££££") + "</span></p>");

            HeaderTop40Singles.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  l<h<|h<| |_ h<|  |,$|_<th<,$| |$|,$  ") + "</span></p>");
            HeaderTop40Singles.AppendLine("<p><span class=\"paper6 ink3\">" + Utility.BlockGraph("  j5ju@j7£ /n,ju@  s{5@j5@ju{5@0@1s{5  ") + "</span></p>");
            HeaderTop40Singles.AppendLine("<p><span class=\"ink6\">" + Utility.BlockGraph("£££££££££££££££££££££££££££££££££££££££") + "</span></p>");

            HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper5 ink1\">" + Utility.BlockGraph("   (l<$|,|h<l4| h4|,h4    ") + "</span></p>");
            HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper5 ink1\">" + Utility.BlockGraph("    j5 @£}j7k5\"m' @sju0   ") + "</span></p>");
            HeaderTravel.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink5\">" + Utility.BlockGraph("££££££££££££££££££££££££££") + "</span></p>");

            HeaderTV.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("h<,,,,,,||") + " ----------------------------</span></p>");
            HeaderTV.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("j5k7j5@ @n") + "</span>");
            HeaderTV.AppendLine("<span class=\"ink6\">" + Utility.BlockGraph("@{%@{%@+%") + "</span>");
            HeaderTV.AppendLine("<span class=\"ink6\"> Top iPlayer</span></p>");
            HeaderTV.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("j5j5\"m' @@") + "</span>");
            HeaderTV.AppendLine("<span class=\"ink6\">" + Utility.BlockGraph("@z5@z5@x4") + "</span></p>");
            HeaderTV.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("*-,,,,,,/.") + " ------------------------</span><span class=\"ink7\"> {pageNo}/4</span></p>");
            
            HeaderWeather.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   h44|h<h<|(|$|h4|$|l    ") + "</span></p>");
            HeaderWeather.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("   *uu?jwj7@ @ @k5@1@k4   ") + "</span></p>");
            HeaderWeather.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("////,,.-,-.,/,/,-.,.,-.///") + "</span></p>");

            HeaderMarkets.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("   xll0|l4|l4|h4|$l<h<,   ") + "</span></p>");
            HeaderMarkets.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper2 ink7\">" + Utility.BlockGraph("   @jj5@k5@k4@k4@1j5bs@   ") + "</span></p>");
            HeaderMarkets.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink2\">" + Utility.BlockGraph("///,--.,-.,-.,-.,.-.-,,///") + "</span></p>");

            HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("j£3kj£3kj£3k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("  h4|h<h<|h<th4h4xl0|$|,  ") + "</span></p>");
            HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("j $kj $kj 'k ") + "<span class=\"paper1 ink6\">" + Utility.BlockGraph("  j7@jwj7@ju?juj5@j5@1s@  ") + "</span></p>");
            HeaderHeadlines.AppendLine("<p>" + Utility.BlockGraph("\"£££\"£££\"£££ ") + "<span class=\"ink1\">" + Utility.BlockGraph("//-.,-,-.,-,.-,-.,-.,.,,//") + "</span></p>");


            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                    _4_~@|@}        ") + "</span><span class=\"ink7\">2/4</span></p>");
            PromoMap.AppendLine("<p>[LINE1]<span class=\"ink4\">" + Utility.SepGraph(" _?0~[FF]%") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE2]<span class=\"ink4\">" + Utility.SepGraph(" j*@@@@@y||t") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE3]<span class=\"ink4\">" + Utility.SepGraph("  ({@@@@@@@?") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE4]<span class=\"ink4\">" + Utility.SepGraph("  !'~@@@@@@!") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE5]<span class=\"ink4\">" + Utility.SepGraph("   ~o@@@@w1") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE6]<span class=\"ink4\">" + Utility.SepGraph("   ~o@@@@wq") + "</span></p>");
            PromoMap.AppendLine("<p>[LINE7]<span class=\"ink4\">" + Utility.SepGraph("  j)\"@[DD]@}") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                p|@t  x@@g@@@@0") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink7 indent\">DATA AT</span><span class=\"ink4\">" + Utility.SepGraph("        n[EE]t +\" @@@@@@") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink7 indent\">[TTT]</span><span class=\"ink4\">" + Utility.SepGraph("          +@@@@@ _4 +@@@@@|0") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink7 indent\">UK TIME</span><span class=\"ink4\">" + Utility.SepGraph("         \"%*o! \"   *@@@@@u") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                          *@[CC]@}") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                      k||@@@@@@@@@") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                   4  '@@@@@@@@@@}z@t") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                  j!   j@@@@@@@@@@@@@") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("              ppp<'  x|[BB]@@@@@@@@@'") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("            \"£!      ! +/'i@@[AA]@qp0") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                        t@@@@@@@@@@@'") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                    h@@@@@@@@?s///?!") + "</span></p>");
            PromoMap.AppendLine("<p><span class=\"ink4 indent\">" + Utility.SepGraph("                   8?' \"'     \"") + "</span></p>");


            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@@@@@@@4@46j4@44@@@@@@@@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@@@u+6$%'5*'bj_ *ha/7@@@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@7+3l+ ( !      ! &$xo@@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@?t;d!             :a>a~@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@/mp)p       ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("ppp") + "</span><span class=\"ink5\">" + Utility.SepGraph("      d':g<o?") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@$2-     ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("x@@@@@@}0") + "</span><span class=\"ink5\">" + Utility.SepGraph("    f a&ax") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@w;, £    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("z££££@£££m0") + "</span><span class=\"ink5\">" + Utility.SepGraph("    $p,g{") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@?-,    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("hu&£££@£££d}") + "</span><span class=\"ink5\">" + Utility.SepGraph("       cq") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@7+-,    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("~@    @   j@4    ") + "</span><span class=\"ink5\">" + Utility.SepGraph("bssx") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@//!    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("@@@@?%@*o@@@5    ") + "</span><span class=\"ink5\">" + Utility.SepGraph("bqrq") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@=,%    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("k@@@ursqz@@@!    ") + "</span><span class=\"ink5\">" + Utility.SepGraph(",,t@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@}<&c0    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("o@u£ss3a@@7    ") + "</span><span class=\"ink5\">" + Utility.SepGraph("£,ltr") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@=/s,    ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("\"o@}2'x@@'     ") + "</span><span class=\"ink5\">" + Utility.SepGraph("ds/l@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@|?q$9     ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("+/@@@/!      ") + "</span><span class=\"ink5\">" + Utility.SepGraph("-rit{") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@7y/9!_              f$2o@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@~}>x%2  0         4e+t@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@@wy%:xa_        'o;t/@@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"ink5\">" + Utility.SepGraph("@@@@@@@@@@@@@@@@>i1&6j ~ e_jj42}@~@@@@@") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"paper1 ink7\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"paper1 ink7\">" + Utility.BlockGraph("  @j5@      n=j5        @k5            ") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"paper1 ink7\">" + Utility.BlockGraph("  @j5@jw@bs@j5j7@jw@j7@ @j5@{5@j5@jws  ") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"paper1 ink7\">" + Utility.BlockGraph("  @zu@ju|ju@j5j5@ju|j5  @j5@x4@zu@ht@  ") + "</span></p>");
            PromoWeather.AppendLine("<p><span class=\"paper1 ink7\">" + Utility.BlockGraph("                                       ") + "</span></p>");

            PromoSport.Append("<br>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("     _r@q0       ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("p0") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("     ,,,,,       ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph(" 7d_~@@@@@}08k   ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5    ") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("@sjw@j7@jw?\"@!") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph(" 5 i|||||||6 j   ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5    ") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("p@j5 ju@j5@ @") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph(" 5 j@@@@@@@5 j   ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph(" 5 j@@@@@@@5 j   ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5    ") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("j7@jw1@_0@jw1") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph(" \",z@@@@@@@u,!   ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("@5    ") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("j5@ju0@zu@_z5") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("    k@@@@@7     ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("x@}0") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("     ,l|<,     ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j};y@") + "</span><span class=\"ink5\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;follows...</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("      \"@!      ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j@5@@") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink7\">" + Utility.BlockGraph("    _x~@}t0    ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j@5@@") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@         ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("p|@@|p") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@        ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("~@@@@@@}") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@       ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("z@@@@@@@@u") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink2\">" + Utility.BlockGraph(" _pppppp       ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j@5@@      ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("h@@@@@@@@@@4") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink2\">" + Utility.BlockGraph(" j@@@@@@@") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("@@@@@@") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j@5@@ ") + "</span>");
            PromoSport.Append("<span class=\"ink5\">" + Utility.BlockGraph("@@@@ ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("@@@@@@@@@@@@") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink2\">" + Utility.BlockGraph(" \"££££££       ") + "</span>");
            PromoSport.Append("<span class=\"ink6\">" + Utility.BlockGraph("j@5@@      ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("*@@@@@@@@@@%") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@       ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("k@@@@@@@@7") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               j@5@@        ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("o@@@@@@?") + "</span></p>");
            PromoSport.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("               *@}@?         ") + "</span>");
            PromoSport.Append("<span class=\"ink4\">" + Utility.SepGraph("£/@@/£") + "</span></p>");

            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph(" pppp_ppp0pppp ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@9@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph(" @@@@j@@@5@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@9@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph(" ||||h|||4|||| ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@9@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink4\">" + Utility.BlockGraph(" @@@@j@@@5@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink4\">" + Utility.BlockGraph(" @@@@j@@@5@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink4\">" + Utility.BlockGraph(" @@@@j@@@5@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@") + "</span>");
            PromoTV.Append("<span class=\"ink4\">" + Utility.BlockGraph(" ££££\"£££!££ ") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph("pppppppppp   ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph("?1;@@@@@@@   ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink5\">" + Utility.BlockGraph("@5@@@@?'/@   ") + "</span>");
            PromoTV.Append("<span class=\"ink2\">" + Utility.BlockGraph("@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@ ") + "</span>");
            PromoTV.Append("<span class=\"ink1\">" + Utility.BlockGraph("@5@@@@@||@   ") + "</span>");
            PromoTV.Append("<span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@ @u@@@@@@@@   @@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@ ££££££££££   @@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@jj@@@@@@@@55@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@jj@@@@@@@@55@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@z@@@@@@@@@@u@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p class=\"doubleHeight\"><span class=\"ink6\">" + Utility.BlockGraph("@@ TV AND ENTERTAINMENT IN A MOMENT @@@") + "</span></p><br>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoTV.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@") + "</span></p>");


            PromoNews.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoNews.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("  £@£h<| @ h<| @$h<| |h4j=   sh<$      ") + "</span></p>");
            PromoNews.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("   @ jws @0jws @0jws ~k4ju   @b{5000   ") + "</span></p>");
            PromoNews.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoNews.Append("<br>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  |||||||||||||||||||||") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@  ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("||||||||||||||||") + "</span></p>");
            PromoNews.Append("<p><span class=\"paper6 ink1\">&nbsp;&nbsp;UP-TO-THE-MINUTE UK AND WORLD NEWS..&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;</span><span class=\"paper5 ink1\">&nbsp;&nbsp;POLITICS, SCIENCE AND BUSINESS...&nbsp;&nbsp;&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;&nbsp;</span><span class=\"paper6 ink1\">&nbsp;&nbsp;THE LATEST SPORT, INCLUDING&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;&nbsp;&nbsp;</span><span class=\"paper5 ink1\">&nbsp;&nbsp;FOOTBALL, TENNIS, CRICKET, GOLF&nbsp;&nbsp;&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;&nbsp;&nbsp;&nbsp;</span><span class=\"paper6 ink1\">&nbsp;&nbsp;RUGBY AND MOTOR RACING...&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span class=\"paper5 ink1\">&nbsp;&nbsp;WEATHER, TV, ENTERTAINMENT&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            PromoNews.Append("<p><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span class=\"paper6 ink1\">&nbsp;&nbsp;AND A GREAT DEAL MORE.........&nbsp;</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@  ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("||||||||||||||||") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@  ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@||||||||||||||||||") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  @@@@@@@@@@@@@@@@@@@@@") + "</span></p>");
            PromoNews.Append("<p><span class=\"ink1\">" + Utility.BlockGraph("                  ,,,,,,,,,,,,,,,,,,,,,") + "</span></p>");
            PromoNews.Append("<br><br>");
            PromoNews.Append("<p><span class=\"paper1 ink6\">&nbsp;&nbsp;Latest news coming up next >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");

            PromoTeletext.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoTeletext.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("  £@£h<| @ h<| @$h<| |h4j=   sh<$      ") + "</span></p>");
            PromoTeletext.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("   @ jws @0jws @0jws ~k4ju   @b{5000   ") + "</span></p>");
            PromoTeletext.Append("<p><span class=\"paper1 ink6\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoTeletext.Append("<br>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@/////////////////@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">...pages of</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;CEEFAX" + Utility.BlockGraph("@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@~@@@") + "&nbsp;&nbsp;<span class=\"ink6\">information</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@~@@@") + "&nbsp;&nbsp;<span class=\"ink6\">summoned</span>&nbsp;<span class=\"ink5\">by</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@~@@@") + "&nbsp;&nbsp;<span class=\"ink5\">you</span>&nbsp;<span class=\"ink6\">to your</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">tv screen at</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">the touch of</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">numbered keys</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">on a remote-</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("@@@@@@@@@@@@@@@") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "&nbsp;&nbsp;<span class=\"ink6\">control pad.</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@") + "</span><span class=\"ink5\">&nbsp;" + Utility.BlockGraph("///////////////") + "</span>");
            PromoTeletext.AppendLine("<span class=\"ink1\">" + Utility.BlockGraph("@@@@@") + "</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink1\">" + Utility.BlockGraph("@@@@@@@@@@@@@@@@@@@@@@@@") + "</span><span class=\"ink5\">&nbsp;&nbsp;You</span><span class=\"ink6\">&nbsp;choose</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("                   ") + "^" + Utility.BlockGraph("      ") + "</span><span class=\"ink6\">what to read</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("                   ") + "^" + Utility.BlockGraph("      ") + "</span><span class=\"ink6\">and when to</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink6 indent\">THE BBC</span><span class=\"ink2\">" + Utility.BlockGraph("        ") + "</span><span class=\"paper2 ink7\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span class=\"ink6\">&nbsp;&nbsp;read it...</span></p>");
            PromoTeletext.AppendLine("<p><span class=\"ink6 indent\">TELETEXT</span><span class=\"ink2\">" + Utility.BlockGraph("       ") + "</span><span class=\"paper2 ink7\">&nbsp;&nbsp;1&nbsp;2&nbsp;3&nbsp;&nbsp;</span>&nbsp;&nbsp;&nbsp;&nbsp;</p>");
            PromoTeletext.AppendLine("<p><span class=\"ink6 indent\">SERVICE</span><span class=\"ink2\">" + Utility.BlockGraph("        ") + "</span><span class=\"paper2 ink7\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;&nbsp;&nbsp;&nbsp;</p>");
            PromoTeletext.AppendLine("<p><span class=\"ink6 indent\">IS CALLED</span><span class=\"ink2\">" + Utility.BlockGraph("      ") + "</span><span class=\"paper2 ink7\">&nbsp;&nbsp;4&nbsp;5&nbsp;6&nbsp;&nbsp;</span>&nbsp;&nbsp;&nbsp;&nbsp;</p>");
            PromoTeletext.AppendLine("<p><span class=\"ink7 indent\">CEEFAX</span><span class=\"ink2\">" + Utility.BlockGraph("         ") + "</span><span class=\"paper2 ink7\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;&nbsp;&nbsp;&nbsp;</p>");

            PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("<,,4<,,4<,,4") + "</span>");
            PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("|<,h|,$|<,h|,$x<lth| |4") + "&nbsp;</span></p>");
            PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("5b(55b(55j|5") + "</span>");
            PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("@5 j@,$@=,j@,$@=n@b@,@1") + "&nbsp;</span></p>");
            PromoCeefax.AppendLine("<p><span class=\"ink7\">" + Utility.BlockGraph("urp5urp5urp5") + "</span>");
            PromoCeefax.AppendLine("<span class=\"paper1 ink6\">&nbsp;&nbsp;" + Utility.BlockGraph("/-,*/,$/-,*/  /%*/*/ /%") + "&nbsp;</span></p>");
            PromoCeefax.AppendLine("<br>");
            PromoCeefax.AppendLine("<p><span class=\"ink2\">" + Utility.BlockGraph("h,,,,,,,,|4       ") + "</span><span>" + Utility.BlockGraph("j       ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("||||<||||||") + "</span></p>");
            PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("  $,l ") + "</span><span class=\"ink2\">");
            PromoCeefax.AppendLine(Utility.BlockGraph(" w5 ") + "</span><span>" + Utility.BlockGraph("5  (0 h  8! 0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("@@@?!+7£@@@") + "</span></p>");
            PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("    \" ") + "</span><span class=\"ink2\">");
            PromoCeefax.Append(Utility.BlockGraph(" }5 ") + "</span><span>" + Utility.BlockGraph("5 d0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|||tp "));
            PromoCeefax.Append("</span><span>" + Utility.BlockGraph("£  ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("@@ ") + "</span><span>" + Utility.BlockGraph("$1 "));
            PromoCeefax.Append("</span><span class=\"ink3\">" + Utility.BlockGraph("% ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("?o@") + "</span></p>");
            PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("@@@@ ") + "</span><span>" + Utility.BlockGraph(",,,,,% "));
            PromoCeefax.Append("</span><span class=\"ink5\">" + Utility.BlockGraph("l|") + "</span><span class=\"paper5 ink6\">" + Utility.BlockGraph("  x|t   "));
            PromoCeefax.Append("</span><span class=\"ink3\">" + Utility.BlockGraph("  @? ") + "</span><span>" + Utility.BlockGraph("1$   ") + "</span><span class=\"ink3\">");
            PromoCeefax.AppendLine(Utility.BlockGraph("%j@") + "</span></p>");
            PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("//// ") + "</span><span class=\"ink1\">");
            PromoCeefax.AppendLine(Utility.BlockGraph("wwww@5 ") + "</span><span>" + Utility.BlockGraph(",$x@@@@@t0  ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/!        /") + "</span></p>");
            PromoCeefax.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("\"£££££ ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/.../% ") + "</span><span>");
            PromoCeefax.AppendLine(Utility.BlockGraph("\"££££///'£") + "</span></p>");

            LogoSunny[0] = "<span class=\"ink6\">" + Utility.BlockGraph("     j") + "</span>";
            LogoSunny[1] = "<span class=\"ink6\">" + Utility.BlockGraph("  (0 h  8! 0") + "</span>";
            LogoSunny[2] = "<span class=\"ink6\">" + Utility.BlockGraph(" d0 ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|||tp ") + "</span><span class=\"ink6\">" + Utility.BlockGraph("£") + "</span>";
            LogoSunny[3] = "<span class=\"ink5\">" + Utility.BlockGraph(" l|") + "</span><span class=\"paper5 ink6\">" + Utility.BlockGraph("  x|t   ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("|") + "</span>";
            LogoSunny[4] = "<span class=\"ink6\">" + Utility.BlockGraph(" ,$") + "</span><span>" + Utility.BlockGraph("x@@@@@t0") + "</span>";
            LogoSunny[5] = "<span>" + Utility.BlockGraph(" \"££££///'£") + "</span>";

            LogoNight[0] = "<span class=\"ink5\">" + Utility.BlockGraph(" ||||<||||||") + "</span>";
            LogoNight[1] = "<span class=\"ink5\">" + Utility.BlockGraph(" @@@?!+7£@@@") + "</span>";
            LogoNight[2] = "<span class=\"ink5\">" + Utility.BlockGraph(" @@ ") + "</span><span>" + Utility.BlockGraph("$1 ") + "</span><span class=\"ink3\">" + Utility.BlockGraph("% ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("?o@") + "</span>";
            LogoNight[3] = "<span class=\"ink3\">" + Utility.BlockGraph(" @? ") + "</span><span>" + Utility.BlockGraph("1$   ") + "</span><span class=\"ink3\">" + Utility.BlockGraph("%j@") + "</span>";
            LogoNight[4] = "<span class=\"ink1\">" + Utility.BlockGraph(" /!        /") + "</span>";

            LogoTV[0] = "<span class=\"ink2\">" + Utility.BlockGraph("h,,,,,,,,|4") + "</span>";
            LogoTV[1] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("  $,l ") + "</span><span class=\"ink2\">" + Utility.BlockGraph(" w5 ") + "</span><span>" + Utility.BlockGraph("0") + "</span>";
            LogoTV[2] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"paper5\">" + Utility.BlockGraph("    \" ") + "</span><span class=\"ink2\">" + Utility.BlockGraph(" }5 ") + "</span><span>" + Utility.BlockGraph("5") + "</span>";
            LogoTV[3] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("@@@@ ") + "</span><span>" + Utility.BlockGraph(",,,,,%") + "</span>";
            LogoTV[4] = "<span class=\"ink2\">" + Utility.BlockGraph("j ") + "</span><span class=\"ink5\">" + Utility.BlockGraph("//// ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("wwww@5") + "</span>";
            LogoTV[5] = "<span class=\"ink2\">" + Utility.BlockGraph("\"£££££ ") + "</span><span class=\"ink1\">" + Utility.BlockGraph("/.../%") + "</span>";

            HeaderLinks.AppendLine("<p><span class=\"paper1 ink5\">" + Utility.BlockGraph("  0p0             p0       _,       ") + "[T]</span></p>");
            HeaderLinks.AppendLine("<p><span class=\"paper1 ink5\">" + Utility.BlockGraph("  7 \"d 8d _,$48$_& \" 8d 8d_up 8,_  _$  ") + "</span></p>");
            HeaderLinks.AppendLine("<p><span class=\"paper1 ink5\">" + Utility.BlockGraph("  5  jj_:h!_5@  j   j_:j_: 5 6 z )8!   ") + "</span></p>");
            HeaderLinks.AppendLine("<p><span class=\"ink5\">" + Utility.BlockGraph("  5p8!\"d0*p&55  \"dp8\"dp\"dp 5 e8k_&\"d___") + "</span></p>");

            PromoLinks.Append(HeaderLinks.ToString().Replace("[T]", "&nbsp;&nbsp;&nbsp;"));
            PromoLinks.Append("<p><span class=\"ink2\">" + Utility.BlockGraph("                ppppppppp0pppp") + "</span><span class=\"ink3\">");
            PromoLinks.AppendLine(Utility.BlockGraph("ppppppp") + "</span></p>");

            PromoLinks.Append("<p><span class=\"ink6\">" + Utility.BlockGraph("       \"+o~@//") + "</span>");
            PromoLinks.Append("<span class=\"ink7\">" + Utility.BlockGraph("jj") + "</span>");
            PromoLinks.Append("<span class=\"paper2 ink3\">" + Utility.BlockGraph("  $,,,   ") + "</span><span class=\"ink2\">" + Utility.BlockGraph("5@@@@@@@@///") + "</span>");
            PromoLinks.Append("<span class=\"ink7\">" + Utility.BlockGraph("z%") + "</span></p>");

            PromoLinks.Append("<p><span class=\"ink3\">" + Utility.BlockGraph("                £££££££££!££") + "</span>");
            PromoLinks.Append("<span class=\"ink7\">" + Utility.BlockGraph("(,,,,,.//!") + "</span></p>");
           
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">If you have any suggestions or feedback</span></p>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">relating to this site, please click on</p>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">the </span><span class=\"ink6\">Twitter (@pfceefax)</span><span class=\"ink7\"> link at the</span></p>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">bottom of the screen.</span></p>");
            PromoLinks.AppendLine("<br>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">If you would like to learn more about</span></p>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">the history of Ceefax,</span><span class=\"ink6\"> the world's</span></p>");
            PromoLinks.AppendLine("<p><span class=\"ink6 indent\">first teletext service, </span><span class=\"ink7\">please click</span></p>");
            PromoLinks.AppendLine("<p><span class=\"ink7 indent\">on the link below:</span></p>");
            PromoLinks.AppendLine("<br>");
            PromoLinks.AppendLine("<p><span class=\"ink5 indent\"><a href=\"http://teletext.mb21.co.uk\" target=\"_new\">http://teletext.mb21.co.uk</a></span></p>");
            PromoLinks.AppendLine("<br><br>");
            PromoLinks.AppendLine("<p><span class=\"ink5 indent\">{PFC_SERVICESTART} ({PFC_TOTALCAROUSELS}/{PFC_TOTALREQUESTS})</span></p>");
            PromoLinks.AppendLine("<br>");
            PromoLinks.AppendLine("<p><span class=\"paper1 ink6\">&nbsp;&nbsp;CEEFAX: The world at your fingertips&nbsp;</span></p>");

            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                                       ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink7 indent\">" + Utility.BlockGraph(" 775<4<h$44") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("         _4_~@|@}           ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink5 indent\">" + Utility.BlockGraph(" 555w15j s5") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("        _?0~@@@@%           ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink7 indent\">" + Utility.BlockGraph(" <$t0p(_ph0pp0p0p0") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph(" j*@@@@@y||t         ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink5 indent\">" + Utility.BlockGraph(" 5 555j*lj 555<5-4") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("  ({@@@@@@@?         ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink5 indent\">" + Utility.BlockGraph(" £!!!!\"\"£\" !!!£!£!") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("  !'~@@@@@@!         ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                     ~o@@@@w1          ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                    j)\"@@@@@@}         ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink7 indent\">" + Utility.BlockGraph("  FROM") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("                x@@g@@@@0        ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                      +\" @@@@@@        ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink6 indent\">" + Utility.BlockGraph("  |<,,l4    ,||||4    ,||||4") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("o@?!       ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink6 indent\">" + Utility.BlockGraph("  @5j}~5?//o55(,~5?//o55($j5") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("j@5") + "</span><span class=\"paper1 ink6\">" + Utility.BlockGraph("j7k7k5  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink6 indent\">" + Utility.BlockGraph("  @urs{55bs@5urs{55bs@5uzuz5") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("j@%") + "</span><span class=\"paper1 ink6\">" + Utility.BlockGraph("j7h4k5  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink6 indent\">" + Utility.BlockGraph("  £££££q}||~5££££q}~@@5££££!") + "</span><span class=\"paper1 ink7\">" + Utility.BlockGraph("*/ ") + "</span><span class=\"paper1 ink6\">" + Utility.BlockGraph("z}~}~5  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                      $@@@@@@@||||x|t  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink7 indent\">" + Utility.BlockGraph("  AND A VERY HAPPY") + "</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("     j@@@@@@@@@@@@@  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                     x|@@@@@@@@@@@@@'  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink7 indent\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NEW YEAR!</span><span class=\"paper1 ink2\">" + Utility.BlockGraph("      ! +/'i@@@@@@@qp0  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                        t@@@@@@@@@@@'  ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                    h@@@@@@@@?s///?!   ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                   8?' \"'     \"        ") + "</span></p>");
            PromoChristmas.AppendLine("<p><span class=\"paper1 ink2 indent\">" + Utility.BlockGraph("                                       ") + "</span></p>");




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
}