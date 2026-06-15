# CMSYS → Cassandra Feature Migration — Roadmap & Guide

> Working guide for re-implementing the CMSYS feature set on Cassandra's architecture.
> Status legend: ✓ done · ▶ in progress · ☐ not started.

## 1. Context

**CMSYS** (`C:\WorkingFolder\PrivateProject\CMSys`) is a legacy **ASP.NET Core 2.1 MVC monolith** — raw ADO.NET SQL against SQL Server, server-rendered Razor views, repository pattern, no tests. It is an Indonesian **motorcycle-dealer ERP** with ~60 features across master data, inventory, sales, finance/accounting, STNK/BPKB document workflows, and reporting.

**Cassandra** is the *same business domain* rebuilt on a modern stack: **.NET 10, Clean Architecture, Event Sourcing + CQRS, PostgreSQL/EF Core, Blazor (Bootstrap) UI, ASP.NET Identity + JWT, xUnit/TDD**. Today the **auth slice**, a **Dealer** registry, and one master-data slice — **Jabatan** — are implemented. **Jabatan is the canonical end-to-end template** every migrated feature mirrors.

**Goal:** re-implement CMSYS's features on Cassandra's architecture, one vertical slice at a time, ordered by dependency. We do **not** port code or data — we extract the *behavior* (entities, rules, workflows) and rebuild it the Cassandra way.

**Locked decisions:**
- **Scope:** full feature parity with CMSYS (all ~60 features).
- **Data:** features only, **fresh database** — no ETL from legacy SQL Server.
- **Reporting:** **deferred** — placeholder final phase; no PDF/Excel export for now.
- **Outlets:** **keep Kios** (outlets) and **Mutasi** (inter-outlet transfers). Kios is a dealer-scoped master entity; Stock is located at a Kios.

---

## 2. Naming & convention translation (CMSYS → Cassandra)

CMSYS code is the *spec*, not the style. Always re-style to Cassandra conventions:

| CMSYS (legacy) | Cassandra (target) |
|---|---|
| Hungarian fields `szName`, `decTotal`, `bActive`, `dtmCreated` | Clean C# `Name`, `Total`, `IsActive`, `CreatedAt` |
| Mutable EF/POCO model + `Repo` with raw SQL | Event-sourced **aggregate** + **read-model projection** + event-store repository |
| Controller does business logic | Logic in **Domain** aggregate + **Application** handler; controller only dispatches |
| `ModelState` / data annotations | **FluentValidation** validator per command (+ DataAnnotations on the Blazor form) |
| Audit columns `CreatedBy/UpdatedBy/dtmCreated` | Carried on **domain events** (`CreatedBy`, `OccurredAt`) → surfaced on read model |
| Status string fields | Past-tense **domain events** (`StockReserved`, `StnkHandedOver`, …) drive state |
| English code; Indonesian domain terms kept where clearer | same — keep Indonesian domain nouns (Jabatan, Kios, Tenor, STNK), English for technical members |

**Single-dealer note:** Cassandra scopes every read model by `DealerId` via `ICurrentDealer` + a global query filter. Mirror that for all dealer-scoped entities. **Kios ≠ dealer** — Kios is an outlet *inside* a dealer, modeled as ordinary dealer-scoped master data.

---

## 3. The Porting Recipe (per vertical slice)

Every feature follows the **Jabatan** slice exactly. Read these as the template:

- **Domain:** `Cassandra.Backend/Cassandra.Domain/Jabatan/Jabatan.cs`, `JabatanId.cs`, `Jabatan/Events/*`, base `Common/AggregateRoot.cs`
- **Application:** `Commands/Jabatan/**` (Command + Handler + Validator), `Queries/Jabatan/GetJabatansQueryHandler.cs`, `Contracts/Jabatan/I*Repository.cs`, `DTOs/Jabatan/JabatanDto.cs`
- **Infrastructure:** `Infrastructure/Jabatan/JabatanRepository.cs`, `JabatanQueryRepository.cs`, `Persistence/Projections/JabatanReadModel.cs`
- **WebApi:** `Controllers/Dealer/JabatanController.cs`
- **WebUi:** `Components/Pages/Dealer/Jabatan.razor`, `Services/MasterDataApiClient.cs`, nav in `Components/Layout/NavMenu.razor`
- **Tests:** `Cassandra.Tests/Jabatan/*`

**Checklist per feature (TDD order, mirrors CLAUDE.md):**

