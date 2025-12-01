# Vehicle History API Endpoints Documentation

## 📋 Overview

The Vehicle History management system provides four main API endpoints for retrieving detailed history information for each vehicle:

1. **Complete History** - All data in one call
2. **Rentals Only** - Rental history with statistics
3. **Maintenances Only** - Maintenance records with statistics
4. **Damages Only** - Damage reports with statistics

---

## 🔐 Authentication

All endpoints require **Admin role** authorization:
```
Authorization: Bearer {JWT_TOKEN}
Role: Admin
```

---

## 📡 API Endpoints

### 1. Get Complete Vehicle History

**Endpoint:** `GET /api/vehicles/{id}/history`

**Description:** Retrieves all history data for a vehicle including rentals, maintenances, damages, and mileage evolution.

**Request:**
```http
GET /api/vehicles/1/history
Authorization: Bearer {token}
```

**Response:**
```json
{
  "vehicle": {
    "id": 1,
    "brand": "Toyota",
    "model": "Corolla",
    "registrationNumber": "ABC123",
    "year": 2023,
    "category": 1,
    "dailyRate": 35.00,
    "status": 0,
    "imageUrl": null,
    "mileage": 16000,
    "fuelType": "Gasoline",
    "seatingCapacity": 5
  },
  "rentals": [...],
  "maintenanceRecords": [...],
  "damageRecords": [...],
  "mileageEvolution": {
    "currentMileage": 16000,
    "initialMileage": 15000,
    "dataPoints": [...],
    "averageMileagePerRental": 250,
    "totalMileageDriven": 1000
  }
}
```

---

### 2. Get Vehicle Rentals

**Endpoint:** `GET /api/vehicles/{id}/rentals`

**Description:** Retrieves all rental records for a specific vehicle with detailed statistics.

**Request:**
```http
GET /api/vehicles/1/rentals
Authorization: Bearer {token}
```

**Response:**
```json
{
  "vehicleId": 1,
  "totalRentals": 4,
  "completedRentals": 4,
  "totalRevenue": 770.00,
  "totalDistanceDriven": 1000,
  "rentals": [
    {
      "id": 4,
      "startDate": "2024-04-01T00:00:00",
      "endDate": "2024-04-05T00:00:00",
      "actualReturnDate": "2024-04-06T00:00:00",
      "totalCost": 175.00,
      "status": 2,
      "startMileage": 15820,
      "endMileage": 16000,
      "notes": "Returned 1 day late, extra charges applied",
      "createdAt": "2024-03-28T00:00:00",
      "customer": {
        "id": 1,
        "firstName": "John",
        "lastName": "Doe",
        "email": "john.doe@example.com",
        "phoneNumber": "+1234567890"
      },
      "distanceDriven": 180,
      "daysRented": 5
    },
    // ... more rentals
  ]
}
```

**Statistics Included:**
- `totalRentals` - Total number of rentals
- `completedRentals` - Number of completed rentals
- `totalRevenue` - Total revenue from completed rentals
- `totalDistanceDriven` - Total kilometers driven across all rentals
- `distanceDriven` - Distance per rental (EndMileage - StartMileage)
- `daysRented` - Duration of each rental

---

### 3. Get Vehicle Maintenances

**Endpoint:** `GET /api/vehicles/{id}/maintenances`

**Description:** Retrieves all maintenance records for a specific vehicle with categorized statistics.

**Request:**
```http
GET /api/vehicles/1/maintenances
Authorization: Bearer {token}
```

**Response:**
```json
{
  "vehicleId": 1,
  "totalMaintenances": 4,
  "completedMaintenances": 3,
  "scheduledMaintenances": 1,
  "inProgressMaintenances": 0,
  "overdueMaintenances": 0,
  "totalMaintenanceCost": 455.00,
  "maintenances": [
    {
      "id": 4,
      "scheduledDate": "2024-05-01T00:00:00",
      "completedDate": null,
      "description": "Scheduled air conditioning service",
      "cost": 150.00,
      "type": 0,
      "status": 0,
      "daysToComplete": null,
      "isOverdue": false,
      "typeName": "Routine",
      "statusName": "Scheduled"
    },
    {
      "id": 3,
      "scheduledDate": "2024-03-15T00:00:00",
      "completedDate": "2024-03-15T00:00:00",
      "description": "Annual vehicle inspection",
      "cost": 50.00,
      "type": 2,
      "status": 2,
      "daysToComplete": 0,
      "isOverdue": false,
      "typeName": "Inspection",
      "statusName": "Completed"
    },
    // ... more maintenances
  ]
}
```

