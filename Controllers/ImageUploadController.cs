using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.AspNetCore.Http;

namespace ImagesApp.Controllers
{
    [Route("api/[controller]")]
    public class ImageUploadController : Controller
    {

        static CloudBlobClient _blobClient;
        const string _blobContainerName = "faceimages";
        private readonly IConfiguration _configuration;
        static CloudBlobContainer _blobContainer;

        public ImageUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> UploadAsync()
        {
            var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var computerVisionEndpoint = _configuration.GetValue<string>("COMPUTER_VISION_ENDPOINT");
            var computerVisionKey = _configuration.GetValue<string>("COMPUTER_VISION_SUBSCRIPTION_KEY");

            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference(_blobContainerName);
            await _blobContainer.CreateIfNotExistsAsync();

            try
            {
                var request = await HttpContext.Request.ReadFormAsync();

                Console.Write(request.Files[0]);

                if (request.Files == null)
                {
                    return BadRequest("Could not upload image");
                }
                var files = request.Files;
                if (files.Count == 0)
                {
                    return BadRequest("No file was selected");
                }

                //upload:
                var blob = _blobContainer.GetBlockBlobReference(files[0].FileName);
                using (var stream = files[0].OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                var imageUrl = blob.Uri.ToString();

                //analize:              
                ComputerVisionClient client = Authenticate(computerVisionEndpoint, computerVisionKey);

                List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
                 {
                  VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                  VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                  VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                  VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                  VisualFeatureTypes.Objects
                 };

                ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, features);

                if (FindPersons(results.Objects) || FindFaces(results.Faces))
                {
                    return Ok("People found!");
                }

                return Ok("No one found");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public bool FindFaces(IList<FaceDescription> faces)
        {
            if (faces.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool FindPersons(IList<DetectedObject> objects)
        {
            foreach (var obj in objects)
            {
                if (obj.ObjectProperty == "person")
                {
                    return true;
                }
            }
            return false;
        }
    }
}




