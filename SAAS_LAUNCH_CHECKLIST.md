# M365 Guard — Azure Marketplace SaaS Launch Checklist

**Target Launch:** Q2 2026  
**Offer Type:** Transactable SaaS  
**Last Updated:** March 14, 2026

---

## 📋 Executive Summary

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **Offer Type** | Transactable | Full SaaS integration, per-user billing |
| **Target Market** | SMB + Mid-Market + Enterprise | Tiered pricing covers all segments |
| **Pricing Model** | Per-seat monthly | Predictable revenue, easy scaling |
| **Architecture** | .NET 10 + Python MCP sidecar | Certificate auth + AI integration |

---

## 💰 Pricing Tiers (Recommended)

| Tier | Price | Tenants | Users | Features |
|------|-------|---------|-------|----------|
| **Starter** | $49/mo | 1 | Up to 50 | Core audits, Excel export |
| **Professional** | $199/mo | 3 | Up to 500 | + CA analysis, dashboards |
| **Enterprise** | $499/mo | 10 | Unlimited | + API access, MCP integration, SLA |

**Competitor Benchmark:**
- Blumira: $144/mo (3 users)
- CoreView: $300+/mo
- AdminDroid: $795/year (~$66/mo)

---

## ✅ Phase 1: Marketplace MVP (2 weeks)

### Technical Requirements

| # | Task | Status | Source | Notes |
|---|------|--------|--------|-------|
| 1 | REST API layer (ASP.NET Core Minimal APIs) | ⬜ TODO | New | Currently console-only |
| 2 | SaaS Fulfillment Client | ⬜ TODO | Port from `FiscalFlow/src/saas/fulfillment_client.py` | ~200 lines |
| 3 | Webhook Handler | ⬜ TODO | Port from `FiscalFlow/src/saas/webhook_handler.py` | ~150 lines |
| 4 | Landing Page + Token Resolution | ⬜ TODO | Port from `FiscalFlow/src/saas/router.py` | ~150 lines |
| 5 | MCP Server Integration | ⬜ TODO | Copy from `Heyson315/M365Guard/src/mcp/` | Complete! |
| 6 | Multi-tenant subscription storage | ⬜ TODO | New | Azure Table or Cosmos DB |

### Partner Center Requirements

| # | Task | Status | Notes |
|---|------|--------|-------|
| 7 | MPN ID active | ✅ DONE | Already enrolled |
| 8 | Publisher profile in Partner Center | ⬜ TODO | https://partner.microsoft.com |
| 9 | SaaS offer created | ⬜ TODO | Transactable type |
| 10 | Technical configuration (landing page URL, webhook URL) | ⬜ TODO | After #2-4 deployed |
| 11 | Plan setup (3 tiers) | ⬜ TODO | Starter/Professional/Enterprise |
| 12 | Marketplace listing (description, screenshots) | ⬜ TODO | Marketing copy |
| 13 | Legal (privacy policy, terms of use) | ⬜ TODO | Standard contract available |

### Azure Infrastructure

| # | Task | Status | Notes |
|---|------|--------|-------|
| 14 | App Service or Container App for API | ⬜ TODO | Recommend Container App |
| 15 | Key Vault for certificates | ✅ DONE | `m365guard-kv` exists |
| 16 | Application Insights | ⬜ TODO | Telemetry |
| 17 | Azure AD App Registration (multi-tenant) | ⬜ TODO | Update existing or create new |

---

## ✅ Phase 2: Enhanced Features (1 week post-launch)

| # | Task | Status | Source |
|---|------|--------|--------|
| 18 | Unified Dashboard | ⬜ TODO | Port from `FiscalFlow/src/m365guard_dashboard.py` |
| 19 | Enhanced Excel exports (charts, VBA) | ⬜ TODO | Enhance existing ExcelExportService |
| 20 | Security Portal integration (Defender alerts) | ⬜ TODO | New Graph API calls |

---

## ✅ Phase 3: Differentiators (Month 2+)

