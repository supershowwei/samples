using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchitectSample.Protocol.Model.Data
{
    [Table("ArticleCategory")]
    public class ClubArticleCategory
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "varchar"), StringLength(900)]
        public string RequiredReadingVideos { get; set; }
    }
}