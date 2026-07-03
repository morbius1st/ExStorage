
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;


// user name: jeffs
// created:   10/20/2025 6:43:55 PM

namespace ExStorSys
{
	public class PropChgEvtArgs
	{
		// public PropertyOwner PropOwner { get; set; }
		public PropertyId PropId { get; set; }
		public DynaValue Value { get; set; }

		[DebuggerStepThrough]
		public PropChgEvtArgs(PropertyId id, DynaValue value)
		{
			// PropOwner = po;
			PropId = id;
			Value = value;
		}

		[DebuggerStepThrough]
		public PropChgEvtArgs(PropertyId id, bool value)
		{
			// PropOwner = po;
			PropId = id;
			Value = new DynaValue(value);
		}

		[DebuggerStepThrough]
		public PropChgEvtArgs(PropertyId id, int value)
		{
			// PropOwner = po;
			PropId = id;
			Value = new DynaValue(value);
		}

		[DebuggerStepThrough]
		public PropChgEvtArgs(PropertyId id, string value)
		{
			// PropOwner = po;
			PropId = id;
			Value = new DynaValue(value);
		}

		// public PropChgEvtArgs(PropertyOwner po, PropertyId id, Enum value)
		[DebuggerStepThrough]
		public PropChgEvtArgs(PropertyId id, Enum value)
		{
			// PropOwner = po;
			PropId = id;
			Value = new DynaValue(value);
		}


	}

}
