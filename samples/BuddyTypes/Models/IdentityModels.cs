using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.SqlServer;
using Microsoft.Framework.DependencyInjection;

namespace BuddyTypes.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public ApplicationDbContext()
        {            
        }
        
        protected override void OnConfiguring(DbContextOptions options)
        {
            options.UseSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Album>().Key(a => a.AlbumId);

            builder.Entity<Genre>().Key(g => g.GenreId);
            builder.Entity<Genre>().Property(g => g.GenreId).GenerateValueOnAdd(generateValue: false);
            builder.Entity<Genre>().OneToMany(a => a.Albums);

            base.OnModelCreating(builder);
        }
    }

    public static class SampleData
    {
        const string imgUrl = "~/Images/placeholder.png";

        private static Dictionary<string, Genre> genres;

        public static Dictionary<string, Genre> Genres
        {
            get
            {
                if (genres == null)
                {
                    var genresList = new Genre[]
                    {
                        new Genre { Name = "Pop" },
                        new Genre { Name = "Rock" },
                        new Genre { Name = "Jazz" }
                    };

                    genres = new Dictionary<string, Genre>();
                    // TODO [EF] Swap to store generated keys when available
                    int genreId = 1;
                    foreach (Genre genre in genresList)
                    {
                        genre.GenreId = genreId++;

                        // TODO [EF] Remove when null values are supported by update pipeline
                        genre.Description = genre.Name + " is great music (if you like it).";

                        genres.Add(genre.Name, genre);
                    }
                }

                return genres;
            }
        }

        public static async Task InitializeDatabase(IServiceProvider serviceProvider)
        {
            using (var db = serviceProvider.GetService<ApplicationDbContext>())
            {
                //await db.Database.EnsureDeletedAsync();
                var sqlServerDb = db.Database as SqlServerDatabase;
                if (sqlServerDb != null)
                {
                    if (await db.Database.EnsureCreatedAsync())
                    {
                        await InsertTestData(serviceProvider);
                    }
                }
                else
                {
                    await InsertTestData(serviceProvider);
                }
            }
        }

        private static async Task InsertTestData(IServiceProvider serviceProvider)
        {
            var albums = GetAlbums(imgUrl, Genres);

            await AddOrUpdateAsync(serviceProvider, g => g.GenreId, Genres.Select(genre => genre.Value));
            await AddOrUpdateAsync(serviceProvider, a => a.AlbumId, albums);

        }

        private static async Task AddOrUpdateAsync<TEntity>(
            IServiceProvider serviceProvider,
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            List<TEntity> existingData;
            using (var db = serviceProvider.GetService<ApplicationDbContext>())
            {
                existingData = db.Set<TEntity>().ToList();
            }

            using (var db = serviceProvider.GetService<ApplicationDbContext>())
            {
                foreach (var item in entities)
                {
                    db.Entry(item).SetState(existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
                        ? EntityState.Modified
                        : EntityState.Added);
                }

                await db.SaveChangesAsync();
            }
        }

        private static Album[] GetAlbums(string imgUrl, Dictionary<string, Genre> genres)
        {
            var albums = new Album[]
            {
                new Album { Title = "The Best Of The Men At Work", Genre = genres["Pop"], Price = 8.99M, Url = imgUrl },
                new Album { Title = "...And Justice For All", Genre = genres["Jazz"], Price = 8.99M, Url = imgUrl },
                new Album { Title = "A Real Live One", Genre = genres["Rock"], Price = 8.99M, Url = imgUrl },
                new Album { Title = "A Matter of Life and Death", Genre = genres["Rock"], Price = 8.99M, Url = imgUrl },
                new Album { Title = "Elegant Gypsy", Genre = genres["Jazz"], Price = 8.99M, Url = imgUrl },
            };

            foreach (var album in albums)
            {
                album.GenreId = album.Genre.GenreId;
            }

            return albums;
        }
    }
}