using System;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using LiveUtils.LiveSeekData.Tests.Metrics;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Data;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Domain;
using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Queries;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Serilog;

namespace LiveUtils.LiveSeekData.Tests
{
    [SetUpFixture]
    public class TestInitializer
    {
        public static IServiceProvider ServiceProvider;
        public static int TotalItems = 1000000;
        public static IConfiguration Config;

        [OneTimeSetUp]
        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            SetupDependencyInjection();
            InitDb();
        }

        private void SetupDependencyInjection()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            var services = new ServiceCollection();
            SetupDb(services);
            services.AddMediatR(typeof(GetPersonsQueryHandler));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            
            ServiceProvider = services.BuildServiceProvider();
        }

        private void SetupDb(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(Config.GetSection(DatabaseSettings.SettingsKey));
            
            var providerType = Config.GetValue<string>(
                $"{DatabaseSettings.SettingsKey}:{nameof(DatabaseSettings.ProviderType)}");

            if (providerType.ToLower() == "SqlServer".ToLower())
            {
                services.AddDbContext<TestDbContext>(x => x.UseSqlServer(Config.GetConnectionString("LiveConnection")));
            }
            else
            {
                var connection = new SqliteConnection(GetTempDbConnection(Config, true));
                connection.Open();
                services.AddDbContext<TestDbContext>(x => x.UseSqlite(connection));
            }

        }
        private void InitDb()
        {
            var ctx = ServiceProvider.GetService<TestDbContext>();
            var dbs = ServiceProvider.GetService<IOptions<DatabaseSettings>>();

            if (dbs.Value.ApplyDatabaseMigrations)
            {
                ctx.Database.EnsureCreated();
                ctx.Database.Migrate();
            }
            
            if (dbs.Value.ApplySeed)
            {
                var persons = Builder<Person>.CreateListOfSize(TotalItems)
                    .All().With(x=>x.Gender=GenerateGender())
                    .Build().ToList();
                ctx.AddRange(persons);
                ctx.SaveChanges();
            }
        }


        public static string GenerateGender()
        {
            var _random = new Random();
            var gender = _random.Next(1, 9) % 2 == 0 ? "Male" : "Famale";
            return $"{gender}";
        }


        private string GetTempDbConnection(IConfiguration configuration,bool isNew=true)
        {
            RemoveTestsFilesDbs();
            
            var providerType = configuration.GetValue<string>(
                $"{DatabaseSettings.SettingsKey}:{nameof(DatabaseSettings.ProviderType)}");
            
            if (providerType== "SqlLite")
            {
                var dir = $"{TestContext.CurrentContext.TestDirectory.HasToEndWith(@"/")}";
                var db = $"DataSource={dir}TestArtifacts/Database/demo.db";
                var cn = isNew ? db.Replace(".db", $"{DateTime.Now.Ticks}.db") : db.Replace("demo.db","demolive.db");
                return cn.ToOsStyle();
            }

            return string.Empty;
        }
        
        private void RemoveTestsFilesDbs()
        {
            string[] keyFiles = { "demo.db" };
            string[] keyDirs = { @"TestArtifacts/Database".ToOsStyle()};

            foreach (var keyDir in keyDirs)
            {
                DirectoryInfo di = new DirectoryInfo(keyDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!keyFiles.Contains(file.Name))
                        file.Delete();
                }
            }
        }
  }

    static class Extentions
    {
        
        public static string HasToEndWith(this string value, string end)
        {
            if (value == null)
                return string.Empty;

            return value.EndsWith(end) ? value : $"{value}{end}";
        }
        public static string ToOsStyle(this string value)
        {
            if (value == null)
                return string.Empty;

            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
                return value.Replace(@"\", @"/");

            return value.Replace(@"/", @"\");
        }
    }
}