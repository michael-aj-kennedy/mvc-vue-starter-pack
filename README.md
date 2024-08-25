## Environment Setup
Download & Install NPM (Initially 10.8.2) <br>
Download & Install NodeJS (Initially 22.6.0) <br>

## Solution Setup
#### Additional code analysis
Microsoft.AspNetCore.Components.Analyzers (web project only) <br>
Microsoft.CodeAnalysis.Analyzers <br>
SonarAnalyzer.CSharp <br>
Install Webpack & Vue using the following guide: https://medium.com/@luylucas10/net-mvc-webpack-and-vue-js-part-1-tools-9227b57d4690 <br>
Webpack will be automatically compiled on view, or by navigating to the app folder (not the solution folder) and running `npm run build` <br>
JsHint <br>
Projects are configured so that everything runs through the business layer.

#### Dependency injection
SimpleInjector to handle DI.<br>
Each project contains a ComponentSetup class, these are called sequentially on app start, therefore DI components are added within their containing project.<br>

#### Logging
Using SeriLog cast to an instance of Microsoft.Extensions.Logging.ILogger for ease of use (i.e. not needing to add SeriLog to every project).<br>
JSNLog for JS logging (currently configured just for JS error logging).<br>
<b>Serilog currently configured to log to a text file. We probably want to review this in the future.</b> It would be nice to use a service like datadog, but there's no doubt a cost associated with doing so.<br>

#### Database
Connection string in AppSettings.json. App defaults to System.Data.SqlClient provider.<br>
Connections handled by DbConnectionFactory. Connection string gets added here on app startup.<br>

#### General testing
All projects should expose `internal` properties to the testing projects. There's a command to add to the appropriate csproj files which will handle this.<br>

#### Integration testing
When applying database updates, always do so by appending `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED` to the top of the SQL statement. This will allow us to run multiple commands within transacitons without causing deadlocks. Ultimately this makes integration testing far easier as we can run an integration test completely within a transaction and then roll said transaction back at the end of the test.

#### Javascript & Vue using Webpack
JS, SCSS and image components held in the ./app folder.<br>
JS entry points in ./app/js/pages. These are automatically built & placed in the wwwroot folder.<br>
Vue components in ./app/js/components. These aren't built automatically unless included in one of the JS entry points.<br>
Webpack recompiles automatically on build. Alternatively point cmd prompt at the root StarterProject.App folder and run npm run build.<br>
Helpers in WebpackHtmlHelper.cs for adding JS to CSHTML pages. These allow us to pass JSON data to an init() function in the JS and avoid variable conflicts, etc.<br>