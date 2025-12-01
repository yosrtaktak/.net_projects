# Quick Start - Role-Based Layouts

## 🎯 What Changed?

Your Car Rental System now has **3 different layouts** based on user role and page type:

### 1. 👨‍💼 Admin/Employee Portal (Dashboard with Sidebar)
- Sidebar navigation on the left
- User info and logout in sidebar
- Dashboard landing page
- Full management features

### 2. 👤 Customer Portal (Top Navigation)
- Navigation bar at top
- User menu dropdown
- Home page landing
- Vehicle browsing focus
- Footer with contact info

### 3. 🔐 Auth Pages (No Navigation)
- Login page - standalone
- Register page - standalone
- Centered design
- Large branding

---

## 🚀 How to Test

### Start the App
```bash
# Terminal 1
cd Backend
dotnet run

# Terminal 2
cd Frontend
dotnet run
```

Open: http://localhost:5001

---

## 🧪 Test Each Role

### 1. Test Admin/Employee Portal

**Steps:**
1. Go to http://localhost:5001/login
2. Click **"Admin Portal"** button (or **"Employee Portal"**)
3. You'll see: ✅ Dashboard with sidebar navigation

**What to Check:**
- ✅ Sidebar on the left with navigation menu
- ✅ Dashboard, Vehicles, Rentals, Customers, Maintenance, Damages links
- ✅ User avatar with initials at bottom
- ✅ Logout button in sidebar
- ✅ Statistics cards showing vehicle/rental counts
- ✅ Quick action buttons

**Try Navigating:**
- Click different menu items in sidebar
- Check that sidebar stays visible
- Test logout button

---

### 2. Test Customer Portal

**Steps:**
1. Go to http://localhost:5001/login
2. Click **"Customer Portal"** button
3. You'll see: ✅ Home page with top navigation

**What to Check:**
- ✅ Navigation bar at top
- ✅ Home, Vehicles, My Rentals buttons
- ✅ User menu (👤) in top-right
- ✅ Beautiful hero section
- ✅ Feature cards
- ✅ Footer at bottom

**Try Navigating:**
- Click user menu dropdown
- Navigate to Vehicles, Rentals
- Test on mobile (resize browser)
- Check mobile hamburger menu

---

### 3. Test Auth Pages

**Steps:**
1. Logout from any account
2. Go to http://localhost:5001/login

**What to Check:**
- ✅ No navigation bar or sidebar
- ✅ Centered login form
- ✅ Large car icon at top
- ✅ Quick login buttons
- ✅ Link to register page

**Also Check Register:**
1. Click "Register here" link
2. See register form - same style
3. No navigation visible

---

## 📊 Layout Comparison

```
┌─────────────────────────────────────────────┐
│         ADMIN/EMPLOYEE PORTAL               │
├──────────┬──────────────────────────────────┤
│          │                                  │
│ SIDEBAR  │     DASHBOARD CONTENT            │
│          │                                  │
│ • Dash   │  [Stats Cards]                   │
│ • Cars   │                                  │
│ • Rent   │  [Quick Actions]                 │
│ • Cust   │                                  │
│          │  [Status Overview]               │
│ [User]   │                                  │
│ [Logout] │                                  │
└──────────┴──────────────────────────────────┘
```

```
┌─────────────────────────────────────────────┐
│         CUSTOMER PORTAL                     │
├─────────────────────────────────────────────┤
│ [Logo] Home  Vehicles  Rentals      [User▼]│
├─────────────────────────────────────────────┤
│                                             │
│            PAGE CONTENT                     │
│         (Hero, Cards, etc.)                 │
│                                             │
├─────────────────────────────────────────────┤
│              FOOTER                         │
│     Links | Contact | Copyright             │
└─────────────────────────────────────────────┘
```

```
┌─────────────────────────────────────────────┐
│         LOGIN/REGISTER PAGE                 │
│                                             │
│            [🚗 CAR ICON]                    │
│                                             │
│          ┌─────────────────┐               │
│          │   LOGIN FORM    │               │
│          │                 │               │
│          │   [Username]    │               │
│          │   [Password]    │               │
│          │                 │               │
│          │   [Login Btn]   │               │
│          │                 │               │
│          │ Quick Login:    │               │
│          │  [Admin]        │               │
│          │  [Employee]     │               │
│          │  [Customer]     │               │
│          └─────────────────┘               │
│                                             │
└─────────────────────────────────────────────┘
```

