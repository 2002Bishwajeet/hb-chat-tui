# AGENTS.md

This file provides a guide for AI agents working with this codebase.

## Project Structure

The solution is divided into three main projects, following a clean, layered architecture:

*   **`ChatClient.Core`:** This is the core of the application. It contains the business logic, interfaces, and domain models. It has no dependencies on the other projects.
*   **`ChatClient.Infrastructure`:** This project contains the implementations of the interfaces defined in `ChatClient.Core`. It handles tasks like making API calls, interacting with the file system, and other infrastructure-related concerns.
*   **`ChatClient.ConsoleUI`:** This is the presentation layer. It's a `Terminal.Gui` application that is responsible for displaying the user interface and handling user input. It depends on `ChatClient.Core` and `ChatClient.Infrastructure`.

## Authentication Flow

The authentication flow is as follows:

1.  The user is prompted to enter their `homebase.id` in the `LoginView`.
2.  The `AuthService`'s `CheckIdentityAsync` method is called to verify the `homebase.id`.
3.  The `AuthService`'s `PrepareAuthenticationRequestUrl` method is called to construct the authentication URL.
4.  The user's default web browser is opened to the authentication URL.
5.  A local web server is started to listen for the redirect from the browser.
6.  When the redirect is received, the `AuthService`'s `FinalizeAppAuthRequest` method is called to exchange the authorization code for an access token and shared secret.
7.  The credentials are saved to a protected file in the user's home directory.

## Getting Started

To run the application, simply execute the `ChatClient.ConsoleUI` project.

```bash
dotnet run --project ChatClient.ConsoleUI/ChatClient.ConsoleUI.csproj
```

## Building the Solution

To build the solution, you can use the following command:

```bash
dotnet build
```

## Running Tests

There are currently no tests in the solution. Adding tests is a great way to contribute!
