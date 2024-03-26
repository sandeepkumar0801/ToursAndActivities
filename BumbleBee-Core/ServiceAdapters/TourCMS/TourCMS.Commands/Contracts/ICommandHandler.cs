using ServiceAdapters.TourCMS.TourCMS.Entities;

namespace ServiceAdapters.TourCMS.TourCMS.Commands.Contracts
{
    public interface ICommandHandler
    {
        object Execute<T>(T inputContext, MethodType methodType, string token, out string apiRequest, out string apiResponse);

        object Execute<T>(T inputContext, string token, MethodType methodType, out string apiRequest, out string apiResponse);
    }
}