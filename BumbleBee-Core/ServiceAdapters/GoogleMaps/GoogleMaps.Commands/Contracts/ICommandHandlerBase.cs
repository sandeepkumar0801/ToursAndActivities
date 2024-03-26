using ServiceAdapters.GoogleMaps.GoogleMaps.Entities;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType);

        bool UploadFeed<T>(T inputContext, MethodType methodType);
    }
}