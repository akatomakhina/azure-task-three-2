using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using ProductProjectAzure.CustomFilters;
using Swashbuckle.Swagger.Annotations;

namespace ProductProjectAzure.Controllers
{
    [RoutePrefix("api/AzureTask")]
    [ProductsExceptionFilterAttribute]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : ApiController
    {
        private readonly string _queueName;
        private readonly string _blobName;
        private readonly string _storageAccount;
        private readonly string _storageKey;

        public HomeController()
        {
            _queueName = ConfigurationManager.AppSettings["AzureQueueName"];
            _blobName = ConfigurationManager.AppSettings["AzureBlobName"];
            _storageAccount = ConfigurationManager.AppSettings["AzureSA"];
            _storageKey = ConfigurationManager.AppSettings["AzureKey"];
        }

        [HttpPost]
        [Route("")]
        [SwaggerOperation("apiAssemblyInfoControllerPostAssemblypost")]
        [SwaggerOperationFilter(typeof(FileUploadOperationFilter))]
        // POST api/values
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var filesToReadProvider = await Request.Content.ReadAsMultipartAsync();

                var content = filesToReadProvider.Contents.First();

                var fileName = content.Headers.ContentDisposition.FileName;
                var fileMeta = content.Headers.ContentType;

                var stream = filesToReadProvider.Contents[0];
                var fileBytes = await stream.ReadAsByteArrayAsync();

                var azureFileName = await AddFileToBlob(fileBytes);

                await AddFileToQueue(fileName, azureFileName, fileMeta.ToString());

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private async Task<string> AddFileToBlob(byte[] fileBytes)
        {
            var saCreds = new StorageCredentials(_storageAccount, _storageKey);
            var saConfig = new CloudStorageAccount(saCreds, true);
            var theB = saConfig.CreateCloudBlobClient();

            var container = theB.GetContainerReference(_blobName);
            await container.CreateIfNotExistsAsync();


            var fileName = _blobName + ":" + Guid.NewGuid();

            var blob = container.GetAppendBlobReference(fileName);
            await blob.CreateOrReplaceAsync();

            blob.AppendFromByteArray(fileBytes, 0, fileBytes.Length);

            return fileName;
        }

        private async Task AddFileToQueue(string fileName, string azureFileName, string fileMeta)
        {
            var saCreds = new StorageCredentials(_storageAccount, _storageKey);
            var saConfig = new CloudStorageAccount(saCreds, true);
            var azQueueClient = saConfig.CreateCloudQueueClient();

            var theQ = azQueueClient.GetQueueReference(_queueName);

            await theQ.CreateIfNotExistsAsync();

            var data = fileName + "," + azureFileName + "," + fileMeta;

            var message = new CloudQueueMessage(data);

            await theQ.AddMessageAsync(message);
        }
    }
}