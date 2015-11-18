using System.Collections.Generic;

namespace St4mpede.Code
{
	public class PropertyData: DataBaseClass, IToCode
	{
		public CommentData Comment { get; set; }
		public string Name { get; set; }
		public System.Type SystemType { get; set; }
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