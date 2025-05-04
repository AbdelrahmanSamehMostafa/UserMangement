using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("Screen")]
public partial class Screen : BaseModel
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }
    public required string AreaName { get; set; }
    public Guid? ParentId { get; set; }
    public bool? IsMenuScreen { get; set; }

    [ForeignKey("ParentId")]
    public virtual Screen? ParentScreen { get; set; }


}

