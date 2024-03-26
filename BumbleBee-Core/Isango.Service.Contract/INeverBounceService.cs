namespace Isango.Service.Contract
{
    public interface INeverBounceService
    {
        bool IsEmailVerified(string emailId, string token);
    }
}