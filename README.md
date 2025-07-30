# wallet-api
# Wallet System API

Este es un sistema de API RESTful para gestionar billeteras y sus movimientos, desarrollado como parte de una prueba técnica. La aplicación sigue los principios de Clean Architecture y se enfoca en la gestión de transferencias de saldo.

Tecnologías Utilizadas

* **.NET 8:** Framework principal.
* **Clean Architecture:** Estructura modular.
* **Entity Framework Core:** ORM para la persistencia.
* **SQL Server:** Base de datos.
* **AutoMapper:** Mapeo de objetos.
* **FluentValidation:** Validación de modelos.
* **xUnit & Moq:** Para pruebas (en progreso).
* **Swagger/OpenAPI:** Documentación interactiva de la API.

Endpoints de la API

 Autenticación (`/api/auth`)

* `POST /api/auth/login`: Obtiene un token JWT.
    * **Body:** `{ "user": "admin", "password": "123456" }`
    * **Nota:** Este es un login simple para la prueba.

 Billeteras (`/api/wallets`)

* `POST /api/wallets`: Crea una nueva billetera.
    * **Body:** `{ "documentId": "string", "name": "string" }`
* `GET /api/wallets/{id}`: Obtiene una billetera por su ID.
    * **Requiere autenticación.**
* `PUT /api/wallets/{id}`: Actualiza el nombre de una billetera (el `dto.Name` en el body se mapea a `UpdateWalletNameDto`).
    * **Body:** `{ "name": "string" }`
    * **Requiere autenticación.**
* `DELETE /api/wallets/{id}`: Desactiva (eliminación lógica) una billetera.
    * **Requiere autenticación.**

Movimientos (`/api/movements`)

* `GET /api/wallets/{walletId}/movements`: Obtiene el historial de movimientos de una billetera.
* `POST /api/movements`: Crea un movimiento de depósito/retiro para una billetera.
    * **Body:** `{ "amount": 0, "type": "Credit" }` (o "Debit")
    * **Nota:** `RelatedWalletId` se gestiona internamente y no debe enviarse en este endpoint.
* `POST /api/movements/{fromWalletId}/transfer`: Realiza una transferencia entre dos billeteras.
    * **Body:** `{ "toWalletId": 0, "amount": 0 }`
    * **Requiere autenticación.**

Validaciones y Manejo de Errores

* Validaciones de entrada (ej., monto > 0, datos requeridos).
* Manejo de errores para saldos insuficientes, billeteras no encontradas, y otros escenarios con códigos de estado HTTP apropiados (400 Bad Request, 404 Not Found, etc.).

Pruebas

* **Pruebas Unitarias e Integración:** Implementadas para cubrir la lógica de negocio y los flujos principales.
* **Estado Actual:** Las pruebas están definidas, pero pueden presentar errores de compilación relacionados con la resolución de dependencias. La lógica de las pruebas y la cobertura de escenarios clave están presentes.

---
