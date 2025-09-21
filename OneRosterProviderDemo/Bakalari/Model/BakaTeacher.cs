using System;

namespace OneRosterProviderDemo.Bakalari.Model;

public class BakaTeacher
{
    public required string Code { get; set; }
    public required string FamilyName { get; set; }
    public required string GivenName { get; set; }
    public required int Deleted { get; set; }
    public required string Role { get; set; }
}