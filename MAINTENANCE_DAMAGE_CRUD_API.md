# Maintenance & Vehicle Damage CRUD API Documentation

## 🎯 Overview

This document details the complete CRUD operations for **Maintenance** and **Vehicle Damage** management with **role-based access control**.

---

## 🔐 Role-Based Access Control

### Access Matrix:

| Operation | Admin | Employee | Customer |
|-----------|-------|----------|----------|
| **Maintenance** |
| View All | ✅ | ✅ | ❌ |
| View Specific | ✅ | ✅ | ❌ |
| Create | ✅ | ✅ | ❌ |
| Update | ✅ | ✅ | ❌ |
| Complete | ✅ | ✅ | ❌ |
| Cancel | ✅ | ❌ | ❌ |
| Delete | ✅ | ❌ | ❌ |
| **Vehicle Damage** |
| View All | ✅ | ✅ | ❌ |
| View Specific | ✅ | ✅ | ✅ (own rentals) |
| Create/Report | ✅ | ✅ | ✅ (for own rentals) |
| Update | ✅ | ✅ | ❌ |
| Start Repair | ✅ | ✅ | ❌ |
| Mark Repaired | ✅ | ✅ | ❌ |
| Delete | ✅ | ❌ | ❌ |

---

## 📋 MAINTENANCE API ENDPOINTS

### Base URL: `/api/maintenances`

---

### 1. GET All Maintenances

**Endpoint:** `GET /api/maintenances`

**Authorization:** Admin, Employee

**Query Parameters:**
```typescript
{
  vehicleId?: number,
  type?: number,        // 0=Routine, 1=Repair, 2=Inspection, 3=Emergency
  status?: number,      // 0=Scheduled, 1=InProgress, 2=Completed, 3=Cancelled
  startDate?: string,   // ISO date
  endDate?: string,     // ISO date
  isOverdue?: boolean
}
```

**Example Request:**
```http
GET /api/maintenances?vehicleId=1&status=0
Authorization: Bearer {token}
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "vehicleId": 1,
    "scheduledDate": "2024-12-01T00:00:00Z",
    "completedDate": null,
    "description": "Oil change and filter replacement",
    "cost": 85.00,
    "type": 0,
    "status": 0,
    "vehicle": {
      "id": 1,
      "brand": "Toyota",
      "model": "Corolla"
    }
  }
]
```

---

### 2. GET Maintenance by ID

**Endpoint:** `GET /api/maintenances/{id}`

**Authorization:** Admin, Employee

**Response:** `200 OK`
```json
{
  "id": 1,
  "vehicleId": 1,
  "scheduledDate": "2024-12-01T00:00:00Z",
  "completedDate": null,
  "description": "Oil change",
  "cost": 85.00,
  "type": 0,
  "status": 0,
  "vehicle": { ... }
}
```

**Errors:**
- `404 Not Found` - Maintenance not found
- `403 Forbidden` - Customer role trying to access

---

### 3. GET Maintenances by Vehicle

**Endpoint:** `GET /api/maintenances/vehicle/{vehicleId}`

**Authorization:** Admin, Employee

**Response:** `200 OK` - Array of maintenance records

---

### 4. GET Overdue Maintenances

**Endpoint:** `GET /api/maintenances/overdue`

**Authorization:** Admin, Employee

**Description:** Returns all scheduled maintenances with past due dates

**Response:** `200 OK` - Array of overdue maintenance records

---

### 5. GET Scheduled Maintenances

**Endpoint:** `GET /api/maintenances/scheduled?startDate=2024-12-01&endDate=2024-12-31`

**Authorization:** Admin, Employee

**Description:** Returns maintenances scheduled within date range

**Default:** Next 30 days if no dates provided

---

### 6. CREATE Maintenance

**Endpoint:** `POST /api/maintenances`

**Authorization:** Admin, Employee

**Request Body:**
```json
{
  "vehicleId": 1,
  "scheduledDate": "2024-12-15T10:00:00Z",
  "description": "Regular 10,000 km service",
  "cost": 150.00,
  "type": 0
}
```

**Response:** `201 Created`
```json
{
  "id": 5,
  "vehicleId": 1,
  "scheduledDate": "2024-12-15T10:00:00Z",
  "description": "Regular 10,000 km service",
  "cost": 150.00,
  "type": 0,
  "status": 0
}
```

**Business Logic:**
- Status automatically set to `Scheduled`
- If scheduled within 1 day and vehicle is Available, sets vehicle to `Maintenance` status

**Errors:**
- `404 Not Found` - Vehicle doesn't exist

---

### 7. UPDATE Maintenance

**Endpoint:** `PUT /api/maintenances/{id}`

**Authorization:** Admin, Employee

**Request Body:** (all fields optional)
```json
{
  "scheduledDate": "2024-12-16T10:00:00Z",
  "description": "Updated description",
  "cost": 175.00,
  "type": 1,
  "status": 1
}
```

**Response:** `200 OK` - Updated maintenance object

