-- Run once against the COVIDPass database as the Entra ID SQL admin, replacing <identity-name>
-- with the `identityName` output of main.bicep. Grants the app's managed identity schema-owner rights
-- so FluentMigrator can create and upgrade the tables on startup.
CREATE USER [<identity-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_ddladmin ADD MEMBER [<identity-name>];
ALTER ROLE db_datareader ADD MEMBER [<identity-name>];
ALTER ROLE db_datawriter ADD MEMBER [<identity-name>];
