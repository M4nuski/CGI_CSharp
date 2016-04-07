using System.IO;

namespace CommonItems
{
    static class CSV_Parser
    {
        public static string[] Parse1D(string filename)
        {
            return Parse1D(filename, new string[0]);
        }

        public static string[] Parse1D(string filename, string[] fallback)
        {
            if (File.Exists(filename))
            {
                var csvFile = File.OpenText(filename);
                var csvList = csvFile.ReadToEnd().Split(',');
                csvFile.Close();

                if (csvList.Length > 0)
                {
                    var outputList = new string[csvList.Length];
                    for (var i = 0; i < csvList.Length; i++)
                    {
                        outputList[i] = csvList[i].Trim();
                    }
                    return outputList;
                }
            }
            return fallback;
        }
    }
}
