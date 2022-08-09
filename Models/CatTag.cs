using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSA2022.Phase2.Backend.Models
{
    public class CatTag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Tag { get; set; }
    }

    public class CatTagDb : DbContext
    {
        public CatTagDb(DbContextOptions options) : base(options) { }
        public DbSet<CatTag> CatTags { get; set; }
    }
}
