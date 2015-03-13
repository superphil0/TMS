using System;
using System.Collections.Generic;
namespace TMS
{
	public class Country
	{
		public int CountryId
		{
			get;
			set;
		}
		public string CountryName
		{
			get;
			set;
		}
		public List<Competition> Competitions
		{
			get;
			set;
		}
	}
}
