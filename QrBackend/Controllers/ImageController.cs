using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QrBackend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace QrBackend.Controllers
{
    public class ImageController : Controller
    {

        //
        //    
        //    string connectionString = configuration.GetConnectionString("DefaultConnection");
        //con = new SqlConnection(connectionString);

        private readonly DataAccessLayer cs;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection con;
        private readonly CloudinaryService _cloudinaryService;

        public ImageController(DataAccessLayer _cs, IConfiguration configuration)
        {
            this.cs = _cs;
            _configuration = configuration;
            _cloudinaryService = new CloudinaryService();

        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("No image uploaded.");

            var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);

            // Save imageUrl to the database here
            SaveImageUrlToDatabase(imageUrl);

            return Ok(new { url = imageUrl });
        }

        private void SaveImageUrlToDatabase(string imageUrl)
        {
                          try
            {
                using (SqlCommand cmd = new SqlCommand("Imageurl"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Imageurl", imageUrl);

                    var dt = cs.Run_SPQuery(cmd);

                }

            }
            catch (Exception ex) { }


        }
    }
}
