# BlazeGate：基于 YARP 的可视化 API 网关

BlazeGate 是一个围绕 **Microsoft YARP** 构建的 API 网关方案。这个仓库不仅包含网关本体，还包含可视化管理面板、RBAC 组件库、远程服务调用封装以及多个集成示例，适合直接部署，也适合拆分成 NuGet 组件接入自己的系统。

## 核心能力

- **动态网关配置**：YARP 路由、集群、目标地址等配置存储在数据库中，并由网关后台服务定时刷新。
- **可视化管理**：提供 `BlazeGate.Dashboard` 管理面板，用于维护服务、路由、目标地址、白名单、RSA 密钥、用户、角色和页面权限。
- **RBAC 权限控制**：内置用户、角色、页面、权限点管理，并支持为不同服务分别分配权限。
- **JWT 认证**：支持基于 RSA 密钥的令牌签发、校验、刷新和注销。
- **服务治理**：支持 YARP 健康检查、负载均衡、转发头处理、会话相关配置扩展。
- **性能优化**：包含响应缓存、响应压缩和单飞缓存（SingleFlight）实现。
- **组件化复用**：仓库中的多个 `src/*` 项目可以单独打包发布并集成到业务系统中。

## 仓库结构

### 运行时项目

| 路径 | 说明 |
| --- | --- |
| `src/BlazeGate` | 主网关服务。负责数据库配置加载、认证授权、Swagger、健康检查、YARP 转发等。 |
| `src/BlazeGate.Dashboard` | 管理后台，基于 Blazor Server。默认通过 `appsettings.json` 中的账号密码登录。 |
| `src/BlazeGate.Cluster` | 纯 YARP 反向代理示例，使用 `appsettings.json` 中的静态 `ReverseProxy` 配置把请求转发到多个 BlazeGate 实例。 |

### 可复用组件

| 项目 | 用途 |
| --- | --- |
| `src/BlazeGate.AspNetCore` | 给业务服务接入 BlazeGate 注册/心跳能力。 |
| `src/BlazeGate.JwtBearer` | 给业务服务接入 BlazeGate 的 JWT Bearer 认证。 |
| `src/BlazeGate.RBAC.Components` | 提供 Blazor/WASM 可复用的 RBAC 页面与认证状态管理。 |
| `src/BlazeGate.Services.Interface` | 服务接口定义。 |
| `src/BlazeGate.Services.Implement.Remote` | 通过 HTTP 调用 BlazeGate API 的远程服务实现。 |
| `src/BlazeGate.Model` | 实体模型、DTO、EF Core 迁移和通用配置对象。 |
| `src/BlazeGate.Common` | 一些公共基础能力。 |

### 示例项目

| 路径 | 说明 |
| --- | --- |
| `samples/BlazeGate.WebApi.Sample` | 演示 Web API 如何接入 BlazeGate 注册、JWT 和雪花 ID。 |
| `samples/BlazeGate.BlazorWasmApp.Sample` | 演示 Blazor WebAssembly 如何接入 RBAC 组件。 |
| `samples/BlazeGate.BlazorWebApp.Sample` | 演示 Blazor Web App 场景。 |
| `samples/BlazeGate.BlazorWebApp.Client.Sample` | `BlazeGate.BlazorWebApp.Sample` 的客户端项目。 |
| `samples/BlazeGate.Components.Sample` | 示例共享组件与页面资源。 |
| `samples/BlazeGate.Model.Sample` | 示例模型与 EF Core 上下文。 |

## 技术栈

- .NET 8（仓库主体）
- 部分示例包含 .NET 9 目标框架（`samples/BlazeGate.BlazorWasmApp.Sample`）
- ASP.NET Core
- Microsoft YARP
- Entity Framework Core + SQL Server
- Blazor Server / Blazor WebAssembly
- Ant Design Blazor
- Autofac

## 快速开始

### 1. 环境准备

- 安装 **.NET 8 SDK**。
- 如果需要完整编译整个解决方案，建议安装 **.NET 9 SDK**（因为示例项目里有 `net9.0` 目标框架）。
- 准备一个 **SQL Server / LocalDB** 实例。

