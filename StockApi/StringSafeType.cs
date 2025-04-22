using Drake.Extensions;
using System;
using System.Linq;

namespace StockApi
{
    public class StringSafeType<T> where T : struct
    {
        public bool IsNumeric = false;
        public bool IsDateTime = false;
        public string StringIfNotNumeric = null; // string to return if string fails numeric.

        public StringSafeType(string stringIfNotNumeric)
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
                string temp = value;

                // set the generic numeric type, converting the string to the numeric type
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int32:
                        IsDateTime = false;

                        temp = new string(value.Where(c => char.IsDigit(c) || "-.".Contains(c)).ToArray());
                        if (!temp._IsInt())
                        {
                            IsNumeric = false;
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
                        IsDateTime = false;

                        temp = new string(value.Where(c => char.IsDigit(c) || "-.".Contains(c)).ToArray());
                        if (!temp._IsDecimal())
                        {
                            IsNumeric = false;
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
                    case TypeCode.DateTime:
                        IsNumeric = false;
                        _numericValue = default(T);

                        DateTime val;
                        if (DateTime.TryParse(temp, out val) == false)
                        {
                            IsDateTime = false;
                            _datetimeValue = null;
                            _stringValue = temp;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsDateTime = true;
                            _datetimeValue = (T)(object)val;
                            _stringValue = _datetimeValue.ToString();
                        }
                        break;
                }
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

        private T? _datetimeValue;
        public T? DateTimeValue
        {
            get { return _datetimeValue; }
            set
            {
                _datetimeValue = value;
                _stringValue = value.ToString();
            }
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
