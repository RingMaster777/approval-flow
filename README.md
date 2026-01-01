# Approval Flow Microservice

A clean, production-ready .NET 9 microservice for managing approval workflows with PostgreSQL, built with domain-driven design, CQRS pattern, and comprehensive business logic.

## 🎯 What Problem Does It Solve?

Many business workflows require multi-step approvals—expense reports, leave requests, purchase orders, etc. This microservice provides a **reusable approval engine** that can be integrated into any system needing approval workflows.

## ✨ Features

- ✅ Create approval requests with title and description
- ✅ Approve/Reject requests with reviewer comments
- ✅ Complete audit trail with state transitions
- ✅ Filter requests by status (Pending/Approved/Rejected)
- ✅ Idempotent operations
- ✅ Centralized error handling
- ✅ Input validation with detailed error messages

## 🏗️ Architecture & Design Decisions

### Why This Structure?

**Clean Architecture (Domain-Application-Infrastructure-API):**

- **Domain**: Pure business entities (no dependencies)
- **Application**: Business logic, CQRS handlers, DTOs
- **Infrastructure**: Database context, external concerns
- **API**: Controllers, middleware, presentation layer

**Why CQRS + MediatR?**

- Clear separation between reads and writes
- Easy to test individual handlers
- Scalable pattern for complex workflows
- Industry-standard approach

**Why FluentValidation?**

- Keeps validation logic separate from business logic
- Reusable validation rules
- Clean, readable validation syntax

**Why SQLite?**

- Zero configuration for demo/development
- Easy to swap for PostgreSQL/SQL Server in production
- Perfect for personal projects and portfolios

### State Machine

```
[Created] → [Pending] → [Approved]
                    ↘ [Rejected]
```

**Business Rules:**

- Only Pending requests can be reviewed
- All state changes are audited
- History is immutable

## 🛠️ Tech Stack

| Layer             | Technology                     |
| ----------------- | ------------------------------ |
| Framework         | ASP.NET Core 8                 |
| Database          | Entity Framework Core + SQLite |
| CQRS              | MediatR                        |
| Validation        | FluentValidation               |
| API Documentation | Swagger/OpenAPI                |

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK

### Run Locally

```bash
# 1. Restore packages
dotnet restore

# 2. Run the application (migrations auto-applied)
dotnet run

# 3. Open Swagger
# Navigate to: https://localhost:7xxx/swagger
```

The database will be created automatically on first run.

## 📡 API Endpoints

### Create Approval Request

```http
POST /api/approvals
Content-Type: application/json

{
  "title": "Vacation Leave Request",
  "description": "Requesting 5 days leave from Jan 15-20",
  "requesterId": "emp001",
  "requesterName": "John Doe"
}
```

**Response:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Vacation Leave Request",
  "status": "Pending",
  "createdAt": "2026-01-01T10:30:00Z"
}
```

### Get All Requests (with optional filter)

```http
GET /api/approvals?status=Pending
```

### Get Single Request

```http
GET /api/approvals/{id}
```

### Review Request (Approve/Reject)

```http
POST /api/approvals/{id}/review
Content-Type: application/json

{
  "isApproved": true,
  "reviewerId": "mgr001",
  "reviewerName": "Jane Smith",
  "comments": "Approved for requested dates"
}
```

### Get Approval History

```http
GET /api/approvals/{id}/history
```

**Response:**

```json
[
  {
    "fromStatus": "Pending",
    "toStatus": "Pending",
    "actorName": "John Doe",
    "comments": "Request created",
    "timestamp": "2026-01-01T10:30:00Z"
  },
  {
    "fromStatus": "Pending",
    "toStatus": "Approved",
    "actorName": "Jane Smith",
    "comments": "Approved for requested dates",
    "timestamp": "2026-01-01T14:20:00Z"
  }
]
```

## 🧪 Testing the API

### Using Swagger (Recommended)

1. Run the application: `dotnet run`
2. Open `https://localhost:7xxx/swagger`
3. Try the endpoints interactively

### Using cURL

