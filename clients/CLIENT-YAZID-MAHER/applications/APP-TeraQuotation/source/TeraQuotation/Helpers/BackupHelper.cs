using System.IO;

namespace TeraQuotation.Helpers;

public static class BackupHelper
{
    private static string GetDbPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "TeraQuotation", "TeraQuotation.db");
    }

    public static async Task<string> BackupAsync()
    {
        var dbPath = GetDbPath();
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var backupFile = Path.Combine(desktop, $"TeraQuotation_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        // Ensure the source database directory exists
        var dbDir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(dbDir!);
        }

        // Copy the database file (if source exists)
        await Task.Run(() =>
        {
            if (File.Exists(dbPath))
            {
                File.Copy(dbPath, backupFile, true);
            }
            else
            {
                // If the database doesn't exist at the app data path,
                // try copying from the app's base directory
                var localDb = "TeraQuotation.db";
                if (File.Exists(localDb))
                {
                    File.Copy(localDb, backupFile, true);
                }
                else
                {
                    // Create an empty marker file
                    File.WriteAllText(backupFile, "-- Empty backup - no database found");
                }
            }
        });

        return backupFile;
    }

    public static async Task<string?> RestoreAsync()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Database files (*.db)|*.db|All files (*.*)|*.*",
            Title = "اختر ملف النسخة الاحتياطية"
        };

        if (dlg.ShowDialog() != true) return null;

        var dbPath = GetDbPath();
        var dbDir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(dbDir!);
        }

        await Task.Run(() => File.Copy(dlg.FileName, dbPath, true));

        // Also copy to local directory for the app to pick up
        var localDb = "TeraQuotation.db";
        await Task.Run(() => File.Copy(dlg.FileName, localDb, true));

        return dlg.FileName;
    }
}
