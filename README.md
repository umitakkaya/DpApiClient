# DpApiClient
Sample &amp; simple hospital app and Docplanner API integration with written in C#

# Getting started
- Clone the app
- Open the project with Visual Studio
- Change the connection strings in the following files `DpApiClient\web.config`, `DpApiClient.Importer\app.config`
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

### Note: This project is an unofficial sample

```
The MIT License (MIT)

Copyright (c) 2015 Ümit AKKAYA

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
