using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace GoogleNewsParser
{
    public class GoogleNews //: News_Parser
    {

        List<Article> _articles;
        string _language;

        public List<Article> Articles { get { return _articles; } }
        //Constructeurs
        public GoogleNews()
        {
            _articles = new List<Article>();
            _language = "fr";
        }
        public GoogleNews(string parameters, int n_page,string language)
        {
            _language = language;
            _articles = Get_News(parameters, n_page);
            
        }


        //Récupération de l'url de recherche google associée à la requete
        public string GoogleUrl(string parameter, int page)
        {
            string googleUrl = "https://www.google.";
            if (_language == "fr")    googleUrl+= String.Format("fr/search?hl={0}&gl={0}&tbm=nws&q={1}",_language,parameter);
            else                      googleUrl += String.Format("com/search?hl={0}&gl={0}&tbm=nws&q={1}", _language, parameter);
            
            if (page > 1)   googleUrl += "&start=" + (page * 10 - 10);

            return googleUrl;
        }
        //Récupère l' url / Titre / Source de chaque articles
        public List<Article> Get_News(string parameter, int number_Page)
        {
            var articles = new List<Article>();
            List<string> urls = new List<string>();
            //On récupère toutes les urls dans le cas où l'on souhaite faire une recherche sur plusieurs pages.
            for (int i = 1; i <= number_Page; i++)
                urls.Add(GoogleUrl(parameter, i));


            //On met tous les resultats dans une liste d'articles
            foreach (string url in urls)
                foreach (Article article in Page_Parser(url))
                    articles.Add(article);

            _articles = articles;
            return articles;
        }
        private List<Article> Page_Parser(string url)
        {
            var list_articles = new List<Article>();
            HtmlDocument doc = NewsParser.Load_page(url);

            //On récupère la zone concernant tous les articles
            HtmlNode articleArea = null;
            try { articleArea = doc.DocumentNode.Descendants("ol").First(); }
            catch { throw new InvalidRequest(); }

            IEnumerable<HtmlNode> articles = articleArea.Descendants("li");

            foreach (HtmlNode info in articles)
            {
                var informations = new List<KeyValuePair<string,string>>();
                Get_URL_Title(info,ref informations);

                foreach (KeyValuePair<string,string> information in informations)
                {
                    string source = "N/A", date = "N/A",time ="N/A";
                    Get_Source(info, ref source);
                    Get_Date_Time(info, ref date, ref time,_language);

                    var article = new Article()
                    {
                        Url = information.Value,
                        Title = information.Key,
                        Source = source,
                        Date = date,
                        Time = time
                    };
                    list_articles.Add(article);
                }

                
            }

            return list_articles;
        }
        private void Get_URL_Title(HtmlNode info, ref List<KeyValuePair<string,string>> informations)
        {
            //On cherche chaque balises <a> </a> correspondantes à une URL
            foreach (HtmlNode links in info.Descendants("a"))
            {
                //On regarde les attributs de chaque URL
                foreach (HtmlAttribute attribut in links.Attributes)
                    //Si une URL est de type "class" c'est qu'elle contient les Articles qui nous interessent.
                    if (links.InnerText.Length != 0 && links.GetAttributeValue("class", null) != "gl")
                    {
                        //Parse l'Url non formatée puis le titre de l'article.
                        string url = Uri.UnescapeDataString(attribut.Value);
                        string title = links.InnerText;
                        var news = new KeyValuePair<string,string>(title,url);
                        informations.Add(news);
                    }
            }
        }
        private void Get_Source(HtmlNode info, ref string source)
        {
                source = info.Descendants("span").First().InnerText;
        }
        private void Get_Date_Time(HtmlNode info,ref  string date, ref string time,string language)
        {
        
            string timeReference = String.Empty;
            string number_test = @"\d\d?\s", format_test = @"^\s\d\d?\s[a-zA-Z]";//Match " 23 m"
            string date_str = info.Descendants("span").First().InnerText.Split('-').Last();
            double number=0;

                if ((Regex.IsMatch(date_str, format_test) && _language == "fr") || (!date_str.Contains("ago") && _language == "en"))
                {
                    DateTime datetime = DateTime.Parse(date_str);
                    date = datetime.ToShortDateString();
                }
                else
                {
                    try
                    {
                        number = Convert.ToDouble(Regex.Match(date_str, number_test).Value);
                    }
                    catch(Exception){number = 0;}

                    timeReference = Regex.Split(date_str, number_test).Last();

                    DateTime now = DateTime.Now;

                    if (timeReference.Contains("sec")) { now = now.Subtract(TimeSpan.FromSeconds(number)); time = String.Format("{0}h{1}min{2}s", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString()); }
                    else if (timeReference.Contains("min")) { now = now.Subtract(TimeSpan.FromMinutes(number)); time = String.Format("{0}h{1}min", now.Hour.ToString(), now.Minute.ToString()); }
                    else if (timeReference.Contains("heure") || timeReference.Contains("hours")) { now = now.Subtract(TimeSpan.FromHours(number)); time = String.Format("{0}h", now.Hour.ToString()); }
                    else if (timeReference.Contains("jour") || timeReference.Contains("day")) now = now.Subtract(TimeSpan.FromDays(number));

                    date = now.ToShortDateString();
                    if (now.Hour < 10 && time != "N/A") time = "0" + time;
            }
        }
       
    }
}
