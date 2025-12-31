# BlazeGate：基于 YARP 的可视化 API 网关

BlazeGate 是一个功能强大、高度可配置的 API 网关解决方案，它基于 **Microsoft YARP (Yet Another Reverse Proxy)** 构建，并集成了**可视化管理面板**、**动态配置**、**RBAC 权限控制**和**分布式服务治理**等核心功能。它旨在为现代微服务架构提供一个集中、高效且易于维护的流量入口。

## 核心特性

BlazeGate 不仅仅是一个简单的反向代理，它通过深度集成和扩展 YARP，提供了企业级 API 网关所需的全套功能：

| 特性类别 | 功能描述 | 关键技术/组件 |
| :--- | :--- | :--- |
| **可视化管理** | 提供基于 **Ant Design Blazor** 的 Web 控制面板，支持对路由、集群、目标服务、用户和权限进行直观的 CRUD 操作。 | Blazor, Ant Design Blazor |
| **动态配置** | 将 YARP 的所有配置（路由、集群、目标）存储在 **SQL Server** 数据库中，实现配置的**热加载**和**实时更新**，无需重启网关服务。 | YARP `LoadFromDatabase` 扩展, EF Core |
| **安全与权限** | 内置 **RBAC (Role-Based Access Control)** 权限管理系统，支持用户、角色、页面和权限点的精细化控制。支持 **JWT Bearer** 认证、**IP 白名单**和 **RSA 密钥**管理。 | ASP.NET Core Identity, JWT, RBAC |
| **服务治理** | 支持**主动 (Active)** 和**被动 (Passive)** 健康检查策略，自动隔离不健康的目标服务。提供多种**负载均衡**策略（如 PowerOfTwoChoices）。 | YARP Health Checks, Load Balancing |
| **性能与扩展** | 支持**响应缓存**和 **Gzip/Brotli 压缩**以提升性能。内置 **Snowflake** 分布式唯一 ID 生成器。支持 **Autofac** 依赖注入。 | Response Caching, Response Compression, Snowflake ID |
| **集群与部署** | 支持**分布式缓存**（基于 SQL Server）和**会话关联 (Session Affinity)**，便于集群部署和高可用性配置。 | Distributed Cache, Session Affinity |

## 技术栈

BlazeGate 采用现代化的 .NET 技术栈构建，确保了高性能和可维护性：

*   **后端框架**: .NET 8+
*   **反向代理**: Microsoft YARP (Yet Another Reverse Proxy)
*   **数据库**: SQL Server (通过 EF Core)
*   **UI 框架**: Blazor WebAssembly / Blazor Server
*   **UI 组件库**: Ant Design Blazor
*   **依赖注入**: Autofac
*   **数据映射**: AutoMapper

## 模块结构

项目结构清晰，职责分离，主要模块包括：

| 模块名称 | 描述 |
| :--- | :--- |
| `src/BlazeGate` | **核心网关服务**。作为 API 网关的入口，负责 YARP 配置加载、认证授权、服务治理等核心逻辑。 |
| `src/BlazeGate.Dashboard` | **可视化管理面板**。基于 Blazor Server 构建，提供用户界面进行网关配置和 RBAC 管理。 |
| `src/BlazeGate.Model` | 共享模型、数据传输对象 (DTO)、EF Core 实体定义以及数据库迁移文件。 |
| `src/BlazeGate.RBAC.Components` | 基于 Blazor 的 RBAC 权限组件库，包含登录、用户、角色、权限等 UI 页面和逻辑。 |
| `src/BlazeGate.Services.Interface` | 服务接口定义（供本地和远程实现使用）。 |
| `src/BlazeGate.Services.Implement` | 服务接口的具体实现（如数据库操作、业务逻辑）。 |
| `src/BlazeGate.Services.Implement.Remote` | 服务接口的远程实现，用于 Dashboard 通过 HTTP 调用网关 API。 |
| `src/BlazeGate.Cluster` | 包含 YARP 的基本配置，可作为独立的反向代理服务运行。 |
| `samples/` | 包含 Blazor 和 Web API 的示例项目，展示如何集成和使用 BlazeGate 的组件。 |

## 快速开始

### 1. 环境准备

*   安装 [.NET 8 SDK](https://dotnet.microsoft.com/download) 或更高版本。
*   准备一个 **SQL Server** 实例。

### 2. 数据库配置

1.  修改 `src/BlazeGate/appsettings.json` 和 `src/BlazeGate.Dashboard/appsettings.json` 文件中的 `ConnectionStrings:DefaultConnection`，指向您的 SQL Server 实例。
2.  **数据库初始化**: 运行项目时，网关和 Dashboard 都会尝试自动执行 EF Core 迁移 (`context.Database.Migrate()`)，创建所需的数据库表（包括 YARP 配置表、用户表等）。

### 3. 运行项目

BlazeGate 包含核心网关服务 (`src/BlazeGate`) 和管理面板 (`src/BlazeGate.Dashboard`)，您需要同时运行这两个项目。

**方法一：使用 Visual Studio 或 Rider**
直接打开解决方案文件，同时启动 `BlazeGate` 和 `BlazeGate.Dashboard` 两个项目。

**方法二：使用命令行**
在项目根目录执行以下命令，分别启动两个服务：

```bash
# 1. 启动核心网关服务 (API Gateway)
dotnet run --project src/BlazeGate

# 2. 启动可视化管理面板 (Dashboard)
# 建议在新终端窗口中运行
dotnet run --project src/BlazeGate.Dashboard
```

### 4. 访问与登录

1.  **访问管理面板**: 访问 `BlazeGate.Dashboard` 启动后显示的地址（默认为 `http://localhost:5000` 或 `https://localhost:5001`，具体端口请查看 `launchSettings.json`）。
2.  **默认管理员账号**: 首次启动时，系统会自动初始化数据库和默认管理员账号。请查阅代码或首次启动日志获取默认管理员信息。

## 贡献与支持

我们欢迎所有形式的贡献，包括但不限于提交 Bug 报告、提出新功能建议或提交 Pull Request。

---
*本项目由 GHYHC 维护。*
