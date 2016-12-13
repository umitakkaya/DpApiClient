using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.Client
{
    public sealed class TimeZones
    {
        private readonly string _name;
        private readonly string _value;

        private static readonly Dictionary<string, TimeZones> instance = new Dictionary<string, TimeZones>();


        public static readonly TimeZones TR = new TimeZones("tr-TR", "Turkey Standard Time");
        public static readonly TimeZones PL = new TimeZones("pl-PL", "Central European Standard Time");

        public TimeZones(string value, string name)
        {
            _name = name;
            _value = value;
            instance[value] = this;
        }

        public override string ToString()
        {
            return _name;
        }

        public static explicit operator TimeZones(string str)
        {
            TimeZones result;
            if (instance.TryGetValue(str, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public static explicit operator TimeZones(Locale locale)
        {
            TimeZones result;
            string localeName = locale.GetValue();
            if (instance.TryGetValue(localeName, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

    }
}
