# Sistema de Registro de Libros

## Estudiante
Dionicio Hernandez Pujols

## Matricula
1000-5331

## Objetivo
Desarrollar una aplicacion para gestionar el registro de un catalogo de libros.

## Requerimientos Implementados
El sistema almacena la informacion en una base de datos cuya tabla principal se denomina `Libros`. Dicha tabla contiene los siguientes campos correspondientes al modelo de datos:
- `LibroId`
- `Titulo`
- `Autor`
- `AnoPublicacion`

## Validaciones
Se implementaron las siguientes reglas de negocio en el formulario de registro:
- Todos los campos son obligatorios.
- Unicidad de registros: No se permite el registro de dos libros que compartan el mismo `Titulo`. El sistema intercepta esta accion y muestra un mensaje de error al usuario indicando el intento de duplicidad.

## Tecnologia Utilizada
- .NET Blazor
- C#
- Entity Framework Core (Gestión de Contexto y Migraciones)

## Estructura del Proyecto

La solucion esta estructurada siguiendo un patron limpio para aplicaciones Blazor. Segun la organizacion del repositorio, los archivos se distribuyen de la siguiente forma:

```text
RegistroLibro
│
├── Components/
│
├── Context/
│   └── Contexto.cs
│
├── Data/
│
├── Extensors/
│
├── Migrations/
│   ├── 20260912215442_Inicial.cs
│   └── ContextoModelSnapshot.cs
│
├── Models/
│   └── Libros.cs
│
├── Services/
│   └── GestionLibros.cs
│
├── appsettings.json
├── Program.cs
└── README.md
