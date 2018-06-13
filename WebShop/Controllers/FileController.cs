using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using PriceImporter.Attributes;
using WebShop.Services.ProductImporter;

namespace WebShop.Controllers
{
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IProductsImporter _importer;

        public FileController(ILogger<FileController> logger, IProductsImporter importer)
        {
            _logger = logger;
            _importer = importer;
        }

        [HttpPost]
        [DisableFormBinding]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            if (!IsMultipartContentType(Request.ContentType))
            {
                var msg = $"Wrong content type '{Request.ContentType}' can only work with multipart/ content types";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary)
                .ToString();
            
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var results = new List<ImportResult>();

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if(hasContentDispositionHeader)
                {
                    if (IsFileContentDisposition(contentDisposition))
                    {
                        results.Add(await _importer.ImportAsync(section.Body));
                        _logger.LogInformation("File imported");

                        // NOTE: another solution would be to save file on a disk and add it
                        // to the processing queue (which should as well be developed)
                        // In this case logic will be a bit more complicated but we could 
                        // handle bigger files better and distribute load to the different machines
                        // for this solution I chose a bit more simple solution with direct data processing
                        // Example:
                        //var targetFilePath = Path.GetTempFileName();
                        //using (var targetStream = System.IO.File.Create(targetFilePath))
                        //{
                        //    await section.Body.CopyToAsync(targetStream);
                        //    result.Add(importScheduler.ScheduleJob(targetFilePath))
                        //}
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return Json(results);
        }

        /// <summary>
        /// Determines whether content disposition of the section has type form-data and filename set 
        /// </summary>
        /// <param name="contentDisposition">The content disposition.</param>
        /// <returns>
        ///   <c>true</c> if it has file content disposition (Content-Disposition: form-data; name="file1"; filename="assignment data export (en-header).csv"); otherwise, <c>false</c>.
        /// </returns>
        private static bool IsFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!string.IsNullOrEmpty(contentDisposition.FileName.ToString())
                       || !string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString()));
        }

        private bool IsMultipartContentType(string contentType) =>
            !string.IsNullOrEmpty(contentType) &&
            contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}