1. **Domain** — aggregate `: AggregateRoot<XId>`, strongly-typed `XId` record, past-tense events in `X/Events/`, `Create`/`Reconstitute` factories, command methods raising events, `Apply(...)` switch. Throw `DomainException` for invariant breaches.
2. **Tests (red)** — `Cassandra.Tests/X/`: aggregate behavior + command-handler + validator tests, *before* implementation.
3. **Application** — `Commands/X/<Op>/` (Command record + Handler + FluentValidation Validator), `Queries/X/` handler, `Contracts/X/IXRepository` + `IXQueryRepository`, `DTOs/X/XDto`.
4. **Infrastructure** — `XRepository` (load by replaying `StoredEvents`, append new events, optimistic version), `XQueryRepository`, `Persistence/Projections/XReadModel`. Register events in `EventTypeRegistry.cs`; add `DbSet` + config + dealer query filter in `AppDbContext.cs`; **EF migration**.
5. **WebApi** — `Controllers/Dealer/XController.cs` with `[Authorize(Roles=...)]`, dispatch + validate, request records.
6. **WebUi** — Blazor page under `Components/Pages/Dealer/`, extend `MasterDataApiClient` (or a new per-area client), add `NavMenu` entry.
7. **DI** — register handlers in `Application/DependencyInjection.cs`; register repositories in `Infrastructure/DependencyInjection.cs`.
8. **Green + review** — `dotnet build` + `dotnet test` green; run `/check`. Use the `/feature` skill to drive a slice; `security-engineer` review for money/PII slices.

---

## 4. Phase Breakdown

Ordered by dependency: master data → inventory → sales → documents → finance → reporting.

### Phase 0 — Cross-cutting foundation  ☐
Shared primitives every later phase needs.
- **Counter / sequential numbering** — service to generate `SO`, registration, invoice, mutasi numbers (CMSYS `Counter`). Decide format strategy once.
- **PostalCode / address lookup** — hierarchical Propinsi→Kabupaten→Kecamatan→Kelurahan reference data (read-only seed; used by Karyawan, Kios, Mediator, Biro, Leasing, sales customer address).
- Confirm shared **`DomainException`** → HTTP problem-response mapping, and extract a reusable Blazor list/form layout from the Jabatan page.

### Phase 1 — Personnel & Organization (master data)  ☐
| Feature | Aggregate | Notes |
|---|---|---|
| Jabatan ✓ | Jabatan | Done — the template. |
| Karyawan (Employee) | Karyawan | name, email, KTP, gender, hire/resign dates, phones, address, **sales limit**, links to Jabatan. |
| Kios (Outlet) | Kios | code, name, address, phone/fax, PIC (Karyawan), **limit**; Stock lives here. |
| Mediator (Agent) | Mediator | name, linked Karyawan, address, **limit**. |
| Limits (Karyawan/Kios/Mediator) | (commands on the above) | Credit/sales quota set + adjust; commands on each aggregate, not a new aggregate. |

### Phase 2 — Product Catalog (master data)  ☐
| Feature | Aggregate | Notes |
|---|---|---|
| Warna (Color) | Warna | code, name. |
| GrupTipeMotor (Type group) | GrupTipeMotor | groups models for pricing/discount. |
| TipeMotor (Model) | TipeMotor | codes (product/AHM/WMS), engine/chassis format, prices (nett, OR & BBN per region), **available colors** (TipeMotorWarna). |
| Kelengkapan (Accessories) | Kelengkapan | name, active. |

### Phase 3 — Financing & Leasing (master data)  ☐
| Feature | Aggregate | Notes |
|---|---|---|
| MetodeKeuangan (Payment method) | MetodeKeuangan | code, name. |
| GlobalLeasing + CabangLeasing | GlobalLeasing | finance companies + branches. |
| GrupTenor + Tenor | GrupTenor / Tenor | installment-term groups and months. |
| DaftarHargaLeasing (price lists) | DaftarHargaLeasing | subsidy/incentive/other per motor-group × tenor-group × leasing. |
| Discount (credit) / DiscountCash / AlokasiDiskon | Discount, DiscountCash | discount matrices by leasing/tenor/level + allocation. |
| DF (down-payment / financing fee) | DF | DF charge config. |

### Phase 4 — Service Bureau & Finance Config (master data)  ☐
| Feature | Aggregate | Notes |
|---|---|---|
| Samsat | Samsat | registration-authority fees/contact. |
| Biro (service bureau) | Biro | processes STNK/BPKB; PPH rate, PIC. |
| BiayaBiroJasa | BiayaBiroJasa | bureau fees. |
| ExpenseType (Jenis Pengeluaran) | ExpenseType | expense categories. |
| Ledger (chart of accounts) | Ledger | account names (used by Phase 8). |
| PelanggaranWilayah (territory violation) | PelanggaranWilayah | violation + penalty config. |

