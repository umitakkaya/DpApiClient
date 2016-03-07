# DpApiClient
Sample &amp; simple hospital app and Docplanner API integration with written in C#

# Getting started
- Clone the app
- Open the project with Visual Studio
- Navigate to `Package Manager Console`
- Select `DpApiClient.Data` from `Default project` dropdown
- Run `Update-Database` command to scaffold the database
- Add the following settings to the `AppSetting` table

  ```
    | Setting name       | Setting value      |
    |--------------------|--------------------|
    | dpapi.clientId     | your_client_id     |
    | dpapi.clientSecret | your_client_secret |
    | dpapi.locale       | (tr-TR | pl-PL)    |
  ```
- To import the doctors (and the resources of these doctors from Docplanner run DpApiClient.Importer project)
- Run the application
