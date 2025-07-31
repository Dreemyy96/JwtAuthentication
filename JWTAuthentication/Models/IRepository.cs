namespace JWTAuthentication.Models
{
    //не используется
    public interface IRepository<T> where T : class
    {
        T this[int id] { get; }
        void AddProducts(params T[] product);
    }
}
