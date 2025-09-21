using System;

namespace OneRosterProviderDemo.Bakalari.Model;

public record BakaRoster
{
    public required IEnumerable<BakaOrg> Organizations { get; init; }
    public required IEnumerable<BakaStudent> Students { get; init; }
    public required IEnumerable<BakaClass> Classes { get; init; }
    public required IEnumerable<BakaTeacher> Teachers { get; init; }
}
