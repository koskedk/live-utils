namespace LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Data;

public class DatabaseSettings
{
    public const string SettingsKey = "DatabaseConfiguration";
    public string ProviderType { get; set; }
    public bool ApplySeed { get; set; }
    public bool ApplyDatabaseMigrations { get; set; }

    public DatabaseSettings()
    {
    }

    public DatabaseSettings(string providerType, bool applySeed, bool applyDatabaseMigrations)
    {
        ProviderType = providerType;
        ApplySeed = applySeed;
        ApplyDatabaseMigrations = applyDatabaseMigrations;
    }
}