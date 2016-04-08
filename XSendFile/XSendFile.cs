using System;
using System.Diagnostics;
using CommonItems;

namespace XSendFile
{
    class XSendFile
    {
        private static DefaultLog log;
        private static CGI_Parser Parser;
        static void Main(string[] args)
        {

#if DEBUG
            Debugger.Launch();
#endif

            log = new DefaultLog();
            try
            {
                Parser = new CGI_Parser();
                if (Parser.CGI_Variables["REQUEST_METHOD"] == "GET")
                {
                    var filePath = Parser.Form_Data["filename"];
                    if (filePath != "") Parser.ReturnFile(filePath);
                }
                else returnError("Wrong request method (should be GET)");
            }
            catch (Exception ex)
            {
                returnError(ex.Message);
            }

        }

        private static void returnError(string Message)
        {
            Message = "Error: " + Message;
            log.WriteLine(Message);
            Parser.ReturnTextPlainHeader();
            Console.WriteLine(Message);
        }
    }
}
