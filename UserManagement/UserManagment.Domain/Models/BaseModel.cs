using System.Linq.Expressions;

namespace UserManagment.Domain.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? InsertedBy { get; set; }
        public DateTime? InsertedDate { get; set; } = DateTime.Now;
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public class SortExpression<T>
        {
            public Expression<Func<T, object>> Expression { get; set; }
            public bool IsDescending { get; set; }
        }
    }

}
