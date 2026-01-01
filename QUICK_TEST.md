# Quick Test Guide

## 🚀 Start the Application

```bash
cd ApprovalFlow
dotnet run
```

Open: **http://localhost:5000/swagger**

---

## 📋 Test Scenario: Complete Workflow

### Step 1: Create First Request

**Endpoint:** `POST /api/approvals`

```json
{
  "title": "MacBook Pro Purchase",
  "description": "Need M3 MacBook Pro for development work. Budget: $2500",
  "requesterId": "EMP001",
  "requesterName": "Rashik Mahmud"
}
```

**Expected:** `201 Created` with response containing the request `id`

**Copy the ID** for next steps (e.g., `a1b2c3d4-...`)

---

### Step 2: Create Second Request

```json
{
  "title": "Annual Leave Request",
  "description": "5 days vacation from Jan 15-20, 2026",
  "requesterId": "EMP002",
  "requesterName": "John Doe"
}
```

**Expected:** `201 Created`

---

### Step 3: View All Pending Requests

**Endpoint:** `GET /api/approvals?status=Pending`

**Expected:** Returns array with 2 requests (both Pending)

---

### Step 4: View Single Request

**Endpoint:** `GET /api/approvals/{id}`

Replace `{id}` with the ID from Step 1.

**Expected:** `200 OK` with full request details

---

### Step 5: Approve First Request

**Endpoint:** `POST /api/approvals/{id}/review`

Use the ID from Step 1.

```json
{
  "isApproved": true,
  "reviewerId": "MGR001",
  "reviewerName": "Sarah Ahmed",
  "comments": "Approved. Budget allocated from Q1."
}
```

**Expected:** `200 OK`, status changed to `Approved`

---

### Step 6: Reject Second Request

**Endpoint:** `POST /api/approvals/{id}/review`

Use the ID from Step 2.

```json
{
  "isApproved": false,
  "reviewerId": "MGR001",
  "reviewerName": "Sarah Ahmed",
  "comments": "Denied. Team at full capacity during requested period."
}
```

**Expected:** `200 OK`, status changed to `Rejected`

---

### Step 7: View Approval History

**Endpoint:** `GET /api/approvals/{id}/history`

Use the ID from Step 1.

**Expected:** Array with 2 history entries:
1. Created (Pending → Pending)
2. Approved (Pending → Approved)

---

### Step 8: Filter by Status

**Get Approved:**
```
GET /api/approvals?status=Approved
```

**Get Rejected:**
```
GET /api/approvals?status=Rejected
```

**Get Pending:**
```
GET /api/approvals?status=Pending
```

---

## ❌ Test Error Scenarios

### 1. Try to Review Already Reviewed Request

**Endpoint:** `POST /api/approvals/{approved-id}/review`

```json
{
  "isApproved": true,
  "reviewerId": "MGR002",
  "reviewerName": "Test User",
  "comments": "Trying again"
}
```

**Expected:** `400 Bad Request`
```json
{
  "statusCode": 400,
  "message": "Cannot review request. Current status: Approved"
}
```

---

### 2. Invalid Request Data

**Endpoint:** `POST /api/approvals`

```json
{
  "title": "",
  "description": "Test",
  "requesterId": "EMP001",
  "requesterName": ""
}
```

**Expected:** `400 Bad Request` with validation errors
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "propertyName": "Title",
      "errorMessage": "Title is required"
    },
    {
      "propertyName": "RequesterName",
      "errorMessage": "Requester name is required"
    }
  ]
}
```

---

### 3. Non-Existent Request

**Endpoint:** `GET /api/approvals/00000000-0000-0000-0000-000000000000`

**Expected:** `404 Not Found`
```json
{
  "statusCode": 404,
  "message": "Approval request with ID 00000000-0000-0000-0000-000000000000 not found"
}
```

---

### 4. Title Too Long

**Endpoint:** `POST /api/approvals`

```json
{
  "title": "This is a very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long title that exceeds 200 characters and should fail validation",
  "description": "Test",
  "requesterId": "EMP001",
  "requesterName": "Test User"
}
```

**Expected:** `400 Bad Request`
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "propertyName": "Title",
      "errorMessage": "Title cannot exceed 200 characters"
    }
  ]
}
```

---

## 🧪 Complete Test Checklist

- [ ] Create approval request successfully
- [ ] Get single request by ID
- [ ] Get all requests without filter
- [ ] Get requests filtered by Pending
- [ ] Get requests filtered by Approved
- [ ] Get requests filtered by Rejected
- [ ] Approve a pending request
- [ ] Reject a pending request
- [ ] View approval history (2+ entries)
- [ ] Validate empty title fails
- [ ] Validate empty requester name fails
- [ ] Validate title > 200 chars fails
- [ ] Validate description > 1000 chars fails
- [ ] Cannot review already-approved request
- [ ] Cannot review already-rejected request
- [ ] 404 for non-existent request ID
- [ ] History returns empty array for request with no transitions

---

## 📊 Expected Database State After Full Test

**ApprovalRequests Table:**
- 2 requests total
- 1 with Status = Approved
- 1 with Status = Rejected

**ApprovalHistories Table:**
- 4 history entries total
- 2 for first request (Created + Approved)
- 2 for second request (Created + Rejected)

---

## 🔍 View Database

```bash
# Install DB Browser for SQLite or use:
sqlite3 approvalflow.db

# View all requests
SELECT Id, Title, Status, CreatedAt FROM ApprovalRequests;

# View all history
SELECT * FROM ApprovalHistories ORDER BY Timestamp;

# Exit
.quit
```

---

## 🎯 Demo Script (2 minutes)

**"Let me show you the Approval Flow API I built..."**

1. **Show Swagger UI** (5 seconds)
   - "Here's the API documentation - 5 endpoints"

2. **Create Request** (15 seconds)
   - "First, a user creates an approval request"
   - Execute POST /api/approvals
   - "Returns 201 with the created request"

3. **View Requests** (10 seconds)
   - "Manager views pending requests"
   - Execute GET /api/approvals?status=Pending

4. **Approve Request** (15 seconds)
   - "Manager approves with comments"
   - Execute POST /api/approvals/{id}/review

5. **Show History** (10 seconds)
   - "Full audit trail is maintained"
   - Execute GET /api/approvals/{id}/history
   - "Shows state transitions and actors"

6. **Test Business Rule** (15 seconds)
   - "Try to approve again - business rule prevents it"
   - Execute POST /api/approvals/{id}/review
   - "Returns 400 - cannot review approved request"

7. **Explain Architecture** (30 seconds)
   - "Built with Clean Architecture"
   - "CQRS pattern using MediatR"
   - "State machine with audit logging"
   - "Production-ready patterns"

**Total: ~2 minutes**

---

## 💡 Tips

- Keep Swagger UI open while testing
- Use "Try it out" button for each endpoint
- Copy IDs immediately after creating requests
- Test error cases to show validation works
- Check database after operations to verify data

---

**Ready to test?** → `dotnet run` → Open Swagger → Start with Step 1!
