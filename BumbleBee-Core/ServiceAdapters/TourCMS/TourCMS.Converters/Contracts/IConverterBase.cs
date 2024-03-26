using ServiceAdapters.TourCMS.TourCMS.Entities;
namespace ServiceAdapters.TourCMS.TourCMS.Converters.Contracts
{
    public interface IConverterBase
    {
        //Date and Deals
        object Convert(object objectResult, object criteria);
        //CheckAvailability
        object Convert(object apiResponse, MethodType methodType, object criteria = null);
        //Booking Use 
        object Convert<T>(string response, T request);
    }
}