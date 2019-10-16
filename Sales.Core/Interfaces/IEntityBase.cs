namespace Sales.Core.Interfaces
{
    public interface IEntityBase : IOrder, IDeleted
    {
        int Id { get; set; }
    }
}