**Statistics Included:**
- `totalMaintenances` - Total maintenance records
- `completedMaintenances` - Number of completed services
- `scheduledMaintenances` - Number of scheduled future services
- `inProgressMaintenances` - Number of ongoing services
- `overdueMaintenances` - Number of overdue scheduled services
- `totalMaintenanceCost` - Total cost of completed maintenances

**Maintenance Types:**
- `0` = Routine
- `1` = Repair
- `2` = Inspection
- `3` = Emergency

**Maintenance Status:**
- `0` = Scheduled
- `1` = InProgress
- `2` = Completed
- `3` = Cancelled

---

### 4. Get Vehicle Damages

**Endpoint:** `GET /api/vehicles/{id}/damages`

**Description:** Retrieves all damage reports for a specific vehicle with severity and repair statistics.

**Request:**
```http
GET /api/vehicles/1/damages
Authorization: Bearer {token}
```

**Response:**
```json
{
  "vehicleId": 1,
  "totalDamages": 3,
  "repairedDamages": 2,
  "underRepairDamages": 1,
  "unresolvedDamages": 0,
  "totalRepairCost": 600.00,
  "minorDamages": 2,
  "moderateDamages": 1,
  "majorDamages": 0,
  "criticalDamages": 0,
  "damages": [
    {
      "id": 3,
      "reportedDate": "2024-04-10T00:00:00",
      "description": "Windshield chip from road debris",
      "severity": 0,
      "repairCost": 200.00,
      "repairedDate": null,
      "reportedBy": "Employee",
      "imageUrl": null,
      "status": 1,
      "rentalId": null,
      "severityName": "Minor",
      "statusName": "UnderRepair",
      "daysToRepair": null,
      "isUnderRepair": true,
      "relatedRental": null
    },
    {
      "id": 2,
      "reportedDate": "2024-03-12T00:00:00",
      "description": "Dent on driver's side door, moderate damage",
      "severity": 1,
      "repairCost": 450.00,
      "repairedDate": "2024-03-20T00:00:00",
      "reportedBy": "Admin Staff",
      "imageUrl": null,
      "status": 2,
      "rentalId": 3,
      "severityName": "Moderate",
      "statusName": "Repaired",
      "daysToRepair": 8,
      "isUnderRepair": false,
      "relatedRental": {
        "id": 3,
        "startDate": "2024-03-05T00:00:00",
        "endDate": "2024-03-12T00:00:00",
        // ... rental details
      }
    },
    // ... more damages
  ]
}
```

**Statistics Included:**
- `totalDamages` - Total damage reports
- `repairedDamages` - Number of repaired damages
- `underRepairDamages` - Number of damages currently being repaired
- `unresolvedDamages` - Number of unresolved damage reports
- `totalRepairCost` - Total cost of all repairs (completed only)
- `minorDamages` - Count by severity level
- `moderateDamages` - Count by severity level
- `majorDamages` - Count by severity level
- `criticalDamages` - Count by severity level

**Damage Severity:**
- `0` = Minor (scratches, small dents)
- `1` = Moderate (larger dents, broken lights)
- `2` = Major (body damage, mechanical issues)
- `3` = Critical (severe damage requiring extensive repair)

**Damage Status:**
- `0` = Reported
- `1` = UnderRepair
- `2` = Repaired
- `3` = Unresolved

---

## 📊 Usage Examples

### Frontend (Blazor) Usage

