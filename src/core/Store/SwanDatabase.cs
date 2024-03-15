using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Option;

namespace Swan.Core.Store
{
    internal class SwanDatabase : ISwanDatabase
    {
        private readonly SwanOption _option;
        private readonly Lazy<string> _connectionString;

        public SwanDatabase(IOptions<SwanOption> option)
        {
            _option = option.Value;
            _connectionString = new Lazy<string>(() => BuildConnectionString(), true);
        }

        //public async Task TestAsync(string oldDir)
        //{
        //    var dbFile = Path.Combine(_option.DataLocation, "swan.db");
        //    if(File.Exists(dbFile))
        //    {
        //        File.Delete(dbFile);
        //    }
        //    await StartAsync();
        //    var tagId = await InsertAsync(new SwanTag
        //    {
        //        Description = "POST",
        //        IsPublic = true,
        //        Name = "post",
        //        Url = "post"
        //    });

        //    var oldPosts = JsonHelper.Deserialize<List<Model.SwanPost>>(await File.ReadAllTextAsync(Path.Combine(oldDir, "obj", "post.json")));
        //    foreach(var  post in oldPosts)
        //    {
        //        var newPost = new SwanPost
        //        {
        //            Content = post.Content,
        //            CreatedAt = post.CreatedAt,
        //            IsPublic = post.IsPublic,
        //            LastModifiedAt = post.LastUpdatedAt,
        //            PublishedAt = post.PublishDate,
        //            TagId = tagId.Value,
        //            Title = post.Title,
        //            Url = post.Link,
        //            Visits = 300
        //        };
        //        await InsertAsync(newPost);
        //    }

        //    var oldReads = JsonHelper.Deserialize<List<Model.SwanRead>>(await File.ReadAllTextAsync(Path.Combine(oldDir, "obj", "read.json")));
        //    foreach (var post in oldReads)
        //    {
        //        var newRead = new SwanRead
        //        {
        //            Translator = post.Translator,
        //            LastModifiedAt = post.LastUpdatedAt,
        //            Author = post.Author,
        //            AuthorCountry = post.AuthorCountry,
        //            Comment = post.Comment,
        //            CreatedAt = post.CreatedAt,
        //            Grade = post.Grade,
        //            IsPublic = post.IsPublic,
        //            BookName = post.BookName
        //        };
        //        await InsertAsync(newRead);
        //    }
        //}

        public async Task StartAsync()
        {
            var startupFile = Path.Combine(AppContext.BaseDirectory, "Store", "startup.sql");
            if (!File.Exists(startupFile))
            {
                throw new Exception($"Failed to find startup.sql from {startupFile}");
            }

            var startupSql = await File.ReadAllTextAsync(startupFile);
            await using SqliteConnection connection = new(_connectionString.Value);
            await connection.ExecuteAsync(startupSql);
        }

        public async Task<long?> InsertAsync<T>(T obj) where T : ISwanObject
        {
            if(obj.CreatedAt == default)
            {
                obj.CreatedAt = DateTime.Now;
            }

            if(obj.LastModifiedAt == default)
            {
                obj.LastModifiedAt = DateTime.Now;
            }

            var properties = T.GetObjectProperties();
            properties.Remove("Id");

            var columns = string.Join(", ", properties.Select(x => $"{StringHelper.Underscored(x)}"));
            var values = string.Join(", ", properties.Select(x => $"@{x}"));
            var sql = $"INSERT INTO {T.ObjectName}({columns}) VALUES({values}) RETURNING id";
            await using SqliteConnection connection = new(_connectionString.Value);
            var id = await connection.ExecuteScalarAsync(sql, obj);
            return id == null ? null : Convert.ToInt64(id);
        }

        public async Task UpdateAsync<T>(T obj) where T : ISwanObject
        {
            obj.LastModifiedAt = DateTime.Now;

            var properties = T.GetObjectProperties();
            properties.Remove("Id");

            var setProperties = new Dictionary<string, string>();
            foreach (var prop in properties)
            {
                setProperties.Add(StringHelper.Underscored(prop), $"@{prop}");
            }

            var sql = $"UPDATE {T.ObjectName} SET {string.Join(", ", setProperties.Select(x => $"{x.Key}='{x.Value}'"))} WHERE id = @id";
            await using SqliteConnection connection = new(_connectionString.Value);
            await connection.ExecuteAsync(sql, obj);
        }

        public async Task<List<T>> QueryAsync<T>(DatabaseQuery queryProperties = null) where T : ISwanObject
        {
            queryProperties ??= new DatabaseQuery();
            var sql = $"SELECT * FROM {T.ObjectName} {queryProperties}";
            await using SqliteConnection connection = new(_connectionString.Value);
            var result = await connection.QueryAsync<T>(sql);
            return result.AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(DatabaseQuery queryProperties = null) where T : ISwanObject
        {
            queryProperties ??= new DatabaseQuery();
            var sql = $"SELECT * FROM {T.ObjectName} {queryProperties}";
            await using SqliteConnection connection = new(_connectionString.Value);
            var result = await connection.QueryFirstOrDefaultAsync<T>(sql);
            return result;
        }

        public async Task DeleteAsync<T>(long id) where T : ISwanObject
        {
            var sql = $"DELETE {T.ObjectName} WHERE id = @id";
            await using SqliteConnection connection = new(_connectionString.Value);
            await connection.ExecuteAsync(sql, new { id });
        }

        private string BuildConnectionString()
        {
            Directory.CreateDirectory(_option.DataLocation);
            var dbFile = Path.Combine(_option.DataLocation, "swan.db");
            var builder = new SqliteConnectionStringBuilder
            {
                Cache = SqliteCacheMode.Shared,
                DataSource = Path.GetFullPath(dbFile),
                Mode = SqliteOpenMode.ReadWriteCreate,
                DefaultTimeout = 30,
                Pooling = true,
                ForeignKeys = true
            };

            return builder.ToString();
        }
    }
}
