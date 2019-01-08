using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductProject.DataAccess.Common.Models;
using ProductProject.DataAccess.Common.Repositories;
using ProductProject.DataAccess.Context;

namespace ProductProject.DataAccess.Repositories
{
    public class ProductDbRepository : IProductRerository
    {
        private ProductProjectContext _context;
        private bool isDisposed = false;
        private TransactionHistoryRepository _transactionHistoryRepository;

        public ProductDbRepository(ProductProjectContext context, TransactionHistoryRepository transactionHistoryRepository)
        {
            _context = context;
            _transactionHistoryRepository = transactionHistoryRepository;
        }

        public async Task<DbProduct> AddProductAsync(DbProduct product)
        {
            if (ReferenceEquals(product, null))
            {
                throw new ArgumentNullException(nameof(product));
            }

            var addedItem = _context.Products.Add(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return addedItem;
        }

        public async Task<IEnumerable<DbProduct>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        public async Task<DbProduct> GetProductByIdAsync(int id)
        {
            return await _context.Products.SingleOrDefaultAsync(p => p.ProductID == id).ConfigureAwait(false);
        }

        public async Task<DbProduct> DeleteProductAsync(DbProduct product)
        {
            if (ReferenceEquals(product, null))
            {
                throw new ArgumentNullException(nameof(product));
            }
            var transactionsForProduct =
                await _transactionHistoryRepository.GetTransactionsByProductId(product.ProductID).ConfigureAwait(false);

            foreach (var transaction in transactionsForProduct)
            {
                _context.TransactionHistories.Remove(transaction);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var deletedItem = _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return deletedItem;
        }

        public async Task<DbProduct> UpdateProductAsync(DbProduct product)
        {
            if (ReferenceEquals(product, null))
            {
                throw new ArgumentNullException(nameof(product));
            }

            var updateItem = await GetProductByIdAsync(product.ProductID).ConfigureAwait(false);

            _context.Entry(updateItem).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return updateItem;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // If need dispose something
                }

                isDisposed = true;
                _context?.Dispose();
                _context = null;
            }
        }

        ~ProductDbRepository()
        {
            Dispose(false);
        }
    }
}
