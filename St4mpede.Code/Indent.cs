using System.Linq;

namespace St4mpede.Code
{
	public class Indent
	{
		public int Level { get; set; }
		public string WhiteSpace { get; set; }

		public Indent()
		{
		}

		public Indent(int level, string whiteSpace)
		{
			this.Level = level;
			this.WhiteSpace = whiteSpace;
		}

		public Indent Add( int indent)
		{
			return new Indent(Level + 1, WhiteSpace);
		}

		public string IndentString()
		{
			return string.Concat(Enumerable.Repeat(WhiteSpace, Level));
		}

		public string IndentString(int indent)
		{
			return string.Concat(Enumerable.Repeat(WhiteSpace, indent));
		}
	}
}
