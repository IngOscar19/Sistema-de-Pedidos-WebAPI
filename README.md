# 🛒 Sistema de Pedidos WebAPI (.NET 8)

Este proyecto implementa un **sistema de pedidos básico** que demuestra el **ciclo de vida de los servicios** (`Transient`, `Scoped` y `Singleton`) en una aplicación ASP.NET Core Web API.

---

##  Tecnologías utilizadas

* **.NET 8 Web API**
* **Swagger / OpenAPI**
* **Inyección de dependencias (DI)**
* **C#**
* **Postman** para pruebas

---

##  Estructura del proyecto

```
📦 Pedidos
 ┣ 📂 Controllers
 ┃ ┗ 📜 PedidoController.cs
 ┣ 📂 Services
 ┃ ┗ 📜 IPedidoService.cs
 ┃ ┗ 📜 PedidoService.cs
 ┣ 📜 Program.cs
 ┣ 📜 README.md
 ┗ 📜 Pedidos.csproj
```

---

## 🧠 Tipos de servicio implementados

| Tipo          | Método de registro  | Ciclo de vida                                | Descripción                                                     |
| ------------- | ------------------- | -------------------------------------------- | --------------------------------------------------------------- |
| **Transient** | `AddKeyedTransient` | Nueva instancia **cada vez** que se solicita | Ideal para objetos ligeros o sin estado.                        |
| **Scoped**    | `AddKeyedScoped`    | Una instancia **por solicitud HTTP**         | Ideal para manejar datos que cambian durante la misma petición. |
| **Singleton** | `AddKeyedSingleton` | Una instancia **para toda la aplicación**    | Ideal para servicios globales o cachés.                         |

En `Program.cs`:

```csharp
builder.Services.AddKeyedTransient<IPedidoService, PedidoService>("transient");
builder.Services.AddKeyedScoped<IPedidoService, PedidoService>("scoped");
builder.Services.AddKeyedSingleton<IPedidoService, PedidoService>("singleton");
```

---

## Cómo se implementó cada tipo

### **Transient**

Cada vez que se llama al servicio, se crea **una nueva instancia** con un nuevo `Guid`.

**Ejemplo:**
`/api/order/transient/info`

Devuelve dos instancias distintas:

```json
{
  "tipo": "TRANSIENT",
  "primeraInstancia": "a90d161a-1185-43c5-b9fb-ab280e5930dd",
  "segundaInstancia": "4cf00ea6-6e9f-4d04-916b-54a5adea8f1d",
  "sonIguales": false
}
```

** Uso ideal:**

* Servicios ligeros, sin estado ni dependencia compartida.
* Por ejemplo, validadores, helpers, o formateadores.

---

### 🔹 **Scoped**

Crea **una única instancia por solicitud HTTP**, compartida dentro del mismo request.

**Ejemplo:**
`/api/order/scoped/info`

Devuelve dos instancias **iguales** (mismo `Guid`), porque pertenecen a la misma petición:

```json
{
  "tipo": "SCOPED",
  "primeraInstancia": "bbb43855-38c3-4601-b246-60c210c87516",
  "segundaInstancia": "bbb43855-38c3-4601-b246-60c210c87516",
  "sonIguales": true
}
```

** Uso ideal:**

* Servicios que gestionan datos del usuario autenticado.
* Servicios de negocio que manejan transacciones por petición.

---

###  **Singleton**

Crea **una sola instancia para toda la aplicación**.

**Ejemplo:**
`/api/order/singleton/info`

Mismo `Guid` en todas las peticiones:

```json
{
  "tipo": "SINGLETON",
  "primeraInstancia": "c7d7531e-7ada-4e7d-ac1f-db5d0960cd31",
  "segundaInstancia": "c7d7531e-7ada-4e7d-ac1f-db5d0960cd31",
  "sonIguales": true
}
```

** Uso ideal:**

* Servicios que almacenan datos en memoria (como cachés o logs).
* Clientes de API o configuraciones globales que no cambian.

---

##  Resultados de las pruebas

Ejemplo de respuesta del endpoint `/api/order/compare`:

```json
{
  "transient": {
    "instancia1": "a90d161a-1185-43c5-b9fb-ab280e5930dd",
    "instancia2": "4cf00ea6-6e9f-4d04-916b-54a5adea8f1d",
    "pedidos1": 0,
    "pedidos2": 0
  },
  "scoped": {
    "instancia1": "bbb43855-38c3-4601-b246-60c210c87516",
    "instancia2": "bbb43855-38c3-4601-b246-60c210c87516",
    "pedidos1": 0,
    "pedidos2": 0
  },
  "singleton": {
    "instancia1": "c7d7531e-7ada-4e7d-ac1f-db5d0960cd31",
    "instancia2": "c7d7531e-7ada-4e7d-ac1f-db5d0960cd31",
    "pedidos1": 3,
    "pedidos2": 3
  }
}
```

**Interpretación:**

* **Transient:** cada instancia tiene un ID distinto.
* **Scoped:** mismo ID dentro de una petición.
* **Singleton:** mismo ID entre todas las peticiones.

---

## 🧭 Diagramas visuales

###  Transient

```
Request 1 ──> [Instance #1]
Request 2 ──> [Instance #2]
Request 3 ──> [Instance #3]
```

Nueva instancia **cada vez** que se inyecta el servicio.

---

###  Scoped

```
Request 1 ──> [Instance A]
Request 2 ──> [Instance B]
Request 3 ──> [Instance C]
```

Misma instancia dentro de un request, **diferente entre requests**.

---

###  Singleton

```
Request 1 ──┐
Request 2 ──┤──> [Instancia Global]
Request 3 ──┘
```

Una única instancia compartida en toda la aplicación.

---

## Conclusión

| Tipo          | Reutilización     | Ejemplo de uso                                |
| ------------- | ----------------- | --------------------------------------------- |
| **Transient** | Nueva cada vez    | Validadores, conversores, formateadores       |
| **Scoped**    | Por solicitud     | Repositorios, servicios de dominio, DbContext |
| **Singleton** | Global            | Logs, cachés, configuración global            |

---

## Ejecución y pruebas

1. Ejecutar el proyecto:

   ```bash
   dotnet run
   ```
2. Abrir en navegador o Postman:

   * `GET http://localhost:5238/api/order/transient/info`
   * `GET http://localhost:5238/api/order/scoped/info`
   * `GET http://localhost:5238/api/order/singleton/info`
   * `GET http://localhost:5238/api/order/compare`
3. Observar cómo cambian los `InstanceId` según el tipo de servicio.

---

**Autor:** Oscar Espinosa Romero
**Fecha:** Octubre 2025
Proyecto académico — Ciclo de vida de servicios en ASP.NET Core
