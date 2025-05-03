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
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification) 
	VALUES('Desarrollador', 1, SYSDATETIME(), SYSDATETIME());
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification)
	VALUES('Administrador', 1, SYSDATETIME(), SYSDATETIME());
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification)
	VALUES('Instructor', 1, SYSDATETIME(), SYSDATETIME());
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification)
	VALUES('Alumno/a', 1, SYSDATETIME(), SYSDATETIME());
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification)
	VALUES('Secretario/a', 1, SYSDATETIME(), SYSDATETIME());
	INSERT INTO [Role] ([Name], [Status], DateCreated, DateModification)
	VALUES('Invitado', 1, SYSDATETIME(), SYSDATETIME());
GO
CREATE TABLE [User](
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(50) NOT NULL,
    [Password] VARCHAR(32) NOT NULL,
    [Status] TINYINT NOT NULL,
    RecoveryEmail VARCHAR(50) NOT NULL,
    DateCreated DATETIME NOT NULL,
    DateModification DATETIME NOT NULL,
	ImageData VARBINARY(MAX) NOT NULL,
	IdRole INT NOT NULL FOREIGN KEY REFERENCES [Role](Id) ON DELETE CASCADE
);
GO