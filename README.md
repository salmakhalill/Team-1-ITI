# Inventory Management System

An ASP.NET Core MVC application for managing inventory operations: products, categories, suppliers, purchases, and sales, with a live stock board, an analytics dashboard, and an AI assistant for natural-language queries over both business policy and live data.

> Built as a graduation training project for ITI's .NET track.

## Demo

https://github.com/user-attachments/assets/01820c85-78de-45c5-a088-79b6117a3d27

## Features

- **Inventory management** — CRUD for products, categories, and suppliers, with search, filtering, and pagination
- **Transactions** — sales and purchases, each composed of line items, with stock automatically deducted on sale and added on purchase
- **Stock board** — a live inventory view with SKU/name search and status filters (All / In Stock / Low Stock / Out of Stock)
- **Reordering** — an "Action Required" view lists low-stock and out-of-stock products with a one-click "Order Now" that opens a pre-filled purchase for that product
- **Site-wide low-stock alert** — a notification bell in the top navbar shows a live count of products needing attention, visible from any page in the app
- **Dashboard** — total products, categories, suppliers, stock quantity, and stock value; low-stock list; monthly sales trend; best-selling products; recent activity feed
- **AI Assistant** — answers operational questions using both business policy and live inventory data

## AI Assistant

The assistant draws on two distinct sources of information:

1. **Policy knowledge (RAG)** — the company's operations policy document is split into chunks, embedded, and indexed at startup. Incoming questions are embedded and matched against these chunks by cosine similarity to ground answers about business rules.
2. **Live data (function calling)** — questions about current stock, sales, or purchases are answered by calling tools backed by the database directly, rather than from the policy text or the model's own assumptions. Available tools:
   - `get_product_stock`
   - `get_low_stock_products`
   - `get_out_of_stock_products`
   - `get_best_selling_products`
   - `get_recent_purchases`
   - `get_recent_sales`
   - `get_suppliers`

The system prompt keeps these two sources distinct, and instructs the assistant to state when information isn't available rather than inferring it.

## Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- [OpenRouter](https://openrouter.ai) API for chat completions and embeddings
- UglyToad.PdfPig for reading the policy document

## Project Structure

```
Controllers/       → HTTP endpoints, one per resource (Products, Sales, Purchases, Suppliers, Categories, Inventory, Dashboard, AI)
Services/          → business logic and data access, separated from controllers
Services/AI/       → AIService (orchestration), RAGService (retrieval), EmbeddingService
ViewComponents/    → reusable page-fragment components with their own data-fetch logic (e.g. the navbar's low-stock alert)
ViewModels/        → page-specific view models
Models/            → EF Core entities
Data/              → InventoryManagementDbContext
```

## Database Schema

<!-- Add the ERD here, e.g.: -->
<!-- ![ERD](docs/erd.png) -->

## Getting Started

```bash
git clone <repo-url>
cd <project-folder>
dotnet restore
```

Create an `appsettings.json` file in the project root (gitignored; each environment keeps its own copy):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-SERVER-Name;Database=InventoryManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "OpenRouter": {
    "ApiKey": "Your Key",
    "Model": "openai/gpt-5.4",
    "EmbeddingModel": "openai/text-embedding-3-small"
  },
  "AllowedHosts": "*"
}
```

Replace `YOUR-SERVER-Name` with your SQL Server instance, and get an OpenRouter API key from [openrouter.ai/keys](https://openrouter.ai/keys).

The policy document the AI assistant reads from is expected at:

```
wwwroot/documents/Inventory_Operations_Replenishment_Policy.pdf
```

Then apply migrations and run:

```bash
dotnet ef database update
dotnet run
```











