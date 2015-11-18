using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public class MethodData : DataBaseClass, IToCode
	{
		public CommentData Comment { get; set; }
		public string Name { get; set; }
		public bool IsConstructor { get; set; }
		public Common.VisibilityScope Scope { get; set; }

		public override IList<string> ToCode()
		{
			if( false==IsConstructor)
			{
				throw new NotImplementedException();
			}
			var ret = new List<string>();
			if (null != Comment)
			{
				ret.AddRange(Comment.ToCode());
			}
			//TODO:Change scope.
			ret.Add(string.Format(
				"{0} {1}()",
				Scope.ToCode(),
				Name));
			ret.Add("{");
			ret.Add("}");
			return ret;
		}
	}
}