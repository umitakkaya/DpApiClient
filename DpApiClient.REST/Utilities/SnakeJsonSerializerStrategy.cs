using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DpApiClient.REST.Utilities
{
    public class SnakeJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            //PascalCase to snake_case
            return string.Concat(clrPropertyName.Select((x, i) => char.IsUpper(x) ? (i > 0 ? "_" : string.Empty) + char.ToLower(x, new CultureInfo("en-US", false)).ToString() : x.ToString()));
        }
    }
}
