using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace St4mpede.Code
{
	public class PropertyData: DataBaseClass, IToCode
	{
		public CommentData Comment { get; set; }
		public string Name { get; set; }

		[XmlIgnore]	//	One cannot serialise a Type.
		public Type SystemType { get; set; }

		/// <summary>You probably want SystemType as it is type safe.
		/// This property is really only here for making serialisation possible.
		/// </summary>
		public string SystemTypeString
		{
			get
			{
				return null == SystemType ?
					null :
					SystemType.FullName;    //	AssemblyQualifiedName?
			}
			set
			{
				if( null == value)
				{
					SystemType = null;
				}
				else
				{
					SystemType = Type.GetType(value);
				}
			}
		}

		public Common.VisibilityScope Scope { get; set; }

		public PropertyData()		{		}

		public override IList<string> ToCode()
		{
			var ret = new List<string>();

			if( null != Comment)
			{
				ret.AddRange(Comment.ToCode());
			}

			ret.Add(string.Format("{0} {1} {2}{{ get; set; }}",
				Scope.ToCode(), SystemType, Name));

			return ret;
		}

	}
}