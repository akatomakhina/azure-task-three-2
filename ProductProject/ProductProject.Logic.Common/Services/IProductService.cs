using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductProject.Logic.Common.Models;

namespace ProductProject.Logic.Common.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();

        Task RemoveProductAsync(int productId);
    }
}
