# Pulani\_Chirwa\_PROG6212\_Part\_2





\# RaceDay – PROG6212 Part 2



\## Project Overview

RaceDay is an ASP.NET Core Web API application developed for the PROG6212 Portfolio of Evidence (Part 2).



The application supports the management of sporting events, participant enrolments, routes, categories and race results.



\## Technologies Used

\- C# and ASP.NET Core Web API

\- .NET 10

\- Entity Framework Core

\- Microsoft SQL Server

\- Swagger / OpenAPI

\- xUnit for automated unit testing

\- GitHub Actions for Continuous Integration



\## Project Structure

\- `RaceDay/RaceDay.API` – Web API application

\- `RaceDay/RaceDay.Tests` – Automated unit tests

\- `RaceDay/RaceDay.slnx` – Visual Studio solution

\- `.github/workflows/dotnet.yml` – GitHub Actions workflow



\## API Functionality

The project includes API controllers for:

\- User registration and login

\- Session-based authentication and logout

\- User profiles

\- Sporting events

\- Event categories

\- Race routes

\- Participant enrolments

\- Race results



API endpoints can be explored and tested using Swagger.



\## Authentication and Authorisation

RaceDay uses session-based authentication.



Users can register, log in, view their authenticated profile and log out. The application includes role-based permission checks for Participant, Organiser and Admin users.



\## Database

The application uses SQL Server with Entity Framework Core.



The database includes the following main entities:

\- Users

\- Events

\- Categories

\- Routes

\- Enrolments

\- Results



Entity Framework Core migrations are included in the API project.



\## Running the Application

1\. Open `RaceDay/RaceDay.slnx` in Visual Studio.

2\. Ensure .NET 10 and SQL Server are available.

3\. Configure the database connection in `RaceDay.API/appsettings.json` for your local environment.

4\. Apply the Entity Framework Core database migrations.

5\. Set `RaceDay.API` as the startup project.

6\. Run the application.

7\. Open Swagger to explore the available API endpoints.



\## Automated Unit Tests

The solution contains three xUnit tests covering event retrieval:



1\. An empty event database returns an empty list.

2\. A nonexistent event returns a NotFound response.

3\. An existing event returns an OK response.



All three tests passed successfully in Visual Studio and GitHub Actions.



\## Continuous Integration (CI/CD)

The project includes a GitHub Actions workflow that automatically:

\- Checks out the repository

\- Sets up .NET 10

\- Restores project dependencies

\- Builds the solution

\- Executes the automated unit tests



The GitHub Actions build-and-test workflow completed successfully with all three unit tests passing.



\## Development Environment

\- Visual Studio 2026

\- C#

\- .NET 10

\- SQL Server

\- Git and GitHub



\## Academic Submission

PROG6212 – Portfolio of Evidence, Part 2



Project: RaceDay



