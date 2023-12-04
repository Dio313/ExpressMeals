namespace ExpressMeals.Domains.helpers;

public interface IFileService
{
    Task<string> SaveFile(byte[] content, string extension, string containerName);
    Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute);
    Task DeleteFile(string fileRoute, string containerName);
}