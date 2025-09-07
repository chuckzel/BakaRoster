/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OneRosterProviderDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneRosterProviderDemo.Serializers
{
    public class CsvSerializer
    {
        private ApiContext db;
        private readonly CsvHelper.Configuration.CsvConfiguration csvConfig;
        public CsvSerializer(ApiContext db)
        {
            this.db = db;
            csvConfig = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task Serialize(Stream outStream)
        {
            using var archive = new ZipArchive(outStream, ZipArchiveMode.Create);
            await WriteFileEntry(archive, "manifest.csv", Manifest());
            await WriteFileEntry(archive, "academicSessions.csv", AcademicSessions());
            await WriteFileEntry(archive, "categories.csv", Categories());
            await WriteFileEntry(archive, "classes.csv", IMSClasses());
            await WriteFileEntry(archive, "courses.csv", Courses());
            await WriteFileEntry(archive, "enrollments.csv", Enrollments());
            await WriteFileEntry(archive, "lineItems.csv", LineItems());
            await WriteFileEntry(archive, "orgs.csv", Orgs());
            await WriteFileEntry(archive, "results.csv", Results());
            await WriteFileEntry(archive, "users.csv", Users());
        }

        private async Task WriteFileEntry(ZipArchive archive, string entryName, string entryValue)
        {
            var entry = archive.CreateEntry(entryName);
            await using var entryStream = entry.Open();
            await using var streamWriter = new StreamWriter(entryStream);
            await streamWriter.WriteAsync(entryValue);
        }

        // https://www.imsglobal.org/oneroster-v11-final-csv-tables#_Toc480293253
        private string[][] manifestValues = new string[][]
        {
            new string[]{ "propertyName", "value" },
            new string[]{ "mainfest.version", "1.0" },
            new string[]{ "oneroster.version", "1.1" },
            new string[]{ "file.academicSessions", "bulk" },
            new string[]{ "file.categories", "bulk" },
            new string[]{ "file.classes", "bulk" },
            new string[]{ "file.classResources", "absent" },
            new string[]{ "file.courses", "bulk" },
            new string[]{ "file.courseResources", "absent" },
            new string[]{ "file.demographics", "absent" },
            new string[]{ "file.enrollments", "bulk" },
            new string[]{ "file.lineItems", "bulk" },
            new string[]{ "file.orgs", "bulk" },
            new string[]{ "file.resources", "absent" },
            new string[]{ "file.results", "bulk" },
            new string[]{ "file.users", "bulk" },
            new string[]{ "source.systemName", "OneRosterProviderDemo" },
            new string[]{ "source.systemCode", "OneRosterProviderDemo" },
        };
        private string Manifest()
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                foreach (var manifestPair in manifestValues)
                {
                    csv.WriteField(manifestPair[0]);
                    csv.WriteField(manifestPair[1]);
                    csv.NextRecord();
                }
            }
            return sb.ToString();
        }

        private string AcademicSessions()
        {
            var sessions = db.AcademicSessions;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                AcademicSession.CsvHeader(csv);
                foreach (var session in sessions)
                {
                    session.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Orgs()
        {
            var orgs = db.Orgs
                .Include(o => o.Parent)
                .Include(o => o.Children);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                Org.CsvHeader(csv);
                foreach (var org in orgs)
                {
                    org.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Courses()
        {
            var courses = db.Courses
                .Include(c => c.SchoolYearAcademicSession)
                .Include(c => c.Org);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                Course.CsvHeader(csv);
                foreach (var course in courses)
                {
                    course.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string IMSClasses()
        {
            var imsClasses = db.IMSClasses
                .Include(k => k.IMSClassAcademicSessions)
                    .ThenInclude(kas => kas.AcademicSession)
                .Include(k => k.Course)
                .Include(k => k.School);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                IMSClass.CsvHeader(csv);
                foreach (var imsClass in imsClasses)
                {
                    imsClass.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Users()
        {
            var users = db.Users
                .Include(u => u.UserOrgs)
                    .ThenInclude(uo => uo.Org)
                .Include(u => u.UserAgents)
                    .ThenInclude(ua => ua.Agent);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                User.CsvHeader(csv);
                foreach (var user in users)
                {
                    user.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Enrollments()
        {
            var enrollments = db.Enrollments
                .Include(e => e.User)
                .Include(e => e.IMSClass)
                .Include(e => e.School);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                Enrollment.CsvHeader(csv);
                foreach (var enrollment in enrollments)
                {
                    enrollment.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Categories()
        {
            var categories = db.LineItemCategories;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                LineItemCategory.CsvHeader(csv);
                foreach (var category in categories)
                {
                    category.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string LineItems()
        {
            var lineItems = db.LineItems;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                LineItem.CsvHeader(csv);
                foreach (var lineItem in lineItems)
                {
                    lineItem.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }

        private string Results()
        {
            var results = db.Results;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                Result.CsvHeader(csv);
                foreach (var result in results)
                {
                    result.AsCsvRow(csv);
                }
            }
            return sb.ToString();
        }
    }
}
