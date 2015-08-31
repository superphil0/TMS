using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMS
{
  public static class StringExtensions
  {
    public static string NormalizeString(this string str)
    {
      return str.Replace("\\", "").Replace("/", "").Replace("?", "").Replace("*", "").Replace("[", "").Replace("]", "").Replace(":", "").Replace("<", "").Replace(">", "").Replace("\"", "").Replace("|","");      
    }

    public static string Left(this string value, int maxLength)
    {
      if (string.IsNullOrEmpty(value)) return value;
      maxLength = Math.Abs(maxLength);

      return (value.Length <= maxLength
             ? value
             : value.Substring(0, maxLength)
             );
    }
  }

  public class Helper
  {

    public static string GetCompetitionArchiveFileName(Competition c)
    {
      return "cache\\arhiva\\" + c.CompetitionCountry.NormalizeString() + "\\" + c.CompetitionName.NormalizeString() + "\\" + "_" + c.CompetitionName.NormalizeString().ToUpper() + ".xlsx";
    }


    public static string GetTeamArchiveFileName(Competition c,Team t)
    {
      return "cache\\arhiva\\" + c.CompetitionCountry.NormalizeString()+ "\\" + c.CompetitionName.NormalizeString()+ "\\" + t.TeamName.NormalizeString()+ ".xlsx";
    }

    public static bool IsFileLocked(FileInfo file)
    {
      FileStream stream = null;

      try
      {
        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }

      //file is not locked
      return false;
    }


    public static int CommonCharacters(string s1, string s2)
    {
      s1 = s1.ToUpper();
      s2 = s2.ToUpper();
      bool[] matchedFlag = new bool[s2.Length];

      for (int i1 = 0; i1 < s1.Length; i1++)
      {
        for (int i2 = 0; i2 < s2.Length; i2++)
        {
          if (!matchedFlag[i2] && s1.ToCharArray()[i1] == s2.ToCharArray()[i2])
          {
            matchedFlag[i2] = true;
            break;
          }
        }
      }

      return matchedFlag.Count(u => u);
    }

    public static string PositionShortName(string position)
    {
      switch (position)
      {
        case "Keeper":
          return "GK";
        case "Centre Back":
          return "CB";
        case "Left-Back":
          return "LB";
        case "Right-Back":
          return "RB";
        case "Defensive Midfield":
          return "DM";
        case "Central Midfield":
          return "CM";
        case "Left Wing":
          return "LW";
        case "Right Wing":
          return "RW";
        case "Right Midfield":
          return "RM";
        case "Left Midfield":
          return "LM";
        case "Attacking Midfield":
          return "AM";
        case "Centre Forward":
          return "CF";
        case "Second Striker":
          return "SS";
        case "Secondary Striker":
          return "SS";
      }
      return "";
    }


    public static string RemoveDiacriticsCustom(string text)
    {
      text = Regex.Replace(text, "ö", "oe");
      text = Regex.Replace(text, "ü", "ue");
      return text;
    }

    public static string RemoveDiacritics(string text, bool includeCustom)
    {
      if (includeCustom == true)
        text = RemoveDiacriticsCustom(text);
      Encoding srcEncoding = Encoding.UTF8;
      Encoding destEncoding = Encoding.GetEncoding(1252); // Latin alphabet

      text = destEncoding.GetString(Encoding.Convert(srcEncoding, destEncoding, srcEncoding.GetBytes(text)));

      string normalizedString = text.Normalize(NormalizationForm.FormD);
      StringBuilder result = new StringBuilder();

      for (int i = 0; i < normalizedString.Length; i++)
      {
        if (!CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]).Equals(UnicodeCategory.NonSpacingMark))
        {
          result.Append(normalizedString[i]);
        }
      }

      return result.ToString();
    }
  }
}
