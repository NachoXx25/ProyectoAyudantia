using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net.Utilities;
using Proyecto_web_api.Application.Services.Interfaces;
using Serilog;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<FileService> _logger;
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
            try{
                var account = new Account(
                EnvReader.GetStringValue("CloudName"),
                EnvReader.GetStringValue("ApiKey"),
                EnvReader.GetStringValue("ApiSecret")
                );
                _cloudinary = new Cloudinary(account);
                _logger.LogInformation("Cloudinary inicializado correctamente.");
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar Cloudinary: {Message}", ex.Message);
                throw new Exception("Error al inicializar Cloudinary", ex);
            }
        }

        /// <summary>
        /// Sube una foto a Cloudinary.
        /// </summary>
        /// <param name="file">Foto a subir</param>
        /// <returns>Resultado de cloudinary</returns>
        public async Task<ImageUploadResult> AddFile(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg" && fileExtension != ".gif" && fileExtension != ".webp" && fileExtension != ".avif" && fileExtension != ".svg" && fileExtension != ".heic" && fileExtension != ".mp4" && fileExtension != ".mov" && fileExtension != ".avi" && fileExtension != ".wmv" && fileExtension != ".flv" && fileExtension != ".mkv")
            {
                throw new Exception("El archivo no es una imagen o video valido (.jpg, .png, .jpeg, .gif, .webp, .avif, .svg, .heic, .mp4, .mov, .avi, .wmv, .flv o .mkv)");
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                throw new Exception($"El archivo {file.Name} es demasiado grande (10MB maximo)");
            }

            var UploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Transformation = new Transformation().Width(1000).Crop("scale").Chain().Quality("auto").Chain().FetchFormat("auto"), Folder = "Ayudantia"
                };
                Log.Information($"Subiendo archivo a Cloudinary: {file.FileName}");
                UploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return UploadResult;
        }

        /// <summary>
        /// Elimina una foto de Cloudinary.
        /// </summary>
        /// <param name="publicId">Id publica y unica de la imagen a eliminar.</param>
        /// <returns>Resultado de clodinary.</returns>
        public async Task<DeletionResult> DeleteFile(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            Log.Information($"Eliminando archivo de Cloudinary: {publicId}");
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }

        /// <summary>
        /// Sube un archivo a Cloudinary.
        /// </summary>
        /// <param name="File">El archivo a subir.</param>
        /// <returns>El resultado de la subida.</returns>
        public Task<UploadResult> UploadFileAsync(IFormFile File)
        {
            throw new NotImplementedException();
        }
    }


}