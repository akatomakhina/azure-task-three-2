﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace ProductProjectAzure.CustomFilters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId == "apiAssemblyInfoControllerPostAssemblypost")
            {
                operation.consumes.Add("multipart/form-data");
                operation.parameters = new[]
                {
                    new Parameter
                    {
                        name = "file",
                        @in = "formData",
                        required = true,
                        type = "file"
                    }
                };
            }
        }
    }
}