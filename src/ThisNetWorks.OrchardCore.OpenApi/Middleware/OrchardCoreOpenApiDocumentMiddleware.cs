﻿using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.AspNetCore.Middlewares;
using System;
using System.Threading.Tasks;

namespace ThisNetWorks.OrchardCore.OpenApi.Middleware
{
    public class OrchardCoreOpenApiDocumentMiddleware : OpenApiDocumentMiddleware
    {
        public OrchardCoreOpenApiDocumentMiddleware(RequestDelegate nextDelegate, IServiceProvider serviceProvider, string documentName, string path, OpenApiDocumentMiddlewareSettings settings) :
            base(nextDelegate, serviceProvider, documentName, path, settings)
        {
        }

        protected override async Task<string> GetDocumentAsync(HttpContext context)
        {
            // TODO cache this, and clear cache on events.
            // We will need a cache provider, that can handle clearing cache from Scoped events.
            //try
            //{
            var doc = await GenerateDocumentAsync(context);
            
            var json = doc.ToJson(SchemaType.OpenApi3, Newtonsoft.Json.Formatting.None);
            return json;
            //}
        }
    }
}
