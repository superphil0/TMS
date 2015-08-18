using System;
using System.Collections.Generic;
namespace TMS
{
  public class Competition
  {

    public Competition()
    {
      Teams = new List<Team>();
    }

    public List<Team> Teams
    {
      get;
      set;
    }
    public string CompetitionId
    {
      get;
      set;
    }
    public string CompetitionType
    {
      get;
      set;
    }
    public string CompetitionCountry
    {
      get;
      set;
    }
    public int CompetitionCountryId
    {
      get;
      set;
    }
    public string CompetitionName
    {
      get;
      set;
    }
    public string Url
    {
      get;
      set;
    }
    public string ListName
    {
      get
      {
        return this.CompetitionCountry + " - " + this.CompetitionName;
      }
    }

  
  }
}
