using System;
using System.Collections;
using System.Xml.Serialization;

namespace SPOC.Common
{
    public class ReturnValue
    {
        [NonSerialized]
        private Hashtable _mapData;

        public ReturnValue(bool hasError, string message)
        {
            HasError = hasError;
            ErrorCode = null;
            Message = message;

        }

        public ReturnValue()
        {
            HasError = true;
            Message = "";
        }

        [XmlIgnore]
        public Hashtable MapData
        {
            get
            {
                return _mapData;
            }
            set
            {
                _mapData = value;
            }
        }

        public object ReturnObject { get; set; }
        public bool HasError { get; set; }

        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public string GetStringValue(string key, string defaultValue)
        {
            var obj = GetValue(key);
            return obj == null ? defaultValue : obj.ToString();
        }

        public string GetStringValue(string key)
        {
            return GetStringValue(key, "");
        }
        public object GetValue(string key)
        {
            if (_mapData == null || !_mapData.ContainsKey(key))
                return null;
            else
                return _mapData[key];
        }
        public void PutValue(string key, object valueObj)
        {
            if (_mapData == null)
                _mapData = new Hashtable(5);
            _mapData[key] = valueObj;
        }
    }
}