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

        public StringBuilder BuildTeletextPage(StringBuilder newPage)
        {
            _mc.MaxPages++;

            // Generate the <div> enclosure that contains the individual page
            StringBuilder sb = new StringBuilder();
            _mc.DisplayHtml.AppendLine($"<div id=\"page{_mc.MaxPages}\" style=\"display:none\">");
            _mc.DisplayHtml.Append(newPage);
            _mc.DisplayHtml.Append("</div>");

            return sb;
        }
    }
}