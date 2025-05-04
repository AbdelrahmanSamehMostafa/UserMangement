using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagment.Domain.Models;

[Table("Group")]
public partial class Group : BaseModel
{

    [StringLength(50)]
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }
    public static SortExpression<Group> SortBy(string sorting)
    {
        // If sorting is null or empty, default to sorting by InsertedDate ascending
        if (string.IsNullOrEmpty(sorting))
        {
            sorting = "default";
        }

        switch (sorting.ToLower())
        {
            case "name_ascending":
                return new SortExpression<Group> { Expression = g => g.Name, IsDescending = false };

            case "name_descending":
                return new SortExpression<Group> { Expression = g => g.Name, IsDescending = true };

            case "default":
            default:
                return new SortExpression<Group> { Expression = g => g.InsertedDate, IsDescending = true };
        }
    }
}

