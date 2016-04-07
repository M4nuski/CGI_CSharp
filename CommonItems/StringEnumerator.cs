using System.Collections.Generic;
using System.Text;

namespace CommonItems
{
    static class StringEnumerator
    {
        public static string StringDictionary(Dictionary<string, string> dict)
        {
            var sb = new StringBuilder();
            foreach (var VARIABLE in dict)
            {
                sb.AppendFormat("{0}: {1}\r\n", VARIABLE.Key, VARIABLE.Value);
            }
            return sb.ToString();
        }

        public static string FileInfoList(List<MultipartParser.FileStruct> Files)
        {
            var sb = new StringBuilder();
            foreach (var VARIABLE in Files)
            {
                sb.AppendLine("Content-Type: " + VARIABLE.ContentType);
                sb.AppendLine("Tag Name: " + VARIABLE.TagName);
                sb.AppendLine("File Name: " + VARIABLE.FileName);
                sb.AppendLine("File Size: " + VARIABLE.Content.Length.ToString("D"));
            }
            return sb.ToString();
        }
    }
}
