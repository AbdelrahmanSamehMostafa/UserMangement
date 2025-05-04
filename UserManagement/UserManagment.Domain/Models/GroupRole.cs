using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("GroupRole")]
public partial class GroupRole : BaseModel
{

    public Guid GroupId { get; set; }
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    public Guid RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}

