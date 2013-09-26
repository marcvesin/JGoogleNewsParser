using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace GoogleNewsParser
{
    //Définit ce qu'est un article.
    public class Article
    {
        string _url;
        string _title;
        string _source;
        string _date;
        string _time;
        XElement _content;
        //Constructeur.
        public Article()
        {
            _url = "N/A";
            _title = "N/A";
            _source = "N/A";
            _date = "N/A";
            _time = "N/A";
        }

        public string Url
        {
            get { return _url; }
            set { _url = Clean_Url(value); }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }/* _title = Decoder(value);*/
        }
        public string Source
        {
            get { return _source; }
            set
            {
                char[] source = value.ToCharArray();
                for (int i = 0; i < source.Length; i++){
                    if (source[i].GetHashCode() == 537796622)
                    {
                        _source = new string(source).Substring(0,i);
                        return;
                    }
                    if(source[i] == '-' && source[i+1] == ' ' )
                    {
                        _source = new string(source).Substring(0,i);
                        return;
                    }
                }
                _source = new string(source);
            }
        }
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public XElement Content
        {
            get { return _content; }
            set { _content = value; }
        }

        //Méthodes permettant le traitement au préalable des données entrées.
        static public string Clean_Url(string url)
        {
            url = url.Replace("/url?q=", String.Empty);
            string type = string.Empty;

            if (url.Contains(".php"))           type = ".php";
            else if (url.Contains(".html"))     type = ".html";
            else if (url.Contains(".shtml"))    type = ".shtml";

            if (type != String.Empty)   return url.Substring(0, url.IndexOf(type) + type.Length);
            if (url.Contains("/?"))     url = url.Substring(0, url.IndexOf("/?"));
            else                        url = url.Substring(0, url.IndexOf("&amp"));

            return url;

        }
        public XElement Get_XmlElement()
        {
            XElement article =
                new XElement("article",
                    new XElement("url", _url),
                    new XElement("title", _title),
                    new XElement("source", _source),
                    new XElement("date",_date),
                    new XElement("time",_time),
                    _content);

            return article;
        }

    }
}
