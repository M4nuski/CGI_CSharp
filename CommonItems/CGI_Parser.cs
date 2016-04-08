using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace CommonItems
{

    public class CGI_Parser
    {
        public Dictionary<string, string> CGI_Variables { get; private set; }
        public Dictionary<string, string> Form_Data { get; private set; }
        public List<MultipartParser.FileStruct> File_Data { get; private set; }

        public List<string> CGI_Variables_List = new List<string>
        {
            "DOCUMENT_ROOT", //   The root directory of your server
            "HTTP_COOKIE", // The visitor's cookie, if one is set
            "HTTP_HOST", //   The hostname of the page being attempted
            "HTTP_REFERER", //    The URL of the page that called your program
            "HTTP_CONTENT_TYPE",

            "HTTP_USER_AGENT", // The browser type of the visitor
            "HTTPS", //   "on" if the program is being called through a secure server
         //   "PATH", //    The system path your server is running under
            "QUERY_STRING", //    The query string (see GET, below)

            "REMOTE_ADDR", // The IP address of the visitor
            "REMOTE_HOST", // The hostname of the visitor (if your server has reverse-name-lookups on; otherwise this is the IP address again)
            "REMOTE_PORT", // The port the visitor is connected to on the web server
            "REMOTE_USER", // The visitor's username (for .htaccess-protected pages)

            "REQUEST_METHOD", // GET or POST
            "REQUEST_URI", // The interpreted pathname of the requested document or CGI (relative to the document root)
            "SCRIPT_FILENAME", // The full pathname of the current CGI
            "SCRIPT_NAME", // The interpreted pathname of the current CGI (relative to the document root)

            "SERVER_ADMIN", // The email address for your server's webmaster
            "SERVER_NAME", // Your server's fully qualified domain name (e.g. www.cgi101.com)
            "SERVER_PORT", // The port number your server is listening on
            "SERVER_SOFTWARE" // The server software you're using (e.g. Apache 1.3)
        };

        //HTTP POST Content Type Markers:
        private const string x_www_form_urlencoded_marker = "x-www-form-urlencoded";
        private const string multipart_formdata_marker = "multipart/form-data";
        //private const string text_plain = "text/plain"; // commented-out because default fallback and useless

        public CGI_Parser()
        {
            CGI_Variables = new Dictionary<string, string>();
            ReadAllVariables();

            var encType = CGI_Variables["HTTP_CONTENT_TYPE"];

            if (CGI_Variables["REQUEST_METHOD"].Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                Form_Data = parseData(x_www_form_urlencoded_marker, CGI_Variables["QUERY_STRING"]);
            }
            else
            {
                if (encType.Contains(multipart_formdata_marker))
                {
                    var mpp = new MultipartParser(Console.OpenStandardInput(), Console.InputEncoding);
                    File_Data = mpp.Files;
                    Form_Data = mpp.Variables;
                }
                else
                {
                    Form_Data = parseData(encType, Console.In.ReadToEnd());
                }
            }
        }

        private static Dictionary<string, string> parseData(string EncType, string input)
        {
            var lines = input.Split('&');

            if (EncType.Contains(x_www_form_urlencoded_marker))
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    lines[i] = WebUtility.UrlDecode(lines[i]);
                }
            }

            var dict = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var keypair = line.Split(new[] { '=' }, 2);
                if (keypair.Length == 2)
                {
                    dict.Add(keypair[0], keypair[1]);
                }
            }

            return dict;
        }

        private void ReadAllVariables()
        {
            foreach (var VARIABLE in CGI_Variables_List)
            {
                CGI_Variables.Add(VARIABLE, getSafeEnvironmentVariable(VARIABLE));
            }
        }
        private static string getSafeEnvironmentVariable(string VAR)
        {
            var result = Environment.GetEnvironmentVariable(VAR);
            return result ?? string.Empty;
        }

        public void ReturnTextHTMLHeader()
        {
            Console.Write("Content-Type: text/html\n\n");
        }
        public void ReturnTextPlainHeader()
        {
            Console.Write("Content-Type: text/plain\n\n");
        }
        public void ReturnHTTPReloadPage()
        {
            Console.Write("Location: " + CGI_Variables["HTTP_REFERER"] + "\n\n");
        }
        public void ReturnHTTPLocation(string location)
        {
            Console.Write("Location: " + location + "\n\n");
        }
        public void ReturnFile(string path)
        {
            Console.Write($"X-Sendfile: {path}\n\n");
        }

    }
}
