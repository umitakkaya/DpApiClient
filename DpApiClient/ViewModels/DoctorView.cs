using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DpApiClient.ViewModels
{
    public partial class DoctorView
    {
        public Doctor Doctor { get; set; }
        public List<Facility> Facilities { get; set; } = new List<Facility>();
        public List<Specialization> Specializations { get; set; } = new List<Specialization>();
        public int[] SelectedFacilityIds { get; set; } = new int[0];
        public int[] SelectedSpecializationIds { get; set; } = new int[0];
    }
}