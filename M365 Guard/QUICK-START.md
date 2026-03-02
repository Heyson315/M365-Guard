# 🚀 Quick Start - NO SECRETS NEEDED!

## ✅ You're Already Configured!

Your app is set up with:
- **Tenant ID:** `e8b875bc-6cf4-4c2e-9ded-1371aaf26563`
- **Client ID:** `dcbaad46-52ed-4d3f-97a1-495cfae1039f` (M365Guard-CA-Analyzer-2026)
- **No client secrets required!** 🔐

---

## 🎯 Run It RIGHT NOW (30 seconds):

### **Step 1: Log in to Azure CLI**
```powershell
az login
```

### **Step 2: Press F5 in Visual Studio**
That's it! The app uses your Azure CLI credentials automatically!

---

## 📊 What You'll See:

```
═══════════════════════════════════════════════════════
   M365 Guard - Security & Audit Suite
   Rahman Finance and Accounting P.L.LC
═══════════════════════════════════════════════════════

🔧 Initializing services...
🔑 Using Azure CLI credentials (development mode)
🔗 Testing Microsoft Graph connection...

✓ Successfully connected to: [Your Tenant Name]

── Sign-In Audits ─────────────────────────────────────
1. 📊 Export NonInteractive Sign-Ins to Excel
2. 📊 Export Interactive Sign-Ins to Excel  ← For Conditional Access!
...
```

---

## 🎯 For Conditional Access Analysis:

**Select option 2** - Interactive Sign-Ins include the **Conditional Access Status** column!

Data exports to: `E:\source\Dashboard for Kings.xlsm`

---

## 🔐 How Authentication Works (No Secrets!):

```
Priority 1: Certificate from Azure Key Vault (Production)
   ↓ not found
Priority 2: Certificate from local certificate store
   ↓ not found
Priority 3: Azure CLI credentials (Development) ✅ ← You're here!
```

---

## ⚡ FASTEST PATH TO YOUR CA DATA:

```powershell
# 1. Ensure logged in
az login

# 2. Run the app (in VS or PowerShell)
cd "E:\source\M365 Guard\M365 Guard"
dotnet run

# 3. Type: 2
# 4. Press Enter

# DONE! Check Dashboard for Kings.xlsm
```

---

## 🎉 **You're literally ONE command away!**

Type: `az login` then Press F5!

No secrets, no certificates needed for development! 🚀
