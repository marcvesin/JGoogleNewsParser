using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NReadability;
using System.Threading;
using System.ComponentModel;

namespace GoogleNewsParser
{
    class Program
    {
        static void Main(string[] args)
        {
//            AlchemyAPI.AlchemyAPI alchemyObj = new AlchemyAPI.AlchemyAPI();
//            alchemyObj.SetAPIKey("e641ec933268d470a1c39569836bad17ced05355");
//            string xml = alchemyObj.URLGetText(@"http://www.lesechos.fr/entreprises-secteurs/air-defense/actu/reuters-00511269-eads-rachetera-jusqu-a-3-75-milliards-d-euros-d-actions-554530.php


            NewsParser parser = new NewsParser("q=Thales&p=1&lang=fr");

            using (StreamWriter r = new StreamWriter("lolilol.txt"))
            {
                r.WriteLine(parser.GetXmlResult());
            }


        }
    }

}
