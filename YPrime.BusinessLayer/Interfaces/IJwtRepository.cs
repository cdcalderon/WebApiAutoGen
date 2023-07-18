namespace YPrime.BusinessLayer.Interfaces
{
    public interface IJwtRepository
    {
        string Encrypt(object modelObject);

        T Decrypt<T>(string encryptedObject) where T : class;
    }
}