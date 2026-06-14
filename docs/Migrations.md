# EF Core Migration 使用说明

## 当前状态

- 项目已添加 `Microsoft.EntityFrameworkCore.Design`。
- `StudentsManagerDbContextFactory.cs` 已提供 `IDesignTimeDbContextFactory<StudentsManagerDbContext>`。
- 已添加本地 `dotnet-ef` 工具清单：`.config/dotnet-tools.json`。
- 已生成初始 Migration：`StudentsManagerSystem/Data/Migrations/20260611070354_InitialCreate.cs`。
- 后续结构变更应优先使用 Migration，不再继续扩大 `EnsureCreated` 和手写补列逻辑。

## 常用命令

```powershell
dotnet tool restore
dotnet tool run dotnet-ef migrations add <MigrationName> --project StudentsManagerSystem/StudentsManagerSystem.csproj --startup-project StudentsManagerSystem/StudentsManagerSystem.csproj --output-dir Data/Migrations
dotnet tool run dotnet-ef database update --project StudentsManagerSystem/StudentsManagerSystem.csproj --startup-project StudentsManagerSystem/StudentsManagerSystem.csproj
```

## 约束

- 默认数据仍集中维护在 `DatabaseInitializer`。
- Migration 只负责结构变更，不写业务种子数据。
- 发布前先备份 `StudentsManagerSystem.db`。
