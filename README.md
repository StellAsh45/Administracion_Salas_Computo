# Administracion_Salas_Computo

Aplicación web en ASP.NET Core MVC para la administración de las salas de sistemas de la universidad USC, permitiendo el control de salas, equipos y usuarios según sus roles, con un flujo de solicitudes, reportes de daños y gestión del estado de ocupación

## Objetivo General

Desarrollar una aplicación web en ASP.NET Core MVC para la administración de las salas de sistemas de la Universidad Santiago de Cali que permita el control de salas, equipos y usuarios según sus roles, con un flujo de solicitudes, reportes de daños y gestión del estado de ocupación.

## Objetivos Específicos

-	Desarrollar una aplicación web en ASP.NET Core MVC que permita la administración integral de las salas de sistemas, incluyendo la gestión de salas, equipos y usuarios.
-	Implementar un sistema de autenticación y roles que garantice el control de acceso y las funciones específicas para administradores, coordinadores y usuarios.
-	Diseñar y gestionar el flujo de solicitudes de préstamo y reportes de daño, permitiendo su aprobación o atención según el rol asignado y manteniendo registro de cada proceso.

## Características

- Gestión de salas: número, capacidad y estado.
- Gestión de equipos: registro, edición, asignación y estados (Disponible, Ocupado, Bloqueado, Mantenimiento).
- Gestión de usuarios y roles: Usuario, Coordinador de Sala, Administrador.
- Flujo de solicitudes: solicitar uso de un equipo y proceso de aceptación/denegación.
- Reportes simples y vistas de ocupación (diaria y semanal).

## Requisitos

- .NET9 SDK instalado.
- Visual Studio o VS Code con extensiones de C# (opcional).

## Configuración local

- Clonar el repositorio:
- Configurar la cadena de conexión en `appsettings.json` dentro del proyecto `Web/MvcSample/`:
`ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=...;"`

## Estructura del repositorio

- `Domain/` — Entidades del dominio.
- `Infrastructure/` — Repositorios y acceso a datos.
- `Services/` — Lógica de negocio.
- `Web/MvcSample/` — Proyecto web (controladores y vistas).
- `Test/ServicesTest/` — Pruebas unitarias (xUnit + Moq).

## Migraciones y base de datos

- Usar Entity Framework Core para migraciones y gestión de la base de datos.
- Usar el administrador de paquetes nuget y seleccionar en el proyecto predeterminado `Infrastructure`.
- Crear una nueva migración con el siguiente comando:
  `add-migration <NombreMigracion>`
- Aplicar las migraciones a la base de datos con el siguiente comando:
  `update-database`

## Ejecutar la aplicación

- Restaurar y compilar:

 `dotnet restore`
 `dotnet build`

- Ejecutar el proyecto web:

 `dotnet run --project Web/MvcSample/MvcSample.csproj`

## Autenticación y roles

- El sistema utiliza roles: Usuario, Coordinador de Sala, Administrador.
- Si no hay usuarios pre-registrados, registra uno y asigna rol vía interfaz de administración o implementa un seeder.
- Asegúrate de configurar correctamente la autenticación en `Program.cs`.

## Pruebas unitarias

- Ejecutar todas las pruebas del proyecto de tests:

 `dotnet test Test/ServicesTest/ServicesTest.csproj`