```bash
# Create request
curl -X POST https://localhost:7xxx/api/approvals \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Equipment Purchase",
    "description": "Need new laptop",
    "requesterId": "emp001",
    "requesterName": "John Doe"
  }'

# Get all pending requests
curl https://localhost:7xxx/api/approvals?status=Pending

# Approve request (replace {id})
curl -X POST https://localhost:7xxx/api/approvals/{id}/review \
  -H "Content-Type: application/json" \
  -d '{
    "isApproved": true,
    "reviewerId": "mgr001",
    "reviewerName": "Jane Smith",
    "comments": "Approved"
  }'

# Get history
curl https://localhost:7xxx/api/approvals/{id}/history
```

## 📂 Project Structure

```
ApprovalFlow/
├── Domain/
│   └── Entities/              # Core business entities
│       ├── ApprovalRequest.cs
│       └── ApprovalHistory.cs
├── Application/
│   ├── Commands/              # Write operations (CQRS)
│   │   ├── CreateApprovalRequestCommand.cs
│   │   └── ReviewApprovalRequestCommand.cs
│   ├── Queries/               # Read operations (CQRS)
│   │   ├── GetApprovalRequestQuery.cs
│   │   ├── GetAllApprovalRequestsQuery.cs
│   │   └── GetApprovalHistoryQuery.cs
│   ├── DTOs/                  # Data transfer objects
│   ├── Validators/            # FluentValidation rules
│   └── Common/
│       └── Exceptions/        # Custom business exceptions
├── Infrastructure/
│   └── Data/
│       └── AppDbContext.cs    # EF Core DbContext
├── API/
│   ├── Controllers/
│   │   └── ApprovalsController.cs
│   └── Middleware/
│       └── ExceptionHandlingMiddleware.cs
├── Migrations/                # EF Core migrations
└── Program.cs                 # Application entry point
```

## 🎓 What This Project Demonstrates

### Backend Engineering Skills

✅ **Clean Architecture** - Proper separation of concerns  
✅ **CQRS Pattern** - Command/Query responsibility segregation  
✅ **Domain Modeling** - State machines, business rules  
✅ **Error Handling** - Centralized exception middleware  
✅ **Validation** - Input validation with FluentValidation  
✅ **Database Design** - Proper relationships, constraints  
✅ **Audit Trail** - Immutable history tracking  
✅ **RESTful API Design** - Proper HTTP verbs and status codes  
✅ **API Documentation** - Swagger/OpenAPI integration

### Production-Ready Patterns

- Idempotent operations
- Business rule enforcement
- Immutable audit logs
- Proper error responses
- Database migrations

## 🔄 How to Extend

### Add Email Notifications

Create a notification handler in MediatR pipeline:

```csharp
public class ApprovalReviewedNotificationHandler :
    INotificationHandler<ApprovalReviewedEvent>
{
    // Send email logic
}
```

### Add Role-Based Authorization

Add role claims checking in middleware:

```csharp
[Authorize(Roles = "Manager")]
public async Task<ActionResult> ReviewRequest(...)
```

### Switch to PostgreSQL

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=approvalflow;..."
  }
}
```

And in `Program.cs`:

```csharp
options.UseNpgsql(connectionString)
```

### Add Multi-Level Approval

Extend `ApprovalRequest` entity:

```csharp
public int RequiredApprovers { get; set; }
public List<Approval> Approvals { get; set; }
```

## 🤔 Interview Talking Points

**"Why did you structure it this way?"**

> I used Clean Architecture to ensure the domain logic is independent of infrastructure concerns. This makes the code testable and allows easy swapping of databases or frameworks without touching business logic.

**"Where would this break at scale?"**

> Currently uses SQLite which is single-file. At scale, I'd move to PostgreSQL with proper indexing on Status and CreatedAt columns. I'd also add caching for frequently-accessed requests and consider event sourcing for the audit trail.

**"How would you extend this?"**

> I'd add MediatR notifications for email alerts, implement role-based authorization, add multi-level approvals, and introduce a deadline/expiry system. The CQRS pattern makes adding features straightforward without modifying existing handlers.

**"How do you ensure data consistency?"**

> State transitions are controlled through business rules in handlers. The database enforces referential integrity, and the audit history is append-only (no updates/deletes). EF Core transactions ensure atomicity.

## 📝 License

MIT - Free to use for learning and portfolio purposes

---

**Time to Build:** ~1 week  
**Lines of Code:** ~800 (excluding migrations)  
**Purpose:** Portfolio project demonstrating backend engineering competency for mid-level .NET positions
