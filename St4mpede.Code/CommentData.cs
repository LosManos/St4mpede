﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public class CommentData: DataBaseClass, IToCode
	{
		private const string XmlCommentPrefix = "///";
		private const string XmlCommentSummary = "summary";

		public IList<string> Summary { get; set; }

		public CommentData()		{		}

		public CommentData(string summaryText)
		{
			Summary = new List<string>
			{
				summaryText
			};
		}

		public CommentData(IList<string> summaryTextRows)
		{
			Summary = summaryTextRows;
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