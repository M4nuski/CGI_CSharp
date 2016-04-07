using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Based on:
// MultipartParser http://multipartparser.codeplex.com
// Reads a multipart form data stream and returns the filename, content type and contents as a stream.
// 2009 Anthony Super http://antscode.blogspot.com

// Added:
// Real multipart and multifile support.
// Reading of other tags inside the multipart data that comes with the file upload form.
// Removed cross comparaison between raw data and text data.
// 2016 Emmanuel Charette http://www.m4nusky.com

namespace CommonItems
{
    public class MultipartParser
    {
        public struct FileStruct
        {
            public string TagName;
            public string FileName;
            public string ContentType;
            public byte[] Content;
        }

        public Dictionary<string, string> Variables { get; private set; }
        public List<FileStruct> Files { get; private set; }
   
        // Delimiters
        private byte[] CRLF;
        private byte[] CRLFCRLF;
        private byte[] QUOTE;
        private byte[] DASHSADHCRLF;
        private byte[] ContentTypeBytes;
        private byte[] NameBytes;
        private byte[] FileNameBytes;

        public MultipartParser(Stream stream)
        {   
            Parse(stream, Encoding.UTF8);
        }

        public MultipartParser(Stream stream, Encoding encoding)
        {
            Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            Variables = new Dictionary<string, string>();
            Files = new List<FileStruct>();

            CRLF = encoding.GetBytes("\r\n");
            CRLFCRLF = encoding.GetBytes("\r\n\r\n");
            QUOTE = encoding.GetBytes("\"");
            DASHSADHCRLF = encoding.GetBytes("--\r\n");
            ContentTypeBytes = encoding.GetBytes("Content-Type:");
            NameBytes = encoding.GetBytes("name=\"");
            FileNameBytes = encoding.GetBytes("filename=\"");

            var data = toByteArray(stream);

#if DEBUG
Debug.WriteLine(encoding.GetString(data));
#endif

            // The first line should contain the delimiter
            var delimiterEndIndex = indexofbyte(data, CRLF, 0);

            if (delimiterEndIndex > -1)
            {
                var delimiter = subArray(data, 0, delimiterEndIndex);

                var parts = splitBytes(data, delimiter);

                if ((parts.Count() > 1) && (indexofbyte(parts[parts.Count() - 1], DASHSADHCRLF, 0) > -1))
                {
                    foreach (var part in parts)
                    {
                        parsePart(part, encoding);
                    }
                }

            }
        }

        private static List<byte[]> splitBytes(byte[] data, byte[] delimiter)
        {
            var results = new List<byte[]>();

            var startIndex = 0;
            var nextIndex = indexofbyte(data, delimiter, 0);

            if (nextIndex == 0)
            {
                startIndex = delimiter.Length;
            }

            while ((nextIndex > -1) && (nextIndex < data.Length))
            {
                nextIndex = indexofbyte(data, delimiter, startIndex);
                if ((nextIndex > -1))
                {
                    results.Add(subArray(data, startIndex, nextIndex - startIndex));
                }
                else if (startIndex < data.Length)
                {
                    results.Add(subArray(data, startIndex, data.Length - startIndex));
                }
                startIndex = nextIndex + delimiter.Length;
            }

            return results;
        }

        private static int indexofbyte(byte[] data, byte[] sequence, int startindex)
        {
            var index = startindex;
            var match = false;
            if (data.Length >= (startindex + sequence.Length))
            {
                var maxindex = data.Length - sequence.Length;

                while ((!match) && (index <= maxindex))
                {
                    if (data[index] == sequence[0])
                    {
                        match = true;
                        for (var i = 1; (i < sequence.Length) && match; i++) //weird loop opt-out optimization
                        {
                            if (data[index + i] != sequence[i]) match = false;
                        }
                    }
                    index++;
                }
            }
            return (match) ? index-1 : -1;
        }

        private static byte[] subArray(byte[] data, int index, int length)
        {
            var result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static byte[] getBlock(byte[] data, byte[] startDelimiter, byte[] stopDelimiter)
        {
            var startIndex = indexofbyte(data, startDelimiter, 0);
            if (startIndex > -1)
            {
                var blockindex = startIndex + startDelimiter.Length;
                var stopIndex = indexofbyte(data, stopDelimiter, blockindex);

                if (stopIndex > -1) return subArray(data, blockindex, stopIndex - blockindex);

            }
            return null;
        }

        private static string getStringFromDataBlock(byte[] data, byte[] startDelimiter, byte[] stopDelimiter,
            Encoding encoding)
        {
            var dataBlock = getBlock(data, startDelimiter, stopDelimiter);
            return dataBlock != null ? encoding.GetString(dataBlock) : string.Empty;
        }

        private void parsePart(byte[] data, Encoding encoding)
        {
            var contentType = getStringFromDataBlock(data, ContentTypeBytes, CRLF, encoding).Trim();
            var fileName = getStringFromDataBlock(data, FileNameBytes, QUOTE, encoding);
            var tagName = getStringFromDataBlock(data, NameBytes, QUOTE, encoding);

            if ((contentType != "") && (fileName != ""))
            {
                var file = new FileStruct
                {
                    ContentType = contentType,
                    FileName = fileName,
                    TagName = tagName
                };

                var contentIndex = indexofbyte(data, CRLFCRLF, 0);
                if (contentIndex > -1)
                {
                    var blockIndex = contentIndex + CRLFCRLF.Length;
                    file.Content = subArray(data, blockIndex, data.Length - (blockIndex + CRLF.Length));
                }

                Files.Add(file);
            }
            else
            {
                var valueIndex = indexofbyte(data, CRLFCRLF, 0);
                if (valueIndex > -1)
                {
                    var blockIndex = valueIndex + CRLFCRLF.Length;
                    var valueBytes = subArray(data, blockIndex, data.Length - (blockIndex + CRLF.Length));
                    var valueString = encoding.GetString(valueBytes);
                    Variables.Add(tagName, valueString);
                }
            }
        }

        private static byte[] toByteArray(Stream stream)
        {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }


    }
}
