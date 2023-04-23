#Elaborado por Aaron Retana y Ericka Yu Min
#Proyecto PetStay - Diseño de Software

#Creación y utilización de la base de datos
create database PetStay;
use PetStay;

#Creación de tablas

create table Estado(
	idEstado int primary key not null auto_increment,
	nombre varchar(50) not null
);

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
    imgDocIdentificacion longblob not null,
    idTipoServicio int not null,
    idEstado int not null,
    foreign key (idEstado) references Estado (idEstado),
    foreign key (idTipoServicio) references TipoServicio(idTipoServicio)
);

create table Anuncio(
	idAnuncio int primary key not null auto_increment,
    titulo varchar(45) not null,
    descripcion varchar(200) not null,
    medioContacto varchar(100) not null,
    imgAnuncio longblob not null,
    idTipoAnuncio int not null,
    idUsuario int not null,
    idEstado int not null,
    foreign key (idEstado) references Estado (idEstado),
    foreign key (idTipoAnuncio) references TipoAnuncio(idTipoAnuncio),
    foreign key (idUsuario) references Usuario(idUsuario)
);

create table Comentario(
	idComentario int not null primary key auto_increment,
    texto varchar(200) not null,
    fecha date not null,
    idUsuario int not null,
    idAnuncio int not null,
    idEstado int not null,
    foreign key (idEstado) references Estado (idEstado),
    foreign key (idAnuncio) references Anuncio(idAnuncio),
    foreign key (idUsuario) references Usuario(idUsuario)
);

#CRUDS

#1. Estado
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudEstado(in opcion int, in idEstado int, in nombre varchar(50))
begin
	if(opcion is null) then
		signal sqlstate '45000' set message_text = 'Debe de ingresar una opcion.';
    elseif(nombre is null or nombre='') then
		signal sqlstate '45000' set message_text = 'El nombre no puede estar vacio.';
	elseif (opcion=1) then
		if(select count(*) from Estado where Estado.nombre=nombre)>0 then
			signal sqlstate '45000' set message_text = 'El nombre del estado ingresada ya existe en la BD';
		else 
			start transaction;
			insert into Estado (nombre) values (nombre);
			commit;
		end if;
	elseif(opcion=2) then
		if(idEstado is null) then
			signal sqlstate '45000' set message_text = 'Debe de ingresar el id del estado.';
        elseif (select count(*) from Estado where Estado.idEstado=idEstado)=0 then
			signal sqlstate '45000' set message_text = 'No se encontro el id del estado.';
		else
			start transaction;
			update Estado set nombre=nombre where Estado.idEstado=idEstado;
            commit;
        end if;
	elseif(opcion=3) then
		start transaction;
		select * from Estado;
        commit;
	else 
		signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
CALL crudEstado(1, null, 'Activo');
CALL crudEstado(1, null, 'Rechazado');
CALL crudEstado(1, null, 'Inactivo');

#2. TipoUsuario
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudTipoUsuario(in opcion int, in idTipoUsuario int, in nombre varchar(45))
begin
    if (opcion is null) then
        signal sqlstate '45000' set message_text = 'Debe de ingresar una opcion.';
    elseif (nombre is null or nombre='') then
        signal sqlstate '45000' set message_text = 'El nombre no puede estar vacio.';
    elseif (length(nombre) > 45) then
        signal sqlstate '45000' set message_text = 'El nombre no puede tener más de 45 caracteres.';
    elseif (opcion = 1) then
		if(select count(*) from TipoUsuario where TipoUsuario.nombre=nombre)>0 then
			signal sqlstate '45000' set message_text = 'El nombre del tipo de usuario ingresada ya existe en la BD';
		else 
			start transaction;
			insert into TipoUsuario (nombre) values (nombre);
			commit;
		end if;
    elseif (opcion = 2) then
        if (idTipoUsuario is null) then
            signal sqlstate '45000' set message_text = 'Debe de ingresar el id del tipo de usuario.';
        elseif (select count(*) from TipoUsuario where TipoUsuario.idTipoUsuario = idTipoUsuario) = 0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del tipo de usuario.';
        else
            start transaction;
            update TipoUsuario set nombre = nombre where TipoUsuario.idTipoUsuario = idTipoUsuario;
            commit;
        end if;
    elseif (opcion = 3) then
        start transaction;
        select * from TipoUsuario;
        commit;
    else
        signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
