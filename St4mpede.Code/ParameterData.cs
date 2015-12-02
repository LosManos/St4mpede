﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace St4mpede.Code
{
	public class ParameterData : DataBaseClass, IToCode
	{
		//[XmlIgnore]	//	One cannot serialise a Type.
		//public Type SystemType { get; set; }

		/// <summary>You probably want SystemType as it is type safe.
		/// This property is really only here for making serialisation possible.
		/// </summary>
		public string SystemTypeString
		{
			get;set;
			//get
			//{
			//	return null == SystemType ?
			//		null :
			//		SystemType.FullName;    //	AssemblyQualifiedName?
			//}
			//set
			//{
			//	if (null == value)
			//	{
			//		SystemType = null;
			//	}
			//	else
			//	{
			//		SystemType = Type.GetType(value);
			//	}
			//}
		}

		public string Name { get; set; }

		public string ToDeclaration()
		{
			return string.Format("{0} {1}", SystemTypeString, Name);
		}

		public override IList<string> ToCode()
		{
			var ret = new List<string>
			{
				"new code"
				//string.Format("{0} {1}", SystemTypeString, Name)
			};
            return ret;
		}
	}
}