### Phase 5 — Inventory & Stock  ☐
First **transactional** phase; depends on Phases 1–4.
| Feature | Aggregate | Notes |
|---|---|---|
| Stock (unit inventory) | Stock | per engine/chassis unit; status lifecycle `TERSEDIA→DIPESAN→TERJUAL→TERKIRIM→RETUR` as past-tense events; located at a Kios. |
| SO (Surat Order / purchase order) + SOItem | SalesOrder (SO) | header + line items, payment type (cash/DF), quota reservation, PPn/subsidy/discount totals, status. |
| Penerimaan SO (goods receipt) | (command) | receiving against an SO **creates Stock units** (engine/chassis). |
| SO Retur | SoRetur | returns against an SO. |
| Mutasi (transfer kirim/terima) | Mutasi | move units + accessories between Kios; outbound/inbound. |

### Phase 6 — Sales  ☐
Most complex aggregate; depends on Phases 1–5.
| Feature | Aggregate | Notes |
|---|---|---|
| RegistrasiPenjualan (sales registration) | RegistrasiPenjualan | unit sale to customer; cash vs credit pricing (uses DaftarHargaLeasing/Discount), DP/installment/TAC, **limit checks** (Karyawan/Kios/Mediator), approval, void, accessories, handover docs. Reserves/sells Stock. |
| PengirimanMotor (delivery) | PengirimanMotor | driver/zone delivery tied to a sale; marks Stock `TERKIRIM`. |

### Phase 7 — Document Workflows  ☐
Depends on Phase 6 (each ties to a RegistrasiPenjualan).
| Feature | Aggregate | Notes |
|---|---|---|
| STNK | Stnk | staged workflow `RECEIVE_FAKTUR→PROCESS→RECEIVE→HANDOVER_STNK` via events; cost fields; bureau assignment. |
| BPKB | Bpkb | `REQUEST→RECEIVE→HANDOVER`; cash vs credit handover. |
| StatusHistory | (projection) | audit trail of document/stock status changes — naturally derivable from the event store. |

### Phase 8 — Finance & Accounting  ☐
Depends on Phases 5–7. Highest complexity (double-entry).
| Feature | Aggregate | Notes |
|---|---|---|
| ARTrans (receivables) | ArTransaction | PENJUALAN / AMBIL_UANG / PELANGGARAN_WILAYAH. |
| APTrans (payables) | ApTransaction | payables to suppliers/bureaus/leasing. |
| CashOut (SO / SO Retur) | CashOut | FSO_CASH / FSO_DF / retur variants. |
| Pembayaran STNK | (command/txn) | STNK fee payments to Samsat. |
| FInvoice (receipt/kwitansi) | FInvoice | payment vouchers. |
| Counter | (Phase 0 if not already) | sequential transaction numbers. |
| Ledger postings | (projections on Ledger) | debit/credit journal, trial balance. |

### Phase 9 — Reporting  ☐ *(deferred)*
Placeholder only. When resumed: build as **Blazor screens over read-model/projection queries** (stock monthly/daily/by-SO/card, sales monthly/annual/per-sales/returns, delivery, SO recap, status history, remaining stock). PDF/Excel export is a later enhancement. Many reports are *free* once the relevant projections exist.

---

## 5. Shared wiring touched by every slice

- `Infrastructure/Persistence/EventStore/EventTypeRegistry.cs` — register every new event type.
- `Infrastructure/Persistence/AppDbContext.cs` — `DbSet`, entity config, dealer query filter; then `dotnet ef migrations add`.
- `Application/DependencyInjection.cs` and `Infrastructure/DependencyInjection.cs` — register handlers / repositories.
- `WebUi/Services/MasterDataApiClient.cs` (or a new per-area client for transactional phases) and `Components/Layout/NavMenu.razor`.

---

## 6. Verification (per slice — run the gate, never batch)

1. `dotnet build` — solution compiles.
2. `dotnet test` — all xUnit green (domain + handler + validator tests written first).
3. `dotnet ef migrations add <Name> --project Cassandra.Backend/Cassandra.Infrastructure --startup-project Cassandra.Backend/Cassandra.WebApi` then `dotnet ef database update`.
4. Run via Aspire: `dotnet run --project Cassandra.AppHost`; exercise the new Blazor page (create/edit/activate/deactivate, role-gated actions) and confirm the endpoint via the page.
5. Run the **`/check`** skill (build + tests + format + diff self-review). Use **`security-engineer`** review for money/PII/auth-touching slices (Phases 6–8).

**Phase exit criteria:** every feature in the phase has aggregate+events, tests, command/query handlers + validators, read-model projection + migration, controller, Blazor page + nav entry, DI wired, and the gate above is green.

---

## 7. Suggested execution cadence

- Knock out master-data phases (1–4) first — near-identical to Jabatan; they unblock everything. Use the `/feature` skill or `backend-engineer` + `frontend-engineer` agents per slice.
- Phases 5–8 are genuine modeling work (workflows, money) — slow down, write richer tests, security-review.
- Phase 9 stays parked until reopened.
