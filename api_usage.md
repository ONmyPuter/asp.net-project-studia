# Reservations API Usage - Experimentation

This document provides example `curl` commands to interact with the Public Reservations API.

**Base URL**: `http://localhost:5207/api/reservations`

## 1. Get All Reservations

```bash
curl -X GET "http://localhost:5207/api/reservations" -H "accept: application/json"
```

## 2. Get Single Reservation

Replace `1` with the ID of the reservation you want to retrieve.

```bash
curl -X GET "http://localhost:5207/api/reservations/1" -H "accept: application/json"
```

## 3. Create Reservation

**Note**: You need valid IDs for `CarId`, `UserId` (optional but recommended), `PickupLocationId`, `DropoffLocationId`, and `InsurancePolicyId`.

Example using explicit IDs (ensure these exist in your DB or adjust accordingly):

```bash
curl -X POST "http://localhost:5207/api/reservations" \
     -H "Content-Type: application/json" \
     -d '{
           "from": "2023-11-01T10:00:00",
           "to": "2023-11-05T10:00:00",
           "carId": 1,
           "userId": "YOUR_USER_ID_HERE",
           "pickupLocationId": 1,
           "dropoffLocationId": 1,
           "insurancePolicyId": 1
         }'
```

*Note: If you do not have a user ID, you can try omitting it if the model allows nulls, or check your database/User implementation.*

## 4. Update Reservation

Replace `1` with the ID of the reservation to update.

```bash
curl -X PUT "http://localhost:5207/api/reservations/1" \
     -H "Content-Type: application/json" \
     -d '{
           "id": 1,
           "from": "2023-12-01T12:00:00",
           "to": "2023-12-05T12:00:00",
           "carId": 1,
           "userId": "YOUR_USER_ID_HERE",
           "pickupLocationId": 2,
           "dropoffLocationId": 2,
           "insurancePolicyId": 1
         }'
```

## 5. Delete Reservation

Replace `1` with the ID of the reservation to delete.

```bash
curl -X DELETE "http://localhost:5207/api/reservations/1" -H "accept: */*"
```
