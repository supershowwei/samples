using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchitectSample.Protocol.Model.Data
{
    public class Member
    {
        [Column("MemberNo")]
        public int Id { get; set; }

        public string NickName { get; set; }

        public List<Club> Clubs { get; set; }
    }
}