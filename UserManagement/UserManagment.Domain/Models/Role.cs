using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("Role")]
public partial class Role : BaseModel
{

    [StringLength(50)]
    public required string Name { get; set; }
    public bool IsDefault { get; set; }

}

