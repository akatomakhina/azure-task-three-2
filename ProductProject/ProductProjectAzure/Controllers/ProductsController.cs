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
        [SwaggerResponse(HttpStatusCode.Created, Description = "Get all channels.", Type = typeof(int))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetChannels()
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid state");
                return BadRequest(ModelState);
            }
            var products = await _productService.GetProductsAsync().ConfigureAwait(false);
            try
            {
                // var productsModel = products.Select(c => Mapper.Map<Product>(products));
                _logger.Info("There are all products");

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return BadRequest("Debug");
        }


        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Deletes an existing channel.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Channel not found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Server error.")]
        public async Task<IHttpActionResult> DeleteChannel([FromUri] int id)
        {
            if (id < 0)
            {
                _logger.Error("Invalid id");
            }
            await _productService.RemoveProductAsync(id).ConfigureAwait(false);
            _logger.Info("Product deleted");
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}