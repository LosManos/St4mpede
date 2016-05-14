using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public class MethodData : DataBaseClass, IToCode
	{
		public CommentData Comment { get; set; }
		public string Name { get; set; }

		public string ReturnTypeString { get; set; }
		public bool Override { get; set; }

		public bool IsConstructor { get; set; }
		public Common.VisibilityScope Scope { get; set; }

		//TODO:Make an ordered list.
		public List<ParameterData> Parameters { get; set; }

		public BodyData Body { get; set; }

		public override IList<string> ToCode()
		{
			var ret = new List<string>();
			if (null != Comment)
			{
				ret.AddRange(Comment.ToCode());
			}

			if (IsConstructor)
			{
				ret.Add(string.Format(
					"{0} {1}({2})",
					Scope.ToCode(),
					Name,
					null == Parameters
						?
						string.Empty
						:
						" " + Parameters.ToMethodParameterDeclarationString() + " "));
			}
			else
			{
				ret.Add(string.Format(
					"{0} {1}{2} {3}({4})",
					Scope.ToCode(),
					Override ? "override " : string.Empty,
					ReturnTypeString,
					Name,
					null == Parameters
						?
						string.Empty
						:
						" " + Parameters.ToMethodParameterDeclarationString() + " "));
			}
			ret.Add("{");
			if (IsConstructor && Parameters != null && Body == null)
			{
				ret.AddRange(Parameters.ToPropertyAssignmentList(_indent.Add(1)));
			}
			if( Body != null)
			{
				ret.AddRange(Body.ToCode(_indent.Add(1)));
			}
			ret.Add("}");
			return ret;
		}
	}
}