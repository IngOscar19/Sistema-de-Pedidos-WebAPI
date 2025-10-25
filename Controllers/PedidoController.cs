using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using MyApp.Namespace;
using Pedidos;

namespace MyApp.Namespace
{
    public class Order
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public DateTime fecha_pedido { get; set; } = DateTime.Now;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Dictionary<string, (IPedidoService s1, IPedidoService s2)> _services;

        public OrderController(
            [FromKeyedServices("transient")] IPedidoService transient1,
            [FromKeyedServices("transient")] IPedidoService transient2,
            [FromKeyedServices("scoped")] IPedidoService scoped1,
            [FromKeyedServices("scoped")] IPedidoService scoped2,
            [FromKeyedServices("singleton")] IPedidoService singleton1,
            [FromKeyedServices("singleton")] IPedidoService singleton2)
        {
            _services = new()
            {
                ["transient"] = (transient1, transient2),
                ["scoped"] = (scoped1, scoped2),
                ["singleton"] = (singleton1, singleton2)
            };
        }

        // ---------- MÉTODOS AUXILIARES ----------
        private object GetInfo(string tipo, string descripcion)
        {
            var (s1, s2) = _services[tipo];
            return new
            {
                Tipo = tipo.ToUpper(),
                Descripcion = descripcion,
                PrimeraInstancia = s1.GetInstanceId(),
                SegundaInstancia = s2.GetInstanceId(),
                SonIguales = s1.GetInstanceId() == s2.GetInstanceId()
            };
        }

        private object GetOrders(string tipo)
        {
            var s1 = _services[tipo].s1;
            return new
            {
                InstanceId = s1.GetInstanceId(),
                OrdersCount = s1.GetOrdersCount(),
                Orders = s1.GetOrders()
            };
        }

        private object AddOrder(string tipo, Order order)
        {
            var s1 = _services[tipo].s1;
            s1.AddOrder(order);
            return new
            {
                Message = $"Pedido agregado ({tipo.ToUpper()})",
                InstanceId = s1.GetInstanceId(),
                TotalOrders = s1.GetOrdersCount()
            };
        }

        // ---------- ENDPOINTS ----------
        [HttpGet("{tipo}/info")]
        public IActionResult GetInfoByType(string tipo)
        {
            var descripcion = tipo switch
            {
                "transient" => "Se crea una nueva instancia cada vez que se solicita",
                "scoped" => "Se crea una instancia por cada solicitud HTTP",
                "singleton" => "Se crea una única instancia para toda la aplicación",
                _ => "Tipo no reconocido"
            };

            if (!_services.ContainsKey(tipo)) return BadRequest("Tipo no válido");
            return Ok(GetInfo(tipo, descripcion));
        }

        [HttpGet("{tipo}/orders")]
        public IActionResult GetOrdersByType(string tipo)
        {
            if (!_services.ContainsKey(tipo)) return BadRequest("Tipo no válido");
            return Ok(GetOrders(tipo));
        }

        [HttpPost("{tipo}/orders")]
        public IActionResult AddOrderByType(string tipo, [FromBody] Order order)
        {
            if (!_services.ContainsKey(tipo)) return BadRequest("Tipo no válido");
            return Ok(AddOrder(tipo, order));
        }

        [HttpGet("compare")]
        public IActionResult CompareAll()
        {
            object BuildCompare(string tipo)
            {
                var (s1, s2) = _services[tipo];
                return new
                {
                    Instancia1 = s1.GetInstanceId(),
                    Instancia2 = s2.GetInstanceId(),
                    Pedidos1 = s1.GetOrdersCount(),
                    Pedidos2 = s2.GetOrdersCount()
                };
            }

            return Ok(new
            {
                Transient = BuildCompare("transient"),
                Scoped = BuildCompare("scoped"),
                Singleton = BuildCompare("singleton")
            });
        }
    }
}
