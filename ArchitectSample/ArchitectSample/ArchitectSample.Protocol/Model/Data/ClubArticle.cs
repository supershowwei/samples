using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchitectSample.Protocol.Model.Data
{
    [Table("Article")]
    public class ClubArticle
    {
        public int Id { get; set; }

        public int ClubId { get; set; }

        public string Topic { get; set; }

        public string Content { get; set; }
    }
}