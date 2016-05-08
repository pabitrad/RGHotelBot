using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Utility
{
    public static class ReplaceChar
    {
        public static string ReplaceCharacter(string _SpChar)
        {
            _SpChar = _SpChar.Replace("&quot;", "&#34;");   //quotation mark
            _SpChar = _SpChar.Replace("&rsquo;", "&#146;");   //Single-quotation mark
            _SpChar = _SpChar.Replace("&pound;", "Pound ");
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
