# Gomango.shop – Full Stack E-Commerce Platform

This repository contains the complete source for **Gomango.shop**, a bilingual (Arabic/English) online store that I personally designed, coded, and deployed end-to-end: frontend, backend, admin panel, DevOps, and Stripe payments.

> 🌍 Live store: [https://Gomango.shop](https://Gomango.shop)

---

## Why It Stands Out

- **Production-ready**: actively serving customers with real payments.
- **Solo-built**: every component—from UI/UX to database design—is authored by me.
- **Hiring-ready**: showcases product thinking, polished UX, secure payment flows, and resilient API design.

---

## Feature Snapshot

- **Instant language switch + RTL/LTR layout** for a culturally tailored UI.
- **Smart checkout**: saved addresses, dual payment options (Stripe + COD), resumable carts.
- **Reward automation**: shipping discount code generated and surfaced inside a celebratory `SuccessForm` (with audio) when Stripe confirms payment.
- **Pending order pipeline**: webhook-safe flow with manual reprocess endpoint so no paid order is lost.
- **Admin workspace**: roles (Admin, Manager, Shipping), catalog management, order dashboards, announcement controls, currency selector.
- **Real-time signals**: SignalR notifications, live customer search logs, sticky navbar, animated announcement bar.

---

## Tech Stack

| Layer | Technologies |
|-------|--------------|
| Frontend | React 18, Vite, Tailwind CSS, React Router, Howler.js, Stripe.js |
| Backend | ASP.NET Core 8, EF Core, SQL Server, SignalR, Stripe.NET |
| Ops & Infra | Webhook signature verification, PendingOrders queue, Azure-style hosting |

---

## Run the Frontend (`Store-React`)

```bash
cd Store-React
cp .env.example .env             # set API_BASE_URL
npm install
npm run dev                      # npm run build && npm run preview for prod check
```

- Default dev URL: `http://localhost:5173`.
- Key UI files: `src/Components/Home/StoreLayout.jsx`, `src/Components/CreateOrder/*`, `src/Components/i18n/I18nContext.jsx`.

---

## Run the Backend (`store-RESTAPIs`)

```bash
cd store-RESTAPIs/OnlineStoreAPIs
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<SQL-CONNECTION>"
dotnet user-secrets set "Stripe:SecretKey" "<STRIPE-SECRET>"
dotnet user-secrets set "Stripe:WebhookSecret" "<WEBHOOK-SECRET>"
dotnet restore
dotnet ef database update
dotnet run
```

Important endpoints:
- `POST /api/Payments/StripeCheckout` – create Checkout Session.
- `POST /api/Payments/StripeWebhook` – validates signature, finalizes orders, assigns reward code.
- `POST /api/Payments/ReprocessPendingOrders` – admin fallback for stuck pending orders.
- `GET /api/Payments/CheckoutStatus?sessionId=...` – polled by frontend to open the success modal.

---

## Architecture Overview

| Path | Description |
|------|-------------|
| `Store-React/src/Components/Home/StoreLayout.jsx` | Core layout, sticky navbar, sidebar, announcement bar. |
| `Store-React/src/Components/CreateOrder` | Address selection, order summary, payment success flow. |
| `Store-React/src/Components/i18n/I18nContext.jsx` | Translation keys and language context. |
| `store-RESTAPIs/OnlineStoreAPIs/Controllers` | REST endpoints for products, orders, payments, customers. |
| `StoreBusinessLayer/OrdersServices/PendingOrdersRepo.cs` | Pending order repository and recovery logic. |

---

## Suggested QA Scenarios

1. Register → add address → place Cash-on-Delivery order → verify success modal (no discount).
2. Place Stripe order → wait for webhook → success modal should show order number, play victory sound, and display shipping discount code.
3. Toggle language/currency during browsing; verify RTL/LTR alignment and translations.
4. Log in as Manager vs Shipping vs Admin to confirm scoped permissions.
5. Call `POST /api/Payments/ReprocessPendingOrders` after a simulated webhook failure to prove resiliency.

---

## Ownership & Contact

I originally built **Gomango.shop** from scratch before selling the live store to a new owner. The entire codebase remains my intellectual property, and I maintain full rights to showcase, reuse, or adapt it.

Feel free to explore the production deployment or reach out for collaboration:

> [https://Gomango.shop](https://Gomango.shop)

All rights reserved. Any redistribution or reuse of the source code requires my written approval.

