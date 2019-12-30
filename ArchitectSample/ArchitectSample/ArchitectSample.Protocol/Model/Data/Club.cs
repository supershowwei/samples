using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchitectSample.Protocol.Model.Data
{
    public class Club
    {
        [Column("ClubID")]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        #region ClubMember

        public DateTime Deadline { get; set; }

        public string Status { get; set; }

        #endregion

        public List<Member> Members { get; set; }

        public List<ClubArticle> Articles { get; set; }
    }
}