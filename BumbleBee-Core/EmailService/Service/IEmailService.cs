using EmailSuitConsole.Models;
using System.Threading.Tasks;


namespace EmailSuitConsole.Service
{
    public interface IEmailService
    {
        //Task SendPrioEmail(UserEmailOptions userEmailOptions);
        Task SendPrioStatusEmail(UserEmailOptions userEmailOptions);
        Task SendtiqetStatusEmail(UserEmailOptions userEmailOptions);
        Task SendTiqetChange(UserEmailOptions userEmailOptions);
        Task SendVariantChange(UserEmailOptions userEmailOptions);


        Task SendTourCMSChange(UserEmailOptions userEmailOptions);
        Task SendRaynaChange(UserEmailOptions userEmailOptions);
        Task SendGlobalTixV3Change(UserEmailOptions userEmailOptions);

        Task SendTourCMSLabelChange(UserEmailOptions userEmailOptions);

        Task SendRaynaOptionsChange(UserEmailOptions userEmailOptions);
        Task SendGlobalTixV3OptionsChange(UserEmailOptions userEmailOptions);


    }
}