# RGFinance

This is my financial dashboard. You can provide your:

1. Assets - what you own $-$ $a_i$
2. Profits - what you earn monthly $-$ $p_j$
3. Expenses - what you spend monthly $-$ $e_k$

Then by running an equation:

$$f(m, a, p, e) = \sum a_i + m \times \big(\sum p_j - \sum e_k\big)$$

you can see your monthly balance and where will you be in $m$ months.

There is a simple diagram showing the next 24 months.

Assets, Profits and Expenses can be provided in PLN, EUR, USD or GOZ (ounce of gold).
They all can be recalculated to PLN, EUR or USD by taking current (on-line) Forex ratios from European Central Bank updated on working days.

Assets have Yearly Interest Rate associated with them, so you can automatically see your interest profits each month. It does not take into account accumulation (yet).

If your monthly balance is in the red you will see when will the bankruptcy happen.
On the other hand when in the black app will tell you when will you become a millionaire!

## Quick Start with Docker 🐳

### Prerequisites

- Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run the Application

```bash
git clone https://github.com/piotrsrodka/RGFinance.git
```

```bash
cd RGFinance
```

```bash
docker-compose up
```

**That's it!** 🎉

- Frontend: http://localhost:4300
- Backend API: http://localhost:5000
- Database runs automatically

### Stop the Application

```bash
docker-compose down
```

## Development

### Stack

Angular 13 |
Dotnet 6 |
MS SQL

### Hybrid Development (Recommended) ⚡

1. Start database: `docker-compose -f docker-compose.dev.yml up`
2. Start backend: `cd WebApi && dotnet run`
3. Start frontend: `cd Website && ng serve`

Navigate to `http://localhost:4300/`

#### Docker Database Management

```bash
# Start dev database
docker-compose -f docker-compose.dev.yml up

# Stop dev database (KEEPS data)
docker-compose -f docker-compose.dev.yml down

# Stop and REMOVE dev data
docker-compose -f docker-compose.dev.yml down -v

# View persistent volumes
docker volume ls
```

#### Database Migrations

```bash
cd WebApi
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Development without Docker

1. Install MS SQL Database
2. Update-database via dotnet-ef tools
3. Front + Backend same as in Hybrid development
