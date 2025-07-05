/// <summary>
/// Commonly‑used filesystem constants for all services.
/// </summary>
public static class LogConstants
{
    public const string Dir = "log";      // no trailing slash → Path.Combine friendly
    public const string Ext = ".txt";
    public const int Default_Retention = 60;
    public const string Output_Template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
}