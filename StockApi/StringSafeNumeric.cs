using Drake.Extensions;
using System;

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
                _stringValue = value;

                // set the generic numeric type, converting the string to the numeric type
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
                            _numericValue = (T)(object)Convert.ToInt32(value);
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
                            _numericValue = (T)(object)Convert.ToDecimal(value);
                        }
                        break;
                }
                if (!IsNumeric)
                    _stringValue = StringIfNotNumeric;
            }
        }

        private T _numericValue;
        public T NumericValue
        {
            get { return _numericValue; }
            set { _numericValue = value; }
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
