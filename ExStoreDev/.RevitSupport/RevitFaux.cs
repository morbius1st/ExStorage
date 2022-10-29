#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   10/16/2022 11:14:25 AM

namespace Autodesk.Revit.DB
{
	public class ForgeTypeId
	{
		public string TypeId { get; }

		public ForgeTypeId(string id)
		{
			TypeId = id;
		}
	}

	public class SpecTypeId
	{
		public static ForgeTypeId Custom  { get; } = new ForgeTypeId("autodesk.spec:custom-1.0.0 ");
		public static ForgeTypeId Length  { get; } = new ForgeTypeId("Length");
	}

	public class UnitTypeId
	{
		public static ForgeTypeId Inches { get; }               = new ForgeTypeId("Inches ");
		public static ForgeTypeId FractionalInches { get; }     = new ForgeTypeId("FractionalInches ");
		public static ForgeTypeId FeetFractionalInches { get; } = new ForgeTypeId("FeetFractionalInches");
		public static ForgeTypeId Feet { get; }                 = new ForgeTypeId("Feet ");
		public static ForgeTypeId UsSurveyFeet { get; }         = new ForgeTypeId("UsSurveyFeet ");
		public static ForgeTypeId Millimeters { get; }          = new ForgeTypeId("Millimeters ");
		public static ForgeTypeId Centimeters { get; }          = new ForgeTypeId("Centimeters ");
		public static ForgeTypeId Decimeters { get; }           = new ForgeTypeId("Decimeters ");
		public static ForgeTypeId Meters { get; }               = new ForgeTypeId("Meters ");
		public static ForgeTypeId MetersCentimeters { get; }    = new ForgeTypeId("MetersCentimeters ");
	}

	public class SymbolTypeId
	{

		public static ForgeTypeId InchDoubleQuote { get; } = new ForgeTypeId("InchDoubleQuote");
		public static ForgeTypeId In { get; }              = new ForgeTypeId("In ");
		public static ForgeTypeId FootSingleQuote { get; } = new ForgeTypeId("FootSingleQuote");
		public static ForgeTypeId Ft { get; }              = new ForgeTypeId("Ft ");
		public static ForgeTypeId Usft { get; }            = new ForgeTypeId("Usft ");
		public static ForgeTypeId Meter { get; }           = new ForgeTypeId("Meter ");
		public static ForgeTypeId Mm { get; }              = new ForgeTypeId("Mm ");
		public static ForgeTypeId Cm { get; }              = new ForgeTypeId("Cm ");
		public static ForgeTypeId Dm { get; }              = new ForgeTypeId("Dm ");
	}

	public class Document
	{
		// bogue DB file title (file name)
		public string Title => "Sample Revit Title.rvt";

		// bogus DB file path
		public string PathName => @"c:\autodesk\temp";


		// example of empty values for testing
		// bogue DB file title (file name)
		public string TitleMt => "";

		// bogus DB file path
		public string PathNameMt => "";

	}



}
