﻿using System;

namespace SPOC.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DecimalPrecisionAttribute : System.Attribute
    {
        public DecimalPrecisionAttribute(byte precision, byte scale)
        {
            Precision = precision;
            Scale = scale;
        }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
    }
}
