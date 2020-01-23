using System.Collections.Generic;
using MayakElectronics.Models;

namespace MayakElectronics
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
        Product DeleteProduct(int productId);
    }
}