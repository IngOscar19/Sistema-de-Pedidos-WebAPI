using MyApp.Namespace;
using Pedidos;

namespace MiApi;


public class PedidoService : IPedidoService
{
    private readonly Guid _instanceId;
    private readonly List<Order> _orders;

    public PedidoService()
    {
        _instanceId = Guid.NewGuid();
        _orders = new List<Order>();
        Console.WriteLine($"[OrderService] Nueva instancia creada: {_instanceId}");
    }

    public void AddOrder(Order order)
    {
        _orders.Add(order);
        Console.WriteLine($"[{_instanceId}] Pedido agregado. Total: {_orders.Count}");
    }

    public List<Order> GetOrders()
    {
        return _orders;
    }

    public Guid GetInstanceId()
    {
        return _instanceId;
    }

    public int GetOrdersCount()
    {
        return _orders.Count;
    }
}