CREATE DATABASE Infobar;

USE Infobar;

CREATE TABLE TipoUsuario(
	Id Integer IDENTITY PRIMARY KEY,
	Descripcion VARCHAR(10)
	)
GO

CREATE TABLE Usuario(
	Id INTEGER IDENTITY PRIMARY KEY,
	Id_Tipo INTEGER,
	Nombre VARCHAR(30),
	Clave VARCHAR(20),
	FOREIGN KEY (Id_Tipo) REFERENCES TipoUsuario (Id)
	)
GO

CREATE TABLE TipoPago(
	Id_TipoPago INTEGER IDENTITY PRIMARY KEY,
	Descripcion VARCHAR(20)
	)
GO


CREATE TABLE TipoProducto(
	Id_TipoProd INTEGER PRIMARY KEY,
	Descripcion VARCHAR(15)
	)
GO

CREATE TABLE Producto(
	Id_Producto INTEGER PRIMARY KEY,
	Id_TipoProd INTEGER,
	Descripcion VARCHAR(60),
	Precio DEC(8,2),
	Imagen VARCHAR(120),
	FOREIGN KEY (Id_TipoProd) REFERENCES TipoProducto(Id_TipoProd)
	)
GO

CREATE TABLE Pedido(
	Id_Pedido INTEGER IDENTITY PRIMARY KEY,
	Id_TipoPago INTEGER,
	Id_Usuario INTEGER,
	Mesa INTEGER,
	Fecha DATE,
	Importe_Total DEC(8,2),
	FOREIGN KEY (Id_TipoPago) REFERENCES TipoPago(Id_TipoPago),
	FOREIGN KEY (Id_Usuario) REFERENCES Usuario(Id)
	)
GO

CREATE TABLE Detalle_Pedido(
	Id_Pedido INTEGER,
	Id_Prod INTEGER,
	Cantidad INTEGER,
	Precio DEC(8,2),
	PrecioTotal DEC(8,2),
	CONSTRAINT IdDetalle PRIMARY KEY (Id_Pedido, Id_Prod),
	FOREIGN KEY (Id_Pedido) REFERENCES Pedido(Id_Pedido),
	FOREIGN KEY (Id_Prod) REFERENCES Producto(Id_Producto),
	)
GO

insert into TipoProducto values (1, 'Comidas');
insert into TipoProducto values (2, 'Bebidas');