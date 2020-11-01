using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Database;
using Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;
using Backend.Models;

namespace LektionWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameInfoController : ControllerBase
    {
        private readonly IGameRepository gr;
        private readonly IWebHostEnvironment env;   
        private readonly IImageRepository imr;
        public GameInfoController(IGameRepository gr, IWebHostEnvironment env, IImageRepository imr)
        {
            this.env = env;
            this.gr = gr;
            this.imr = imr;            
        }

        [HttpGet]
        public async Task<IEnumerable<GameInfo>> Get()
        {
            var games = await gr.Get();
            return games;
        }
        [HttpGet("{id}")]
        public async Task<GameInfo> GetGame(int id)
        {
            return await gr.Get(id);
        }

        [HttpPost]
        public async Task<GameInfo> Add(GameInfo newGame)
        {
            return await gr.Add(newGame);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await gr.Delete(id);
        }

        [HttpPut]
        public async Task<GameInfo> Update(GameInfo newGame)
        {
            return await gr.Update(newGame);
        }

        [HttpPost("/img")]
        public async Task<GameInfo> AddGameImage([FromForm] PostGameImage gameImage)
        {
            GameInfo exGame = await gr.Get(gameImage.Id);
            
            var file = gameImage.PostImage;
            var ext = Path.GetExtension(file.FileName).Replace(".", string.Empty);
            string Name = exGame.Id + "." + ext;

            string path = Path.Combine(env.ContentRootPath, "GamesImage\\" + Name);
            using(var stream = new FileStream(path, FileMode.OpenOrCreate)) 
            {
                await file.CopyToAsync(stream);
            }

            string url = "GamesImage\\" + Name;
            exGame.Image = url;
            await gr.Update(exGame);
            return exGame;            
        }
        [HttpGet("/img/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            GameInfo game = await gr.Get(id);
            if(game == null) 
            {
                throw new ArgumentException("The ID dose not exist");
            }
            if(game.Image == null) 
            {
                throw new ArgumentException("The game dose not have a image");
            }
            var imgSrc = Path.Combine(env.ContentRootPath, game.Image);
            if(System.IO.File.Exists(imgSrc)) 
            {
                return PhysicalFile(imgSrc, "image/png");
            }
            else {
                throw new ArgumentException("File not found or wrong file type");
            }
        }

    }
}
