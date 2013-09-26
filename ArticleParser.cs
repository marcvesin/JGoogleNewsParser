using HtmlAgilityPack;
using NReadability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;



namespace GoogleNewsParser
{
    public class ArticleParser //: News_Parser
    {

        Article _article;
        public Article Article { get { return _article; } }

        public ArticleParser()
        {
            _article = new Article();
        }
        public ArticleParser(string url)
        {
            _article = Get_Article_Content(url);
        }
        public Article Get_Article_Content(string url)
        {
            XElement content = Get_Content(url);

            var article = new Article(){Content = content };
            _article = article;

            return article;
           
        }
        static public XElement Get_Content(string url)
        {
            WebTranscodingResult page = Extract_content(url);

            XElement content =  new XElement("content");

            if (page != null && page.ContentExtracted == true)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page.ExtractedContent);

                HtmlNode bodyElement = doc.DocumentNode.Descendants("body").First();

                var tags = new Dictionary<string, IEnumerable<HtmlNode>>();
                tags.Add("h1", bodyElement.Descendants("h1"));
                tags.Add("h2", bodyElement.Descendants("h2"));
                tags.Add("p", bodyElement.Descendants("p"));
                tags.Add("br", bodyElement.Descendants("br"));

                 foreach (var tag in tags.Keys)
                     foreach (var value in tags[tag])
                         if (tag.Count() != 0 && value.InnerText.Length > 10)
                             content.Add(new XElement(tag, value.InnerText.Trim()));
            }


            return content;
        }
        static public WebTranscodingResult Extract_content(string url)
        {

            DomSerializationParams param = new DomSerializationParams();
            var t = new NReadability.NReadabilityWebTranscoder();

            var input = new WebTranscodingInput(url);

            param.PrettyPrint = true;
            param.DontIncludeDocTypeMetaElement = true;
            input.DomSerializationParams = param;
            WebTranscodingResult page = null;

            try
            {
                page = t.Transcode(input);
            }
            catch
            {
                //Console.WriteLine("Message : " + e.Message);
                Console.WriteLine("Error Extract_content Article Parser"); 
            }

            return page;
        }
      
    }
}
//private void Get_Date_Time(string url, ref string date, ref string time)
//{
//    string test1 = @"(?'date'\d{4}[./-]\d{2}[./-]\d{2})" + //Format YYYY-MM-DDTHH:mm:ss
//                @"(?'divers'T)" +
//                @"(?'heure'\d{2}:\d{2})";

//    string test2 = @"(?'date'[03]\d[./-][01]\d[./-]\d{2,4})" +//Format JJ/MM/YYYY HH:mm
//                @"(?'divers'.*(?=[012]\d:[012345]\d))?" +
//                @"(?'heure'[012]\d:[012345]\d)?";

//    //string test3 = @"(?'jour'\d?\d)" +//Format jj mois yyyy hh:mm
//    //            @"(?'divers1'.*(?=(?i)(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[ûu]t|septembre|octobre|novembre|d[ée]cembre)))" +
//    //            @"(?'mois'(?i)(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[ûu]t|septembre|octobre|novembre|d[ée]cembre))" +
//    //            @"(?'divers2'.*(?=20[01]\d))" +
//    //            @"(?'annee'20[01]\d)" +
//    //            @"(?'divers2'.*(?=[012]\d:[012345]\d))" +
//    //            @"(?'heure'[012]\d:[012345]\d)?";

//    ////le 21/03/13 à 12h22
//    //string test4 = @"";


//    HtmlDocument doc = News_Parser.Load_page(url); ;
//    HtmlNode body = News_Parser.Return_Treated_HTML(doc);

//    if (Regex.IsMatch(doc.DocumentNode.InnerHtml, test1))
//    {
//        foreach (Match m in Regex.Matches(body.InnerHtml, test1))
//        {
//            date = m.Groups["date"].Value;
//            time = m.Groups["heure"].Value;
//            break;
//        }
//    }
//    else if (Regex.IsMatch(body.InnerHtml, test2))
//    {
//        foreach (Match m in Regex.Matches(doc.DocumentNode.InnerHtml, test2))
//        {
//            if (m.Groups["date"].Success && m.Groups["heure"].Success)
//            {
//                date = m.Groups["date"].Value;
//                time = m.Groups["heure"].Value;
//                break;
//            }
//            else if (m.Groups["date"].Success)
//            {
//                date = m.Groups["date"].Value;
//            }
//        }

//    }

//    //else if (Regex.IsMatch(body.InnerHtml, test3))
//    //{
//    //    foreach (Match m in Regex.Matches(doc.DocumentNode.InnerHtml, test3))
//    //        if (m.Groups["jour"].Success && m.Groups["mois"].Success && m.Groups["annee"].Success && m.Groups["heure"].Success)
//    //        {
//    //            date = m.Groups["jour"].Value + " " + m.Groups["mois"].Value + " " + m.Groups["annee"].Value;
//    //            time = m.Groups["heure"].Value;
//    //            break;
//    //        }
//    //}


//}