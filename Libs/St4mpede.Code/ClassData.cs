using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public class ClassData : DataBaseClass, IToCode
	{
		public bool IsPartial { get; set; }
		public CommentData Comment { get; set; }
		public string Name { get; set; }
		public List<PropertyData> Properties { get; set; }
		public List<MethodData> Methods { get; set; }

		public override IList<string> ToCode()
		{
			var res = new List<string>();
			if (null != Comment)
			{
				res.AddRange(Comment.ToCode());
			}
			res.Add(string.Format("{0} {1}class {2}", 
				"public", 
				IsPartial ? "partial " : string.Empty,
				Name));
			res.Add("{");
			if (null != Properties)
			{
				res.AddRange(Properties.ToCode(_indent.Add(1)));
				res.Add(string.Empty);
			}
			if (null != Methods)
			{
				res.AddRange(Methods.ToCode(_indent.Add( 1)));
			}
			res.Add("}");
			return res;
		}
	}
}
