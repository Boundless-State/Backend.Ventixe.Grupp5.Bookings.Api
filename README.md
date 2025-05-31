# 📦 Ventixe.Bookings.Grupp5.Api

A RESTful API for managing event bookings. Supports creating, filtering, updating, and retrieving booking-related data with JWT authentication and Swagger UI integration.

---

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server (or Azure SQL)
- Visual Studio 2022+ or Visual Studio Code

### 🔧 Setup

1. Clone the repository.
2. Update your connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "SqlConnection": "your db connstring"
}
```
3. Optional: Update the `Auth` section if you use an identity server or custom JWT provider.

4. Run the following command in terminal to create database:
```bash
dotnet ef database update
```

5. Run the project using Visual Studio or:
```bash
dotnet run --project Ventixe.Bookings.Grupp5.Api
```

6. Open your browser to:  
```
https://localhost:5001/swagger
```

---

## 🔐 Authentication

All endpoints require **JWT Bearer Token** in the `Authorization` header:

```
Authorization: Bearer <your_token>
```

---

## 🔍 Swagger UI

Swagger UI is available at:

```
https://localhost:5001/swagger
```

- All endpoints are documented
- Includes schemas for all DTOs

---

## 🛠️ API Endpoints

### 📄 GET `/api/booking`

Get all bookings or filtered bookings.

**Query Parameters (optional):**

- `eventId` (int)
- `statusId` (int)
- `userId` (string)

---

### 📄 GET `/api/booking/statistics`

Get aggregated booking statistics.

---

### 📄 GET `/api/booking/{id}`

Get a booking by ID.

---

### ✅ POST `/api/booking`

Create a new booking.

**Request Body Example:**

```json
{
  "eventId": 1,
  "userId": "abc123",
  "statusId": 1
}
```

---

### 🔁 PUT `/api/booking/{id}`

Update a booking.

**Request Body Example:**

```json
{
  "eventId": 1,
  "userId": "abc123",
  "statusId": 2
}
```

---

## 🧪 Example: C# Client Call

### 🔑 Generate Token

Assume you already got a JWT from an external Auth API.

---

### 📥 Create Booking

```csharp
var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:5001");
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "{your_token}");

var newBooking = new
{
    eventId = 1,
    userId = "abc123",
    statusId = 1
};

var response = await client.PostAsJsonAsync("/api/booking", newBooking);
var result = await response.Content.ReadAsStringAsync();

Console.WriteLine(result);
```

---

### 📊 Get Booking Statistics

```csharp
var response = await client.GetAsync("/api/booking/statistics");
var json = await response.Content.ReadAsStringAsync();
Console.WriteLine(json);
```

---

## 📚 Technologies Used

- ASP.NET Core 9 Web API
- Entity Framework Core (Code-First)
- SQL Server
- Swashbuckle (Swagger UI)
- JWT Authentication

---

## ✍️ Authors

Developed by Grupp 5, Ventixe – as part of the ASP.NET advanced coursework.
