using System;
using System.IO;

namespace CommonItems
{
    class DefaultLog
    {
        private StreamWriter logFile;

        public DefaultLog()
        {
            var name = System.Reflection.Assembly.GetEntryAssembly().Location;
            name = name.Remove(name.LastIndexOf('.'));
            logFile = name != "" ? File.AppendText(name + ".log") : File.AppendText("DefaultLog.log");
            logFile.WriteLine();
            logFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        }

        public void WriteLine(string value)
        {
            logFile.WriteLine(value);
            logFile.Flush(); //slow but required for interpretation inside cmd.exe (handle is closed before data flush)
        }
    }
}
