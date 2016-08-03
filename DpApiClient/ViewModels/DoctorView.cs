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

        private List<Facility> _facilities = new List<Facility>();
        public List<Facility> Facilities
        {
            get { return _facilities; }
            set { _facilities = value; }
        }

        private List<Specialization> _specializations = new List<Specialization>();
        public List<Specialization> Specializations
        {
            get { return _specializations; }
            set { _specializations = value; }
        }

        private int[] _selectedFacilityIds = new int[0];
        public int[] SelectedFacilityIds
        {
            get { return _selectedFacilityIds; }
            set { _selectedFacilityIds = value; }
        }

        private int[] _selectedSpecializationIds = new int[0];
        public int[] SelectedSpecializationIds
        {
            get { return _selectedSpecializationIds; }
            set { _selectedSpecializationIds = value; }
        }
    }
}