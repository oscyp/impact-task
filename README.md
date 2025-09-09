# Tenders.Guru Facade API

REST API that provides users access to data available in https://tenders.guru/pl/api with additional functionalities.

## Technical Requirements

- Each endpoint must support pagination
- The application must be able to run locally
- The amount of data handled by the application should be limited to the first 100 pages of the source API

## Functional Requirements

- User should be able to retrieve tenders
- User should be able to filter tenders by price in EUR
- User should be able to retrieve tenders ordered by price in EUR
- User should be able to filter tenders by date
- User should be able to retrieve tenders ordered by date
- User should be able to retrieve tenders won by a given supplier by providing supplier's ID
- User should be able to retrieve tender with a given ID

## Quality Requirements

- Scalability
- Maintainability
- Testability
- Readability

## Response Format

Each tender response includes:
- Tender's ID
- Tender's Date
- Tender's Title
- Tender's Description
- Tender's Amount in EUR
- Tender's suppliers
  - Supplier's ID
  - Supplier's Name

## How to Run

### Prerequisites
- .NET 8.0 SDK

### Running the Application

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd impact-task
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   cd Tenders.Guru.Facade.Api
   dotnet run
   ```

   The API will be available at:
   - HTTP: `http://localhost:5135`
   - HTTPS: `https://localhost:7210`

4. **Access Swagger UI** (for testing and documentation)
   - Navigate to `https://localhost:7210/swagger` or `http://localhost:5135/swagger`

5. **Test with HTTP requests** (alternative to Swagger)
   - Use the `Tenders.Guru.Facade.Api.http` file with your IDE (Visual Studio Code, JetBrains IDEs, etc.)
   - This file contains ready-to-use example requests for all API endpoints
   - Simply click the "Send Request" button next to each request in your IDE

## API Endpoints

### 1. Get Tender by ID
```
GET /tenders/{tenderId}
```
Retrieves a specific tender by its ID.

**Example:**
```bash
curl -X GET "http://localhost:5135/tenders/123"
```

### 2. Search Tenders
```
POST /tenders
```
Searches for tenders with various filtering and ordering options. **All parameters are optional.**

**Request Body:**
```json
{
  "pageParams": {
    "pageIdx": 0,
    "pageSize": 50
  },
  "priceInEur": 805.41,
  "date": "2023-05-03",
  "orderBy": "date",
  "order": 0,
  "supplierId": 88
}
```

**Parameters:**
- `pageParams`: Pagination settings
  - `pageIdx`: Page index (0-based)
  - `pageSize`: Number of items per page
- `priceInEur`: Filter by price in EUR
- `date`: Filter by date (format: "YYYY-MM-DD")
- `orderBy`: Sort field - `"date"` or `"price_eur"`
- `order`: Sort direction - `0` for ascending, `1` for descending
- `supplierId`: Filter by supplier ID

**Examples:**

Order by price (descending):
```bash
curl -X POST "http://localhost:5135/tenders" \
  -H "Content-Type: application/json" \
  -d '{
    "pageParams": {"pageIdx": 0, "pageSize": 10},
    "priceInEur": 50000,
    "orderBy": "price_eur",
    "order": 1
  }'
```

Order by date (ascending):
```bash
curl -X POST "http://localhost:5135/tenders" \
  -H "Content-Type: application/json" \
  -d '{
    "pageParams": {"pageIdx": 0, "pageSize": 20},
    "orderBy": "date",
    "order": 0
  }'
```

## Running Tests

```bash
cd Tenders.Guru.Facade.Api.Tests
dotnet test
```

## Configuration

The application uses configuration from `appsettings.json` and `appsettings.Development.json` files to connect to the external tenders.guru API.