call crudTipoUsuario(1, null, 'Administrador');
call crudTipoUsuario(1, null, 'Usuario');


#3. TipoAnuncio
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudTipoAnuncio(in opcion int, in idTipoAnuncio int, in nombre varchar(50))
begin
	if(opcion is null) then
		signal sqlstate '45000' set message_text = 'Debe ingresar una opción.';
    elseif(nombre is null or nombre='') then
		signal sqlstate '45000' set message_text = 'El nombre no puede estar vacío.';
	elseif (opcion=1) then
		if(select count(*) from TipoAnuncio where TipoAnuncio.nombre=nombre)>0 then
			signal sqlstate '45000' set message_text = 'El nombre del tipo de anuncio ingresada ya existe en la BD';
		else
			start transaction;
			insert into TipoAnuncio (nombre) values (nombre);
			commit;
		end if;
	elseif(opcion=2) then
		if(idTipoAnuncio is null) then
			signal sqlstate '45000' set message_text = 'Debe ingresar el ID del tipo de anuncio.';
        elseif (select count(*) from TipoAnuncio where TipoAnuncio.idTipoAnuncio = idTipoAnuncio) = 0 then
			signal sqlstate '45000' set message_text = 'No se encontró el ID del tipo de anuncio.';
		else
			start transaction;
			update TipoAnuncio set nombre = nombre where TipoAnuncio.idTipoAnuncio = idTipoAnuncio;
            commit;
        end if;
	elseif(opcion=3) then
		start transaction;
		select * from TipoAnuncio;
        commit;
	else 
		signal sqlstate '45000' set message_text = 'Opción inválida.';
    end if;
end//
CALL crudTipoAnuncio(1, null, 'Adopción');
CALL crudTipoAnuncio(1, null, 'Servicios');

#4. TipoServicio
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudTipoServicio(in opcion int, in idTipoServicio int, in nombre varchar(50))
begin
	if(opcion is null) then
		signal sqlstate '45000' set message_text = 'Debe ingresar una opcion.';
	elseif(nombre is null or nombre='') then
		signal sqlstate '45000' set message_text = 'El nombre no puede estar vacio.';
	elseif(opcion=1) then
		if(select count(*) from TipoServicio where TipoServicio.nombre=nombre)>0 then
			signal sqlstate '45000' set message_text = 'El nombre del tipo de servicio ingresada ya existe en la BD';
		else
			start transaction;
			insert into TipoServicio (nombre) values (nombre);
			commit;
		end if;
	elseif(opcion=2) then
		if(idTipoServicio is null) then
			signal sqlstate '45000' set message_text = 'Debe ingresar el id del tipo de servicio.';
		elseif(select count(*) from TipoServicio where TipoServicio.idTipoServicio=idTipoServicio)=0 then
			signal sqlstate '45000' set message_text = 'No se encontro el id del tipo de servicio.';
		else
			start transaction;
			update TipoServicio set nombre=nombre where TipoServicio.idTipoServicio=idTipoServicio;
			commit;
		end if;
	elseif(opcion=3) then
		start transaction;
		select * from TipoServicio;
		commit;
	else
		signal sqlstate '45000' set message_text = 'Opcion invalida.';
	end if;
end//
CALL crudTipoServicio(1, null, 'Cuidado de Mascotas');
CALL crudTipoServicio(1, null, 'Hospedaje');

