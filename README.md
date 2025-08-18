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
They all can be recalculated to PLN, EUR or USD by taking current (on-line) Forex ratios.

Assets have Yearly Interest Rate associated with them, so you can automatically see your interest profits each month. It does not take into account accumulation (yet).

## Stack

Angular 13
Dotnet 6
MS SQL

## Development server

Angular run `ng serve` for a dev server. Navigate to `http://localhost:4300/`.
For backend `dotnet run` in Api Project directory
Remember to update-database via dotnet-ef tools

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
