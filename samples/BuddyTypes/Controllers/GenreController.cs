using BuddyTypes.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BuddyTypes.Controllers
{
   // [Route("api/genre")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext context;

        public GenreController(ApplicationDbContext dbContext)
        {
            context = dbContext;
        }

        //[HttpPost]
        //public async Task<Genre> Post()
        //public async Task<Album> Album([FromBody] Album postedAlbum)
        //{
        //    int id = 0;// postedAlbum.Id;

        //    Genre genre = null;
        //    if (id < 1)
        //        genre = context.Genres.Add(new Genre());
        //    else
        //    {

        //        //genre = await context.Genres.Include(ctx => ctx.Albums).FirstOrDefaultAsync(g => g.GenreId == id);
        //        genre = await context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

        //        //album = await context.Albums
        //        //    .Include(ctx => ctx.Genre)
        //        //    .FirstOrDefaultAsync(alb => alb.AlbumId == id);

        //        if (genre == null)
        //            throw new Exception("Invalid genre id");
        //    }


        //    // ModelBinding doesn't work right at the moment
        //    if (!await TryUpdateModelAsync(genre))
        //        throw new Exception("Model binding failed.");

        //    if (ModelState.IsValid)
        //    {
        //        Console.WriteLine(ModelState);
        //    }

        //    int result = await context.SaveChangesAsync();

        //    if (ModelState.IsValid)
        //    {
        //        Console.WriteLine(ModelState);
        //    }

        //    return genre;
        //}

        public IActionResult GenreEdit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenreEdit(GenreDTO gDto)
        {
            Genre genre = null;
            bool s = ModelState.IsValid;
            ModelState.Clear();
            await TryValidateModelAsync(gDto);

            if (ModelState.IsValid)
            {
#if ASPNET50
                genre = AutoMapper.Mapper.Map<Genre>(gDto);
                await TryValidateModelAsync(genre);
                context.Genres.Add(genre);
                context.SaveChanges();
#endif
            }
            return View(gDto);
        }

        [HttpPost]
        //  public Genre Post([FromBody]Genre genre)
        public async Task<Genre> Post([FromBody]GenreDTO gDto)
        {
            Genre genre = null;
            bool s = ModelState.IsValid;
            //  ModelState.Clear();
            // await TryValidateModelAsync(gDto);

            if (ModelState.IsValid)
            {
#if ASPNET50
                genre = AutoMapper.Mapper.Map<Genre>(gDto);
                genre.GenreId = 5;
                context.Genres.Add(genre);
                context.SaveChanges();

#endif
            }

            return genre;
        }

        [HttpPut("{id:int}")]
        public async Task<Genre> Put(int id, [FromBody]GenreDTO gDTO)
        {
            var genre = context.Genres.Include(g => g.Albums).FirstOrDefault(g => g.GenreId == id);
#if ASPNET50
            genre = AutoMapper.Mapper.Map<GenreDTO, Genre>(gDTO, genre);
            context.Entry(genre).SetState(EntityState.Modified);
            var x = await context.SaveChangesAsync();
            Console.WriteLine(x);
#endif

            return genre;
        }
    }
}
