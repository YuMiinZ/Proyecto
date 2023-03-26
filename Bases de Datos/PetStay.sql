#Elaborado por Aaron Retana y Ericka Yu Min
#Proyecto PetStay - Diseño de Software

#Creación y utilización de la base de datos
create database PetStay;
use PetStay;

#Creación de tablas

create table TipoUsuario(
	idTipoUsuario int primary key not null auto_increment,
    nombre varchar(45) not null
);

create table TipoAnuncio(
	idTipoAnuncio int primary key not null auto_increment,
    nombre varchar(45) not null
);

create table TipoServicio(
	idTipoServicio int primary key not null auto_increment,
    nombre varchar(45) not null
);

create table Usuario(
	idUsuario int primary key not null auto_increment,
    nombre varchar(45) not null,
    apellido varchar(45) not null,
    correo varchar(100) not null,
    contrasenia varchar(45) not null,
    idTipoUsuario int not null,
    foreign key (idTipoUsuario) references TipoUsuario (idTipoUsuario)
);

create table Solicitud(
	idSolicitud int primary key not null auto_increment,
    nombre varchar(45) not null,
    cedula int not null,
    provincia varchar(45) not null,
    canton varchar(45) not null,
    distrito varchar(45) not null,
    telefonoPrincipal varchar(45) not null,
    telefonoSecundario varchar(45) not null,
    imgDocIdentificacion blob not null,
    idTipoServicio int not null,
    foreign key (idTipoServicio) references TipoServicio(idTipoServicio)
);

create table Anuncio(
	idAnuncio int primary key not null auto_increment,
    titulo varchar(45) not null,
    descripcion varchar(200) not null,
    medioContacto varchar(100) not null,
    imgAnuncio blob not null,
    idTipoAnuncio int not null,
    idUsuario int not null,
    foreign key (idTipoAnuncio) references TipoAnuncio(idTipoAnuncio),
    foreign key (idUsuario) references Usuario(idUsuario)
);

create table Comentario(
	idComentario int not null primary key auto_increment,
    texto varchar(200) not null,
    fecha date not null,
    idUsuario int not null,
    idAnuncio int not null,
    foreign key (idAnuncio) references Anuncio(idAnuncio),
    foreign key (idUsuario) references Usuario(idUsuario)
);

#CRUDS