---

### 8. COMPLETE Maintenance

**Endpoint:** `PUT /api/maintenances/{id}/complete`

**Authorization:** Admin, Employee

**Request Body:**
```json
{
  "completedDate": "2024-12-01T14:30:00Z",
  "actualCost": 95.00,
  "notes": "Additional parts required"
}
```

**Business Logic:**
- Sets status to `Completed`
- Updates vehicle status back to `Available` if no other pending maintenances
- Can override cost with actual cost

**Response:** `200 OK`

**Errors:**
- `400 Bad Request` - Already completed

---

### 9. CANCEL Maintenance

**Endpoint:** `PUT /api/maintenances/{id}/cancel`

**Authorization:** Admin only

**Response:** `200 OK`

**Errors:**
- `400 Bad Request` - Cannot cancel completed maintenance

---

### 10. DELETE Maintenance

**Endpoint:** `DELETE /api/maintenances/{id}`

**Authorization:** Admin only

**Response:** `204 No Content`

---

## 🛠️ VEHICLE DAMAGE API ENDPOINTS

### Base URL: `/api/vehicledamages`

---

### 1. GET All Damages

**Endpoint:** `GET /api/vehicledamages`

**Authorization:** Admin, Employee

**Query Parameters:**
```typescript
{
  vehicleId?: number,
  rentalId?: number,
  severity?: number,    // 0=Minor, 1=Moderate, 2=Major, 3=Critical
  status?: number,      // 0=Reported, 1=UnderRepair, 2=Repaired, 3=Unresolved
  startDate?: string,
  endDate?: string,
  unresolvedOnly?: boolean
}
```

**Example Request:**
```http
GET /api/vehicledamages?vehicleId=1&unresolvedOnly=true
Authorization: Bearer {token}
```

**Response:** `200 OK` - Array of damage records

---

### 2. GET Damage by ID

**Endpoint:** `GET /api/vehicledamages/{id}`

**Authorization:** 
- Admin, Employee: Can view any damage
- Customer: Can view damages from their own rentals

**Response:** `200 OK`
```json
{
  "id": 1,
  "vehicleId": 1,
  "rentalId": 5,
  "reportedDate": "2024-11-20T00:00:00Z",
  "description": "Scratch on rear bumper",
  "severity": 0,
  "repairCost": 150.00,
  "repairedDate": null,
  "reportedBy": "john@example.com",
  "imageUrl": "https://...",
  "status": 1,
  "vehicle": { ... },
  "rental": {
    "id": 5,
    "customer": { ... }
  }
}
```

**Errors:**
- `403 Forbidden` - Customer accessing damage not from their rental

---

### 3. GET Damages by Vehicle

**Endpoint:** `GET /api/vehicledamages/vehicle/{vehicleId}`

**Authorization:** Admin, Employee

**Response:** `200 OK` - Array of damage records for the vehicle

---

### 4. GET Damages by Rental

**Endpoint:** `GET /api/vehicledamages/rental/{rentalId}`

**Authorization:**
- Admin, Employee: Can view any rental's damages
- Customer: Can view damages from their own rental only

**Response:** `200 OK` - Array of damage records

---

### 5. GET Unresolved Damages

**Endpoint:** `GET /api/vehicledamages/unresolved`

**Authorization:** Admin, Employee

**Description:** Returns all damages that are NOT repaired (Reported, UnderRepair, Unresolved)

**Response:** `200 OK` - Array of unresolved damages

---

### 6. CREATE/REPORT Damage

**Endpoint:** `POST /api/vehicledamages`

**Authorization:**
- Admin, Employee: Can report damage for any vehicle/rental
- Customer: Can only report damage for their own rentals (rentalId required)

**Request Body:**
```json
{
  "vehicleId": 1,
  "rentalId": 5,
  "reportedDate": "2024-11-20T00:00:00Z",
  "description": "Small dent on driver's side door",
  "severity": 1,
  "repairCost": 250.00,
  "reportedBy": "John Doe",
  "imageUrl": "https://..."
}
```

**Business Logic:**
- Status automatically set to `Reported`
- `reportedBy` defaults to current user's email if not provided
- If severity is Major or Critical, vehicle status set to `Maintenance`
- Customers MUST provide rentalId and must own that rental

**Response:** `201 Created`

**Errors:**
- `400 Bad Request` - Customer didn't provide rental ID
- `403 Forbidden` - Customer reporting for someone else's rental

---

### 7. UPDATE Damage

**Endpoint:** `PUT /api/vehicledamages/{id}`

**Authorization:** Admin, Employee

**Request Body:** (all fields optional)
```json
{
  "description": "Updated description",
  "severity": 2,
  "repairCost": 300.00,
  "status": 1
}
```

**Response:** `200 OK` - Updated damage object

---

### 8. START REPAIR

**Endpoint:** `PUT /api/vehicledamages/{id}/start-repair`

**Authorization:** Admin, Employee

