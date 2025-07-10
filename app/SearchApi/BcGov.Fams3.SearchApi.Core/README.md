# SearchAPI Core Classes

This repository defines a shared set of .NET 8 classes for the SearchAPI domain. These classes are distributed via NuGet and are intended to be consumed by related services for request and response modeling.

## 🧩 Subcomponents

This package includes multiple sub-namespaces:

- `BcGov.Fams3.SearchApi.Core.Adapters`
- `BcGov.Fams3.SearchApi.Core.BatchFactory`
- `BcGov.Fams3.SearchApi.Core.Configuration`
- `BcGov.Fams3.SearchApi.Core.DependencyInjection`
- `BcGov.Fams3.SearchApi.Core.MassTransit`
- `BcGov.Fams3.SearchApi.Core.OpenTracing`

These contain core classes, used across SearchAPI components.

---

## 📦 NuGet Package Info

This project uses the following NuGet packaging configuration (defined in `BcGov.Fams3.SearchApi.Contracts.csproj`):

```xml
    <TargetFramework>net8.0</TargetFramework>
    <Authors>PathFinder</Authors>
    <Company>BcGov</Company>
    <Description>A core Library for BCGOV SearchApi Related Products</Description>
    <Version>1.1.0</Version>
    <PackageVersion>1.1.0</PackageVersion>
    <GeneratePackageOnBuild>True</GeneratePackageOnBuild>
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
dotnet nuget push ./bin/Release/BcGov.Fams3.SearchApi.Core.1.1.0.nupkg \
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
