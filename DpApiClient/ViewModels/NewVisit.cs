using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DpApiClient.ViewModels
{
    public class NewVisit
    {
        public Visit visit { get; set; }
        public VisitPatient Patient { get; set; }
        public List<Facility> Facilities { get; set; }
        public List<ForeignDoctorService> ForeignDoctorServices { get; set; }
    }
}