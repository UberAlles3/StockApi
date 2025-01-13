﻿using Drake.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockApi
{
    public class StringSafeNumeric<T> where T : IConvertible
    {
        public bool IsNumeric = false;
        public string StringIfNotNumeric = null; // string to return if string fails numeric.

        public StringSafeNumeric(string stringIfNotNumeric)
        {
            StringIfNotNumeric = stringIfNotNumeric;
        }

        private string _stringValue;
        public string StringValue
        {
            get => _stringValue;
            set
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    // Built-in Byte type.
                    case TypeCode.Int32:
                        //_stringValue = value.Replace("%", "");
                        if (!value.IsInt())
                        {
                            _numericValue = (T)(object)(int)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;  
                            _numericValue = (T)(object)(int)Convert.ToInt32(value);
                        }
                        break;
                    case TypeCode.Decimal:
                        //_stringValue = value.Replace("%", "");
                        if (!value.IsDecimal())
                        {
                            _numericValue = (T)(object)(decimal)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;
                            _numericValue = (T)(object)(decimal)Convert.ToDecimal(value);
                        }
                        break;
                }
                if(!IsNumeric)
                    _stringValue = StringIfNotNumeric;
            }
        }

        private T _numericValue;
        public T NumericValue
        {
            get { return _numericValue; }
            set { _numericValue = value; }
        }
    }
}
