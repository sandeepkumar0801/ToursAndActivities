namespace ServiceAdapters.NeverBounce
{
    public interface INeverBounceAdapter
    {
        bool IsEmailNbVerified(string email, string token);
    }
}