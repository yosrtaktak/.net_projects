# ?? Quick Reference: Admin Features

## Customer Edit Feature

### Access:
```
1. Login as Admin
2. Navigate to /customers
3. Click any customer
4. Click "Edit Customer" button
```

### What You Can Edit:
- ?? First Name & Last Name
- ?? Phone Number
- ?? Driver License Number
- ?? Date of Birth
- ?? Address
- ? **Membership Tier** (Standard ? Silver ? Gold ? Platinum)

### API Endpoint:
```http
PUT /api/users/{userId}/customer
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "driverLicenseNumber": "DL123456",
  "dateOfBirth": "1990-01-01",
  "address": "123 Main St",
  "tier": "Gold"
}
```

---

## Employee Management

### Access:
```
1. Login as Admin
2. Click Sidebar ? System Management ? Employees
3. Navigate to /admin/employees
```

### Features:

#### ?? View All Staff
- See all employees and admins
- View roles, contact info, join dates
- Real-time search

#### ? Create Employee
```
Click "Add Employee"
Fill form:
  - First Name
  - Last Name
  - Email
  - Phone Number
  - Password
  - Role (Employee or Admin)
Click "Create Employee"
```

#### ?? Change Role
```
Click role change icon (??)
Select new role:
  - Employee
  - Admin
  - Customer
Click "Change Role"
```

#### ??? Delete Employee
```
Click delete icon (???)
Confirm deletion
Note: Cannot delete if user has active rentals
```

### API Endpoints:

#### List Employees:
```http
GET /api/users/employees
Authorization: Bearer {admin-token}
```

#### Create Employee:
```http
POST /api/users/employees
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "email": "newemployee@test.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+1234567890",
  "password": "SecurePass123!",
  "role": "Employee"
}
```

#### Change Role:
```http
PUT /api/users/{userId}/role
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "role": "Admin"
}
```

#### Delete Employee:
```http
DELETE /api/users/{userId}
Authorization: Bearer {admin-token}
```

---

## Navigation Quick Guide

### Admin Sidebar Structure:
```
?? Dashboard
?? Fleet Management
  ??? Manage Vehicles
  ??? Categories
  ??? Maintenance
  ??? Damages
?? Business Management
  ??? All Rentals
  ??? Customers          ? Customer edit here
  ??? Reports
?? System Management      ? NEW SECTION
  ??? Employees           ? Employee management
```

---

## Color Coding

### Membership Tiers:
- ?? **Standard**: Default (Gray)
- ?? **Silver**: Secondary (Light Gray)
- ?? **Gold**: Warning (Yellow)
- ?? **Platinum**: Primary (Blue)

### User Roles:
- ?? **Admin**: Error (Red)
- ?? **Employee**: Info (Blue)
- ? **Customer**: Default (Gray)

---

## Common Tasks

### Upgrade Customer to Gold:
```
1. Go to /customers
2. Click customer
3. Click "Edit Customer"
4. Change Tier to "Gold"
5. Click "Save Changes"
```

### Create Admin User:
```
1. Go to /admin/employees
2. Click "Add Employee"
3. Fill details
4. Set Role to "Admin"
5. Click "Create Employee"
```

### Demote Admin to Employee:
```
1. Go to /admin/employees
2. Find admin in list
3. Click role change icon (??)
4. Select "Employee"
5. Click "Change Role"
```

---

## Validation Rules

### Customer Edit:
- First Name: 2-50 characters, required
- Last Name: 2-50 characters, required
- Phone: Valid phone format, required
- License: Max 20 characters
- Date of Birth: Must be 18+ years old
- Address: Max 200 characters

### Employee Creation:
- Email: Valid email, unique, required
- First Name: 2-50 characters, required
- Last Name: 2-50 characters, required
- Phone: Valid phone format, required
- Password: Min 6 characters, required
- Role: Must be "Employee" or "Admin"

---

## Troubleshooting

### "Cannot delete employee"
**Cause**: Employee has active or reserved rentals
**Solution**: Complete or cancel their rentals first

### "Email already exists"
**Cause**: Email is registered to another user
**Solution**: Use a different email

### "Edit button not showing"
**Cause**: Not logged in as Admin
**Solution**: Login as Admin user

### "Employees link not in sidebar"
**Cause**: Not an Admin
**Solution**: Only Admins see System Management section

---

## Quick Test

### Test Customer Edit:
```bash
# 1. Login as admin
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin@123"}'

# 2. Edit customer (replace {userId} and {token})
curl -X PUT http://localhost:5000/api/users/{userId}/customer \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Updated",
    "lastName": "Customer",
    "phoneNumber": "+1234567890",
    "driverLicenseNumber": "DL999",
    "dateOfBirth": "1990-01-01",
    "address": "New Address",
    "tier": "Gold"
  }'
```

### Test Employee Creation:
```bash
# Create employee (with admin token)
curl -X POST http://localhost:5000/api/users/employees \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newemployee@test.com",
    "firstName": "New",
    "lastName": "Employee",
    "phoneNumber": "+1234567890",
    "password": "Test@123",
    "role": "Employee"
  }'
```

---

## Default Admin Credentials
```
Email: admin@test.com
Password: Admin@123
```

---

## ?? Success Indicators

### Customer Edit Working:
- ? Edit button visible on customer details page
- ? Dialog opens with pre-filled data
- ? Changes save successfully
- ? Snackbar shows success message
- ? Customer data refreshes automatically

### Employee Management Working:
- ? Employees page loads at /admin/employees
- ? Statistics cards show correct counts
- ? Can create new employees
- ? Can change roles
- ? Can delete employees without rentals
- ? Search filters work in real-time

---

## ?? UI Layouts

### Customer Edit Dialog:
```
???????????????????????????????
? ?? Edit Customer            ?
???????????????????????????????
? [First Name]  [Last Name]   ?
? [Phone]       [License]     ?
? [Date of Birth] [Tier ?]    ?
? [Address (multi-line)]      ?
???????????????????????????????
?        [Cancel] [Save]      ?
???????????????????????????????
```

### Employee Management Page:
```
???????????????????????????????????????
? ?? Employee Management              ?
?                      [Add Employee] ?
???????????????????????????????????????
? [?? Total] [?? Admins] [?? Staff]  ?
???????????????????????????????????????
? [?? Search...]                      ?
???????????????????????????????????????
? Name    Contact  Role   Actions     ?
? ?????   ????????  ????   ????????   ?
? John    email@    Admin  [??][???]  ?
? Jane    email@    Emp    [??][???]  ?
???????????????????????????????????????
```

---

*For detailed documentation, see: `ADMIN_CUSTOMER_EMPLOYEE_MANAGEMENT_COMPLETE.md`*
