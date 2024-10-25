using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace QrBackend.Models
{
    public class CloudinaryService
    {

        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            Account account = new Account("dq5kj6rdg", "598269295844842", "9YVjF5k0gMeA0W_lx_FKDydQwfQ");
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Ensure the file is not null or empty
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.");
            }

            // Get the file extension
            var extension = Path.GetExtension(file.FileName);

            // Create a new unique filename using ticks and the original file extension
            var newFileName = $"{DateTime.Now.Ticks}{extension}";

            using (var stream = new MemoryStream())
            {
                // Copy the uploaded file to the MemoryStream
                await file.CopyToAsync(stream);

                // Reset the position of the stream to the beginning
                stream.Position = 0;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(newFileName, stream), // Use the MemoryStream directly
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                // Check if _cloudinary is initialized
                if (_cloudinary == null)
                {
                    throw new InvalidOperationException("Cloudinary client is not initialized.");
                }

                // Upload the image to Cloudinary
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Check if uploadResult is null
                if (uploadResult == null)
                {
                    throw new Exception("Upload failed, result is null.");
                }

                // Return the URL of the uploaded image
                return uploadResult.Url.ToString();
            }
        }





    }
}
