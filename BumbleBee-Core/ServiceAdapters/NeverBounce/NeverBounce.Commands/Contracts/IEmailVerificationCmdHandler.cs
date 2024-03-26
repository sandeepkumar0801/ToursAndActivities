namespace ServiceAdapters.NeverBounce.NeverBounce.Commands.Contracts
{
    public interface IEmailVerificationCmdHandler
    {
        object GetResults(string authString, string email);
    }
}