using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Database;
using Backend.Models;
using Backend.Repository;
using Dapper;
using Microsoft.Data.Sqlite;

namespace GameWebApi.Repositorys
{

    public class GameRepository : IGameRepository
    {
        //private List<GameInfo> Games;
               
        private readonly DatabaseConfig databaseConfig;
        private readonly IDatabaseService db;
        public GameRepository(DatabaseConfig dbConfig, IDatabaseService db)
        {
            databaseConfig = dbConfig;
            this.db = db;
        }
        public async Task<GameInfo> Add(GameInfo NewGame)
        {
            return await db.Add(NewGame);
        }

        public async Task<bool> Delete(int id)
        {
            return await db.Delete(id);
        }

        public async Task<GameInfo> Get(int id)
        {
            return await db.Get(id);
        }

        public async Task<IEnumerable<GameInfo>> Get()
        {
            var games = await db.Get();
            return games;
        }


        public async Task<GameInfo> Update(GameInfo NewGame)
        {
            return await db.Update(NewGame);
        }
    }
}

