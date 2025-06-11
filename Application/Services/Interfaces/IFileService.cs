using CloudinaryDotNet.Actions;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Sube una foto a Cloudinary.
        /// </summary>
        /// <param name="file">Foto a subir</param>
        /// <returns>Resultado de cloudinary</returns>
        Task<ImageUploadResult> AddFile(IFormFile file);

        /// <summary>
        /// Elimina una foto de Cloudinary.
        /// </summary>
        /// <param name="publicId">Id publica y unica de la imagen a eliminar.</param>
        /// <returns>Resultado de clodinary.</returns>
        Task<DeletionResult> DeleteFile(string publicId);
    }
}