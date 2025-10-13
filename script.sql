CREATE DATABASE BDTask
GO

USE BDTask
GO

CREATE TABLE Tag
(
	TagId	int				primary key		identity,
	Title	varchar(200)	not null		unique
)

INSERT INTO Tag VALUES ('Estudo')
INSERT INTO Tag VALUES ('Trabalho')
