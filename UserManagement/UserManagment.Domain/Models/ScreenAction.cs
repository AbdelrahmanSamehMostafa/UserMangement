using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagment.Common.Enum;

namespace UserManagment.Domain.Models;

public partial class ScreenAction : BaseModel
{
    [Key]
    public Guid Id { get; set; }
    [StringLength(50)]
    public required string? ActionDisplayName { get; set; }
    public Guid ScreenId { get; set; }
    [ForeignKey("ScreenId")]
    public virtual Screen Screen { get; set; } = null!;
    public string? PolicyName { get; set; }
 
    public ActionType ActionType { get; set; } = ActionType.List;
 
 
}

