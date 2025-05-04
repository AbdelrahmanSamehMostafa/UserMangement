using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("GroupUser")]
public partial class GroupUser : BaseModel
{

    public Guid GroupId { get; set; }
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

