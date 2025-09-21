namespace OneRosterProviderDemo.Bakalari;

public sealed class MapSettings
{
    public string TeacherUsernameTemplate { get; init; } = "{0}";
    public string StudentUsernameTemplate { get; init; } = "{1}.{0}";
}