```csharp
// Get complete history
var historyResponse = await Http.GetFromJsonAsync<VehicleHistoryResponse>(
    $"api/vehicles/{vehicleId}/history"
);

// Get only rentals
var rentalsResponse = await Http.GetFromJsonAsync<VehicleRentalsResponse>(
    $"api/vehicles/{vehicleId}/rentals"
);

// Get only maintenances
var maintenancesResponse = await Http.GetFromJsonAsync<VehicleMaintenancesResponse>(
    $"api/vehicles/{vehicleId}/maintenances"
);

// Get only damages
var damagesResponse = await Http.GetFromJsonAsync<VehicleDamagesResponse>(
    $"api/vehicles/{vehicleId}/damages"
);
```

### JavaScript/Fetch Usage

```javascript
// Get rentals with statistics
fetch('/api/vehicles/1/rentals', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
.then(response => response.json())
.then(data => {
  console.log(`Total Rentals: ${data.totalRentals}`);
  console.log(`Total Revenue: $${data.totalRevenue}`);
  console.log(`Distance Driven: ${data.totalDistanceDriven} km`);
});
```

---

## 🧪 Testing the Endpoints

### Using cURL

```bash
# Get vehicle history
curl -X GET "https://localhost:5000/api/vehicles/1/history" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get rentals
curl -X GET "https://localhost:5000/api/vehicles/1/rentals" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get maintenances
curl -X GET "https://localhost:5000/api/vehicles/1/maintenances" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get damages
curl -X GET "https://localhost:5000/api/vehicles/1/damages" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Using Swagger/OpenAPI

1. Start the backend
2. Navigate to `https://localhost:5000/swagger`
3. Authorize with your JWT token
4. Try the endpoints under the "Vehicles" section

---

## 🎯 Response Codes

| Code | Description |
|------|-------------|
| 200 | Success - Returns the requested data |
| 401 | Unauthorized - Missing or invalid JWT token |
| 403 | Forbidden - User doesn't have Admin role |
| 404 | Not Found - Vehicle with specified ID doesn't exist |
| 500 | Internal Server Error - Check server logs |

---

## 📈 Performance Considerations

### Optimization Tips:

1. **Pagination** (Future Enhancement):
   - Add `page` and `pageSize` query parameters for large datasets
   - Example: `/api/vehicles/1/rentals?page=1&pageSize=10`

2. **Filtering** (Future Enhancement):
   - Add date range filters: `?startDate=2024-01-01&endDate=2024-12-31`
   - Add status filters: `?status=Completed`

3. **Caching**:
   - Consider caching frequently accessed vehicle history
   - Use ETag headers for conditional requests

4. **Lazy Loading**:
   - The complete history endpoint loads all relations
   - Use specific endpoints (rentals/maintenances/damages) for faster responses

---

## 🔄 Related Endpoints

These endpoints work in conjunction with:

- `GET /api/vehicles` - List all vehicles
- `GET /api/vehicles/{id}` - Get vehicle details
- `GET /api/rentals` - Manage rentals
- `POST /api/maintenances` - Create maintenance records (if implemented)
- `POST /api/damages` - Report damages (if implemented)

---

## 📝 Notes

1. All dates are returned in ISO 8601 format (UTC)
2. All monetary values are in decimal format with 2 decimal places
3. Navigation properties (Customer, Rental) are included where relevant
4. Calculated fields (distanceDriven, daysRented, etc.) are computed on the server
5. Enum values are returned as integers with corresponding string names

---

## ✅ Testing Checklist

Before deployment, verify:

- [ ] All endpoints return 200 for valid requests
- [ ] Proper 404 returned for non-existent vehicles
- [ ] Proper 401/403 for unauthorized access
- [ ] Statistics calculations are accurate
- [ ] Related entities (Customer, Rental) are properly loaded
- [ ] Date/time handling is correct
- [ ] Decimal precision is maintained for costs
- [ ] Enum values and names are correct
- [ ] No N+1 query problems (check with SQL profiler)

---

## 🚀 Next Steps

After implementing these endpoints, consider adding:

1. **Export functionality** - Export history to PDF/Excel
2. **Charts/Graphs** - Visual representation of history data
3. **Notifications** - Alert for overdue maintenance or unrepaired damages
4. **Analytics** - Vehicle performance metrics and trends
5. **Comparison** - Compare history across multiple vehicles
6. **Reports** - Generate detailed reports for management

---

**Documentation Version:** 1.0  
**Last Updated:** 2024-11-28  
**API Version:** v1
