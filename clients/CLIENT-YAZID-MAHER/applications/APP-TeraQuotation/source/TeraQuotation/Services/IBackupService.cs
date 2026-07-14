namespace TeraQuotation.Services;

public interface IBackupService
{
    Task BackupAsync();
    Task<List<string>> GetBackupFilesAsync();
    Task<bool> RestoreAsync(string backupFilePath);
}
