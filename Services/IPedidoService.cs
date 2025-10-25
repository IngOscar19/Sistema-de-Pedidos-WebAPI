using MyApp.Namespace;

namespace Pedidos;

public interface IPedidoService
{
    void AddOrder(Order order);
    List<Order> GetOrders();
    Guid GetInstanceId();
    int GetOrdersCount();
}
