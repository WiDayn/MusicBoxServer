using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace MusicBoxServer.Controllers
{
    [Route("api/images")]
    public class ImageController : Controller
    {
        private readonly string _externalPath;

        public ImageController(IConfiguration configuration)
        {
            _externalPath = configuration["ExternalPath"];
        }

        [HttpGet("{*filePath}")]
        public IActionResult GetImage(string filePath)
        {
            var fullPath = Path.Combine(_externalPath, filePath);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            using (var srcImage = System.Drawing.Image.FromFile(fullPath))
            {
                var newImage = new Bitmap(225, 225);
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(srcImage, 0, 0, 225, 225);
                }

                var stream = new MemoryStream();
                newImage.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "image/png");
            }
        }
    }
}
