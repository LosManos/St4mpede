using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public class ClassData : DataBaseClass, IToCode
	{
		public CommentData Comment { get; set; }
		public string Name { get; set; }
		public IList<PropertyData> Properties { get; set; }
		public IList<MethodData> Methods { get; set; }

		public override IList<string> ToCode()
		{
			var res = new List<string>();
			res.AddRange(Comment.ToCode());
			//TODO:	Set scope visibility.
			res.Add(string.Format("public class {0}", Name));
			res.Add("{");
			res.AddRange(Properties.ToCode(_indentLevel+1));
			res.Add(string.Empty);
			res.AddRange(Methods.ToCode(_indentLevel + 1));
			res.Add("}");
			return res;
		}
	}
}
