namespace TeraQuotation.Services;

public class BackupService : IBackupService
{
    public Task BackupAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetBackupFilesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> RestoreAsync(string backupFilePath)
    {
        throw new NotImplementedException();
    }
}
