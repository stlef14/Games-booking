# Games-booking

POC Hockey Games Booking System.

---

## Getting Started

This repository contains the bare-bones scaffolding for a Clean Architecture solution with .NET 9.

### Prerequisites

Ensure you have the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed.

---

## How to Build and Test

### 1. Using the build script

A Bash build automation script is available at the root: [build.sh](file:///Users/slef/Personal/development/Games-booking/build.sh).

* **Build & Run Tests**:
  ```bash
  ./build.sh --clean --test
  ```

* **Build only**:
  ```bash
  ./build.sh
  ```

### 2. Using standard .NET CLI

If you prefer to run commands directly:

* **Build the solution**:
  ```bash
  dotnet build
  ```

* **Run all tests**:
  ```bash
  dotnet test
  ```

---

## How to Run the Web API Locally

To start the API development server:

```bash
dotnet run --project src/GamesBooking.Api --launch-profile http
```

Once running, you can access the API:

* **Root** (service info):
  ```bash
  curl http://localhost:5293/
  ```

* **Games**:
  ```bash
  curl http://localhost:5293/api/games
  ```

---

## Architecture Layout

* **`src/`**
  * `GamesBooking.Domain`: Core Entities and interfaces.
  * `GamesBooking.Application`: Application services, CQRS Commands/Queries, interfaces.
  * `GamesBooking.Infrastructure`: Persistence, EF Core database context, external APIs.
  * `GamesBooking.Api`: Web API endpoints and Program entry point.
* **`tests/`**
  * `GamesBooking.UnitTests`: Unit tests for Application and Domain logic.
  * `GamesBooking.IntegrationTests`: End-to-end API integration tests using WebApplicationFactory.
