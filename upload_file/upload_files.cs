using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommonItems;

namespace upload_files
{
    //You may have to rename it to upload_files.cgi instead of .exe for safety reasons
    //The server will require the interpreter of .cgi files to be set as "cmd.exe /c"
    class upload_files
    {
        private static string[] acceptedExt = { "ZIP", "JPG", "PNG", "MP3", "PDF", "TXT", "LOG" }; //default list
        private const string extFileName = "upload_ext.txt"; //csv file containing updated/shared list

        private const string downloadPath = "\\uploads\\";

        private const int maxFileSize = 25000000;

        private static CGI_Parser CGI_Data;
        private static DefaultLog log;

        private static List<string> errorList = new List<string>(); 

        static void Main(string[] args)
        {

            //load updated/shared list if it exists
            acceptedExt = CSV_Parser.Parse1D(extFileName, acceptedExt);

#if DEBUG
            Debugger.Launch();
#endif

            //start logfile
            log = new DefaultLog();

            try
            {
                //parse data
                CGI_Data = new CGI_Parser();

                log.WriteLine(StringEnumerator.StringDictionary(CGI_Data.Form_Data));
                log.WriteLine(StringEnumerator.FileInfoList(CGI_Data.File_Data));

                var path = CGI_Data.CGI_Variables["DOCUMENT_ROOT"]  + downloadPath; // C:\root \uploads\
                var localPath = new DirectoryInfo(path);
                if (!localPath.Exists) localPath.Create();

                if (CGI_Data.File_Data.Count > 0)
                {
                    foreach (var uploadedFile in CGI_Data.File_Data)
                    {
                        if (acceptedExt.Contains(uploadedFile.FileName.ToUpperInvariant().Substring(uploadedFile.FileName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase)+1)))
                        {
                            if (uploadedFile.Content.Length <= maxFileSize)
                            {
                                var filepath = path + uploadedFile.FileName;
                                if (!File.Exists(filepath))
                                {
                                    var localFile = File.Create(filepath);
                                    localFile.Write(uploadedFile.Content, 0, uploadedFile.Content.Length);
                                    localFile.Flush();
                                    localFile.Close();
                                }
                                else returnError("File already exists on server.");
                            }
                            else returnError("File size over maximum permitted.");
                        }
                        else returnError("File extension not permitted.");
                    } 
                    //foreach
                }
                else returnError("No files uploaded (?).");
            }

            catch (Exception ex)
            {
                returnError(ex.Message);
            }



            if (errorList.Count == 0)
            {
                CGI_Data.ReturnHTTPReloadPage();
            }
            else
            {
                log.WriteLine(StringEnumerator.StringDictionary(CGI_Data.CGI_Variables));

                CGI_Data.ReturnTextPlainHeader();
                foreach (var error in errorList)
                {
                    Console.WriteLine(error);
                }
            }
        }

        private static void returnError(string message)
        {
            errorList.Add(message);
            log.WriteLine("ERROR: " + message);
        }
    }
}