**Description:** Changes status from Reported to UnderRepair

**Business Logic:**
- Sets vehicle to `Maintenance` status if currently Available

**Response:** `200 OK`

**Errors:**
- `400 Bad Request` - Can only start repair on reported damages

---

### 9. MARK AS REPAIRED

**Endpoint:** `PUT /api/vehicledamages/{id}/repair`

**Authorization:** Admin, Employee

**Request Body:**
```json
{
  "repairedDate": "2024-11-25T00:00:00Z",
  "actualRepairCost": 275.00,
  "repairNotes": "Replaced door panel"
}
```

**Business Logic:**
- Sets status to `Repaired`
- Updates repair cost if provided
- Checks if vehicle can be set back to `Available`:
  - No other unresolved damages
  - No pending maintenances
  
**Response:** `200 OK`

**Errors:**
- `400 Bad Request` - Already repaired

---

### 10. DELETE Damage

**Endpoint:** `DELETE /api/vehicledamages/{id}`

**Authorization:** Admin only

**Response:** `204 No Content`

---

## 🎯 Business Rules

### Maintenance Rules:

1. **Vehicle Status Management:**
   - When maintenance is scheduled within 1 day → Vehicle set to `Maintenance`
   - When maintenance is completed → Vehicle set to `Available` (if no other pending work)
   - Cancelled maintenance doesn't affect vehicle status

2. **Status Transitions:**
   - `Scheduled` → `InProgress` → `Completed`
   - `Scheduled` → `Cancelled` (Admin only)
   - Cannot cancel completed maintenance

3. **Overdue Logic:**
   - Maintenance is overdue if `status == Scheduled` AND `scheduledDate < today`

### Vehicle Damage Rules:

1. **Vehicle Status Management:**
   - Major/Critical damage → Vehicle immediately set to `Maintenance`
   - Starting repair → Vehicle set to `Maintenance`
   - Completing repair → Check if vehicle can return to `Available`

2. **Status Transitions:**
   - `Reported` → `UnderRepair` → `Repaired`
   - `Reported` → `Unresolved` (if cannot be repaired)

3. **Customer Permissions:**
   - Can only report damages for rentals they own
   - Can only view damages from their rentals
   - Must link damage to a rental (cannot report general damages)

4. **Repair Completion:**
   - Vehicle returns to `Available` only if:
     - All damages are repaired
     - No pending maintenances exist

---

## 💡 Usage Examples

### Example 1: Schedule Routine Maintenance

```http
POST /api/maintenances
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "vehicleId": 1,
  "scheduledDate": "2024-12-10T09:00:00Z",
  "description": "5,000 km service - oil change, brake check, tire rotation",
  "cost": 120.00,
  "type": 0
}
```

### Example 2: Customer Reports Damage

```http
POST /api/vehicledamages
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "vehicleId": 1,
  "rentalId": 23,
  "reportedDate": "2024-11-28T00:00:00Z",
  "description": "Discovered scratch on passenger door when returning vehicle",
  "severity": 0,
  "repairCost": 100.00
}
```

### Example 3: Employee Starts Repair

```http
PUT /api/vehicledamages/15/start-repair
Authorization: Bearer {employee_token}
```

### Example 4: Complete Repair with Actual Cost

```http
PUT /api/vehicledamages/15/repair
Authorization: Bearer {employee_token}
Content-Type: application/json

{
  "repairedDate": "2024-11-29T00:00:00Z",
  "actualRepairCost": 125.00,
  "repairNotes": "Required additional buffing compound"
}
```

### Example 5: Get Overdue Maintenances

```http
GET /api/maintenances/overdue
Authorization: Bearer {admin_token}
```

---

## 🧪 Testing Checklist

### Maintenance:
- [ ] Admin can create, update, complete, cancel, delete
- [ ] Employee can create, update, complete (not cancel/delete)
- [ ] Customer cannot access any maintenance endpoints
- [ ] Vehicle status changes appropriately
- [ ] Overdue maintenances are correctly identified
- [ ] Cannot cancel completed maintenance
- [ ] Completing maintenance returns vehicle to Available (when appropriate)

### Vehicle Damage:
- [ ] Admin can do everything
- [ ] Employee can report, update, repair (not delete)
- [ ] Customer can report for own rentals only
- [ ] Customer can view own rental damages only
- [ ] Customer must provide rental ID when reporting
- [ ] Major/Critical damage sets vehicle to Maintenance
- [ ] Repairing damage returns vehicle to Available (when appropriate)
- [ ] Cannot mark already-repaired damage as repaired again

---

## 📚 Related Documentation

- `VEHICLE_HISTORY_API.md` - Vehicle history endpoints
- `VEHICLE_HISTORY_FRONTEND_GUIDE.md` - Frontend integration
- `IMPLEMENTATION_COMPLETE.md` - Complete feature overview

---

**API Version:** 1.0  
**Last Updated:** 2024-11-28  
**Status:** ✅ Complete - Ready for Testing
