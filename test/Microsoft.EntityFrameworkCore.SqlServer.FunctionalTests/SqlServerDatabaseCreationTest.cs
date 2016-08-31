// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Specification.Tests.TestUtilities.Xunit;
using Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests
{
    public class SqlServerDatabaseCreationTest
    {
        [Fact]
        public async Task Exists_returns_false_when_database_doesnt_exist()
        {
            await Exists_returns_false_when_database_doesnt_exist_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task Exists_returns_false_when_database_with_filename_doesnt_exist()
        {
            await Exists_returns_false_when_database_doesnt_exist_test(async: false, file: true);
        }

        [Fact]
        public async Task ExistsAsync_returns_false_when_database_doesnt_exist()
        {
            await Exists_returns_false_when_database_doesnt_exist_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task ExistsAsync_returns_false_when_database_with_filename_doesnt_exist()
        {
            await Exists_returns_false_when_database_doesnt_exist_test(async: true, file: true);
        }

        private static async Task Exists_returns_false_when_database_doesnt_exist_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: false, useFileName: file))
            {
                using (var context = new BloggingContext(testDatabase))
                {
                    var creator = context.GetService<IRelationalDatabaseCreator>();

                    Assert.False(async ? await creator.ExistsAsync() : creator.Exists());

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
                }
            }
        }

        [Fact]
        public async Task Exists_returns_true_when_database_exists()
        {
            await Exists_returns_true_when_database_exists_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task Exists_returns_true_when_database_with_filename_exists()
        {
            await Exists_returns_true_when_database_exists_test(async: false, file: true);
        }

        [Fact]
        public async Task ExistsAsync_returns_true_when_database_exists()
        {
            await Exists_returns_true_when_database_exists_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task ExistsAsync_returns_true_when_database_with_filename_exists()
        {
            await Exists_returns_true_when_database_exists_test(async: true, file: true);
        }

        private static async Task Exists_returns_true_when_database_exists_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: true, useFileName: file))
            {
                using (var context = new BloggingContext(testDatabase))
                {
                    var creator = context.GetService<IRelationalDatabaseCreator>();

                    Assert.True(async ? await creator.ExistsAsync() : creator.Exists());

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
                }
            }
        }

        [Fact]
        public async Task EnsureDeleted_will_delete_database()
        {
            await EnsureDeleted_will_delete_database_test(async: false, open: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureDeleted_will_delete_database_with_filename()
        {
            await EnsureDeleted_will_delete_database_test(async: false, open: false, file: true);
        }

        [Fact]
        public async Task EnsureDeletedAsync_will_delete_database()
        {
            await EnsureDeleted_will_delete_database_test(async: true, open: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureDeletedAsync_will_delete_database_with_filename()
        {
            await EnsureDeleted_will_delete_database_test(async: true, open: false, file: true);
        }

        [Fact]
        public async Task EnsureDeleted_will_delete_database_with_opened_connections()
        {
            await EnsureDeleted_will_delete_database_test(async: false, open: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureDeleted_will_delete_database_with_filename_with_opened_connections()
        {
            await EnsureDeleted_will_delete_database_test(async: false, open: true, file: true);
        }

        [Fact]
        public async Task EnsureDeletedAsync_will_delete_database_with_opened_connections()
        {
            await EnsureDeleted_will_delete_database_test(async: true, open: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureDeletedAsync_will_delete_database_with_filename_with_opened_connections()
        {
            await EnsureDeleted_will_delete_database_test(async: true, open: true, file: true);
        }

        private static async Task EnsureDeleted_will_delete_database_test(bool async, bool open, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: true, useFileName: file))
            {
                if (!open)
                {
                    testDatabase.Connection.Close();
                }

                using (var context = new BloggingContext(testDatabase))
                {
                    var creator = context.GetService<IRelationalDatabaseCreator>();

                    Assert.True(async ? await creator.ExistsAsync() : creator.Exists());

                    if (async)
                    {
                        Assert.True(await context.Database.EnsureDeletedAsync());
                    }
                    else
                    {
                        Assert.True(context.Database.EnsureDeleted());
                    }

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);

                    Assert.False(async ? await creator.ExistsAsync() : creator.Exists());

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
                }
            }
        }

        [Fact]
        public async Task EnsuredDeleted_noop_when_database_doesnt_exist()
        {
            await EnsuredDeleted_noop_when_database_doesnt_exist_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsuredDeleted_noop_when_database_with_filename_doesnt_exist()
        {
            await EnsuredDeleted_noop_when_database_doesnt_exist_test(async: false, file: true);
        }

        [Fact]
        public async Task EnsuredDeletedAsync_noop_when_database_doesnt_exist()
        {
            await EnsuredDeleted_noop_when_database_doesnt_exist_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsuredDeletedAsync_noop_when_database_with_filename_doesnt_exist()
        {
            await EnsuredDeleted_noop_when_database_doesnt_exist_test(async: true, file: true);
        }

        private static async Task EnsuredDeleted_noop_when_database_doesnt_exist_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: false, useFileName: file))
            {
                using (var context = new BloggingContext(testDatabase))
                {
                    var creator = context.GetService<IRelationalDatabaseCreator>();

                    Assert.False(async ? await creator.ExistsAsync() : creator.Exists());

                    if (async)
                    {
                        Assert.False(await creator.EnsureDeletedAsync());
                    }
                    else
                    {
                        Assert.False(creator.EnsureDeleted());
                    }

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);

                    Assert.False(async ? await creator.ExistsAsync() : creator.Exists());

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
                }
            }
        }

        [Fact]
        public async Task EnsureCreated_can_create_schema_in_existing_database()
        {
            await EnsureCreated_can_create_schema_in_existing_database_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureCreated_can_create_schema_in_existing_database_with_filename()
        {
            await EnsureCreated_can_create_schema_in_existing_database_test(async: false, file: true);
        }

        [Fact]
        public async Task EnsureCreatedAsync_can_create_schema_in_existing_database()
        {
            await EnsureCreated_can_create_schema_in_existing_database_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureCreatedAsync_can_create_schema_in_existing_database_with_filename()
        {
            await EnsureCreated_can_create_schema_in_existing_database_test(async: true, file: true);
        }

        private static async Task EnsureCreated_can_create_schema_in_existing_database_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(useFileName: file))
            {
                await RunDatabaseCreationTest(testDatabase, async);
            }
        }

        [Fact]
        public async Task EnsureCreated_can_create_physical_database_and_schema()
        {
            await EnsureCreated_can_create_physical_database_and_schema_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureCreated_can_create_physical_database_with_filename_and_schema()
        {
            await EnsureCreated_can_create_physical_database_and_schema_test(async: false, file: true);
        }

        [Fact]
        public async Task EnsureCreatedAsync_can_create_physical_database_and_schema()
        {
            await EnsureCreated_can_create_physical_database_and_schema_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsureCreatedAsync_can_create_physical_database_with_filename_and_schema()
        {
            await EnsureCreated_can_create_physical_database_and_schema_test(async: true, file: true);
        }

        private static async Task EnsureCreated_can_create_physical_database_and_schema_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: false, useFileName: file))
            {
                await RunDatabaseCreationTest(testDatabase, async);
            }
        }

        private static async Task RunDatabaseCreationTest(SqlServerTestStore testStore, bool async)
        {
            using (var context = new BloggingContext(testStore))
            {
                var creator = context.GetService<IRelationalDatabaseCreator>();

                Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);

                if (async)
                {
                    Assert.True(await creator.EnsureCreatedAsync());
                }
                else
                {
                    Assert.True(creator.EnsureCreated());
                }

                Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);

                if (testStore.Connection.State != ConnectionState.Open)
                {
                    await testStore.Connection.OpenAsync();
                }

                var tables = await testStore.QueryAsync<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");
                Assert.Equal(1, tables.Count());
                Assert.Equal("Blogs", tables.Single());

                var columns = (await testStore.QueryAsync<string>(
                    "SELECT TABLE_NAME + '.' + COLUMN_NAME + ' (' + DATA_TYPE + ')' FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'Blogs' ORDER BY TABLE_NAME, COLUMN_NAME")).ToArray();
                Assert.Equal(15, columns.Length);

                Assert.Equal(
                    new[]
                    {
                        "Blogs.AndChew (varbinary)",
                        "Blogs.AndRow (timestamp)",
                        "Blogs.Cheese (nvarchar)",
                        "Blogs.CupOfChar (int)",
                        "Blogs.ErMilan (int)",
                        "Blogs.Fuse (smallint)",
                        "Blogs.George (bit)",
                        "Blogs.Key1 (nvarchar)",
                        "Blogs.Key2 (varbinary)",
                        "Blogs.NotFigTime (datetime2)",
                        "Blogs.On (real)",
                        "Blogs.OrNothing (float)",
                        "Blogs.TheGu (uniqueidentifier)",
                        "Blogs.ToEat (tinyint)",
                        "Blogs.WayRound (bigint)"
                    },
                    columns);
            }
        }

        [Fact]
        public async Task EnsuredCreated_is_noop_when_database_exists_and_has_schema()
        {
            await EnsuredCreated_is_noop_when_database_exists_and_has_schema_test(async: false, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsuredCreated_is_noop_when_database_with_filename_exists_and_has_schema()
        {
            await EnsuredCreated_is_noop_when_database_exists_and_has_schema_test(async: false, file: true);
        }

        [Fact]
        public async Task EnsuredCreatedAsync_is_noop_when_database_exists_and_has_schema()
        {
            await EnsuredCreated_is_noop_when_database_exists_and_has_schema_test(async: true, file: false);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.IsSqlLocalDb)]
        public async Task EnsuredCreatedAsync_is_noop_when_database_with_filename_exists_and_has_schema()
        {
            await EnsuredCreated_is_noop_when_database_exists_and_has_schema_test(async: true, file: true);
        }

        private static async Task EnsuredCreated_is_noop_when_database_exists_and_has_schema_test(bool async, bool file)
        {
            using (var testDatabase = await SqlServerTestStore.CreateScratchAsync(createDatabase: false, useFileName: file))
            {
                using (var context = new BloggingContext(testDatabase))
                {
                    context.Database.EnsureCreated();

                    if (async)
                    {
                        Assert.False(await context.Database.EnsureCreatedAsync());
                    }
                    else
                    {
                        Assert.False(context.Database.EnsureCreated());
                    }

                    Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
                }
            }
        }

        private static IServiceProvider CreateServiceProvider()
            => new ServiceCollection().AddEntityFrameworkSqlServer().BuildServiceProvider();

        private class BloggingContext : DbContext
        {
            private readonly SqlServerTestStore _testStore;

            public BloggingContext(SqlServerTestStore testStore)
            {
                _testStore = testStore;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseSqlServer(_testStore.ConnectionString)
                    .UseInternalServiceProvider(CreateServiceProvider());

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Blog>(b =>
                    {
                        b.HasKey(e => new { e.Key1, e.Key2 });
                        b.Property(e => e.AndRow).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
                    });
            }

            public DbSet<Blog> Blogs { get; set; }
        }

        public class Blog
        {
            public string Key1 { get; set; }
            public byte[] Key2 { get; set; }
            public string Cheese { get; set; }
            public int ErMilan { get; set; }
            public bool George { get; set; }
            public Guid TheGu { get; set; }
            public DateTime NotFigTime { get; set; }
            public byte ToEat { get; set; }
            public char CupOfChar { get; set; }
            public double OrNothing { get; set; }
            public short Fuse { get; set; }
            public long WayRound { get; set; }
            public float On { get; set; }
            public byte[] AndChew { get; set; }
            public byte[] AndRow { get; set; }
        }
    }
}
