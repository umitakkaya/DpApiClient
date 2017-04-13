using DpApiClient.Data;
using DpApiClient.REST.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpApiClient.REST.DTO;
using System.Data.Entity;
using DpApiClient.Data.Repositories;
using DpApiClient.Core;

namespace DpApiClient.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportManager.Import();
        }
    }
}
