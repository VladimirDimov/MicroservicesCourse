namespace Catalog.API.Repositories
{
    using Catalog.API.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProductRepository
    {
        Task CreateProduct(Product product);
        Task<Product> GetProduct(string id);
        Task<bool> DeleteProduct(string id);
        Task<bool> UpdateProduct(Product product);
        Task<IEnumerable<Product>> GetProductByCategory(string categoryName);
        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProducts();
    }
}
