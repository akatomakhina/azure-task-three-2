using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductProject.DataAccess.Common.Repositories;
using ProductProject.Logic.Common.Exceptions;
using ProductProject.Logic.Common.Models;
using ProductProject.Logic.Common.Services;

namespace ProductProject.Logic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRerository _productRepository;

        public ProductService(IProductRerository productRepository)
        {
            if (productRepository.Equals(null))
            {
                throw new ArgumentNullException(nameof(productRepository));
            }

            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var dbProducts = await _productRepository.GetAllProductsAsync().ConfigureAwait(false);
            var dbDB = dbProducts.Select(p => Mapper.Map<Product>(p));
            return dbDB;
        }

        public async Task RemoveProductAsync(int productId)
        {
            var existedProduct = await _productRepository.GetProductByIdAsync(productId).ConfigureAwait(false);

            if (ReferenceEquals(existedProduct, null))
            {
                throw new RequestedResourceNotFoundException($"Channel with id {productId}");
            }

            var deletedItem = await _productRepository.DeleteProductAsync(existedProduct).ConfigureAwait(false);
        }
    }
}
