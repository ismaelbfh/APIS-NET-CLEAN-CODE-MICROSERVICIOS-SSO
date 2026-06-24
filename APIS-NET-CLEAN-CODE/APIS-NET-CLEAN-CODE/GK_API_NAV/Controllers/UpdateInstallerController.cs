using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GK_API_NAV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateInstallerController : ControllerBase
    {
        // Ruta completa donde se encuentra el MSI en el servidor
        private readonly string _dirPath = @"C:\AppGestionProduccionWPF\publish";
        private readonly string _manifestFile = @"C:\AppGestionProduccionWPF\manifest.json";

        /// <summary>
        /// Endpoint para obtener la versión del MSI.
        /// </summary>
        [HttpGet("version")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public IActionResult GetVersion()
        {
            if (!System.IO.File.Exists(_manifestFile))
            {
                return NotFound(new { error = "Archivo MANIFIESTO no encontrado" });
            }

            // Leer el contenido del archivo de manifiesto
            string lJsonContent = System.IO.File.ReadAllText(_manifestFile);

            try
            {
                // Parsear el JSON y extraer el nodo "version"
                JObject lManifestObj = JObject.Parse(lJsonContent);
                string version = lManifestObj["version"]?.ToString();

                if (string.IsNullOrEmpty(version))
                {
                    return NotFound(new { error = "La versión no se encontró en el manifiesto" });
                }

                // Devolver únicamente el valor de la versión
                return Ok(new { version });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar el manifiesto", details = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para descargar el MSI.
        /// </summary>
        [HttpGet("download")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public IActionResult DownloadUpdate()
        {
            if (!Directory.Exists(_dirPath))
                return NotFound(new { error = "Directorio /publish no encontrado" });

            string tempZip = Path.Combine(Path.GetTempPath(), "UpdateDlls.zip");

            if (System.IO.File.Exists(tempZip))
                System.IO.File.Delete(tempZip);

            System.IO.Compression.ZipFile.CreateFromDirectory(_dirPath, tempZip);

            var stream = new FileStream(tempZip, FileMode.Open, FileAccess.Read);
            return File(stream, "application/zip", "UpdateDlls.zip");
        }
    }
}
