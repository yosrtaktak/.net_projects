# Quick Test: Customer Booking with Tier-Based Pricing

## Test Scenario Setup

### Test Users with Different Tiers

You'll need to test with customers at different tier levels to verify the pricing logic.

#### Update Customer Tiers (SQL)
```sql
USE CarRentalDB;

-- Set different tier levels for testing
UPDATE AspNetUsers SET Tier = 0 WHERE Email = 'standard@test.com';  -- Standard (no discount)
UPDATE AspNetUsers SET Tier = 1 WHERE Email = 'silver@test.com';    -- Silver (5% off)
UPDATE AspNetUsers SET Tier = 2 WHERE Email = 'gold@test.com';      -- Gold (10% off)
UPDATE AspNetUsers SET Tier = 3 WHERE Email = 'platinum@test.com';  -- Platinum (15% off)
```

---

## Test 1: Standard Tier Customer (No Discount)

### Steps:
1. Login as `standard@test.com`
2. Go to "Browse Vehicles"
3. Click "Rent Now" on a vehicle with daily rate $100
4. Select dates: 3 days apart

### Expected Result:
```
? Your Information Card:
   - Tier: Standard badge (default color)
   - NO discount alert shown

? Price Summary:
   - Rental Duration: 3 days
   - Daily Rate: $100.00
   - Total Cost: $300.00
   
? NO base amount or discount line shown
? Confirm button: "Confirm Booking - $300.00"
```

---

## Test 2: Silver Tier Customer (5% Discount)

### Steps:
1. Login as `silver@test.com`
2. Go to "Browse Vehicles"
3. Click "Rent Now" on a vehicle with daily rate $100
4. Select dates: 3 days apart

### Expected Result:
```
? Your Information Card:
   - Tier: Silver badge (info color)
   - Discount alert: "?? You get 5% off with Silver Tier!"

? Price Summary:
   - Rental Duration: 3 days
   - Daily Rate: $100.00
   - Base Amount: $300.00
   - ??? Silver Discount (5%): -$15.00
   ?????????????????????
   - Total Cost: $285.00
   
? Confirm button: "Confirm Booking - $285.00"
```

---

## Test 3: Gold Tier Customer (10% Discount)

### Steps:
1. Login as `gold@test.com`
2. Go to "Browse Vehicles"
3. Click "Rent Now" on a vehicle with daily rate $100
4. Select dates: 3 days apart

### Expected Result:
```
? Your Information Card:
   - Tier: Gold badge (warning color)
   - Discount alert: "?? You get 10% off with Gold Tier!"

? Price Summary:
   - Rental Duration: 3 days
   - Daily Rate: $100.00
   - Base Amount: $300.00
   - ??? Gold Discount (10%): -$30.00
   ?????????????????????
   - Total Cost: $270.00
   
? Confirm button: "Confirm Booking - $270.00"
```

---

## Test 4: Platinum Tier Customer (15% Discount)

### Steps:
1. Login as `platinum@test.com`
2. Go to "Browse Vehicles"
3. Click "Rent Now" on a vehicle with daily rate $100
4. Select dates: 3 days apart

### Expected Result:
```
? Your Information Card:
   - Tier: Platinum badge (primary color)
   - Discount alert: "?? You get 15% off with Platinum Tier!"

? Price Summary:
   - Rental Duration: 3 days
   - Daily Rate: $100.00
   - Base Amount: $300.00
   - ??? Platinum Discount (15%): -$45.00
   ?????????????????????
   - Total Cost: $255.00
   
? Confirm button: "Confirm Booking - $255.00"
```

---

## Test 5: Complete Booking Flow

### Steps:
1. Login as any tier customer
2. Book a vehicle for 5 days (e.g., Dec 10-15)
3. Click "Confirm Booking"
4. Wait for success message
5. Navigate to "My Rentals"

### Expected Result:
```
? Success message: "Booking confirmed! Total: $XXX.XX"
? Redirected to /my-rentals after 2 seconds
? New booking appears in My Rentals with:
   - Status: Reserved
   - Total Cost: Matches booking page amount
   - Dates: Correct pick-up and return dates
```

---

## Test 6: Price Recalculation

### Steps:
1. Login as Gold tier customer (10% off)
2. Go to booking page
3. Select vehicle ($100/day)
4. Select dates: 3 days ? **Total: $270.00**
5. Change dates to: 5 days
6. Observe price update

### Expected Result:
```
Initial (3 days):
- Base: $300.00
- Discount: -$30.00
- Total: $270.00

Updated (5 days):
- Base: $500.00
- Discount: -$50.00
- Total: $450.00

? Price updates automatically when dates change
? Discount percentage stays consistent (10%)
? Button updates to show new total
```

---

## Test 7: Verify No Manual Strategy Selection

### Steps:
1. Login as any customer
2. Navigate to booking page
3. Inspect the form

