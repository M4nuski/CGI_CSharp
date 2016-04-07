using System;
using System.Diagnostics;
using CommonItems;

namespace form_log
{
    class Program
    {

        private static CGI_Parser CGI_Data;

        //You may have to rename it to form_log.cgi instead of .exe for safety reasons
        //The server will require the interpreter of .cgi files to be set as "cmd.exe /c"
        static void Main(string[] args)
        {

#if DEBUG
            Debugger.Launch();
#endif

            var logFile = new DefaultLog();

            if (args.Length == 0)
            {
                logFile.WriteLine("args[]");
            }
            else
            {
                for (var i = 0; i < args.Length; i++)
                {
                    logFile.WriteLine($"args[{i}]: {args[i]}");
                }
            }

            try
            {
                CGI_Data = new CGI_Parser();

                logFile.WriteLine("CGI Variables:");
                logFile.WriteLine(StringEnumerator.StringDictionary(CGI_Data.CGI_Variables));
                logFile.WriteLine("Form Data:");
                logFile.WriteLine(StringEnumerator.StringDictionary(CGI_Data.Form_Data));
                logFile.WriteLine("FIle(s) Data:");
                logFile.WriteLine(StringEnumerator.FileInfoList(CGI_Data.File_Data));

                CGI_Data.ReturnHTTPReloadPage();
            }
            catch (Exception ex)
            {
                logFile.WriteLine("Error parsing data: " + ex.Message);

                CGI_Data.ReturnTextPlainHeader();
                Console.WriteLine("Error parsing data: " + ex.Message);

            }
        }
    }
}
