# Active Together
## Local Environment Setup Documentation
### User Secrets
#### Web.Server project:
```
{
  "POSTGRESQLCONNSTR_AZURE_DEVELOP": "<db connection string>",
  "POSTGRESQLCONNSTR_LOCAL": "Host=host.docker.internal;Port=6666;Database=postgres;Username=postgres; Password=Test1234;",
  "EXTERNALAUTHENTICATION": {
    "FACEBOOK": {
      "APPID": "<id>",
      "APPSECRET": "<secret>"
    }
  },
  "AZURE_STORAGE_ACCOUNT_DEV": "<base url to storage>",
  "AZURE_STORAGE_ACCOUNT_NAME": "<account name>",
  "AZURE_STORAGE_ACCOUNT_KEY": "<storage account key>",
  "NO-REPLY-EMAIL-CONFIGURATION": {
    "Email": "<email>",
    "Password": "<password>",
    "SmtpServer": "<e.g.: send.one.com>",
    "SmtpPort": "<number e.g.: 587>",
    "FeedbackEmailRecipient": "<email to send feedback to>"
  }
}
```

#### DatabaseSeeding project:
```
{
  "POSTGRESQLCONNSTR_AZURE_DEVELOP": "<db connection string>",
  "POSTGRESQLCONNSTR_LOCAL": "Host=host.docker.internal;Port=6666;Database=postgres;Username=postgres; Password=Test1234;",
  "OsmFilePath": "G:\\downloads\\denmark-latest.osm",
  "OsmSaveFolderPath": "C:\\Users\\Hampus\\Desktop\\",
  "OsmSaveFilePath": "C:\\Users\\Hampus\\Desktop\\2023-3-17 19-54-57-OpenStreetMapExctraction.json",
  "AZURE_STORAGE_ACCOUNT_DEV": "<base url to storage>"
}
```

### Local Database
We use the Postgres with GIS extension (postgis).
Run the docker command:
```
docker run --name ActiveTogether -p 6666:5432 -e POSTGRES_PASSWORD=Test1234 -d postgis/postgis
```

## Database Seeding
To seed the database, run the DatabaseSeeding Project.

## DB Migration

### Adding a migration to your database.
In your startup item in the VS main window top bar:
Select Web.Server

In the Package Manager Console:
Select "Default project" -> EntityLib
If no migrations exists in the EntityLib/Migrations folder, run the command:
```
Add-Migration InitialMigration
```
To update the database:
```
Update-Database
```

### Resetting the database
Use PgAdmin 4 and connect to the database.

Use the sql code in the database-reset.txt file to drop all tables in the database.
Remember to update the sql when adding new tables!
