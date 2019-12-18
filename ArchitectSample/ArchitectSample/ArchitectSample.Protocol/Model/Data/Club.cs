using System.ComponentModel.DataAnnotations.Schema;

namespace ArchitectSample.Protocol.Model.Data
{
    public class Club
    {
        [Column("ClubID")]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}