CREATE DATABASE KMSysControlAdminDB
GO
	USE KMSysControlAdminDB
GO
CREATE TABLE [Role](
	Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(30) NOT NULL,
	[Status] TINYINT NOT NULL,
	DateCreated DATETIME NOT NULL,
	DateModification DATETIME NOT NULL
);
GO
    INSERT INTO [Role] VALUES('Desarrollador');
	INSERT INTO [Role] VALUES('Administrador');
	INSERT INTO [Role] VALUES('Instructor');
    INSERT INTO [Role] VALUES('Alumno/a');
    INSERT INTO [Role] VALUES('Secretario/a');
	INSERT INTO [Role] VALUES('Invitado');
GO