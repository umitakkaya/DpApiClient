using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.Client
{
    public sealed class Locale
    {
        private readonly string _name;
        private readonly string _value;

        private static readonly Dictionary<string, Locale> instance = new Dictionary<string, Locale>();


        public static readonly Locale TR = new Locale("tr-TR", "https://www.doktortakvimi.com/");
        public static readonly Locale PL = new Locale("pl-PL", "https://www.znanylekarz.pl/");

        public Locale(string value, string name)
        {
            _name = name;
            _value = value;
            instance[value] = this;
        }

        public string GetValue()
        {
            return _value;
        }

        public override string ToString()
        {
            return _name;
        }

        public static explicit operator Locale(string str)
        {
            Locale result;
            if (instance.TryGetValue(str, out result))
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
