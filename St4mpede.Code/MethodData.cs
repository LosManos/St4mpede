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

		//TODO:Make an ordered list.
		public List<ParameterData> Parameters { get; set; }

		public override IList<string> ToCode()
		{
			if( false==IsConstructor)
			{
				throw new NotImplementedException("Not IsConstructor is not implemented.");
			}
			var ret = new List<string>();
			if (null != Comment)
			{
				ret.AddRange(Comment.ToCode());
			}
			//TODO:Change scope.
			ret.Add(string.Format(
				"{0} {1}({2})",
				Scope.ToCode(),
				Name, 
				null == Parameters
					?
					string.Empty
					:
					" " + Parameters.ToCode() + " " ));
			ret.Add("{");
			ret.Add("}");
			return ret;
		}
	}
}