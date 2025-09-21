using System;

namespace OneRosterProviderDemo.Bakalari.Model;

public class BakaClass
{
    public required string Code { get; set; }
    public int StartYear { get; set; }
    public required string TeacherId { get; set; }
    public required string ShortName { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
}
