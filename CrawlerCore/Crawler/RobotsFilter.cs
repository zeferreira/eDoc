using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace DocCore
{
    /// <summary> 
    /// Represents the rules for a specific domain for a specific host  
    /// (ie it aggregates all the rules that match the UserAgent, plus the special * rules) 
    ///  
    /// http://www.robotstxt.org/ 
    /// </summary> 
    /// <remarks> 
    ///  
    /// </remarks> 
    public class RobotsFilter
    {
        #region Private Fields: _FileContents, _UserAgent, _Server, _DenyUrls, _LogString
        private string _FileContents;
        private string _UserAgent;
        private string _Server;
        /// <summary>lowercase string array of url fragments that are 'denied' to the UserAgent for this RobotsTxt instance</summary> 
        private ArrayList _DenyUrls = new ArrayList();
        private string _LogString = string.Empty;
        #endregion

        //default roles
        string defaultRobotsAllow = @"User-agent: *" + Environment.NewLine + "Allow: /" ;
        string defaultRobotsDeny = @"User-agent: *" + Environment.NewLine + "Disallow: /";

        #region Constructors: require starting Url and UserAgent to create an object
        private RobotsFilter()
        { }

        public RobotsFilter(Uri startPageUri, string userAgent)
        {
            _UserAgent = userAgent;
            _Server = startPageUri.Host;

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(startPageUri.Scheme + "://" +  startPageUri.Authority + "/robots.txt");
            try
            {
                System.Net.HttpWebResponse webresponse = (System.Net.HttpWebResponse)req.GetResponse();

                using (System.IO.StreamReader stream = new System.IO.StreamReader(webresponse.GetResponseStream(), true))
                {
                    _FileContents = stream.ReadToEnd();
                } // stream.Close(); 

                string[] fileLines = _FileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                ProcessRoles(fileLines);
            }
            catch (System.Net.WebException we)
            {
                HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Robots.txt não encontrado");
                    Console.WriteLine("Assumindo regra padrão (tudo permitido)");
                    _FileContents = defaultRobotsAllow;
                    string[] fileLines = _FileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    ProcessRoles(fileLines);
                }
                if (errorResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Acesso negado");
                    Console.WriteLine("Assumindo regra padrão (tudo negado)");
                    _FileContents = defaultRobotsDeny;
                    string[] fileLines = _FileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    ProcessRoles(fileLines);
                }
                if (errorResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Acesso negado");
                    Console.WriteLine("Assumindo regra padrão (tudo negado)");
                    _FileContents = defaultRobotsDeny;
                    string[] fileLines = _FileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    ProcessRoles(fileLines);
                }


                //_FileContents = String.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        private void ProcessRoles(string[] fileLines)
        {
            bool rulesApply = false;
            foreach (string line in fileLines)
            {
                RobotInstruction ri = new RobotInstruction(line);
                switch (ri.Instruction[0])
                {
                    case '#':   //then comment - ignore 
                        break;
                    case 'u':   // User-Agent 
                        if ((ri.UrlOrAgent.IndexOf("*") >= 0)
                            || (ri.UrlOrAgent.IndexOf(_UserAgent) >= 0))
                        { // these rules apply 
                            rulesApply = true;
                            Console.WriteLine(ri.UrlOrAgent + " " + rulesApply);
                        }
                        else
                        {
                            rulesApply = false;
                        }
                        break;
                    case 'd':   // Disallow 
                        if (rulesApply)
                        {
                            _DenyUrls.Add(ri.UrlOrAgent.ToLower());
                            Console.WriteLine("D " + ri.UrlOrAgent);
                        }
                        else
                        {
                            Console.WriteLine("D " + ri.UrlOrAgent + " is for another user-agent");
                        }
                        break;
                    case 'a':   // Allow 
                        Console.WriteLine("A" + ri.UrlOrAgent);
                        break;
                    default:
                        // empty/unknown/error 
                        Console.WriteLine("# Unrecognised robots.txt entry [" + line + "]");
                        break;
                }
            }

        }

        public static string[] GetLines(string content)
        {
            string[] result = new string[1];
            string temp = @"Header\r\n=====\r\n\r\nParagraph";
            string temp2 = @"User-agent: *\r\nAllow: /engine/about\r\nAllow: /engine/search\r\nDisallow: /";
            string teste = @"# robots.txt - robot exclusion file - back-end server version - no robots!\n# ========================================================================\n\nUser-agent: htdig/3.1.6\nDisallow: /htbin-post\nDisallow: /cgi-bin\nDisallow: /entrez\nDisallow: /COG\...";

            int patternCounter = Regex.Matches(temp, @"\r\n").Count;
            string tmpStr = teste;

            if (patternCounter > 0)
            {
                tmpStr = temp.Replace(@"\r\n", @" \r\n2 ");
            }
            else
            {
                tmpStr = temp.Replace(@"\n", @" \n ");
            }

            string[] lines = temp.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines3 = temp.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            string[] lines2 = teste.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines4 = tmpStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines5 = tmpStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines6 = Regex.Split(temp, "\r\n|\r|\n");
            string[] lines7 = temp.Split(new[] { '\r', '\n' });
            
            string[] lines8 = temp2.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine(lines.ToString());
            Console.WriteLine(lines2.ToString());
            Console.WriteLine(lines3.ToString());
            Console.WriteLine(lines4.ToString());
            Console.WriteLine(lines5.ToString());
            Console.WriteLine(lines6.ToString());
            Console.WriteLine(lines7.ToString());
            Console.WriteLine(lines8.ToString());

            return result;
        }

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        #region Methods: Allow
        /// <summary> 
        /// Does the parsed robots.txt file allow this Uri to be spidered for this user-agent? 
        /// </summary> 
        /// <remarks> 
        /// This method does all its "matching" in lowercase - it expects the _DenyUrl  
        /// elements to be ToLower() and it calls ToLower on the passed-in Uri... 
        /// </remarks> 
        public bool Allowed(Uri uri)
        {
            if (_DenyUrls.Count == 0) return true;

            string url = uri.AbsolutePath.ToLower();
            foreach (string denyUrlFragment in _DenyUrls)
            {
                if (url.Length >= denyUrlFragment.Length)
                {
                    if (url.Substring(0, denyUrlFragment.Length) == denyUrlFragment)
                    {
                        return false;
                    } // else not a match 
                } // else url is shorter than fragment, therefore cannot be a 'match' 
            }
            if (url == "/robots.txt") return false;
            // no disallows were found, so allow 
            return true;
        }
        #endregion

        #region Private class: RobotInstruction
        /// <summary> 
        /// Use this class to read/parse the robots.txt file 
        /// </summary> 
        /// <remarks> 
        /// Types of data coming into this class 
        /// User-agent: * ==> _Instruction='User-agent', _Url='*' 
        /// Disallow: /cgi-bin/ ==> _Instruction='Disallow', _Url='/cgi-bin/' 
        /// Disallow: /tmp/ ==> _Instruction='Disallow', _Url='/tmp/' 
        /// Disallow: /~joe/ ==> _Instruction='Disallow', _Url='/~joe/' 
        /// </remarks> 
        private class RobotInstruction
        {
            private string _Instruction;
            private string _Url = string.Empty;

            /// <summary> 
            /// Constructor requires a line, hopefully in the format [instuction]:[url] 
            /// </summary> 
            public RobotInstruction(string line)
            {
                string instructionLine = line;
                int commentPosition = instructionLine.IndexOf('#');
                if (commentPosition == 0)
                {
                    _Instruction = "#";
                }
                if (commentPosition >= 0)
                {   // comment somewhere on the line, trim it off 
                    instructionLine = instructionLine.Substring(0, commentPosition);
                }
                if (instructionLine.Length > 0)
                {   // wasn't just a comment line (which should have been filtered out before this anyway 
                    string[] lineArray = instructionLine.Split(':');
                    _Instruction = lineArray[0].Trim().ToLower();
                    if (lineArray.Length > 1)
                    {
                        _Url = lineArray[1].Trim();
                    }
                }
            }
            /// <summary> 
            /// Lower-case part of robots.txt line, before the colon (:) 
            /// </summary> 
            public string Instruction
            {
                get { return _Instruction; }
            }
            /// <summary> 
            /// Lower-case part of robots.txt line, after the colon (:) 
            /// </summary> 
            public string UrlOrAgent
            {
                get { return _Url; }
            }
        }
        #endregion
    } 
}
