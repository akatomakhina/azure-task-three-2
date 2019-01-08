using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductProject.DataAccess.Common.Models;

namespace ProductProject.DataAccess.Common.Repositories
{
    public interface IProductRerository
    {
        Task<DbProduct> AddProductAsync(DbProduct product);

        Task<DbProduct> DeleteProductAsync(DbProduct product);

        Task<DbProduct> UpdateProductAsync(DbProduct product);

        Task<DbProduct> GetProductByIdAsync(int id);

        Task<IEnumerable<DbProduct>> GetAllProductsAsync();
    }
}
