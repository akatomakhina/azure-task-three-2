using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using ProductProject.Logic.Common.Models;
using ProductProject.Logic.Common.Services;
using ProductProjectAzure.CustomFilters;
using Swashbuckle.Swagger.Annotations;

namespace ProductProjectAzure.Controllers
{
    [RoutePrefix("api/AzureTask/products")]
    [ProductsExceptionFilterAttribute]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly ILog _logger;

        public ProductsController(IProductService productService, ILog logger)
        {
            if (productService.Equals(null))
            {
                throw new ArgumentNullException(nameof(productService));
            }

            if (logger.Equals(null))
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _productService = productService;
            _logger = logger;
        }


        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Get all products.", Type = typeof(int))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetProducts()
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid state");
                return BadRequest(ModelState);
            }
            //var products = await _productService.GetProductsAsync().ConfigureAwait(false);
            try
            {
                // var productsModel = products.Select(c => Mapper.Map<Product>(products));
                //var products = await _productService.GetProductsAsync().ConfigureAwait(false);


                var products = await _productService.GetProductsAsync().ConfigureAwait(false);
                //var channelsModel = channels.Select(c => Mapper.Map<ChannelModel>(channels));
                var model = new List<Product>();
                foreach (var pr in products)
                {
                    model.Add(new Product
                    {
                        ProductID = pr.ProductID,
                        Name = pr.Name,
                        ProductNumber = pr.ProductNumber,
                        MakeFlag = pr.MakeFlag,
                        FinishedGoodsFlag = pr.FinishedGoodsFlag,
                        Color = pr.Color,
                        SafetyStockLevel = pr.SafetyStockLevel,
                        ReorderPoint = pr.ReorderPoint,
                        StandardCost = pr.StandardCost,
                        ListPrice = pr.ListPrice,
                        Size = pr.Size,
                        SizeUnitMeasureCode = pr.SizeUnitMeasureCode,
                        WeightUnitMeasureCode = pr.WeightUnitMeasureCode,
                        Weight = pr.Weight,
                        DaysToManufacture = pr.DaysToManufacture,
                        ProductLine = pr.ProductLine,
                        Class = pr.Class,
                        Style = pr.Style,
                        ProductSubcategoryID = pr.ProductSubcategoryID,
                        ProductModelID = pr.ProductModelID,
                        SellStartDate = pr.SellStartDate,
                        SellEndDate = pr.SellEndDate,
                        DiscontinuedDate = pr.DiscontinuedDate,
                        rowguid = pr.rowguid,
                        ModifiedDate = pr.ModifiedDate
                    });
                }
                _logger.Info("There are all products");
                return Ok(model);


                //_logger.Info("There are all products");

                //return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return BadRequest("Debug");
        }


        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Deletes an existing product.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Product not found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Server error.")]
        public async Task<IHttpActionResult> DeleteProductFromHistory([FromUri] int id)
        {
            if (id < 0)
            {
                _logger.Error("Invalid id");
            }

            try
            {
                await _productService.RemoveProductAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            _logger.Info("Product deleted");
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}