---

## 🎨 Visual Differences

### Admin/Employee
```
✨ Style: Professional, data-focused
🎨 Color: Blue theme
📍 Navigation: Left sidebar (always visible)
📊 Features: Stats, tables, management tools
👤 User Menu: In sidebar with avatar
```

### Customer
```
✨ Style: Friendly, browsing-focused
🎨 Color: Gradient hero, welcoming
📍 Navigation: Top bar (horizontal)
📊 Features: Vehicle cards, simple booking
👤 User Menu: Top-right dropdown
📄 Footer: Contact info, links
```

### Auth Pages
```
✨ Style: Minimal, centered
🎨 Color: Clean white card
📍 Navigation: None
📊 Features: Form only
🎯 Focus: Login/register action
```

---

## 🔄 Login Flow

```
1. Visit /login
   └─ EmptyLayout (no navigation)

2. Click Quick Login:
   ├─ Admin → Redirect to /admin
   │           └─ AdminLayout (sidebar)
   │
   ├─ Employee → Redirect to /admin
   │             └─ AdminLayout (sidebar)
   │
   └─ Customer → Redirect to /
                 └─ CustomerLayout (top nav)

3. Navigate around:
   - Admin/Employee: Use sidebar
   - Customer: Use top navigation
```

---

## 📱 Mobile Experience

### Admin/Employee
- Hamburger button to toggle sidebar
- Sidebar slides in/out
- Full-width content when sidebar hidden

### Customer
- Hamburger menu for navigation
- Collapsible menu drawer
- Footer stacks vertically

### Auth Pages
- Responsive form width
- Touch-friendly buttons
- Centered on all screen sizes

---

## 🎯 Quick Login Credentials

| Portal | Username | Password | Redirect |
|--------|----------|----------|----------|
| 👨‍💼 Admin | admin | Admin@123 | `/admin` |
| 💼 Employee | employee | Employee@123 | `/admin` |
| 👤 Customer | customer | Customer@123 | `/` |

---

## ✅ Success Indicators

### Admin Portal Working:
- ✅ See sidebar on left
- ✅ Dashboard shows statistics
- ✅ User info in sidebar footer
- ✅ Can navigate to all management pages

### Customer Portal Working:
- ✅ See navigation bar at top
- ✅ Home page with hero section
- ✅ User menu in top-right
- ✅ Footer at bottom

### Auth Pages Working:
- ✅ No navigation visible
- ✅ Form is centered
- ✅ Quick login buttons work
- ✅ Redirects after login

---

## 🐛 Troubleshooting

### Login redirects to wrong page?
Check the Login.razor code handles role-based redirect:
- Admin/Employee → `/admin`
- Customer → `/`

### Sidebar not showing?
1. Make sure you logged in as admin/employee
2. Check you're on an admin page (`/admin`, `/customers`, etc.)
3. Try refreshing the page

### Top nav not showing?
1. Make sure you're on a customer page (`/`, `/vehicles`)
2. Check login was successful
3. Verify CustomerLayout is applied

### No navigation at all?
1. Check if you're on login/register page (correct behavior)
2. Try logging in first

---

## 📚 File Reference

**Layouts:**
- `Frontend/Layout/AdminLayout.razor` - Admin/Employee
- `Frontend/Layout/CustomerLayout.razor` - Customer
- `Frontend/Layout/EmptyLayout.razor` - Auth pages

**Updated Pages:**
- `Frontend/Pages/Login.razor` - EmptyLayout
- `Frontend/Pages/Register.razor` - EmptyLayout
- `Frontend/Pages/Home.razor` - CustomerLayout
- `Frontend/Pages/AdminDashboard.razor` - AdminLayout

**Configuration:**
- `Frontend/App.razor` - Dynamic layout assignment

---

## 🎉 That's It!

Your role-based layout system is ready! 

**Try it now:**
1. Start both backend and frontend
2. Visit http://localhost:5001/login
3. Click each quick login button
4. See the different experiences!

Each role now has a tailored interface! 🎊
