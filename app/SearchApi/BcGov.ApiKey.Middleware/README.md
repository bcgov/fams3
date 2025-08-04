# BcGov.ApiKey.Middleware Class

This repository defines a shared .NET 8 class for the SearchAPI public domain. These class is distributed via NuGet and is intended to be consumed by related services for request and response modeling.

This package is consumed by the public DynamicsAdapter and the private RequestAdapters and SearchApi.Adapters.

A lightweight, self-contained ASP.NET Core middleware for validating API keys and restricting access to trusted hosts.
   - This middleware is designed for internal service-to-service authentication and supports:
   - API key validation via HTTP headers
   - Trusted host filtering
   - Bypass rules for Swagger and health endpoints

## 🧩 Subcomponents

This package includes multiple sub-namespaces:

- N/A


This package contains a single class: ApiKeyMiddleware, and is distributed as a minimal utility for applications requiring basic API key protection.
- ✅ Compatible with .NET 8
- ☁️ Designed for internal APIs and microservices

---

## 📦 NuGet Package Info

This project uses the following NuGet packaging configuration (defined in `BcGov.ApiKey.Middleware.csproj`):

```xml
    <TargetFramework>net8.0</TargetFramework>
    <Version>1.1.0</Version>
    <PackageReadmeFile>README.md</PackageReadmeFile>
```
✅ This README.md is included in the .nupkg for improved NuGet UI support.
ℹ️ This complies with .NET 8+ and newer NuGet recommendations for package hygiene.

🔐 Security of Dependencies
There are currently no direct package dependencies specified in the .csproj. If dependencies are added in the future, please ensure they are scanned for vulnerabilities using 
```bash
dotnet list package --vulnerable
```

### 🗂 Versioning Convention
NuGet versioning for this package follows the conventions below:
| Version Pattern | Description                     |
|-----------------|---------------------------------|
| `1.1.x.x`       | Compatible with **.NET 8+**     |
| `1.0.x.x`       | Legacy builds for **.NET Core 3.1** |

---

#### 🔄 When Making Changes

If you modify this project:

1. **Update** the `<PackageVersion>` in the `.csproj` file.
2. **Rebuild** the project in Release mode to regenerate the `.nupkg`:
   ```bash
   dotnet build -c Release
   ```
3. **Push** the new package to Nexus using one of the following methods:
   - NuGet CLI
   - Nexus Web UI

### 🚀 Uploading to Nexus
There are two ways to upload the NuGet package to Nexus:

#### ✅ Method 1: Using the CLI (Recommended)
Run the following command from the project directory:
```bash
dotnet nuget push ./bin/Release/BcGov.ApiKey.Middleware.1.1.0.nupkg \
  --api-key not-used \
  --source nexus
```
💡 Nexus OSS server is not configured to use API keys, the argument is not used but still required.

#### 🔐 What Actually Authenticates a NuGet Push to Nexus OSS
Nexus OSS uses HTTP Basic Authentication to verify credentials during a dotnet nuget push. This authentication is handled automatically by the NuGet client, and it pulls credentials from one of the following sources:

1. NuGet.Config file (this is our current setup):
When you run dotnet within your project, the client will locate and use credentials defined in your NuGet.Config, for example:

   ```xml
   <packageSourceCredentials>
   <nexus>
      <add key="Username" value="%NEXUS_REPO_USER%" />
      <add key="ClearTextPassword" value="%NEXUS_REPO_PASSWORD%" />
   </nexus>
   </packageSourceCredentials>
   ```

2. Environment variables:
These can be used to dynamically inject credentials in CI/CD pipelines or shell sessions.

3. Interactive prompt:
If credentials aren’t preconfigured, dotnet nuget push will prompt the user for a username and password.

#### 🖥️ Method 2: Using the Web UI

1. Open your browser and go to: [Nexus Web UI](https://nexus-https-dfb30e-tools.apps.silver.devops.gov.bc.ca/#browse/welcome)
2. Navigate to the appropriate **NuGet repository**.
3. Click on the **“Upload”** tab.
4. Select your `.nupkg` file.
5. Click **Upload** to complete the process.


#### 📘 For uploading detailed steps and issues, refer to:
Update Private NuGet Package - [Wiki](https://wiki.justice.gov.bc.ca/wiki/spaces/FAMS3IMP/pages/98370042/Update+Private+Nuget+Package)

### 🛠 Build Configuration
Ensure the following when building:
1. Target framework: .NET 8.0
2. GeneratePackageOnBuild=true allows automatic .nupkg generation
3. The README is bundled into the package for NuGet UI

To build:
```bash
dotnet build -c Release
```
To package manually:
```bash
dotnet pack -c Release
```
