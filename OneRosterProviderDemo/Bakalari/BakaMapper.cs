using System.Globalization;
using System.Text;
using OneRosterProviderDemo.Bakalari.Model;
using OneRosterProviderDemo.Models;
using OneRosterProviderDemo.Vocabulary;

namespace OneRosterProviderDemo.Bakalari;

public class BakaMapper
{
    private readonly MapSettings _settings;

    public BakaMapper(MapSettings settings)
    {
        _settings = settings;
    }

    private IEnumerable<AcademicSession> Sessions(BakaRoster roster) => [];

    private IEnumerable<Org> Orgs(BakaRoster roster) => [
        .. roster.Organizations.Select(o => new Org
        {
            Id = o.Code.Trim(),
            Type = OrgType.school,
            Name = o.Name.Trim(),
        })
    ];

    private IEnumerable<LineItemCategory> LineItemCategories(BakaRoster roster) => [];

    private IEnumerable<Course> Courses(BakaRoster roster) => [];

    private IEnumerable<IMSClass> Classes(BakaRoster roster) => [];

    private IEnumerable<LineItem> LineItems(BakaRoster roster) => [];

    private IEnumerable<User> Users(BakaRoster roster) => [
        .. roster.Teachers.Select(t => new User()
        {
            Id = t.Code.Trim(),
            Username = MapUsername(_settings.TeacherUsernameTemplate, t.FamilyName, t.GivenName),
            FamilyName = ToTitleCase(t.FamilyName.Trim()),
            GivenName = ToTitleCase(t.GivenName.Trim()),
            Role = RoleType.teacher,
        }),
        .. roster.Students.Select(s => new User()
        {
            Id = s.Code.Trim(),
            Username = MapUsername(_settings.StudentUsernameTemplate, s.FamilyName, s.GivenName),
            FamilyName = ToTitleCase(s.FamilyName.Trim()),
            GivenName = ToTitleCase(s.GivenName.Trim()),
            Role = RoleType.student,
        })
    ];

    private IEnumerable<Enrollment> Enrollments(BakaRoster roster) => [];

    private IEnumerable<Result> Results(BakaRoster roster) => [];

    private IEnumerable<IMSClassAcademicSession> ClassSessions(BakaRoster roster) => [];

    private IEnumerable<UserAgent> UserAgents(BakaRoster roster) => [];

    private IEnumerable<UserOrg> UserOrgs(BakaRoster roster) => [];

    private IEnumerable<Demographic> Demographics(BakaRoster roster) => [];

    private IEnumerable<Resource> Resources(BakaRoster roster) => [];

    private static string MapOrgFieldsToType(string fields)
    {
        char kkovLetter = fields.FirstOrDefault(char.IsLetter);
        // based on the KKOV field code, which should be the first thing in the string
        // the only letter in KKOV should map to the category of education recieved after completion (číselník BADV)
        return kkovLetter switch
        {
            'C' => "school", // základní škola
            'D' or 'E' or 'H' or 'J' or 'K' or 'L' or 'M' => "school", // střední škola
            'N' or 'P' => "college", // vyšší odborná škola
            'R' or 'T' or 'V' => "university", // vysoká škola
            _ => "school",
        };
    }

    private static string MapUsername(string template, string familyName, string givenName) =>
    RemoveDiacritics(string.Format(
        template,
        familyName.Trim().Split(' ')[0],
        givenName.Trim().Split(' ')[0]
    ).ToLowerInvariant());

    private static string ToTitleCase(string input) =>
        Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());

    private static string RemoveDiacritics(string input) =>
        string.Concat(input.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            .Normalize(NormalizationForm.FormC);
}

