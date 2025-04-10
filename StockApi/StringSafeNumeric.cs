using Drake.Extensions;
using System;
using System.Linq;

namespace StockApi
{
    public class StringSafeNumeric<T> where T : IConvertible
    {
        public bool IsNumeric = false;
        public string StringIfNotNumeric = null; // string to return if string fails numeric.

        public StringSafeNumeric(string stringIfNotNumeric)
        {
            _stringValue = stringIfNotNumeric;
            StringIfNotNumeric = stringIfNotNumeric;
        }

        private string _stringValue;
        public string StringValue
        {
            get => _stringValue;
            set
            {
                _stringValue = value;
                string temp = new string(value.Where(c => char.IsDigit(c) || "-.".Contains(c)).ToArray());

                // set the generic numeric type, converting the string to the numeric type
                switch (Type.GetTypeCode(typeof(T)))
                {
                    // Built-in Byte type.
                    case TypeCode.Int32:
                        if (!temp._IsInt())
                        {
                            _numericValue = (T)(object)(int)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;
                            _numericValue = (T)(object)Convert.ToInt32(temp);
                        }
                        break;
                    case TypeCode.Decimal:
                        if (!temp._IsDecimal())
                        {
                            _numericValue = (T)(object)(decimal)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;
                            _numericValue = (T)(object)Convert.ToDecimal(temp);
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
            set
            {
                _numericValue = value;
                _stringValue = value.ToString();
            }
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
