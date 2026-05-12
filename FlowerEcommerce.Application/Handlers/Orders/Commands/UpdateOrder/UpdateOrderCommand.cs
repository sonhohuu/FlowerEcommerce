using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;

namespace FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand : IRequest<TResult>
    {
        [SwaggerIgnore]
        public ulong Id { get; set; }
        public string? CustomerName { get; init; }
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Note { get; init; }
        public OrderStatus Status { get; init; }
        public PaymentMethod? PaymentMethod { get; init; }
        public List<OrderItemDto>? Items { get; init; }

        private class MappingConfig : IRegister
        {
            public void Register(TypeAdapterConfig config)
            {
                config.NewConfig<UpdateOrderCommand, Order>()
                    .IgnoreNullValues(true);
            }
        }
    }
}
