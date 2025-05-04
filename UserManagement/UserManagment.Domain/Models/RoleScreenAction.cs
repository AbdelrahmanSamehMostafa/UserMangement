using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("RoleScreenAction")]
public partial class RoleScreenAction : BaseModel
{

    public Guid RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    public Guid ScreenActionId { get; set; }
    [ForeignKey("ScreenActionId")]
    public virtual ScreenAction ScreenAction { get; set; } = null!;

   


}

