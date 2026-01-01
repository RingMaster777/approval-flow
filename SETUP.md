# Setup Instructions

## Prerequisites

- .NET 8 SDK installed ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))

## Getting Started

### 1. Navigate to the project directory

```bash
cd ApprovalFlow
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Run the application

```bash
dotnet run
```

The application will start on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

### 4. Access Swagger UI

Open your browser and navigate to:
- **HTTP:** http://localhost:5000/swagger
- **HTTPS:** https://localhost:5001/swagger

## Quick Test Workflow

### Step 1: Create an Approval Request

**Endpoint:** `POST /api/approvals`

```json
{
  "title": "Laptop Purchase Request",
  "description": "Need a new MacBook Pro for development work",
  "requesterId": "emp001",
  "requesterName": "John Doe"
}
```

**Expected Response:** Status 201, returns the created request with a unique ID.

### Step 2: View All Pending Requests

**Endpoint:** `GET /api/approvals?status=Pending`

You should see the request you just created.

### Step 3: Get Request Details

**Endpoint:** `GET /api/approvals/{id}`

Replace `{id}` with the ID from Step 1.

### Step 4: Approve the Request

**Endpoint:** `POST /api/approvals/{id}/review`

```json
{
  "isApproved": true,
  "reviewerId": "mgr001",
  "reviewerName": "Jane Smith",
  "comments": "Approved. Budget allocated."
}
```

**Expected Response:** Status 200, request status changed to "Approved".

### Step 5: View Approval History

**Endpoint:** `GET /api/approvals/{id}/history`

You should see two history entries:
1. Request created (Pending → Pending)
2. Request approved (Pending → Approved)

## Testing Edge Cases

### Try to Review an Already Reviewed Request

Try Step 4 again with the same request ID.

**Expected:** Status 400 with error message "Cannot review request. Current status: Approved"

### Create Request with Invalid Data

Try creating a request with an empty title:

```json
{
  "title": "",
  "description": "Test",
  "requesterId": "emp001",
  "requesterName": "John Doe"
}
```

**Expected:** Status 400 with validation errors.

### Request Non-Existent Approval

**Endpoint:** `GET /api/approvals/00000000-0000-0000-0000-000000000000`

**Expected:** Status 404 with error message.

## Database Location

The SQLite database is created at:
```
ApprovalFlow/approvalflow.db
```

You can inspect it using:
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- VS Code SQLite extension
- Command line: `sqlite3 approvalflow.db`

## Clean Database (Start Fresh)

```bash
# Stop the application (Ctrl+C)
# Delete the database file
rm approvalflow.db

# Run again - fresh database will be created
dotnet run
```

## Common Issues

### Port Already in Use

If port 5000 is already taken, you can change it in `Program.cs` or `appsettings.json`.

### Migration Errors

If you see migration errors:

```bash
# Remove existing database
rm approvalflow.db

# Remove migrations folder
rm -rf Migrations

# Recreate migration
dotnet ef migrations add InitialCreate

# Run application
dotnet run
```

## Development Mode

To run in development mode with detailed logging:

```bash
dotnet run --environment Development
```

## Building for Production

```bash
# Build release version
dotnet build --configuration Release

# Publish self-contained executable
dotnet publish --configuration Release --output ./publish
```

---

**Next Steps:**
- Try all endpoints in Swagger
- Test error scenarios
- Review the code structure
- Extend with your own features
