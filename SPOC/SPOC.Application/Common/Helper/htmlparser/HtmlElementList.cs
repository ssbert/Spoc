using System.Collections;

namespace SPOC.Common.Helper.htmlparser
{
	public class HtmlElementList : ArrayList
	{
		// Constructors
		public HtmlElementList ()
		{
		}
		
		
		// Methods
		public void AddHtmlElement (HtmlElementType elementType, string sFullText, string sValue, string sTagAttributes)
		{
			SPOC.Common.Helper.htmlparser.HtmlElement value = new SPOC.Common.Helper.htmlparser.HtmlElement();
			value.ElementType = elementType;
			value.FullText = sFullText;
			value.Value = sValue;
			value.TagAttributes = sTagAttributes;
			this.Add(value);
		}
		
		public void AddHtmlElement (HtmlElementType elementType, string sFullText, string sValue)
		{
			this.AddHtmlElement(elementType, sFullText, sValue, "");
		}
		
	}
}
