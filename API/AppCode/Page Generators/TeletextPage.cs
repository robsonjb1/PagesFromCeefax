using System;
using System.Text;

namespace PagesFromCeefax
{
    public abstract class TeletextPage
    {
        protected readonly MagazineContent _mc;

        protected TeletextPage(MagazineContent mc)
        {
            _mc = mc;
        }
        
        public StringBuilder BuildTeletextPage(ref int pageNo, StringBuilder content)
        {
            StringBuilder sb = new StringBuilder();
            pageNo++;

            sb.AppendLine(String.Format("<div id=\"page{0}\" style=\"display:none\">", pageNo));
            sb.Append(content);
            sb.AppendLine("</div>");

            return sb;
        }
    }
}