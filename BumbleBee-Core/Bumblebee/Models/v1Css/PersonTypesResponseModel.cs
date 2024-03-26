using System;
using System.Collections.Generic;

namespace WebAPI.Models.v1Css
{
    #region Old Model

    public class Persontype
    {
        public string name { get; set; }
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public int personTypeId { get; set; }
    }

    #endregion Old Model

    #region New Model

    public class PersonTypesResponseModelV1
    {
        public Bookablepersontypesindateranges bookablePersonTypesInDateRanges { get; set; }
    }

    public class Bookablepersontypesindateranges
    {
        public List<Availablepersontype> AvailablePersonTypes { get; set; }
        public List<Availabledate> AvailableDates { get; set; }
    }

    public class Availablepersontype
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public List<Persontype> personTypes { get; set; }
    }

    public class Availabledate
    {
        public DateTime? AvailableOn { get; set; }
        public int Capacity { get; set; }
    }

    /// <summary>
    /// Available Person Types as per date range.
    /// </summary>
    public class BookablePersonTypesAvailablePersons
    {
        /// <summary>
        /// List of Bookable person for the given product option id and date range.
        /// </summary>
        public List<Availablepersontype> AvailablePersonTypes { get; set; }
    }

    public class BookablepersontypesAvailableDates
    {
        public List<Availabledate> AvailableDates { get; set; }
    }

    #endregion New Model
}