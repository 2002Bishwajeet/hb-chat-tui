# C# CLI Chat Client

## What it Does

This project is a command-line interface (CLI) chat client built with C#. It provides a text-based user interface for one-on-one conversations. The application is designed with a decoupled, three-project architecture (Core, Infrastructure, ConsoleUI) to ensure a clean separation of concerns. It features offline-first capabilities, allowing users to view messages and compose replies even when disconnected. Real-time message streaming is handled via WebSockets, and a local SQLite database is used for offline storage.

## How to Run Locally

### Prerequisites

*   .NET 8 SDK

### Instructions

1.  Clone the repository to your local machine.
2.  Open a terminal and navigate to the `ChatClient.ConsoleUI` directory.
3.  Run the following command to start the application:

    ```bash
    dotnet run
    ```
