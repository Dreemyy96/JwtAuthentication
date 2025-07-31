namespace JWTAuthentication.Models
{
    //не используется
    public class ProductRepository : IRepository<Product>
    {
        private Dictionary<int, Product> _products = new Dictionary<int, Product>()
        {
            [1] = new Product { Name = "Banana", Price = 120M },
            [2] = new Product { Name = "Apple", Price = 100M },
            [3] = new Product { Name = "Orange", Price = 80M }
        };

        public Product this[int id] => _products[id];

        public void AddProducts(params Product[] product)
        {
            int id = _products.Last().Key + 1;
            for(int i = 0; i < product.Length; i++)
            {
                _products[id] = product[i];
                id++;
            }
        }
    }
}