### Expected Result:
```
? NO radio buttons for pricing strategies visible
? NO "Pricing Options" section
? NO ability to select Standard/Weekend/Seasonal/Loyalty
? Pricing automatically applied based on tier
```

---

## Test 8: Admin Can Still Select Strategies

### Steps:
1. Login as Admin
2. Go to "Manage Rentals"
3. Click "Create Rental"
4. Look at the form

### Expected Result:
```
? Customer dropdown IS visible
? Pricing strategy selection IS available:
   - Standard
   - Weekend
   - Seasonal
   - Loyalty
? Admin can choose any strategy manually
? Different UI from customer booking page
```

---

## Browser Console Checks

### During Booking:
```javascript
// Check Network tab (F12 ? Network)
// When dates change, you should see:

POST /api/rentals/calculate-price
Request Body:
{
  "vehicleId": 1,
  "userId": "user-guid-here",
  "startDate": "2024-12-10",
  "endDate": "2024-12-15",
  "pricingStrategy": "loyalty"  // ? Always "loyalty" for customers
}

Response:
{
  "totalPrice": 450.00,
  "numberOfDays": 5,
  "dailyRate": 100.00,
  "strategyUsed": "loyalty",
  "discount": 10.0  // Shows discount % if applicable
}
```

### During Booking Submission:
```javascript
POST /api/rentals
Request Body:
{
  "userId": "user-guid-here",
  "vehicleId": 1,
  "startDate": "2024-12-10",
  "endDate": "2024-12-15",
  "pricingStrategy": "loyalty"  // ? Always "loyalty"
}

Response:
{
  "id": 123,
  "userId": "user-guid",
  "vehicleId": 1,
  "startDate": "2024-12-10T00:00:00Z",
  "endDate": "2024-12-15T00:00:00Z",
  "totalCost": 450.00,  // ? Matches calculated price
  "status": "Reserved",
  ...
}
```

---

## Common Issues & Solutions

### Issue 1: No discount showing for Silver/Gold/Platinum
**Check**: User's tier in database
```sql
SELECT Id, Email, UserName, Tier FROM AspNetUsers WHERE Email = 'test@test.com';
```
**Fix**: Update tier if needed
```sql
UPDATE AspNetUsers SET Tier = 2 WHERE Email = 'gold@test.com'; -- 2 = Gold
```

### Issue 2: Total Cost shows ${NaN} or incorrect format
**Check**: Browser console for JavaScript errors
**Fix**: Ensure `priceCalculation` is not null and has `TotalPrice` property

### Issue 3: Price doesn't recalculate when dates change
**Check**: Browser console for API errors
**Fix**: 
- Verify backend is running
- Check `/api/rentals/calculate-price` endpoint
- Ensure dates are valid (end > start)

### Issue 4: "Unable to load customer profile"
**Check**: Customer record exists for user
```sql
SELECT * FROM AspNetUsers WHERE Email = 'test@test.com';
-- If Tier is NULL, set it:
UPDATE AspNetUsers SET Tier = 0 WHERE Email = 'test@test.com';
```

---

## Quick Verification Checklist

- [ ] Standard tier customers see no discount
- [ ] Silver tier customers see 5% discount
- [ ] Gold tier customers see 10% discount  
- [ ] Platinum tier customers see 15% discount
- [ ] Discount alert appears for Silver/Gold/Platinum
- [ ] No pricing strategy selection for customers
- [ ] Total cost formatted as `$XXX.XX` (not `${XXX.XX}`)
- [ ] Price recalculates when dates change
- [ ] Booking creates with correct total cost
- [ ] Total in "My Rentals" matches booking page
- [ ] Admin still has pricing strategy selection

---

## Expected Pricing for Common Scenarios

| Daily Rate | Days | Tier | Base | Discount | Total |
|-----------|------|------|------|----------|-------|
| $50 | 3 | Standard | $150 | $0 (0%) | **$150.00** |
| $50 | 3 | Silver | $150 | -$7.50 (5%) | **$142.50** |
| $50 | 3 | Gold | $150 | -$15 (10%) | **$135.00** |
| $50 | 3 | Platinum | $150 | -$22.50 (15%) | **$127.50** |
| $100 | 5 | Standard | $500 | $0 (0%) | **$500.00** |
| $100 | 5 | Silver | $500 | -$25 (5%) | **$475.00** |
| $100 | 5 | Gold | $500 | -$50 (10%) | **$450.00** |
| $100 | 5 | Platinum | $500 | -$75 (15%) | **$425.00** |
| $200 | 7 | Standard | $1400 | $0 (0%) | **$1400.00** |
| $200 | 7 | Silver | $1400 | -$70 (5%) | **$1330.00** |
| $200 | 7 | Gold | $1400 | -$140 (10%) | **$1260.00** |
| $200 | 7 | Platinum | $1400 | -$210 (15%) | **$1190.00** |

Use these values to verify calculations are correct!