### 2. 配置数据库与服务地址

至少检查以下配置文件：

- `/home/runner/work/BlazeGate/BlazeGate/src/BlazeGate/appsettings.json`
- `/home/runner/work/BlazeGate/BlazeGate/src/BlazeGate.Dashboard/appsettings.json`

重点配置项：

| 配置项 | 位置 | 说明 |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | 网关、Dashboard | BlazeGate 元数据和权限数据使用的数据库连接。 |
| `BlazeGate:BlazeGateAddress` | Dashboard、示例项目 | Dashboard/客户端调用 BlazeGate API 时使用的网关地址。默认是 `http://localhost:5182`。 |
| `Login:Username` / `Login:Password` | Dashboard | Dashboard 登录账号，默认值为 `admin` / `admin`。 |

> `src/BlazeGate` 和 `src/BlazeGate.Dashboard` 启动时都会执行 `IDbInitializer.Initialize()`，如果存在待执行迁移会自动执行。

### 3. 启动 BlazeGate 与 Dashboard

在仓库根目录执行：

```bash
dotnet run --project ./src/BlazeGate
dotnet run --project ./src/BlazeGate.Dashboard
```

默认开发地址：

| 服务 | 默认地址 | 来源 |
| --- | --- | --- |
| BlazeGate 网关 | `http://localhost:5182` | `src/BlazeGate/Properties/launchSettings.json` |
| Dashboard | `http://localhost:5270` | `src/BlazeGate.Dashboard/Properties/launchSettings.json` |

常用入口：

- 网关 Swagger：`http://localhost:5182/swagger`
- 网关健康检查：`http://localhost:5182/api/health`
- Dashboard：`http://localhost:5270`

### 4. 登录管理后台

Dashboard 默认读取 `/home/runner/work/BlazeGate/BlazeGate/src/BlazeGate.Dashboard/appsettings.json` 中的登录信息：

- 用户名：`admin`
- 密码：`admin`

注意：

- 这是 **Dashboard 自身的后台登录**。
- 业务系统的用户、角色、页面权限、RSA 密钥、服务配置等数据由 BlazeGate 数据库管理，并通过 Dashboard 页面维护。

### 5. 可选：启动集群入口

如果你希望再加一层纯 YARP 入口，可以启动：

```bash
dotnet run --project ./src/BlazeGate.Cluster
```

`src/BlazeGate.Cluster/appsettings.json` 默认把请求转发到：

- `http://localhost:5182/`
- `http://localhost:5183/`

适合本地演示多实例转发、健康检查和负载均衡。

## 业务系统如何接入

### Web API 接入

参考 `samples/BlazeGate.WebApi.Sample/Program.cs`，典型接入方式包括：

- `builder.AddBlazeGateJwtBearer()`：接入 BlazeGate JWT 认证
- `builder.AddBlazeGate()`：接入 BlazeGate 服务注册/配置
- `builder.Services.AddBlazeGateSnowFlake()`：接入雪花 ID 服务

示例项目还展示了以下配置：

- `BlazeGate:ServiceName`
- `BlazeGate:Token`
- `BlazeGate:Address`
- `Jwt:PublicKey`

### Blazor / RBAC 接入

参考以下项目：

- `samples/BlazeGate.BlazorWasmApp.Sample`
- `samples/BlazeGate.BlazorWebApp.Sample`

核心入口是：

- `services.AddBlazeGateRBAC()`

该组件会注册：

- 菜单与页面数据服务
- Token 存储服务（Cookie / LocalStorage / SessionStorage / Memory / File）
- 认证状态管理
- 本地化支持

## 常用开发命令

```bash
dotnet build ./BlazeGate.sln
dotnet test ./BlazeGate.sln --no-build
```

说明：

- 当前解决方案中没有独立测试项目，`dotnet test` 主要用于确认解决方案可测试目标是否正常。
- `publish-nuget.ps1` 用于批量更新版本、打包并发布多个组件到 NuGet。

## 发布 NuGet

如需批量更新版本、打包并发布组件，可参考仓库根目录下的 `publish-nuget.ps1`。