#5. Usuario
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudUsuario(in opcion int, in idUsuario int, in nombre varchar(45), in apellido varchar(45), 
in correo varchar(100), in contrasenia varchar(45), in idTipoUsuario int)
begin
    if(opcion is null) then
        signal sqlstate '45000' set message_text = 'Debe ingresar una opcion.';
    elseif(nombre is null or nombre='') then
        signal sqlstate '45000' set message_text = 'El nombre no puede estar vacio.';
    elseif(apellido is null or apellido='') then
        signal sqlstate '45000' set message_text = 'El apellido no puede estar vacio.';
    elseif(correo is null or correo='') then
        signal sqlstate '45000' set message_text = 'El correo no puede estar vacio.';
    elseif(contrasenia is null or contrasenia='') then
        signal sqlstate '45000' set message_text = 'La contrasenia no puede estar vacia.';
    elseif(idTipoUsuario is null) then
        signal sqlstate '45000' set message_text = 'El id del tipo de usuario no puede estar vacio.';
	elseif(select count(*) from TipoServicio where TipoServicio.idTipoServicio=idTipoServicio)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del tipo de servicio.';
    elseif(opcion=1) then
        if(select count(*) from Usuario where Usuario.correo=correo)>0 then
			signal sqlstate '45000' set message_text = 'El correo ingresado ya existe en la BD';
		else
			start transaction;
			insert into Usuario (nombre, apellido, correo, contrasenia, idTipoUsuario) values 
            (nombre, apellido, correo, MD5(contrasenia), idTipoUsuario);
			commit;
		end if;
    elseif(opcion=2) then
        if(idUsuario is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id del usuario.';
        elseif(select count(*) from Usuario where Usuario.idUsuario=idUsuario)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del usuario.';
        else
            start transaction;
            update Usuario set nombre=nombre, apellido=apellido, correo=correo, contrasenia=MD5(contrasenia), 
            idTipoUsuario=idTipoUsuario where Usuario.idUsuario=idUsuario;
            commit;
        end if;
    elseif(opcion=3) then
        start transaction;
        select * from Usuario;
        commit;
    else
        signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
CALL crudUsuario(1, null, 'Juan', 'Perez', 'juan.perez@gmail.com', 'admin', 1);
CALL crudUsuario(1, null, 'Melany', 'Salas', 'melany.salas@gmail.com', 'user', 2);
CALL crudUsuario(1, null, 'Admin', 'Administrador', 'admin', 'admin', 1);

#6. Solicitud
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
delimiter //
create procedure crudSolicitud(in opcion int, in idSolicitud int, in nombre varchar(45), in cedula int, in provincia varchar(45), 
in canton varchar(45), in distrito varchar(45), in telefonoPrincipal varchar(45), in telefonoSecundario varchar(45), 
in imgDocIdentificacion blob, in idTipoServicio int, in idEstado int)
begin
    if(opcion is null) then
        signal sqlstate '45000' set message_text = 'Debe ingresar una opcion.';
    elseif(nombre is null or nombre='') then
        signal sqlstate '45000' set message_text = 'El nombre no puede estar vacio.';
    elseif(cedula is null) then
        signal sqlstate '45000' set message_text = 'La cedula no puede estar vacia.';
    elseif(provincia is null or provincia='') then
        signal sqlstate '45000' set message_text = 'La provincia no puede estar vacia.';
    elseif(canton is null or canton='') then
        signal sqlstate '45000' set message_text = 'El canton no puede estar vacio.';
    elseif(distrito is null or distrito='') then
        signal sqlstate '45000' set message_text = 'El distrito no puede estar vacio.';
    elseif(telefonoPrincipal is null or telefonoPrincipal='') then
        signal sqlstate '45000' set message_text = 'El telefono principal no puede estar vacio.';
    elseif(telefonoSecundario is null or telefonoSecundario='') then
        signal sqlstate '45000' set message_text = 'El telefono secundario no puede estar vacio.';
    elseif(imgDocIdentificacion is null) then
        signal sqlstate '45000' set message_text = 'La imagen de documento de identificacion no puede estar vacia.';
    elseif(idTipoServicio is null) then
        signal sqlstate '45000' set message_text = 'El id del tipo de servicio no puede estar vacio.';
    elseif(select count(*) from TipoServicio where TipoServicio.idTipoServicio=idTipoServicio)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del tipo de servicio.';
	elseif(idEstado is null) then
		signal sqlstate '45000' set message_text = 'El id del estado no puede ser nulo.';
	elseif(select count(*) from Estado where Estado.idEstado=idEstado)=0 then
		signal sqlstate '45000' set message_text = 'El id del estado no existe.';
    elseif(opcion=1) then
		start transaction;
		insert into Solicitud (nombre, cedula, provincia, canton, distrito, telefonoPrincipal, telefonoSecundario, 
        imgDocIdentificacion, idTipoServicio, idEstado) values (nombre, cedula, provincia, canton, distrito, telefonoPrincipal, 
        telefonoSecundario, imgDocIdentificacion, idTipoServicio, idEstado);
		commit;
    elseif(opcion=2) then
        if(idSolicitud is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id de la solicitud.';
        elseif(select count(*) from Solicitud where Solicitud.idSolicitud=idSolicitud)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id de la solicitud.';
        else
            start transaction;
            update Solicitud set nombre=nombre, cedula=cedula, provincia=provincia, canton=canton, distrito=distrito, 
            telefonoPrincipal=telefonoPrincipal, telefonoSecundario=telefonoSecundario, imgDocIdentificacion=imgDocIdentificacion, 
            idTipoServicio=idTipoServicio, idEstado=idEstado where Solicitud.idSolicitud=idSolicitud;
            commit;
        end if;
    elseif(opcion=3) then
        start transaction;
        select * from Solicitud;
        commit;
    else
        signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
CALL crudSolicitud(1, NULL, 'Juan Perez', 123456789, 'San Jose', 'Escazu', 'San Rafael', '8888-8888', '7777-7777', NULL, 1,1);
select @@secure_file_priv

set @img = LOAD_FILE('C:/Users/yumii/OneDrive/Escritorio/TEC/I Semestre/Diseño de Software/Proyecto/Visual Studio - Project/PetStay/wwwroot/images/puppy.png');
set @img = load_file('C:/ProgramData/MySQL/MySQL Server 8.0/Uploads/prueba 2.jpg');
select @img;

CALL crudSolicitud(1, null, 'Juan Perez', 123456789, 'San Jose', 'Escazu', 'San Rafael', '8888-8888', '8888-8889', 
LOAD_FILE('C:/ProgramData/MySQL/MySQL Server 8.0/Uploads/prueba.png'), 1, 1);

#7. Anuncio
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
#Opcion 4: Eliminar
delimiter //
create procedure crudAnuncio(
    in opcion int, in idAnuncio int, in titulo varchar(45), in descripcion varchar(200), in medioContacto varchar(100),
    in imgAnuncio blob, in idTipoAnuncio int, in idUsuario int, in idEstado int
)
begin
    if(opcion is null) then
        signal sqlstate '45000' set message_text = 'Debe ingresar una opcion.';
    elseif(titulo is null or titulo='') then
        signal sqlstate '45000' set message_text = 'El titulo no puede estar vacio.';
    elseif(descripcion is null or descripcion='') then
        signal sqlstate '45000' set message_text = 'La descripcion no puede estar vacia.';
    elseif(medioContacto is null or medioContacto='') then
        signal sqlstate '45000' set message_text = 'El medio de contacto no puede estar vacio.';
    elseif(imgAnuncio is null) then
        signal sqlstate '45000' set message_text = 'La imagen del anuncio no puede estar vacia.';
    elseif(idTipoAnuncio is null) then
        signal sqlstate '45000' set message_text = 'El id del tipo de anuncio no puede estar vacio.';
    elseif(select count(*) from TipoAnuncio where TipoAnuncio.idTipoAnuncio=idTipoAnuncio)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del tipo de anuncio.';
    elseif(idUsuario is null) then
        signal sqlstate '45000' set message_text = 'El id del usuario no puede estar vacio.';
    elseif(select count(*) from Usuario where Usuario.idUsuario=idUsuario)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del usuario.';
	elseif(idEstado is null) then
		signal sqlstate '45000' set message_text = 'El id del estado no puede ser nulo.';
	elseif(select count(*) from Estado where Estado.idEstado=idEstado)=0 then
		signal sqlstate '45000' set message_text = 'El id del estado no existe.';
    elseif(opcion=1) then
        start transaction;
        insert into Anuncio (titulo, descripcion, medioContacto, imgAnuncio, idTipoAnuncio, idUsuario, idEstado) values 
        (titulo, descripcion, medioContacto, imgAnuncio, idTipoAnuncio, idUsuario, idEstado);
        commit;
    elseif(opcion=2) then
        if(idAnuncio is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id del anuncio.';
        elseif(select count(*) from Anuncio where Anuncio.idAnuncio=idAnuncio)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del anuncio.';
        else
            start transaction;
            update Anuncio set titulo=titulo, descripcion=descripcion, medioContacto=medioContacto, 
            imgAnuncio=imgAnuncio, idTipoAnuncio=idTipoAnuncio, idUsuario=idUsuario, idEstado=idEstado
            where Anuncio.idAnuncio=idAnuncio;
            commit;
        end if;
    elseif(opcion=3) then
        start transaction;
        select * from Anuncio;
        commit;
    elseif(opcion=4) then
        if(idAnuncio is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id del anuncio.';
        elseif(select count(*) from Anuncio where Anuncio.idAnuncio=idAnuncio)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del anuncio.';
        else
            start transaction;
            update Anuncio set idEstado=idEstado where Anuncio.idAnuncio=idAnuncio;
            commit;
        end if;
    else
        signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
CALL crudAnuncio(1, 1, 'Perrito en Adopción', 'Perro raza pequeña, desparacitado', '(+506) 7894 6543', NULL, 1, 1,1);

#8. Comentario
#Opcion 1: Insertar
#Opcion 2: Actualizar todo
#Opcion 3: Mostrar todo
#Opcion 4: Eliminar
delimiter //
create procedure crudComentario(in opcion int, in idComentario int, in texto varchar(200), in fecha date, 
in idUsuario int, in idAnuncio int, in idEstado int)
begin
    if(opcion is null) then
        signal sqlstate '45000' set message_text = 'Debe ingresar una opcion.';
    elseif(texto is null or texto='') then
        signal sqlstate '45000' set message_text = 'El texto no puede estar vacio.';
    elseif(fecha is null) then
        signal sqlstate '45000' set message_text = 'La fecha no puede estar vacia.';
    elseif(idUsuario is null) then
        signal sqlstate '45000' set message_text = 'El id del usuario no puede estar vacio.';
    elseif(idAnuncio is null) then
        signal sqlstate '45000' set message_text = 'El id del anuncio no puede estar vacio.';
	elseif(select count(*) from Anuncio where Anuncio.idAnuncio=idAnuncio)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del anuncio.';
    elseif(select count(*) from Usuario where Usuario.idUsuario=idUsuario)=0 then
        signal sqlstate '45000' set message_text = 'No se encontro el id del usuario.';
	elseif(idEstado is null) then
		signal sqlstate '45000' set message_text = 'El id del estado no puede ser nulo.';
	elseif(select count(*) from Estado where Estado.idEstado=idEstado)=0 then
		signal sqlstate '45000' set message_text = 'El id del estado no existe.';
    elseif(opcion=1) then
        start transaction;
        insert into Comentario (texto, fecha, idUsuario, idAnuncio, idEstado) values 
        (texto, fecha, idUsuario, idAnuncio, idEstado);
        commit;
    elseif(opcion=2) then
        if(idComentario is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id del comentario.';
        elseif(select count(*) from Comentario where Comentario.idComentario=idComentario)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del comentario.';
        else
            start transaction;
            update Comentario set texto=texto, fecha=fecha, idUsuario=idUsuario, 
            idAnuncio=idAnuncio, idEstado=idEstado where idComentario=idComentario;
            commit;
        end if;
    elseif(opcion=3) then
        start transaction;
        select * from Comentario;
        commit;
    elseif(opcion=4) then
        if(idComentario is null) then
            signal sqlstate '45000' set message_text = 'Debe ingresar el id del comentario.';
        elseif(select count(*) from Comentario where Comentario.idComentario=idComentario)=0 then
            signal sqlstate '45000' set message_text = 'No se encontro el id del comentario.';
        else
            start transaction;
            update Comentario set idEstado=idEstado where Comentario.idComentario=idComentario;
            commit;
        end if;
    else
        signal sqlstate '45000' set message_text = 'Opcion invalida.';
    end if;
end//
CALL crudComentario(1, null, 'Este es un comentario de prueba', CURDATE(), 1, 1,1);

delimiter //
CREATE PROCEDURE `verificar_contrasenia`(
  IN `password` VARCHAR(50),
  IN `hash` VARCHAR(32),
  OUT `verificacion` BOOL
)
BEGIN
  DECLARE `pwd` VARCHAR(50);
  SET `pwd` = MD5(`password`);
  IF `pwd` = `hash` THEN
    SET `verificacion` = TRUE;
  ELSE
    SET `verificacion` = FALSE;
  END IF;
END//
CALL verificar_contrasenia('admin', '21232f297a57a5a743894a0e4a801fc3', @verificacion);
SELECT @verificacion as Verificacion;

select * from Anuncio;