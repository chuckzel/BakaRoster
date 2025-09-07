/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneRosterProviderDemo.Serializers;
using OneRosterProviderDemo.Models;

namespace OneRosterProviderDemo.Controllers
{
    [ApiController]
    public class CsvController : Controller
    {
        private ApiContext db;
        public CsvController(ApiContext _db)
        {
            db = _db;
        }
        [HttpGet]
        [Route("csv/bulk")]
        public async Task Bulk()
        {
            Response.ContentType = "binary/octet-stream";
            Response.Headers.ContentDisposition = "attachment; filename=oneroster.zip";
            var serializer = new CsvSerializer(db);
            await serializer.Serialize(Response.BodyWriter.AsStream());
        }
    }
}