| # | Task | Status | Source |
|---|------|--------|--------|
| 21 | AI Anomaly Detection | ⬜ TODO | Port from `FiscalFlow/src/anomaly_detection.py` or use ML.NET |
| 22 | Additional MCP plugins (Intune, Defender, Purview) | ⬜ TODO | Extend plugin architecture |
| 23 | D365 Business Central integration | ⬜ TODO | New development |
| 24 | Workspace/team management | ⬜ TODO | New development |

---

## 🏆 Competitive Edge (Our Differentiators)

| Feature | M365 Guard | Blumira | CloudGate | Maester | AdminDroid |
|---------|------------|---------|-----------|---------|------------|
| **MCP Integration** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **AI Anomaly Detection** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **No-Secrets Auth** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **CIS Benchmarks** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **Multi-Tenant** | ✅ | ✅ | ✅ | ❌ | ✅ |
| **Excel Export** | ✅ | ❌ | ❌ | ❌ | ✅ |
| **SaaS Marketplace** | 🔜 | ✅ | ✅ | ❌ | ✅ |

**Key Message:** MCP integration is our unique differentiator for the AI-native Copilot era.

---

## 📂 Source Files Reference

### Ready to Integrate (from existing codebases)

```
# MCP Server (Complete!)
e:\source\Heyson315\M365Guard\src\mcp\m365_mcp_server.py
e:\source\Heyson315\M365Guard\src\mcp\plugins\sharepoint_tools\tools.py
e:\source\Heyson315\M365Guard\MCP_INTEGRATION_DEMO.md

# SaaS Modules (Port to .NET)
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\saas\fulfillment_client.py
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\saas\webhook_handler.py
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\saas\router.py

# REST API Pattern
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\m365guard_api.py

# Dashboard
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\m365guard_dashboard.py

# Anomaly Detection
e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\src\anomaly_detection.py
```

### Existing M365 Guard Assets

```
e:\source\M365 Guard\M365 Guard\Services\AuthenticationService.cs
e:\source\M365 Guard\M365 Guard\Services\ConditionalAccessService.cs
e:\source\M365 Guard\M365 Guard\Services\ExcelExportService.cs
e:\source\M365 Guard\M365 Guard\Services\GraphService.cs
e:\source\M365 Guard\M365 Guard\Services\IntuneDeviceService.cs
```

---

## 🔑 Key Decisions Pending

| # | Decision | Options | Recommendation |
|---|----------|---------|----------------|
| 1 | Architecture | A) Pure .NET / B) Hybrid .NET+Python | **A) Pure .NET** (single deployment) |
| 2 | MCP Strategy | Keep Python sidecar / Port to .NET | **Keep Python** (faster time-to-market) |
| 3 | ML Features | ML.NET / Python microservice | **ML.NET** (single runtime) |
| 4 | Database | Azure SQL / Cosmos DB / Table Storage | **Table Storage** (simple, cheap) |
| 5 | Hosting | App Service / Container Apps / AKS | **Container Apps** (modern, scalable) |

---

## 📅 Timeline

```
Week 1-2:  Phase 1 MVP
           Day 1-2:   Integrate MCP server
           Day 3-5:   Port SaaS Fulfillment Client to .NET
           Day 6-7:   Port Webhook Handler to .NET
           Day 8-9:   Add ASP.NET Core Minimal APIs
           Day 10-12: Landing page + token resolution
           Day 13-14: Integration testing + Partner Center submission

Week 3:    Phase 2 Enhanced Features
           Unified dashboard, enhanced exports

Week 4+:   Phase 3 Differentiators
           AI anomaly detection, additional MCP plugins
```

---

## 📚 Related Documents

- [ARCHITECT_EMAIL_MERGER_PLAN.md](./ARCHITECT_EMAIL_MERGER_PLAN.md) — Tech team email
- [FISCALFLOW_MERGER_PROPOSAL.md](./FISCALFLOW_MERGER_PROPOSAL.md) — Full technical analysis
- [MCP_INTEGRATION_DEMO.md](../Heyson315/M365Guard/MCP_INTEGRATION_DEMO.md) — MCP documentation

---

## ✍️ Sign-Off

| Role | Name | Date | Approved |
|------|------|------|----------|
| Product Owner | | | ⬜ |
| Tech Lead / Architect | | | ⬜ |
| Dev Lead | | | ⬜ |

---

*Generated: March 14, 2026*
