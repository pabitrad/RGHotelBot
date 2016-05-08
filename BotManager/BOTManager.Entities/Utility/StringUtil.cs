using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Security;
using System.Data;
using RG.Utility;

namespace BOTManager.Entities.Utility
{
    /// <summary>
    /// This class consists of static functions which are
    /// being used for performing string operations;
    /// </summary>
    public class StringUtil
    {
        public StringUtil()
        {
        }


        public static string filterStringForXMLAndDB(string value)
        {
            value = value.Trim();
            value = StringUtil.FilterString(value, true, true);
            value = value.Replace("amp;", "");
            value = StringUtil.EscapeInvalidXmlChars(value);
            value = StringUtil.ReplaceSpecialCharacter(value);
            return value;
        }

        /// <summary>
        /// This function parses the basic string for strings on the basis of regular expression provided
        /// (Based on  regular Expression)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regularExpression"></param>
        /// <returns> Returns the string array consisting of the matched strings</returns>
        public static string[] ParseForStrings(string str, string regularExpression)
        {
            Match[] arr = FindSubstrings(str, regularExpression, false);
            string[] urlStrings = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                urlStrings[i] = arr[i].Value;
            }

            return urlStrings;

        }


        public static string GetStringResult(string strInputText, string strSearchPattern, int GroupNo)
        {
            string strResult = string.Empty;
            try
            {
                Regex objRegex = new Regex(strSearchPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match m in objRegex.Matches(strInputText))
                {
                    strResult = m.Groups[GroupNo].Value.Trim();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex.ToString());
            }
            return strResult;
        }


        public static DataTable FGetDataTable(string StrInput, string StrSearchPattern, int NoOfColumn)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Column0");

