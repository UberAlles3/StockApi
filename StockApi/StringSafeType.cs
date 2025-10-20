using Drake.Extensions;
using System;
using System.Linq;

namespace StockApi
{
    public class StringSafeType<T> where T : struct
    {
        public bool IsNumeric = false;
        public bool IsDateTime = false;
        public bool HasAbbreviation = false;
        public string Abbreviation = "";
        public string StringIfNotNumeric = null; // string to return if string fails numeric.
        public string ExpandedString = "";
        public string Format = "";

        public StringSafeType(string stringIfNotNumeric, string format = "")
        {
            _stringValue = stringIfNotNumeric;
            StringIfNotNumeric = stringIfNotNumeric;
            Format = format;
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

                        temp = new string(value.Where(c => char.IsDigit(c) || "-.BbTtMmKk".Contains(c)).ToArray());
                        if (temp.Length > 0 && "BbTtMmKk".Contains(temp.Last().ToString())) // Number is abbreviated. i.e. 3.71B or 4k
                        {
                            HasAbbreviation = true;
                            Abbreviation = temp.Last().ToString();
                            ExpandedString = ConvertNumericSuffix(temp).ToString();
                        }

                        if (!HasAbbreviation && !temp._IsInt())
                        {
                            IsNumeric = false;
                            _numericValue = (T)(object)(int)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;
                            if (HasAbbreviation)
                                _numericValue = (T)(object)Convert.ToInt32(ExpandedString);
                            else
                                _numericValue = (T)(object)Convert.ToInt32(temp);
                        }
                        break;
                    case TypeCode.Decimal:
                        IsDateTime = false;

                        temp = new string(value.Where(c => char.IsDigit(c) || "-.BbTtMmKk".Contains(c)).ToArray());
                        if (temp.Length > 0 && "BbTtMmKk".Contains(temp.Last().ToString())) // Number is abbreviated. i.e. 3.71B or 4k
                        {
                            HasAbbreviation = true;
                            Abbreviation = temp.Last().ToString();
                            ExpandedString = ConvertNumericSuffix(temp).ToString();
                        }

                        if (!HasAbbreviation && !temp._IsDecimal())
                        {
                            IsNumeric = false;
                            _numericValue = (T)(object)(decimal)0;
                            if (StringIfNotNumeric != null)
                                _stringValue = StringIfNotNumeric;
                        }
                        else
                        {
                            IsNumeric = true;
                            if(HasAbbreviation)
                                _numericValue = (T)(object)Convert.ToDecimal(ExpandedString);
                            else
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
                if (Format != "")
                {
                    switch (Type.GetTypeCode(typeof(T)))
                    {
                        case TypeCode.Int32:
                            _stringValue = Convert.ToInt32(value).ToString(Format);
                            break;
                        case TypeCode.Decimal:
                            _stringValue = Convert.ToDecimal(value).ToString(Format);
                            break;
                    }
                }
                else
                    _stringValue = value.ToString();
                
                IsNumeric = true;
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
                IsDateTime = true;
            }
        }

        public decimal ConvertNumericSuffix(string value)
        {
            string temp = "";
            decimal number = 0;

            if (value.IndexOf("B") > 0 || value.IndexOf("T") > 0)
            {
                temp = value.Replace("B", "").Replace("T", "");
                number = Convert.ToDecimal(temp) * 1000000000;
            }
            else if (value.IndexOf("M") > 0)
            {
                temp = value.Replace("M", "");
                number = Convert.ToDecimal(temp) * 1000000;
            }
            else if (value.IndexOf("k") > 0)
            {
                temp = value.Replace("k", "");
                number = Convert.ToDecimal(temp) * 1000;
            }
            else
                number = Convert.ToDecimal(value);
 
            return number;
        }

        public string AbbreviateNumeric(decimal value)
        {
            decimal tempVal = value;

            if (value > 1000000000) // billion
            {
                tempVal = tempVal / 1000000000;
                return tempVal.ToString("0.00") + "B";
            }
            if (value > 1000000) // million
            {
                tempVal = tempVal / 1000000;
                return tempVal.ToString("0.00") + "M";
            }
            if (value > 1000) // thousand
            {
                tempVal = tempVal / 1000;
                return tempVal.ToString("0.00") + "K";
            }

            return value.ToString("0.00");
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
