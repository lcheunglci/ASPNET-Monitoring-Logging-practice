using eCommerce.Docker.Api.ApiModels;
using eCommerce.Docker.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Docker.Api.Controllers
{
    public class QuickOrderController : ControllerBase
    {
        private readonly IQuickOrderLogic _quickOrderLogic;
        private readonly ILogger<QuickOrderController> _logger;

        public QuickOrderController(ILogger<QuickOrderController> logger, IQuickOrderLogic quickOrderLogic)
        {
            _quickOrderLogic = quickOrderLogic;
            _logger = logger;
        }

        [HttpPost]
        public Guid SubmitQuickOrder(QuickOrder order)
        {
            _logger.LogInformation($"Submitting order for {order.Quantity} of {order.ProductId}.");
            return _quickOrderLogic.PlaceQuickOrder(order, 1234); // ideally, get customer id from authentication system/User claim
        }

    }
}
