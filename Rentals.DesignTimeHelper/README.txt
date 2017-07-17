DesignTimeHelper is a helper project to assist Code First Migrations to create 
and update an instance of the database for the application.

It contains an app.config with the connection string configuration for where the database is to be located.

To create or update a database, set this project as the startup project, 
then in the Package Manager Console select the Rentals.DAL as the Default Project,
then use the add-migration command to create a migration, if the model has changed and
use update-database to run the migrations and modify/create the database.