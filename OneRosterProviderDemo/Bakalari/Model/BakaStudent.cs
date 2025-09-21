using System;

namespace OneRosterProviderDemo.Bakalari.Model;

public class BakaStudent
{
    public required string Code { get; set; }
    public required string FamilyName { get; set; }
    public required string GivenName { get; set; }
    public int ClassRegNumber { get; set; }
    public required string ClassShortName { get; set; }
}
