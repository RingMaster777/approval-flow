# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Client (Browser/API Client)          │
└────────────────────┬────────────────────────────────────┘
                     │ HTTP/HTTPS
                     │
┌────────────────────▼────────────────────────────────────┐
│                   API Layer                              │
│  ┌────────────────────────────────────────────────┐     │
│  │  ApprovalsController                           │     │
│  │  - POST   /api/approvals                       │     │
│  │  - GET    /api/approvals                       │     │
│  │  - GET    /api/approvals/{id}                  │     │
│  │  - POST   /api/approvals/{id}/review           │     │
│  │  - GET    /api/approvals/{id}/history          │     │
│  └────────────────────┬───────────────────────────┘     │
└────────────────────────┼───────────────────────────────┘
                         │
                         │ MediatR
                         │
┌────────────────────────▼───────────────────────────────┐
│               Application Layer (CQRS)                  │
│                                                         │
│  Commands (Write):          Queries (Read):            │
│  ├─ CreateApprovalRequest   ├─ GetApprovalRequest     │
│  └─ ReviewApprovalRequest   ├─ GetAllApprovalRequests │
│                              └─ GetApprovalHistory      │
│                                                         │
│  Validators:                 DTOs:                     │
│  ├─ CreateValidator          ├─ CreateApprovalRequestDto│
│  └─ ReviewValidator          ├─ ReviewApprovalRequestDto│
│                              └─ ApprovalRequestResponseDto│
└────────────────────────┬───────────────────────────────┘
                         │
                         │ EF Core
                         │
┌────────────────────────▼───────────────────────────────┐
│              Infrastructure Layer                       │
│  ┌────────────────────────────────────────────────┐   │
│  │           AppDbContext                         │   │
│  │  DbSet<ApprovalRequest>                        │   │
│  │  DbSet<ApprovalHistory>                        │   │
│  └────────────────────┬───────────────────────────┘   │
└────────────────────────┼───────────────────────────────┘
                         │
                         │
┌────────────────────────▼───────────────────────────────┐
│                Domain Layer                             │
│  ┌────────────────────────────────────────────────┐   │
│  │  ApprovalRequest                               │   │
│  │  - Id, Title, Description                      │   │
│  │  - Status: Pending/Approved/Rejected           │   │
│  │  - Requester Info, Reviewer Info               │   │
│  │  - Timestamps                                  │   │
│  │                                                │   │
│  │  ApprovalHistory (Audit Trail)                 │   │
│  │  - State transitions                           │   │
│  │  - Actor information                           │   │
│  │  - Timestamps                                  │   │
│  └────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         │
                         │
                    ┌────▼────┐
                    │ SQLite  │
                    │ Database│
                    └─────────┘
```

## Request Flow

### Creating an Approval Request

```
1. Client → POST /api/approvals (JSON payload)
              ↓
2. ApprovalsController receives request
              ↓
3. Maps DTO to CreateApprovalRequestCommand
              ↓
4. Sends command to MediatR
              ↓
5. CreateApprovalRequestHandler executes:
   - Creates ApprovalRequest entity (Status = Pending)
   - Creates initial ApprovalHistory entry
   - Saves to database via EF Core
              ↓
6. Returns ApprovalRequestResponseDto
              ↓
7. Controller returns 201 Created with Location header
```

### Reviewing an Approval Request

```
1. Client → POST /api/approvals/{id}/review (JSON payload)
              ↓
2. ApprovalsController receives request
              ↓
3. Maps DTO to ReviewApprovalRequestCommand
              ↓
4. Sends command to MediatR
              ↓
5. ReviewApprovalRequestHandler executes:
   - Fetches existing ApprovalRequest
   - Validates business rules (must be Pending)
   - Updates request status (Approved/Rejected)
   - Creates new ApprovalHistory entry
   - Saves changes via EF Core
              ↓
6. Returns ApprovalRequestResponseDto
              ↓
7. Controller returns 200 OK
```

## State Machine

```
                    ┌──────────┐
                    │ Created  │
                    └────┬─────┘
                         │
                         │ (Automatically transitions)
                         ▼
                    ┌──────────┐
             ┌──────│ Pending  │
             │      └────┬─────┘
             │           │
             │           │ Review Action
             │           │
             │      ┌────┴─────┐
             │      │          │
             │      ▼          ▼
             │  ┌─────────┐  ┌─────────┐
             └─►│Approved │  │Rejected │
                └─────────┘  └─────────┘
                    (Final States)
