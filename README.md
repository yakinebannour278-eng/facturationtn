# FacturationTN - Invoice Management System

A simple invoicing and billing management application built with ASP.NET Core Blazor. Helps manage clients, products, and invoices easily.

## Features

- **Client Management** - Add, edit, and manage customer information
- **Invoice Management** - Create invoices with multiple line items
- **Product Catalog** - Manage products and pricing
- **Dashboard** - View sales and tax reports
- **System Settings** - Configure application parameters

## Tech Stack

- ASP.NET Core 10
- Blazor (WebAssembly/Server)
- Entity Framework Core
- SQL Server
- Bootstrap 5

## Project Structure

```
FacturationApp/
├── Components/
│   ├── Pages/
│   │   ├── Clients/
│   │   ├── Factures/
│   │   ├── Produits/
│   │   └── Dashboard/
│   ├── Layout/
│   └── Shared/
├── Services/
│   ├── ClientService.cs
│   ├── FactureService.cs
│   ├── ProduitService.cs
│   ├── DashboardService.cs
│   └── ParametreService.cs
├── Models/
│   ├── Client.cs
│   ├── Facture.cs
│   ├── Produit.cs
│   └── Parametre.cs
├── Data/
│   └── AppDbContext.cs
└── wwwroot/
```

## Getting Started

### Requirements
- .NET 10 SDK
- SQL Server
- Visual Studio or VS Code

### Installation

1. Clone the repo
```bash
git clone https://github.com/yourusername/facturationtn.git
cd facturationtn-main
```

2. Restore packages
```bash
dotnet restore
```

3. Update connection string in `appsettings.json`

4. Run migrations
```bash
dotnet ef database update
```

5. Start the app
```bash
dotnet run
```

The app will run on `https://localhost:5001`

## How to Use

### Adding a Client
1. Go to Clients page
2. Click Add New
3. Enter client info and save

### Creating an Invoice
1. Go to Factures (Invoices)
2. Click New Invoice
3. Select client
4. Add products
5. Save

### Checking Reports
1. Open Dashboard
2. View Sales metrics
3. Check Tax reports

## Services

- **ClientService** - Handles client operations
- **FactureService** - Manages invoices
- **ProduitService** - Product management
- **DashboardService** - Dashboard calculations
- **ParametreService** - Settings management

All services use dependency injection and are configured in Program.cs

## Database

Main tables:
- **Clients** - Customer data
- **Factures** - Invoice records
- **Produits** - Product list
- **Parametres** - Configuration


## Database Diagram

```
┌─────────────┐
│   Client    │
├─────────────┤
│ Id (PK)     │
│ Nom         │
│ Email       │
│ Telephone   │
│ Adresse     │
└─────────────┘
       │
       │ 1:Many
       │
       ▼
┌─────────────────┐
│    Facture      │
├─────────────────┤
│ Id (PK)         │
│ Numero          │
│ ClientId (FK)   │
│ DateFacture     │
│ MontantHT       │
│ MontantTVA      │
│ MontantTTC      │
│ Status          │
└─────────────────┘
       │
       │ 1:Many
       │
       ▼
┌──────────────────┐
│  LigneFacture    │
├──────────────────┤
│ Id (PK)          │
│ FactureId (FK)   │
│ ProduitId (FK)   │
│ Quantite         │
│ PrixUnitaireHT   │
│ MontantTVA       │
│ MontantTTC       │
└──────────────────┘
       │
       │ Many:1
       │
       ▼
┌─────────────┐
│  Produit    │
├─────────────┤
│ Id (PK)     │
│ Reference   │
│ Designation │
│ PrixHT      │
│ TauxTVA     │
└─────────────┘

Additional Tables:
├── Parametre (App settings: key-value pairs)
└── TVAParTaux (Tax rates configuration)
```

---


## Key Relationships

### 1. Client → Facture (1:Many)
- One client can have multiple invoices
- Each invoice belongs to one client
- **Example**: Client "ABC Corp" has invoices INV-001, INV-002, INV-003

### 2. Facture → LigneFacture (1:Many)
- One invoice has multiple line items
- **Example**: Invoice INV-001 has 3 products listed

### 3. LigneFacture → Produit (Many:1)
- Multiple line items can reference the same product
- **Example**: Product "Laptop" appears in 5 different invoices
