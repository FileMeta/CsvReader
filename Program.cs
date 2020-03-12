using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileMeta
{
    class Program
    {
        const string c_csvTestPart1 =
@"A,B,C,D,E
 A , B , C , D , E 
1,2,3,4,5,6,7,8,9,10
""101"",""102"",""103"",""104"",""105""
""Has,embedded,commas"",""Has""""Embedded""""Quotes"",""Has,Embedded""""Commas""""and,Quotes""
";

        // Need different escaping for part 2
        const string c_csvTestPart2 = 
"Followed by CR\rFollowed by LF\nFollowed by CRLF\r\nFollowed by LFCR (which results in a blank line)\n\rEndOfFile";

        /// <summary>
        /// Unit test for CsvReader class
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// This is a minimal baseline unit test. More tests can/should be added over time.
        /// </remarks>
        static void Main(string[] args)
        {
            try
            {
                using (var reader = new CsvReader(new StringReader(c_csvTestPart1 + c_csvTestPart2)))
                {
                    var line = reader.Read();
                    AssertLength(line, 5);
                    for (int i = 0; i<5; ++i)
                    {
                        AssertEqual(line[i], $"{(char)('A' + i)}");
                    }

                    line = reader.Read();
                    AssertLength(line, 5);
                    for (int i = 0; i < 5; ++i)
                    {
                        AssertEqual(line[i], $" {(char)('A' + i)} ");
                    }

                    line = reader.Read();
                    AssertLength(line, 10);
                    for (int i=0; i<10; ++i)
                    {
                        AssertEqual(line[i], $"{i+1}");
                    }

                    line = reader.Read();
                    AssertLength(line, 5);
                    for (int i = 0; i < 5; ++i)
                    {
                        AssertEqual(line[i], $"{i + 101}");
                    }

                    line = reader.Read();
                    AssertLength(line, 3);
                    AssertEqual(line[0], "Has,embedded,commas");
                    AssertEqual(line[1], "Has\"Embedded\"Quotes");
                    AssertEqual(line[2], "Has,Embedded\"Commas\"and,Quotes");

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], "Followed by CR");

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], "Followed by LF");

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], "Followed by CRLF");

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], "Followed by LFCR (which results in a blank line)");

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], string.Empty);

                    line = reader.Read();
                    AssertLength(line, 1);
                    AssertEqual(line[0], "EndOfFile");

                    line = reader.Read();
                    Assert(line == null); // End of File
                }

                Console.WriteLine("All tests succeeded.");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void Assert(bool assertion)
        {
            if (!assertion)
            {
                throw new Exception("Failed test");
            }
        }

        static void AssertEqual(string a, string b)
        {
            if (!string.Equals(a, b, StringComparison.Ordinal))
            {
                throw new Exception($"Failed test: \"{a}\" != \"{b}\"");
            }
        }

        static void AssertLength(string[] line, int expectedLength)
        {
            if (line == null)
            {
                throw new Exception("Failed test: Premature end of file (returned null)");
            }
            if (line.Length != expectedLength)
            {
                throw new Exception($"Failed test: Found {line.Length} elements. Expected {expectedLength}");
            }
        }


    }
}
