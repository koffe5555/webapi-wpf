using System;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Database
{
    public class DatabaseService : IDatabaseService
    {
        private DatabaseConfig databaseConfig;
        public DatabaseService(DatabaseConfig dbConfig)
        {
            databaseConfig = dbConfig;
        }

        public void Setup()
        {
            using(SqliteConnection connection = new SqliteConnection(databaseConfig.Name))
            {
                var table = connection.Query<string>("SELECT Name FROM sqlite_master WHERE type='table' AND name = 'Games'");
                var tableName = table.FirstOrDefault();
                if(!string.IsNullOrEmpty(tableName) && tableName == "Games")
                {
                    return;
                }
                using(var sr = new StreamReader(databaseConfig.StructureFile)) 
                {
                        var queries = sr.ReadToEnd();
                        connection.Execute(queries);
                }
            }
        }

        public async Task<GameInfo> Update(GameInfo game)
        {
            using(var con = new SqliteConnection(databaseConfig.Name))
            {
                var res = await con.ExecuteAsync("UPDATE Games SET Name=@Name, Grade=@Grade, Description=@Description, Image=@Image WHERE Id=@Id", game);
                return game;
            }
        }
        public async Task<GameInfo> Add(GameInfo game)
        {
            using(var connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await connection.ExecuteAsync("INSERT INTO Games (Name, Description, Grade, Image) VALUES (@Name, @Description, @Grade, @Image)", game);
                var lastInsert = await connection.QueryAsync<GameInfo>("SELECT Id, Name, Description, Grade, Image FROM Games ORDER BY Id DESC");
                return new GameInfo() {
                     Name = game.Name,
                     Description = game.Description,
                     Id = lastInsert.FirstOrDefault<GameInfo>().Id,
                     Grade = game.Grade
                };
            }
        }
        public async Task<IEnumerable<GameInfo>> Get()
        {
            using(var con = new SqliteConnection(databaseConfig.Name))
            {
                var res = await con.QueryAsync<GameInfo>("SELECT Id, Name, Description, Image, Grade FROM Games ORDER BY Id ASC");
                return res;
            }
        }
        public async Task<GameInfo> Get(int Id)
        {
            using(var con = new SqliteConnection(databaseConfig.Name))
            {
                var res = await con.QueryAsync<GameInfo>("SELECT Id, Name, Description, Grade, Image FROM Games WHERE Id=@Id", new { Id });
                return res.FirstOrDefault<GameInfo>();
            }
        }
        public async Task<bool> Delete(int Id)
        {
            using(var connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await connection.ExecuteAsync("DELETE FROM Games WHERE Id=@Id", new { Id });
                if(res > -1) 
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
        }       
    }
}
