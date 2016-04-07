## CGI_CSharp
CGI (Common Gateway Interface) implementation in C#

- Parse form data to a dictionnary.
- Independant of GET/POST method.
- Support url-encoded and multipart-form-data.
- Support of multiple files.

## CommonItems
Parsing and helpers

- CGI_Parser.cs:	Base class for CGI
- CSV_Parser.cs:	Small class to read CSV file with fallback
- DefaultLog.cs:	Default log file handling
- FormatNumber.cs:	Number formatting wrapper
- MultipartParser.cs:	Multipart Parser for POST
- SafeParse.cs:	Wrapper for string to number parsing with fallback
- StringEnumerator.cs:	Helper class to build string with the content of Dictionaries and List

## /form_log
- Basic example of CGI form data recovery and parsing.
- Enumerate all form data, CGI data from server, and file data if any.
- Include form_log.html demo page

## /upload_files
- Simple handler for uploading files and form data
- Include upload_files.html demo page
- NOTE: basically no content sanitizing. All form data should be checked for escape and/or illegal chars before re-use.