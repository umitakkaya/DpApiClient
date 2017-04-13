using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    /// <summary>
    /// Detailed error messages and error message will be parsed into this
    /// {
    ///     "errors": {
    ///         "slots[1].end": [
    ///             "End date should be less than or equal to 11.11.2015, 11:45."
    ///         ]
    ///     },
    ///     "message": "Invalid arguments"
    /// }
    /// </summary>
    public class DPResponse
    {
        public dynamic Errors { get; set; }

        public string Message { get; set; }
    }
}
