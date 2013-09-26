using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleNewsParser
{
    public class NewsExceptions : Exception
    {}

    public class GoogleException : NewsExceptions
    { }

    public class InvalidParameters : GoogleException
    {
        public InvalidParameters()
            : base()
        { 
            HelpLink = "Invalid Parameters. Please enter : "+
                        "\n\t q : parameters "+
                        "\n\t p : page count "+
                        "\n\t lang : language ( either fr or en )."+
                        "\nExemple : q=Thales;Hollande&p=1 or q=Thales;Hollande&p=1&lang=fr"; 
        }
    
    }

    public class InvalidRequest : GoogleException
    {
        public InvalidRequest()
            : base()
        {
            HelpLink = "No valid informations available for your request.";
        }

    }
}
