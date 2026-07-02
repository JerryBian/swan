# CLAUDE.md

This file provides guidance to Claude Code when working in this repository.

## Project

Swan — a .NET 10 personal blog engine. Content stored as JSON in a git-backed store, served via ASP.NET Core MVC with Bootstrap 5 UI.

## Build & Run

```bash
npm install              # frontend deps (Bootstrap, EasyMDE, highlight.js, etc.)
npm run build            # runs build.ps1 — SCSS→CSS, JS bundling via uglify
dotnet build Swan.sln    # builds both projects
dotnet run --project src/web
```

Two projects: `src/core/Swan.Core.csproj` (class library) and `src/web/Swan.csproj` (MVC app). No tests.

## Configuration

Bound to `SwanOption` (`src/core/Option/SwanOption.cs`). Config via `swan` JSON section + `ENV_`-prefixed env vars.

Key settings: `AssetLocation` (GitStore data path), `SkipGitOperation` (default true; when false, git push/pull on start/stop/hourly), `AdminUserName`/`AdminPassword`, `BaseUrl`, `GTagId`.

## Architecture

### Data layer (`src/core/`)

All entities extend `SwanObject`: `Id` (random), `CreatedAt`, `LastUpdatedAt`, `IsPublic`, `GetGitStorePath()`.

Types: `SwanPost`, `SwanRead`, `PostTag`, `PostSeries`, `SwanPage`, `SwanLog`.

**`SwanService`** — generic CRUD over `IGitStore` (JSON files in git). Uses `AsyncReaderWriterLock` + `IMemoryCache`. Admin bool controls public vs all visibility. `BulkUpdatePageHitsAsync` batches page hit increments under a single write lock. File uploads are validated against a blocklist of executable extensions.

**`StoreObject`** — populates typed lists from git JSON, converts Markdown→HTML (Markdig), cross-links posts↔tags↔series↔pages, generates metadata HTML snippets and recommendations.

**`SwanLogService`** — appends to `obj/_log.json`, prunes >30 days. Thread-safe via `SemaphoreSlim`.

**`SwanStore`** — in-memory IP blacklist (5-min cache) + page hit queue (`ConcurrentQueue`, deduplicated by IP+URL with 1-min window).

**`GitFileLogger`** — custom `ILogger` writing to git store via background thread.

### Web layer (`src/web/`)

**Program.cs**: ForwardedHeaders (Cloudflare), output/response caching, cookie auth, middleware pipeline: `BlacklistMiddleware` → `RequestSniffMiddleware` → MVC.

**Controllers**: `Home` (landing, sitemap), `Post` (list/detail/archive/tags/series/RSS), `Read` (books), `Account` (cookie login/logout, IP blacklist on fail), `Admin` (CRUD for all entities, file upload/management, log/stat views — behind `[Authorize]` + `[AutoValidateAntiforgeryToken]`).

**Middleware**: `BlacklistMiddleware` (403 if IP blacklisted), `RequestSniffMiddleware` (log admin requests, track page hits).

**Hosted Services**: `MonitorHostedService` (auto-stop after 3 days non-prod), `GitFileHostedService` (git pull on start, push hourly + on stop), `PageHitHostedService` (flush hit queue to GitStore hourly).

### Frontend

Bootstrap 5.3.8 + Bootswatch Zephyr theme, Bootstrap Icons, EasyMDE (admin markdown editor), highlight.js, anchor-js. SCSS at `wwwroot/css/`, compiled via build.ps1. Two bundles: public (`style.min.css` + `script.min.js`) and admin (`admin.min.css` + `admin.min.js`).

### Key NuGet deps

`GitStore`, `DotNext.Threading`, `Markdig`, `HtmlAgilityPack`, `System.ServiceModel.Syndication`.

## Deployment

Docker multi-stage (SDK 10.0 → aspnet 10.0). Version in `VERSION`. GitHub Actions on push to master: build multi-arch image → push to Docker Hub (`cnbian/swan`) + GHCR → SSH deploy to VPS.
