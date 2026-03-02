# Solution Documentation

**Candidate Name:** Kaustubh Sawant  
**Completion Date:** [Date]

---

## Problems Identified

_Describe the issues you found in the original implementation. Consider aspects like:_

- Architecture and design patterns
  
  - DTOs not used for request/response.  
  - HTTP API design conventions not followed, all CRUD endpoints configured for HTTP POST.
  - Abstractions missing for the Service and Repository layers.
  - Dependency Injection missing for Service layer, direct instantiation in controller without DI.  
  - No clear separation of concerns, business logic scattered across Data and Presentation layers.  
  - Tight coupling of Persistence / Repository logic within the Service layer without abstraction.  

- Code quality and maintainability

  - HTTP response status codes not documented.
  - Logging not implemented.
  - Async pattern not implemented for the Persistence / data access layer.
  - Use of generic try-catch blocks.
  - Generic error messages, non-standard HTTP status codes used.
  - SQL connection string hardcoded in the code.

- Security vulnerabilities

  - SQL injection vulnerability observed in the Persistence operations.
  - Authentication / Authorization missing for the endpoints.

- Performance concerns

  - Synchronous operations used in the application, must be replaced with async operations.

- Testing gaps
  
  - Direct inistantiation using new keyword for the service layer made unit testing difficult.
  - Unit tests missing for the controller and service layers.

---

## Architectural Decisions

_Explain the architecture you chose and why. Consider:_

- Design patterns applied

  - Layered application implemented, separated the controller, service and repository layers.
  - Used abstractions to define behaviours, enabling loose coupling and easy swappability in the future.
  - Used Dependency Injection to instantiate objects of service and repository layers.
  - Used DTOs for request / response.
  - SOLID principles implemented.
  - DRY pattern implemented for mapping of DB reader to DTO.

- Project structure changes

  - Folder structure changes done to implement layered architecture, use of DTOs and abstractions.

- Technology choices

  - Used async operations for database access.
  - Used configuration from appsettings.json file.

- Separation of concerns

  - Layered architecture implemented.
  - Controller layer handles HTTP request / response logic.
  - Service layer handles business logic.
  - Repository layer handles database logic.

- API Design

  - API documention for possible HTTP status codes for the response.
  - HTTP conventions followed for resource-based URLs, proper HTTP verbs used.
    - `GET /api/todo` - Get all todos
    - `GET /api/todo/{id}` - Get specific todo by id
    - `POST /api/todo` - Create new todo
    - `PUT /api/todo/{id}` - Update existing todo
    - `DELETE /api/todo/{id}` - Delete a todo by id
  - Proper HTTP status codes returned.

- Security implementation

  - Use of parameterized queries to prevent SQL injection.
  - Implemented exception handling middleware for consistent error responses

- Logging

  - Used Serilog for logging of API requests and operations, for observability and tracking.

---

## Trade-offs

_Discuss compromises you made and the reasoning behind them. Consider:_

- What did you prioritize?
  - Simplicity and layered application.
  - Separation of Concerns and Dependency Injection.
  - Use of DTOs.
  - API documentation of HTTP status codes.

- What did you defer or simplify?
  - Simplified the API endpoints as per HTTP conventions.
  - Used unit testing for proper code testing.
  - Used abstractions wherever necessary.
  - Authentication and authorization was low prioritized.

- What alternatives did you consider?
  - Use of EntityFramework for database access.

---

## How to Run

### Prerequisites

- .NET 8 SDK
- Powershell / terminal
- Visual Studio 2022 or VS Code

### Build

```bash
dotnet clean
dotnet build
```

### Run

```bash
dotnet run --project TodoApi
```

### Test

```bash
dotnet test
```

---

## API Documentation

### Endpoints

#### Create TODO

```
Method: POST
URL: /api/Todo
Request: 
{
  "title": "Todo 1",
  "description": "Description 1",
  "isCompleted": false
}

Response: 
{
  "id": 1,
  "title": "Todo 1",
  "description": "Description 1",
  "isCompleted": false,
  "createdAt": "2026-03-02T06:27:29.4678963Z"
}
```

#### Get TODO(s)

```
Method: GET
URL: /api/Todo
Response: 
[
  {
    "id": 1,
    "title": "Todo 1",
    "description": "Description 1",
    "isCompleted": false,
    "createdAt": "2026-03-02T11:57:29.4678963+05:30"
  }
]
```

#### Update TODO

```
Method: PUT
URL: /api/Todo/{id}
Request Body: 
{
  "id": 1,
  "title": "Todo 1 updated",
  "description": "Todo 1 description updated",
  "isCompleted": true
}
Response: 
{
  "id": 1,
  "title": "Todo 1 updated",
  "description": "Todo 1 description updated",
  "isCompleted": true,
  "createdAt": "2026-03-02T00:52:33.7783051+05:30"
}
```

#### Delete TODO

```
Method: DELETE
URL: /api/Todo/{id}
Response: 
{
  "statusCode": 200,
  "message": "Todo deleted successfully",
  "success": true
}
```

---

## Future Improvements

_What would you do if you had more time? Consider:_

- Use pagination while fetching all resources from the database.
- Use ORM for data access / repository layer, preferably EntityFramweork with migration.
- Implement authentication / authorization for the endpoints.
- Extend exception handling for a more diverse set of exceptions.
- Use input validation and add constraints for data and database tables.
- Improve test coverage with more fine-grained unit tests.
- Containerize the application using docker for deployment.
- Implement request id based tracing of requests for observability.
