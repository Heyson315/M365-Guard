# FiscalFlow → M365 Guard Merger Proposal

**Date:** March 14, 2026  
**Prepared For:** Tech Team & Architecture Review  
**Status:** Draft for Discussion

---

## Executive Summary

The archived **FiscalFlow** project (`e:\source\_archive\HHR-CPA-duplicates\FiscalFlow\`) contains production-ready modules that can accelerate M365 Guard's Azure Marketplace launch. This document outlines the merger opportunity, technical considerations, and recommended phased approach.

### 🚀 Key Discovery: MCP Server Already Built!

During this analysis, we discovered that **`Heyson315/M365Guard`** already has a complete MCP (Model Context Protocol) server with:
- Plugin-based architecture
- SharePoint & QuickBooks integrations
- ASP.NET website integration ready
- Full documentation

**This is a major competitive advantage** — no other M365 security tool offers MCP integration.

---

## Current State Comparison

| Capability | M365 Guard (.NET 10) | FiscalFlow (Python) | Heyson315/M365Guard (Python) |
|------------|---------------------|----------------------|------------------------------|
| **Authentication** | Certificate-based ✅ | JWT + OAuth2 ✅ | JWT + OAuth2 ✅ |
| **Excel Export** | EPPlus (basic) | Charts + VBA macros | Enhanced reports |
| **REST API** | ❌ Console-only | ✅ Full FastAPI | ✅ Azure Functions |
| **SaaS Fulfillment** | ❌ Not implemented | ✅ Marketplace APIs v2 | ❌ Not implemented |
| **Webhook Handlers** | ❌ Not implemented | ✅ Subscription lifecycle | ❌ Not implemented |
| **Dashboard** | ❌ Not implemented | ✅ Unified multi-service | ✅ Security dashboard |
| **AI/ML** | ❌ Not implemented | ✅ Anomaly detection | ❌ Not implemented |
| **Audit Logging** | Basic | ✅ Immutable trail | ✅ Full audit |
| **MCP Server** | ❌ Not implemented | ❌ Not implemented | ✅ **COMPLETE** |
| **Plugin Architecture** | ❌ Not implemented | ❌ Not implemented | ✅ **COMPLETE** |

---

## MCP Server Architecture (Already Built!)

### What's Available in `Heyson315/M365Guard`

```
src/mcp/
├── m365_mcp_server.py     # FastMCP server with plugin architecture
├── plugins/
│   ├── sharepoint_tools/  # SharePoint permissions analysis
│   │   ├── plugin.json    # Plugin manifest
│   │   └── tools.py       # Tool implementations
│   └── quickbooks_tools/  # QuickBooks integration
│       └── tools.py       # Tool implementations
└── __init__.py
```

### MCP Server Features

1. **Dynamic Plugin Loading**
   - Plugins discovered at runtime via `plugin.json` manifests
   - On-demand loading via `load_toolset` tool
   - Modular, scalable architecture

2. **Built-in Tools**
   - `list_available_toolsets` - Show available plugins
   - `load_toolset` - Load a plugin by name
   - `get_security_dashboard` - Compliance dashboard
   - `get_audit_findings` - CIS control results
   - `analyze_security_gaps` - Risk prioritization
   - `generate_remediation_plan` - PowerShell scripts
   - `analyze_sharepoint_permissions` - SharePoint audit

3. **ASP.NET Integration Ready**
   ```csharp
   // qwe_integration/Services/EasyAiSecurityService.cs
   var response = await _httpClient.GetAsync(
       "http://localhost:8080/mcp/security-status"
   );
   ```

### MCP Competitive Edge 🏆

| Competitor | MCP Support |
|------------|-------------|
| Blumira | ❌ No |
| CloudGate | ❌ No |
| Maester | ❌ No |
| AdminDroid | ❌ No |
| **M365 Guard** | ✅ **Full MCP + Plugin Architecture** |

**This is a unique differentiator for the AI-native era!**

---

## Modules Identified for Merge

### CRITICAL DISCOVERY: MCP Server Already Built! 🚀

**Location:** `e:\source\Heyson315\M365Guard\src\mcp\`

A **production-ready MCP (Model Context Protocol) server** already exists with:

| Component | Path | Status |
|-----------|------|--------|
| **MCP Server Core** | `src/mcp/m365_mcp_server.py` | ✅ Complete |
| **Plugin Architecture** | `src/mcp/plugins/` | ✅ Complete |
| **SharePoint Tools Plugin** | `src/mcp/plugins/sharepoint_tools/` | ✅ Complete |
| **QuickBooks Plugin** | `src/mcp/plugins/quickbooks_tools/` | ✅ Complete |
| **ASP.NET Integration** | `qwe_integration/` | ✅ Complete |

**MCP Tools Exposed:**
```python
@mcp_tool("get_security_dashboard")      # Compliance dashboard
@mcp_tool("get_audit_findings")          # CIS control results
@mcp_tool("analyze_security_gaps")       # Risk analysis
@mcp_tool("generate_remediation_plan")   # Auto-fix scripts
@mcp_tool("analyze_sharepoint_permissions")  # SharePoint audit
```

**Impact:** This puts us **months ahead** — MCP integration is already done!

---

### Phase 1: Critical for Marketplace Launch (2 weeks)

| Module | Source Path | Purpose | Effort |
|--------|-------------|---------|--------|
| **SaaS Fulfillment Client** | `FiscalFlow/src/saas/fulfillment_client.py` | Microsoft Marketplace Fulfillment APIs v2 | M |
| **Webhook Handler** | `FiscalFlow/src/saas/webhook_handler.py` | Subscription lifecycle (activate, suspend, unsubscribe) | M |
| **Landing Page Router** | `FiscalFlow/src/saas/router.py` | Token resolution + activation flow | S |
| **REST API Layer** | `FiscalFlow/src/m365guard_api.py` | FastAPI endpoints with CORS, rate limiting | L |
| **MCP Server** | `Heyson315/M365Guard/src/mcp/` | ✅ **ALREADY DONE** — just integrate | S |

**Blocker Resolution:** M365 Guard is currently console-only. Without REST API + SaaS modules, Marketplace listing is not possible.

### Phase 2: High Value (1 week)

| Module | Source Path | Purpose | Effort |
|--------|-------------|---------|--------|
| **Unified Dashboard** | `src/m365guard_dashboard.py` | Multi-service compliance scoring | M |
| **Excel Template Generator** | `src/m365guard_excel_template.py` | Pivot tables, charts, VBA macros | S |
| **Security Portal** | `src/security_portal/router.py` | Defender alerts + threat integration | M |

### Phase 3: Future Differentiators

| Module | Source Path | Purpose | Effort |
|--------|-------------|---------|--------|
| **AI Anomaly Detection** | `src/anomaly_detection.py` | ML-based fraud/threat detection | L |
| **D365 Integration** | `src/d365/` | Business Central connector | L |
| **Immutable Audit Logging** | `src/audit_logging.py` | SOC 2 compliance trail | M |

---

## Technical Architecture Decision

### Option A: Port Python to .NET (Recommended)

```
FiscalFlow (Python)  →  Rewrite  →  M365 Guard (.NET 10)

Pros:
✅ Single runtime/language stack
✅ Easier deployment (single artifact)
✅ Certificate auth pattern preserved
✅ Better for ISV Marketplace pattern

Cons:
❌ Rewrite effort (~3 weeks for Phase 1)
❌ Lose Python ML libraries (anomaly detection)
```

### Option B: Hybrid Architecture

```
M365 Guard (.NET) ←→ REST ←→ FiscalFlow APIs (Python)

Pros:
✅ Faster time-to-market (days vs weeks)
✅ Preserve existing Python code
✅ ML libraries available (sklearn, pandas)

Cons:
❌ Two runtimes to maintain
❌ Deployment complexity (2 containers)
❌ Network latency between services
```

### Option C: Microservices (Future State)

```
┌─────────────────────────────────────────────────────┐
│                    API Gateway                      │
├──────────┬──────────┬──────────┬───────────────────┤
│  Auth    │  Audit   │  SaaS    │  Analytics        │
│  (.NET)  │  (.NET)  │ (Python) │  (Python+ML)      │
└──────────┴──────────┴──────────┴───────────────────┘

Best for: Long-term scalability
Not recommended for: Initial Marketplace launch (too complex)
```

---

## Key Code Snippets (For Review)

### SaaS Fulfillment Client (Python → .NET port needed)

```python
# FiscalFlow: src/saas/fulfillment_client.py
class FulfillmentApiClient:
    async def resolve_token(self, marketplace_token: str) -> SubscriptionDetails
    async def activate_subscription(self, subscription_id: str, plan_id: str) -> bool
    async def get_subscription(self, subscription_id: str) -> Subscription
    async def update_subscription(self, subscription_id: str, plan_id: str) -> bool
    async def delete_subscription(self, subscription_id: str) -> bool
```

### Unified Dashboard (Service Aggregation)

```python
# FiscalFlow: src/m365guard_dashboard.py
class M365GuardDashboard:
    services = ["intune", "entra", "exchange", "sharepoint", "azure_security"]
    
    def calculate_compliance_score(self) -> float
    def get_summary_by_service(self) -> dict
    def generate_executive_report(self) -> bytes  # PDF export
```

### AI Anomaly Detection

```python
# FiscalFlow: src/anomaly_detection.py
class AnomalyDetector:
    model = IsolationForest(contamination=0.1, random_state=42)
    
    def train(self, historical_data: pd.DataFrame) -> None
    def detect(self, current_data: pd.DataFrame) -> List[Anomaly]
    def explain(self, anomaly: Anomaly) -> str  # Human-readable explanation
```

---

## Recommended Approach

### Week 1-2: Phase 1 (MVP for Marketplace)

```
Day 1-2:   Integrate MCP server from Heyson315/M365Guard ✅ (already done!)
Day 3-5:   Port SaaS Fulfillment Client to .NET
Day 6-7:   Port Webhook Handler to .NET
Day 8-9:   Implement Landing Page in M365 Guard
Day 10-12: Add REST API layer (ASP.NET Core minimal APIs)
Day 13-14: Integration testing + Marketplace submission prep
```

### Week 3: Phase 2 (Enhanced Features)

```
Day 15-17: Port Unified Dashboard
Day 18-19: Enhanced Excel exports (charts, pivot tables)
Day 20-21: Security Portal integration
```

### Post-Launch: Phase 3 (Differentiators)

```
Month 2:   AI Anomaly Detection (decide: Python microservice vs .NET port)
Month 2:   Add more MCP plugins (Intune, Defender, Purview)
Month 3:   D365 Integration
Ongoing:   Immutable Audit Logging enhancements
```

---

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Port introduces bugs | High | Unit tests from FiscalFlow as spec |
| ML libraries unavailable in .NET | Medium | Use ML.NET or keep Python microservice |
| Timeline slip | Medium | Phase 1 only for MVP launch |
| Architect prefers different approach | Low | This doc as discussion starter |

---

## Questions for Architecture Review

1. **Runtime Decision:** Option A (.NET only) vs Option B (Hybrid)?
2. **ML Strategy:** Port anomaly detection to ML.NET or keep Python microservice?
3. **MCP Strategy:** Keep MCP server as Python sidecar or port to .NET?
4. **Timeline:** Is 2-week MVP realistic with current team capacity?
5. **Testing:** Should we port FiscalFlow tests as regression suite?
6. **CI/CD:** Single pipeline or separate for .NET/Python components?

---

## Attachments (For Deep Dive)

### FiscalFlow Modules (Archive)

| File | Location | Lines of Interest |
|------|----------|-------------------|
| SaaS Fulfillment | `_archive/.../FiscalFlow/src/saas/fulfillment_client.py` | All (~200 lines) |
| Webhook Handler | `_archive/.../FiscalFlow/src/saas/webhook_handler.py` | All (~150 lines) |
| REST API | `_archive/.../FiscalFlow/src/m365guard_api.py` | Lines 1-100 |
| Dashboard | `_archive/.../FiscalFlow/src/m365guard_dashboard.py` | Lines 1-100 |
| Anomaly Detection | `_archive/.../FiscalFlow/src/anomaly_detection.py` | Lines 1-80 |

### MCP Server (Already Built - Heyson315/M365Guard)

| File | Location | Purpose |
|------|----------|---------|
| **MCP Server Core** | `Heyson315/M365Guard/src/mcp/m365_mcp_server.py` | Plugin-based MCP server |
| **SharePoint Plugin** | `Heyson315/M365Guard/src/mcp/plugins/sharepoint_tools/tools.py` | Permissions analysis |
| **QuickBooks Plugin** | `Heyson315/M365Guard/src/mcp/plugins/quickbooks_tools/tools.py` | Financial integration |
| **Integration Demo** | `Heyson315/M365Guard/MCP_INTEGRATION_DEMO.md` | Full documentation |
| **VS Code MCP Config** | `Heyson315/M365Guard/.vscode/mcp.json` | Docker + GitHub MCP |

### GitHub MCP Server (Reference Implementation)

| File | Location | Purpose |
|------|----------|---------|
| **Full MCP Server** | `Heyson315/github-mcp-server/` | Go implementation, production-ready |
| **Documentation** | `Heyson315/github-mcp-server/README.md` | Integration guides |

---

## Next Steps

- [ ] Tech team review of this proposal
- [ ] Architecture decision on runtime (Option A/B/C)
- [ ] Sprint planning for Phase 1
- [ ] Assign developer(s) for port work
- [ ] Update M365 Guard roadmap

---

**Document Owner:** [Your Name]  
**Review Deadline:** [TBD]  
**Decision Meeting:** [TBD]
