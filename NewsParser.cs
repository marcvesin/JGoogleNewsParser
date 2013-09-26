using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace GoogleNewsParser
{
    public class NewsParser
    {
        public List<Article> ArticleList;
        //static readonly object _locker = new object();

        public NewsParser()
        {
            ArticleList = new List<Article>();
        }
        public NewsParser(string parameter, int page, string language)
        {
            ArticleList = new List<Article>();
            GetNews(parameter, page,language);
        }
        public NewsParser(string parameters)
        {
            Dictionary<string, string> dicParameters = ParametersCheck(parameters);
            List<string> query = dicParameters["q"].Split(';').ToList();

            foreach(string queryParameters in query)
                GetNews(queryParameters, Convert.ToInt32(dicParameters["p"]), dicParameters["lang"]);
        }

        private  Dictionary<string,string> ParametersCheck(string parameters)
        {
            ArticleList = new List<Article>();
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            foreach (string p in parameters.Split('&'))
            {
                List<string> p1 = p.Split('=').ToList();
                if (dicParameters.Keys.Contains(p1.First())) throw new InvalidParameters();
                else
                {
                    if (p1.First() != "q" &&  p1.First() != "p" && p1.First() != "lang") throw new InvalidParameters();
                    if (p1.First() == "lang" && (p1.Last() != "fr" && p1.Last() != "en")) throw new InvalidParameters();

                    dicParameters.Add(p1.First(), p1.Last());
                }
            }

           if(!dicParameters.Keys.Contains("q")) throw new InvalidParameters();
           if(!dicParameters.Keys.Contains("p")) dicParameters.Add("p","1");
           if(!dicParameters.Keys.Contains("lang")) dicParameters.Add("lang","fr");
            
            return dicParameters;
        }
        public List<Article> GetNews(string parameter, int n_page, string language)
        {
            List<Article> google_articles = new GoogleNews(parameter, n_page,language).Articles;

            List<Task> tasks = new List<Task>();
            foreach (Article article in google_articles)
                tasks.Add(Task.Run(() => Article_Parser(article)));

            Task.WaitAll(tasks.ToArray());

            foreach (Article article in google_articles)
                ArticleList.Add(article);

            return google_articles;
        }
        public List<Article> Articles { get { return ArticleList; } }

        public void Article_Parser(Article article)
        {
            //Console.WriteLine("\tBegin article parser => PAGE : {0}", article.Title);

            Article tempArticle = new Article();
            try
            {
                tempArticle = new ArticleParser(article.Url).Article;
            }
            catch{ //Console.WriteLine(e.Message);
                Console.WriteLine("Error url NewsParser"); }

            article.Content = tempArticle.Content;
        }

        static public HtmlDocument Load_page(string url)
        {
            HtmlDocument page = new HtmlWeb().Load(url);

            Console.WriteLine("Google page Loaded...");
            page.OptionFixNestedTags = true;
            page.OptionAutoCloseOnEnd = true;

            return page;
        }
        //Fonction permettant de supprimer le Header et les script du code afin de le rendre plus léger et donc rapide à lire et traiter
        //public static HtmlNode Return_Treated_HTML(HtmlDocument doc)
        //{

        //    var htmlElement = doc.DocumentNode.Element("html");
        //    htmlElement.Element("head").Remove();
        //    //Remove script.
        //    var l = htmlElement.Descendants("script");
        //    for (int i = l.Count() - 1; i >= 0; i--)
        //        htmlElement.SelectSingleNode(l.ElementAt(i).XPath).Remove();

        //    return htmlElement;
        //}

        public XDocument GetXmlResult()
        {
            XDocument doc = new XDocument(new XElement("Google_news"));
            foreach (Article article in ArticleList)
                doc.Root.Add(article.Get_XmlElement());

            return doc;
        
        }
        public void Display_Result()
        {
            foreach (var elem in ArticleList)
            {
                Console.WriteLine("\n\nTITRE : " + elem.Title);
                Console.WriteLine("\tSOURCE : " + elem.Source);
                Console.WriteLine("\tURL : " + elem.Url);
                Console.WriteLine("\tDATE : " + elem.Date);
                Console.WriteLine("\tHEURE : " + elem.Time);
            }
        }
    }
}