```

**State Transition Rules:**
- `Created → Pending`: Automatic on creation
- `Pending → Approved`: Only via review with isApproved=true
- `Pending → Rejected`: Only via review with isApproved=false
- No transitions from `Approved` or `Rejected` (final states)

## Middleware Pipeline

```
HTTP Request
      ↓
┌─────────────────────────────────┐
│ ExceptionHandlingMiddleware     │ ← Catches all exceptions
└─────────────┬───────────────────┘
              ↓
┌─────────────────────────────────┐
│ Routing Middleware              │
└─────────────┬───────────────────┘
              ↓
┌─────────────────────────────────┐
│ Controller Action               │
└─────────────┬───────────────────┘
              ↓
┌─────────────────────────────────┐
│ MediatR Pipeline                │
│  1. Command Validation          │
│  2. Handler Execution           │
│  3. Response Mapping            │
└─────────────┬───────────────────┘
              ↓
         HTTP Response
```

## Exception Handling Flow

```
Exception Thrown in Handler
         ↓
ExceptionHandlingMiddleware catches
         ↓
    Exception Type?
         ├─ NotFoundException → 404 Not Found
         ├─ BusinessRuleException → 400 Bad Request
         ├─ ValidationException → 400 Bad Request + Validation Errors
         └─ Any Other → 500 Internal Server Error
         ↓
    Logged to Logger
         ↓
    JSON Error Response to Client
```

## Database Schema

```sql
┌─────────────────────────────────────────┐
│ ApprovalRequests                        │
├─────────────────────────────────────────┤
│ Id (GUID) PK                            │
│ Title (string, 200)                     │
│ Description (string, 1000)              │
│ RequesterId (string, 100)               │
│ RequesterName (string, 100)             │
│ Status (enum: 0=Pending, 1=Approved,    │
│         2=Rejected)                     │
│ CreatedAt (datetime)                    │
│ ReviewedAt (datetime, nullable)         │
│ ReviewerId (string, 100, nullable)      │
│ ReviewerName (string, 100, nullable)    │
│ ReviewComments (string, nullable)       │
└───────────────┬─────────────────────────┘
                │ 1:N
                │
┌───────────────▼─────────────────────────┐
│ ApprovalHistories                       │
├─────────────────────────────────────────┤
│ Id (GUID) PK                            │
│ ApprovalRequestId (GUID) FK             │
│ FromStatus (enum)                       │
│ ToStatus (enum)                         │
│ ActorId (string, 100)                   │
│ ActorName (string, 100)                 │
│ Comments (string, nullable)             │
│ Timestamp (datetime)                    │
└─────────────────────────────────────────┘
```

## Key Design Patterns Used

1. **CQRS (Command Query Responsibility Segregation)**
   - Commands: Modify state (CreateApprovalRequest, ReviewApprovalRequest)
   - Queries: Read state (GetApprovalRequest, GetAllApprovalRequests, GetApprovalHistory)

2. **Mediator Pattern**
   - MediatR decouples controllers from handlers
   - Single entry point for all business operations

3. **Repository Pattern**
   - EF Core DbContext acts as repository
   - Abstraction over data access

4. **State Pattern**
   - ApprovalStatus enum with controlled transitions
   - Business rules enforce valid state changes

5. **Audit Log Pattern**
   - All state changes recorded in ApprovalHistory
   - Immutable (append-only) audit trail

6. **DTO Pattern**
   - Separation between domain entities and API contracts
   - Input DTOs validate data before domain logic

7. **Middleware Pattern**
   - Centralized exception handling
   - Cross-cutting concerns separated from business logic

## Scalability Considerations

### Current Limitations (SQLite):
- Single-file database
- Limited concurrent write performance
- Not suitable for distributed systems

### Production Improvements:

1. **Database:**
   - Migrate to PostgreSQL/SQL Server
   - Add indexes on `Status`, `CreatedAt`, `RequesterId`
   - Implement connection pooling

2. **Caching:**
   - Add Redis for frequently-accessed requests
   - Cache approval counts by status

3. **Events:**
   - Publish domain events for state changes
   - Enable integration with notification systems

4. **API:**
   - Add pagination to GET /api/approvals
   - Implement rate limiting
   - Add API versioning

5. **Observability:**
   - Add structured logging (Serilog)
   - Application Insights / OpenTelemetry
   - Health checks endpoint

## Extension Points

The architecture supports easy extension:

- **New approval types**: Add properties to ApprovalRequest
- **Multi-level approval**: Add ApprovalLevel entity
- **Notifications**: Add MediatR notification handlers
- **Authorization**: Add policy-based authorization
- **Webhooks**: Add webhook dispatching on state changes
- **File attachments**: Add file storage service
