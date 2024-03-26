using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.Search
{
    #region SearchActivities Request classes https://api.test.hotelbeds.com/activity-api/3.0/activities

    public class SearchRq
    {
        public SearchRq()
        {
            Filters = new List<Filter>();
            From = string.Empty;
            To = string.Empty;
            Language = string.Empty;
            Pagination = new Pagination();
            Order = string.Empty;
        }

        public SearchRq(List<Filter> lstFilters, string fromDate, string toDate, string language, Pagination paging, string order)
        {
            Filters = lstFilters;
            From = fromDate;
            To = toDate;
            Language = language;
            Pagination = paging;
            Order = order;
        }

        public List<Filter> Filters { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string Language { get; set; }

        public Pagination Pagination { get; set; }

        public string Order { get; set; }
    }

    public class Filter
    {
        public Filter()
        {
            SearchFilterItems = new List<ISearchfilteritem>();
        }

        public Filter(List<ISearchfilteritem> lstSearchFilterItems)
        {
            SearchFilterItems = lstSearchFilterItems;
        }

        public List<ISearchfilteritem> SearchFilterItems { get; set; }
    }

    public interface ISearchfilteritem
    {
        string Type { get; set; }
    }

    public class Searchfilteritem : ISearchfilteritem
    {
        public Searchfilteritem()
        {
            Type = string.Empty;
            Value = string.Empty;
        }

        public Searchfilteritem(string type, string value)
        {
            Type = type;
            Value = value;
        }

        #region ISearchfilteritem Members

        public string Type { get; set; }

        #endregion ISearchfilteritem Members

        public string Value { get; set; }
    }

    public class SearchfilteritemGps : ISearchfilteritem
    {
        public SearchfilteritemGps()
        {
            Type = string.Empty;
            Longitude = string.Empty;
            Latitude = string.Empty;
        }

        public SearchfilteritemGps(string type, string longitude, string latitude)
        {
            Type = type;
            Longitude = longitude;
            Latitude = latitude;
        }

        #region ISearchfilteritem Members

        public string Type { get; set; }

        #endregion ISearchfilteritem Members

        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    public class Pagination
    {
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
    }

    #endregion SearchActivities Request classes https://api.test.hotelbeds.com/activity-api/3.0/activities
}