            //int intCount = 1;
            Regex objRegx = new Regex(StrSearchPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (Match var in objRegx.Matches(StrInput))
            {
                if (var.Groups[0].ToString().IndexOf(@"hdncityName"" value") >= 0
                    || var.Groups[0].ToString().IndexOf(@"<input type=""button""") >= 0
                    || var.Groups[0].ToString().IndexOf(@"<input type=""submit""") >= 0
                    || var.Groups[0].ToString().IndexOf(@"<input type=""image""") >= 0
                    || var.Groups[0].ToString().IndexOf(@"_hdnBrandName""") >= 0
                    )
                {
                    if (var.Groups[0].ToString().IndexOf(@"AdditionalBECheckAvailContBtn") >= 0
                        || var.Groups[0].ToString().IndexOf(@"Availability") >= 0
                        //** by vivek #Ticket 00005079
                        ///|| var.Groups[0].ToString().IndexOf(@"value=""Continue""") >= 0
                        || (var.Groups[0].ToString().IndexOf(@"value=""Doorgaan""") >= 0 && var.Groups[0].ToString().IndexOf(@"<input type=""submit""") >= 0)
                        || var.Groups[0].ToString().IndexOf(@"value=""View &amp; Book Now""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"value=""Book Now""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"value=""Check price &amp; book""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"value=""Confirm""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"value=""檢查是否可用""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"value=""View Rooms &amp; Rates""") >= 0
                        || var.Groups[0].ToString().IndexOf(@"V110_C1_CBtn") >= 0
                        || var.Groups[0].ToString().IndexOf(@"V0$C3$CBtn") >= 0
                        )
                    { }
                    else { continue; }
                }
                dt.Rows.Add(new object[] { var.Groups[0].ToString() });
            }
            return dt;

        }

        /// <summary>
        /// This is the actual function called by the ParseForStrings function to parse for strings
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matchPattern"></param>
        /// <param name="findAllUnique"></param>
        /// <returns>returns the array of matches</returns>
        private static Match[] FindSubstrings(string source, string matchPattern, bool findAllUnique)
        {
            SortedList uniqueMatches = new SortedList();
            Match[] retArray = null;
            Regex RE = new Regex(matchPattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);
            if (findAllUnique)
            {
                for (int counter = 0; counter < theMatches.Count; counter++)
                {
                    if (!uniqueMatches.ContainsKey(theMatches[counter].Value))
                    {
                        uniqueMatches.Add(theMatches[counter].Value,
                            theMatches[counter]);
                    }
                }
                retArray = new Match[uniqueMatches.Count];
                uniqueMatches.Values.CopyTo(retArray, 0);
            }
            else
            {
                retArray = new Match[theMatches.Count];
                theMatches.CopyTo(retArray, 0);
            }
            return (retArray);
        }

        /// <summary>
        /// The function Provides the remaining string from the mainstring provided the string to be removed
        /// (Based on indexing)
        /// {Condition Appllied
        /// </summary>
        /// <param name="mainString"></param>
        /// <param name="fromString"></param>
        /// <param name="including"></param>
        /// <returns></returns>
        public static string GetRemainingString(string mainString, string fromString, bool including)
        {
            string tempString = String.Empty;
            int tempIndex = mainString.IndexOf(fromString);
            if (tempIndex == -1)
            {
                return String.Empty;
            }
            int fromStringLen = fromString.Length;
            int mainStringLen = mainString.Length;
            if (including == true)
            {
                tempString = mainString.Substring(tempIndex, mainStringLen - tempIndex - fromStringLen);
            }
            else if (including == false)
            {
                tempString = mainString.Substring(tempIndex + fromStringLen, mainStringLen - tempIndex - fromStringLen);

            }
            return tempString;
        }
        /// <summary>
        /// Checks whether some string is present in a string or not
        /// </summary>
        /// <param name="text">source string</param>
        /// <param name="toCheck">string to check</param>
        /// <returns>true if found, otherwise false</returns>
        public static bool CheckForExistance(string text, string toCheck)
        {
            bool exists = false;
            string regularExpression = toCheck;
            Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match match = regex.Match(text);
            if (match != null && match.Length > 0)
                exists = true;
            return exists;
        }
        public static string GetStringBetween(string mainString, string firstString, string secondString)
        {

            int mainStringLen = mainString.Length;
            int firstStringIndex = mainString.IndexOf(firstString);
            int firstStringLen = firstString.Length;
            int secondStringIndex = mainString.IndexOf(secondString);
            int secondStringLen = secondString.Length;
            string tempString = mainString.Substring(firstStringIndex + firstStringLen, secondStringIndex - firstStringIndex - firstStringLen);
            return tempString;
        }

        public static int NextIndexOfString(string mainString, string ofString, int position)
        {
            int i = 0;
            int index = 0;
            string tempString = String.Empty;
            while (i < position)
            {
                index = index + mainString.IndexOf(ofString);
                if (i > 0)
                {
                    index = index + ofString.Length;
                }
                tempString = StringUtil.GetRemainingString(mainString, ofString, false);
                mainString = tempString;
                i++;
            }

            return index;
        }

        public static string GetStringUpto(string mainString, string ofString)
        {
            string tempString = StringUtil.GetRemainingString(mainString, ofString, true);
            tempString = mainString.Replace(tempString, "");
            return tempString;
        }


        public static string ClearString(string str)
        {
            if ((str != null) && (str.Length != 0))
            {
                str = str.Replace("&nbsp;", "");
                str = str.Trim();
            }
            return str;
        }
        public static string GetReverseString(string str)
        {
            char[] charArr;
            StringBuilder sb = new StringBuilder();
            //char[] charRevArr = new char[str.Length];
            charArr = str.ToCharArray();
            int i;
            for (i = charArr.Length - 1; i >= 0; i--)
            {
                sb.Append(charArr[i].ToString());

            }

            return sb.ToString();
        }

        public static string[] SplitStringByFirstNonIntegerChar(string str)
        {
            bool hasRateFound = false;
            StringBuilder intStr = new StringBuilder();
            StringBuilder remStr = new StringBuilder();
            string[] strArr = new string[] { String.Empty, String.Empty };
            char[] charArr;
            charArr = str.Trim().ToCharArray();
            int i = 0;
            for (i = 0; i < charArr.Length; i++)
            {

                if ((char.IsDigit(charArr[i]) & (!hasRateFound)) || charArr[i] == '.')
                {

                    intStr.Append(charArr[i]);
                }
                else
                {
                    hasRateFound = true;
                    remStr.Append(charArr[i]);
                }

            }
            strArr[0] = StringUtil.GetReverseString(intStr.ToString().Trim());
            strArr[1] = StringUtil.GetReverseString(remStr.ToString().Trim());


            return strArr;

        }


        public static MatchCollection GetStringsBetweenTags(string sourceStr, string startTag, string endTag)
        {
            string regularExpression = startTag + @"[\s\S]+?" + endTag;
            Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(sourceStr);
            return (matches);

        }

        public static string GetStringBetweenTags(string sourceStr, string startTag, string endTag, bool filter)
        {
            string regularExpression = startTag + @"[\s\S]+?" + endTag;
            Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match match = regex.Match(sourceStr);
            string tempStr = match.Value;
            if (filter)
                tempStr = FilterString(match.Value, true, false);
            return (tempStr);

        }
        //		public static string GetStringBetweenTags(string sourceStr, string startTag,string endTag)
        //		{
        //			
        //			string regularExpression=startTag+@"[\s\S]+?"+endTag;
        //			Regex regex=new Regex(regularExpression,RegexOptions.IgnoreCase|RegexOptions.Multiline);
        //			Match match=regex.Match(sourceStr);
        //			return(match.Value);
        //		}

        public static string FilterString(string input, bool removeTags, bool removeWhiteSpaces)
        {
            char[] trimchars = new char[] { ' ', '.', ',', '?', '\'', '"', '<', '>', '*', '#' };
            input = input.Replace("\r\n", "");
            input = input.Replace("&nbsp;", "");
            if (removeTags)
            {
                string regularExpression = "<.+?>";
                Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                input = regex.Replace(input, "");
            }
            if (removeWhiteSpaces)
            {
                input = input.Replace("  ", "");
            }
            return (input.Trim(trimchars).Trim());

        }

        public static string EscapeInvalidXmlChars(string text)
        {
            string s = SecurityElement.Escape(text);
            return s;
        }

        public static string ReplaceSpecialCharacter(string _SpChar)
        {
            _SpChar = _SpChar.Replace("&quot;", "&#34;");   //quotation mark
            //_SpChar = _SpChar.Replace("'", "&#39;");        //apostrophe
            _SpChar = _SpChar.Replace("&lt;", "&#60;");     //less-than
            _SpChar = _SpChar.Replace("&gt;", "&#62;");     //greater-than
            _SpChar = _SpChar.Replace("&nbsp;", "&#160;");  //non-breaking space
            _SpChar = _SpChar.Replace("&iexcl;", "i");      //inverted exclamation mark
            _SpChar = _SpChar.Replace("¢", "&#162;");       //cent
            _SpChar = _SpChar.Replace("£", "&#163;");       //pound
            _SpChar = _SpChar.Replace("¤", "&#164;");       //currency
            _SpChar = _SpChar.Replace("¥", "&#165;");       //yen
            _SpChar = _SpChar.Replace("§", "&#167;");       //section
            _SpChar = _SpChar.Replace("¨", "&#168;");       //spacing diaeresis
            _SpChar = _SpChar.Replace("©", "&#169;");       //copyright
            _SpChar = _SpChar.Replace("ª", "&#170;");       //feminine ordinal indicator
            _SpChar = _SpChar.Replace("¬", "&#172;");       //negation
            _SpChar = _SpChar.Replace("®", "&#174;");       //registered trademark
            _SpChar = _SpChar.Replace("¯", "&#175;");
            _SpChar = _SpChar.Replace("°", "&#176;");       //spacing macron
            _SpChar = _SpChar.Replace("²", "&#178;");       //superscript 2
            _SpChar = _SpChar.Replace("³", "&#179;");       //superscript 3
            _SpChar = _SpChar.Replace("´", "&#180;");       //spacing acute
            _SpChar = _SpChar.Replace("µ", "&#181;");       //micro
            _SpChar = _SpChar.Replace("¶", "&#182;");       //paragraph
            _SpChar = _SpChar.Replace("¸", "&#184;");       //spacing cedilla
            _SpChar = _SpChar.Replace("¹", "&#185;");       //superscript 1
            _SpChar = _SpChar.Replace("º", "&#186;");       //masculine ordinal indicator
            _SpChar = _SpChar.Replace("¿", "&#191;");       //inverted question mark
            _SpChar = _SpChar.Replace("À", "&#192;");       //capital a, grave accent
            _SpChar = _SpChar.Replace("Á", "&#193;");       //capital a, acute accent
            _SpChar = _SpChar.Replace("Â", "&#194;");       //capital a, circumflex accent
            _SpChar = _SpChar.Replace("Ã", "&#195;");       //capital a, tilde
            _SpChar = _SpChar.Replace("Ä", "&#196;");       //capital a, umlaut mark
            _SpChar = _SpChar.Replace("Å", "&#197;");       //capital a, ring
            _SpChar = _SpChar.Replace("Æ", "&#198;");       //capital ae
            _SpChar = _SpChar.Replace("Ç", "&#199;");       //capital c, cedilla
            _SpChar = _SpChar.Replace("È", "&#200;");       //capital e, grave accent
            _SpChar = _SpChar.Replace("É", "&#201;");       //capital e, acute accent
            _SpChar = _SpChar.Replace("Ê", "&#202;");       //capital e, circumflex accent
            _SpChar = _SpChar.Replace("Ë", "&#203;");       //capital e, umlaut mark
            _SpChar = _SpChar.Replace("Ì", "&#204;");       //capital i, grave accent
            _SpChar = _SpChar.Replace("Í", "&#205;");       //capital i, acute accent
            _SpChar = _SpChar.Replace("Î", "&#206;");       //capital i, circumflex accent
            _SpChar = _SpChar.Replace("Ï", "&#207;");       //capital i, umlaut mark
            _SpChar = _SpChar.Replace("Ð", "&#208;");       //capital eth, Icelandic
            _SpChar = _SpChar.Replace("Ñ", "&#209;");       //capital n, tilde

            _SpChar = _SpChar.Replace("Ò", "&#210;");       //capital o, grave accent
            _SpChar = _SpChar.Replace("Ó", "&#211;");       //capital o, grave accent
            _SpChar = _SpChar.Replace("Ô", "&#212;");       //capital o, circumflex accent
            _SpChar = _SpChar.Replace("Õ", "&#213;");       //capital o, tilde
            _SpChar = _SpChar.Replace("Ö", "&#214;");       //capital o, umlaut mark  
            _SpChar = _SpChar.Replace("Ø", "&#216;");       //capital o, slash
            _SpChar = _SpChar.Replace("Ù", "&#217;");       //capital u, grave accent
            _SpChar = _SpChar.Replace("Ú", "&#218;");       //capital u, acute accent
            _SpChar = _SpChar.Replace("Û", "&#219;");       //capital u, circumflex accent
            _SpChar = _SpChar.Replace("Ü", "&#220;");       //capital u, umlaut mark
            _SpChar = _SpChar.Replace("Ý", "&#221;");       //capital y, acute accent
            _SpChar = _SpChar.Replace("Þ", "&#222;");       //capital THORN, Icelandic
            _SpChar = _SpChar.Replace("ß", "&#223;");       //small sharp s, German 
            _SpChar = _SpChar.Replace("à", "&#224;");       //small a, grave accent 
            _SpChar = _SpChar.Replace("á", "&#225;");       //small a, acute accent 
            _SpChar = _SpChar.Replace("â", "&#226;");       //small a, circumflex accent 
            _SpChar = _SpChar.Replace("ã", "&#227;");       //small a, tilde   
            _SpChar = _SpChar.Replace("ä", "&#228;");       //small a, umlaut mark 
            _SpChar = _SpChar.Replace("å", "&#229;");       //small a, ring   
            _SpChar = _SpChar.Replace("æ", "&#230;");       //small ae
            _SpChar = _SpChar.Replace("ç", "&#231;");       //small c, cedilla 
            _SpChar = _SpChar.Replace("è", "&#232;");       //small e, grave accent 
            _SpChar = _SpChar.Replace("é", "&#233;");       //small e, acute accent  
            _SpChar = _SpChar.Replace("ê", "&#234;");       //small e, circumflex accent 
            _SpChar = _SpChar.Replace("ë", "&#235;");       //small e, umlaut mark  

            _SpChar = _SpChar.Replace("ì", "&#236;");       //small i, grave accent 
            _SpChar = _SpChar.Replace("í", "&#237;");       //small i, acute accent 
            _SpChar = _SpChar.Replace("î", "&#238;");       //small i, circumflex accent 
            _SpChar = _SpChar.Replace("ï", "&#239;");       //small i, umlaut mark   
            _SpChar = _SpChar.Replace("ð", "&#240;");       //small eth, Icelandic  
            _SpChar = _SpChar.Replace("ñ", "&#241;");       //small n, tilde  
            _SpChar = _SpChar.Replace("ò", "&#242;");       //small o, grave accent   
            _SpChar = _SpChar.Replace("ó", "&#243;");       //small o, acute accent  
            _SpChar = _SpChar.Replace("ô", "&#244;");       //small o, circumflex accent  
            _SpChar = _SpChar.Replace("õ", "&#245;");       //small o, tilde  
            _SpChar = _SpChar.Replace("ö", "&#246;");       //small e, umlaut mark  
            _SpChar = _SpChar.Replace("ø", "&#248;");       //small o, slash 

            _SpChar = _SpChar.Replace("ù", "&#249;");       //small u, grave accent
            _SpChar = _SpChar.Replace("ú", "&#250;");       //small u, acute accent
            _SpChar = _SpChar.Replace("û", "&#251;");       //small u, circumflex accent 
            _SpChar = _SpChar.Replace("ü", "&#252;");       //small u, umlaut mark 
            _SpChar = _SpChar.Replace("ý", "&#253;");       //small y, acute accent 
            _SpChar = _SpChar.Replace("þ", "&#254;");       //small thorn, Icelandic 
            _SpChar = _SpChar.Replace("ÿ", "&#255;");       //small y, umlaut mark 


            _SpChar = _SpChar.Replace("<i>", "");
            _SpChar = _SpChar.Replace("</i>", "");
            _SpChar = _SpChar.Replace("<li>", "");
            _SpChar = _SpChar.Replace("<br>", "");
            _SpChar = _SpChar.Replace("<P>", "");
            _SpChar = _SpChar.Replace("</p>", "");
            _SpChar = _SpChar.Replace("<LI>", "");
            _SpChar = _SpChar.Replace("<BR>", "");
            _SpChar = _SpChar.Replace("<b>", "");
            _SpChar = _SpChar.Replace("</b>", "");
            _SpChar = _SpChar.Replace("\n", "");
            _SpChar = _SpChar.Replace("\t", "");
            _SpChar = _SpChar.Replace("\r", "");
            _SpChar = _SpChar.Replace("<p>", "");
            _SpChar = _SpChar.Replace("</P>", "");
            _SpChar = _SpChar.Replace("<B>", " ");
            _SpChar = _SpChar.Replace("</B>", " ");
            _SpChar = _SpChar.Replace("<BR>", " ");
            _SpChar = _SpChar.Replace("\n", "");
            _SpChar = _SpChar.Replace("\t", "");
            _SpChar = _SpChar.Replace("\r", "");

            return _SpChar;
        }


    }
}
