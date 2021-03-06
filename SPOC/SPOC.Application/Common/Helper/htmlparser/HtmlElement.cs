namespace SPOC.Common.Helper.htmlparser
{
	public class HtmlElement
	{
		// Constructors
		public HtmlElement ()
		{
		}
		
		
		// Properties
		public HtmlElementType ElementType
		{
			get
			{
				return this._elementType;
			}
			set
			{
				this._elementType = value;
			}
		}
		
		public string FullText
		{
			get
			{
				return this._fullText;
			}
			set
			{
				this._fullText = value;
			}
		}
		
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}
		
		public string TagAttributes
		{
			get
			{
				return this._tagAttributes;
			}
			set
			{
				this._tagAttributes = value;
			}
		}
		
		
		// Instance Fields
		private  HtmlElementType _elementType;
		private  string _fullText;
		private  string _value;
		private  string _tagAttributes;
	}
}
