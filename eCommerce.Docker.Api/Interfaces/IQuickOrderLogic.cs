using eCommerce.Docker.Api.ApiModels;

namespace eCommerce.Docker.Api.Interfaces
{
    public interface IQuickOrderLogic
    {
        Guid PlaceQuickOrder(QuickOrder order, int customerId);
    }
}
