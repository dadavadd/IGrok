A robust and lightweight backend service for managing software licenses and user authentication. Built with modern .NET, it features JWT-based authentication, refresh tokens, hardware ID (HWID) locking, and a secure admin API.

## ✨ Key Features

-   **JWT Authentication**: Secure access with short-lived JWTs and long-lived refresh tokens.
-   **License Management**: Admin API to create, delete, and manage user licenses.
-   **Subscription Control**: Set expiration dates for user subscriptions.
-   **Hardware ID (HWID) Locking**: Bind a user license to a specific machine.
-   **Account Status**: Activate or deactivate user accounts on the fly.
-   **Secure Admin API**: Admin endpoints are protected by a configurable API Key.
-   **Rate Limiting**: Protects public endpoints from brute-force attacks.
-   **API Documentation**: Integrated Swagger/OpenAPI for easy API exploration and testing.
-   **Clean Architecture**: Uses Minimal APIs for a clean and performant endpoint structure.
-   **Testable**: Includes both unit and integration tests for reliability.

## 🛠️ Tech Stack

-   **.NET 10** / ASP.NET Core
-   **Minimal APIs**
-   **Entity Framework Core**
-   **SQLite** for the database
-   **xUnit** for testing

## 🚀 Getting Started

### Prerequisites

-   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

### 1. Clone the repository

```bash
git clone https://github.com/dadavadd/IGrok.git
cd IGrok
```

### 2. Configure the application

The main configuration is in `IGrok/IGrok/appsettings.json`. For development, it's recommended to create an `appsettings.Development.json` file to override secrets.

**`IGrok/IGrok/appsettings.Development.json`:**

```json
{
  "AdminSettings": {
    "ApiKey": "CHANGE_THIS_TO_A_SECURE_KEY"
  },
  "JwtOptions": {
    "SecretKey": "CHANGE_THIS_TO_A_LONG_SECRET_KEY_MIN_32_CHARS"
  }
}
```

### 3. Setup the database

The project uses EF Core migrations. To create and seed the database, run:

```bash
dotnet ef database update --project IGrok/IGrok
```

### 4. Run the application

```bash
dotnet run --project IGrok/IGrok/IGrok.csproj
```

The API will be available at `https://localhost:7XXX` and `http://localhost:5XXX`. The Swagger UI can be accessed at `https://localhost:7XXX/swagger`.

## 📝 API Endpoints

The API is versioned under `/api/v1`.

### Auth API (`/api/v1/auth`)

-   `POST /login`: Authenticates a user with their `Key` and `Hwid`. Returns an `AccessToken` and `RefreshToken`.
-   `POST /refresh`: Refreshes an expired `AccessToken` using a valid `RefreshToken`.

### Admin API (`/api/v1/admin`)

> **Note:** All admin endpoints require an `X-Api-Key` header for authorization.

-   `POST /users`: Gets a list of users.
-   `POST /users`: Creates a new user license.
-   `PUT /users/{key}/hwid`: Updates or resets a user's HWID.
-   `PATCH /users/{key}/status`: Activates or deactivates a user account.
-   `DELETE /users/{key}`: Permanently deletes a user.

## ✅ Testing

The solution includes a test project with unit and integration tests. To run all tests, use the following command from the root directory:

```bash
dotnet test
```
