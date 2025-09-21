using System;
using Dapper;
using Microsoft.Data.SqlClient;
using OneRosterProviderDemo.Bakalari.Model;

namespace OneRosterProviderDemo.Bakalari;

public class BakaDb
{
    private readonly SqlConnection _connection;
    public BakaDb(SqlConnection _connection)
    {
        this._connection = _connection;
    }

    public async Task<BakaRoster> GetRoster()
    {
        using var result = await _connection.QueryMultipleAsync(Queries.Students + Queries.Classes + Queries.Organizations + Queries.Teachers);
        var students = await result.ReadAsync<BakaStudent>();
        var classes = await result.ReadAsync<BakaClass>();
        var organizations = await result.ReadAsync<BakaOrg>();
        var teachers = await result.ReadAsync<BakaTeacher>();
        return new BakaRoster
        {
            Organizations = organizations,
            Students = students,
            Classes = classes,
            Teachers = teachers
        };
    }

    static class Queries
    {
        public const string Students =
            $"""
            SELECT
                [INTERN_KOD] AS {nameof(BakaStudent.Code)},
                [PRIJMENI] AS {nameof(BakaStudent.FamilyName)},
                [JMENO] AS {nameof(BakaStudent.GivenName)},
                [C_TR_VYK] AS {nameof(BakaStudent.ClassRegNumber)},
                [TRIDA] AS {nameof(BakaStudent.ClassShortName)}
            FROM dbo.zaci;
            """;
        public const string Classes =
            $"""
            SELECT
                [KOD_TRID] AS {nameof(BakaClass.Code)},
                [NASTUP] AS {nameof(BakaClass.StartYear)},
                [TRIDNICTVI] AS {nameof(BakaClass.TeacherId)},
                [ZKRATKA] AS {nameof(BakaClass.ShortName)},
                [NAZEV] AS {nameof(BakaClass.Name)},
                [ROCNIK] AS {nameof(BakaClass.Year)}
            FROM dbo.tridy;
            """;
        public const string Organizations =
            $"""
            SELECT
                [KOD_ORG] AS {nameof(BakaOrg.Code)},
                [NAZEV] AS {nameof(BakaOrg.Name)},
                [OBORY] AS {nameof(BakaOrg.Fields)}
            FROM dbo.organiz;
            """;

        public const string Teachers =
            $"""
            SELECT
                [INTERN_KOD] AS {nameof(BakaTeacher.Code)},
                [PRIJMENI] AS {nameof(BakaTeacher.FamilyName)},
                [JMENO] AS {nameof(BakaTeacher.GivenName)},
                [FUNKCE] AS {nameof(BakaTeacher.Role)},
                [DELETED_RC] AS {nameof(BakaTeacher.Deleted)}
            FROM dbo.ucitele;
            """;
    }
}
