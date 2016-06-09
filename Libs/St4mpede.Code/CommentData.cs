using System;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public class CommentData: DataBaseClass, IToCode
	{
		private const string XmlCommentPrefix = "///";
		private const string XmlCommentSummary = "summary";

		/// <summary>This is the collection of comment rows.
		/// It is a class, and not an interface, because otherwise serialising won't work.
		/// </summary>
		public List<string> Summary { get; set; }

		public CommentData()		{		}

		public CommentData(string summaryText)
		{
			Summary = new List<string>
			{
				summaryText
			};
		}

		public CommentData(IEnumerable<string> summaryTextRows)
		{
			Summary = summaryTextRows.ToList();
		}

		public override IList<string> ToCode()
		{
			if( null == Summary || false == Summary.Any())
			{
				return new List<string>();
			}
			var rows = new List<string>();
			rows.Add(string.Format("{0} {1} {2}",
				XmlCommentPrefix,
				XmlElementStart(XmlCommentSummary),
				Summary[0]));
			for( var i= 1; i<Summary.Count; ++i )
			{
				rows.Add(string.Format("{0} {1}",
					XmlCommentPrefix,
					Summary[i]));
			}
			rows.Add(string.Format("{0} {1}",
				XmlCommentPrefix,
				XmlElementEnd(XmlCommentSummary)));
			return rows;
		}

		private string XmlElementStart(string elementName)
		{
			return string.Format("<{0}>", elementName);
		}

		private string XmlElementEnd( string elementName)
		{
			return string.Format("</{0}>", elementName);
		}
	}
}