using System;

namespace SPOC.Common.Helper.htmlparser
{
	internal class HtmlParserException : Exception
	{
		// Constructors
		public HtmlParserException (string sMessage, string sContent, int nIndex, int nLineCounter, int nColumnCounter) : base(sMessage)
		{
			this._sHtmlContent = sContent;
			this._nCharIndex = nIndex;
			this._nLineCounter = nLineCounter;
			this._nColumnCounter = nColumnCounter;
			string str0 = sMessage;
			str0 = str0 + "\n (HtmlParserError) ";
			string text2 = str0;
			str0 = string.Concat(new string[]{text2, "\nEncounter error at line ", nLineCounter.ToString(), ", column ", nColumnCounter.ToString()});
		}
		
		
		// Instance Fields
		private  string _sHtmlContent;
		private  int _nCharIndex;
		private  int _nLineCounter;
		private  int _nColumnCounter;
	}
}
