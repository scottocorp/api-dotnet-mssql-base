-- Adhoc commands:

-- Create the database:
-- docker exec -it msss-container "bash"
-- /opt/mssql-tools/bin/sqlcmd -S localhost -U SA
-- <enter password>
-- CREATE DATABASE [msss-db];
-- GO

-- Create the table:
-- cd scripts
-- docker cp photos.sql msss-container:/tmp
-- docker exec -it msss-container "bash"
-- /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -i /tmp/photos.sql
-- <enter password>

-- Other commands:
-- DELETE FROM [msss-db].dbo.photos
-- GO

-- Reset the ID:
-- DBCC CHECKIDENT ('[msss-db].dbo.photos', RESEED, 0);
-- GO

USE [msss-db]
CREATE TABLE photos ( 
  ID INT IDENTITY(1,1) PRIMARY KEY,
  Filename VARCHAR(25),
  Timestamp DATETIME,
  Day DATETIME,
  Altitude FLOAT,
  Lng DECIMAL(9, 6),
  Lat DECIMAL(9, 6),
  CameraLng DECIMAL(9, 6),
  CameraLat DECIMAL(9, 6),
  Pitch FLOAT,
  Zoom FLOAT,
  Bearing FLOAT,
  Enabled BIT
)
go
