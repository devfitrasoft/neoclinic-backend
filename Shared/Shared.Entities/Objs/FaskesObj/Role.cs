﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.FaskesObj
{
    /// <summary>Maps to <b>sys_role</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_role")]
    public class Role
    {
        [Key, Column("id")] public int Id { get; set; }
        
        [Required, MaxLength(50), Column("name")] 
        public string Name { get; set; } = null!;

        [Column("is_active")] public bool? IsActive { get; set; }
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }

        public ICollection<Auth> Auths { get; set; } = new List<Auth>();    // collection of authorizations a role has
    }
}
