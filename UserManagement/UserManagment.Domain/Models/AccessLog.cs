using System.ComponentModel.DataAnnotations.Schema;
using UserManagment.Common.Enum;

namespace UserManagment.Domain.Models;

[Table("AccessLog")]
public partial class AccessLog : BaseModel
{
    public Guid UserId { get; set; }
    public AccessStatus AccessStatus { get; set; }

}

