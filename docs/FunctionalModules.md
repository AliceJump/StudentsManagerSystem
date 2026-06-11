# 学生管理系统功能模块详细文档

## 1. 文档目的

本文档用于说明学生管理系统当前所有功能模块的业务职责、用户流程、代码实现、数据流和功能边界。后续开发任务应优先参考本文档与 `README.md`，保持术语、模块划分和业务流程一致。

## 2. 项目定位

学生管理系统是一个基于 .NET 8.0 + WPF 的课程项目，用于管理学生档案、学籍、成绩、基础数据和查询统计。系统当前已从界面原型进入真实数据接入阶段，使用 SQLite + EF Core 作为数据层，使用 Repository 封装数据访问，使用 Service 承载部分业务校验、日志和保存逻辑。

## 3. 总体架构

### 3.1 界面架构

- 主窗口：`StudentsManagerSystem/MainWindow.xaml`、`StudentsManagerSystem/MainWindow.xaml.cs`
- 功能页：`StudentsManagerSystem/Views/*`
- 子窗口：新增、修改、查看详情等表单窗口
- 导航方式：主窗口左侧菜单切换，右侧 `Frame` 动态加载功能页

### 3.2 数据架构

- EF Core 上下文：`StudentsManagerSystem/Data/StudentsManagerDbContext.cs`
- 数据库配置：`StudentsManagerSystem/Data/DatabaseConfiguration.cs`
- 数据库初始化与种子数据：`StudentsManagerSystem/Data/DatabaseInitializer.cs`
- Repository：`StudentsManagerSystem/Data/SqlServer/*Repository.cs`
- 说明：`Data/SqlServer` 是历史目录名，当前 Repository 实际通过 EF Core 访问 SQLite。

### 3.3 业务架构

- 学生业务：`StudentsManagerSystem/Services/StudentService.cs`
- 成绩业务：`StudentsManagerSystem/Services/ScoreService.cs`
- 基础数据业务：`StudentsManagerSystem/Services/BasicDataService.cs`
- 统一结果：`StudentsManagerSystem/Services/ServiceResult.cs`
- 通用校验：`StudentsManagerSystem/Common/InputValidator.cs`
- 日志：`StudentsManagerSystem/Common/AppLogger.cs`
- CSV：`StudentsManagerSystem/Common/CsvExportHelper.cs`

## 4. 主界面模块

### 4.1 模块职责

主界面负责系统导航、页面承载、当前时间显示和基础权限入口控制。

### 4.2 用户流程

1. 用户登录成功后进入主界面。
2. 左侧菜单默认停留在首页。
3. 点击菜单后，右侧内容区域加载对应功能页面。
4. 非 Admin 用户点击基础数据管理时会被限制访问。

### 4.3 功能入口

- 首页
- 学生档案管理
- 学生学籍管理
- 学生成绩管理
- 基础数据管理
- 信息查询统计

### 4.4 实现文件

- `StudentsManagerSystem/MainWindow.xaml`
- `StudentsManagerSystem/MainWindow.xaml.cs`
- `StudentsManagerSystem/ViewModels/MainViewModel.cs`

### 4.5 功能边界

- 当前仅对基础数据管理做角色级入口控制。
- 细粒度按钮权限、数据范围权限尚未实现。

## 5. 登录与权限模块

### 5.1 模块职责

登录模块负责用户身份验证，权限模块负责根据角色控制可访问功能。

### 5.2 用户流程

1. 程序启动后先初始化数据库。
2. 显示登录窗口。
3. 用户输入用户名和密码。
4. 系统校验用户是否存在、是否启用、密码哈希是否匹配。
5. 登录成功后记录显示名和角色，再进入主窗口。
6. 登录失败时显示错误提示并写入日志。

### 5.3 数据模型

- `User.Username`：登录账号
- `User.PasswordHash`：SHA256 密码哈希
- `User.DisplayName`：显示名称
- `User.Role`：角色，当前种子数据包含 `Admin` 和 `User`
- `User.IsActive`：账号是否启用
- `User.LastLoginAt`：最近登录时间

### 5.4 默认种子账号

- 管理员：`admin`，默认密码来自 `App.config` 的 `AdminPassword`，未配置时为 `Admin@123`
- 普通用户：`teacher`，默认密码为 `Teacher@123`

### 5.5 实现文件

- `StudentsManagerSystem/Views/Login/LoginWindow.xaml`
- `StudentsManagerSystem/Views/Login/LoginWindow.xaml.cs`
- `StudentsManagerSystem/Data/SqlServer/UsersRepository.cs`
- `StudentsManagerSystem/Models/User.cs`
- `StudentsManagerSystem/App.xaml.cs`

### 5.6 功能边界

- 当前没有用户管理界面。
- 当前没有密码修改功能。
- 当前权限只控制基础数据管理入口。

## 6. 学生档案管理模块

### 6.1 模块职责

学生档案管理用于维护学生基本信息，并展示家庭信息、奖励记录、处分记录和体检信息。

### 6.2 子模块

- 学生基本信息
- 家庭信息
- 奖励记录
- 处分记录
- 体检信息

### 6.3 学生基本信息功能

#### 6.3.1 已实现能力

- 查询学生列表
- 按关键字搜索：学号、姓名、院系、专业、班级
- 排序：学号升序、学号降序、姓名升序、院系升序、班级升序
- 分页：每页 10、20、50 条
- 新增学生
- 修改学生
- 删除学生
- 查看学生详情
- CSV 导入学生基本信息
- CSV 导出学生基本信息
- 保存前校验学号、姓名、身份证号、手机号、邮箱、组织信息
- 保存前检查学号和身份证号重复
- 增删改和导入导出写入日志

#### 6.3.2 用户流程

1. 进入学生档案管理。
2. 默认显示学生基本信息。
3. 可输入关键字搜索学生。
4. 可选择排序方式和分页大小。
5. 点击新增打开学生编辑窗口。
6. 点击修改打开学生编辑窗口并带入选中学生。
7. 点击删除弹出确认框，确认后删除学生及关联记录。
8. 点击查看详情打开详情窗口。
9. 点击导入选择 CSV 文件，系统按学号新增或更新。
10. 点击导出将当前筛选结果导出为 CSV。

#### 6.3.3 数据流

1. `StudentArchiveView` 接收用户操作。
2. `StudentService` 执行业务校验、重复检查、日志记录。
3. `StudentRepository` 通过 EF Core 访问 SQLite。
4. `StudentsManagerDbContext` 映射 `Students` 等表。

#### 6.3.4 删除规则

删除学生时，Repository 会同时清理以下关联数据：

- 家庭信息
- 奖励记录
- 处分记录
- 体检信息
- 学籍注册
- 学籍变动
- 奖助学金
- 毕业信息
- 成绩记录

### 6.4 家庭/奖励/处分/体检功能

#### 6.4.1 当前能力

- 页面和数据模型已存在。
- 当前页面从数据库读取由 `DatabaseInitializer` 维护的初始化示例数据。
- 新增、修改、删除、搜索暂未完整接入数据库。

#### 6.4.2 后续边界

- 后续应按学生基本信息同样的 Repository + Service + Window 流程接入 CRUD。
- 不应绕过现有数据层直接在页面中写数据库。

### 6.5 实现文件

- `StudentsManagerSystem/Views/StudentArchive/StudentArchiveView.xaml`
- `StudentsManagerSystem/Views/StudentArchive/StudentArchiveView.xaml.cs`
- `StudentsManagerSystem/Views/StudentArchive/StudentEditWindow.xaml`
- `StudentsManagerSystem/Views/StudentArchive/StudentEditWindow.xaml.cs`
- `StudentsManagerSystem/Views/StudentArchive/StudentDetailWindow.xaml`
- `StudentsManagerSystem/Views/StudentArchive/StudentDetailWindow.xaml.cs`
- `StudentsManagerSystem/Models/Student.cs`
- `StudentsManagerSystem/Services/StudentService.cs`
- `StudentsManagerSystem/Data/SqlServer/StudentRepository.cs`

## 7. 学生学籍管理模块

### 7.1 模块职责

学生学籍管理用于维护学生在校期间的注册、学籍变动、奖助学金和毕业信息。

### 7.2 子模块

- 学籍注册
- 学籍变动
- 奖助学金
- 毕业信息

### 7.3 已实现能力

- 各子模块列表展示
- 各子模块关键字搜索
- 新增记录
- 修改记录
- 删除记录
- 查看详情或只读查看
- 表单基础校验
- 重复记录检查
- 数据通过 Repository 持久化到 SQLite

### 7.4 用户流程

1. 进入学生学籍管理。
2. 切换顶部 Tab 选择业务类型。
3. 输入关键字后点击搜索。
4. 点击新增打开对应编辑窗口。
5. 点击修改对选中记录编辑。
6. 点击删除弹出确认框，确认后删除记录。
7. 点击查看详情打开只读窗口。

### 7.5 数据流

1. `StudentStatusView` 根据当前 Tab 调用对应查询方法。
2. 编辑窗口完成校验后调用 `StudentStatusRepository`。
3. Repository 通过 EF Core 访问 SQLite。

### 7.6 主要校验

- 学号格式
- 姓名格式
- 学年格式
- 学期合法值
- 金额非负
- 必填日期和状态
- 单条记录重复检查

### 7.7 实现文件

- `StudentsManagerSystem/Views/StudentStatus/StudentStatusView.xaml`
- `StudentsManagerSystem/Views/StudentStatus/StudentStatusView.xaml.cs`
- `StudentsManagerSystem/Views/StudentStatus/RegistrationEditWindow.xaml`
- `StudentsManagerSystem/Views/StudentStatus/RegistrationEditWindow.xaml.cs`
- `StudentsManagerSystem/Views/StudentStatus/StatusChangeEditWindow.xaml`
- `StudentsManagerSystem/Views/StudentStatus/StatusChangeEditWindow.xaml.cs`
- `StudentsManagerSystem/Views/StudentStatus/ScholarshipEditWindow.xaml`
- `StudentsManagerSystem/Views/StudentStatus/ScholarshipEditWindow.xaml.cs`
- `StudentsManagerSystem/Views/StudentStatus/GraduationEditWindow.xaml`
- `StudentsManagerSystem/Views/StudentStatus/GraduationEditWindow.xaml.cs`
- `StudentsManagerSystem/Models/StudentStatus.cs`
- `StudentsManagerSystem/Data/SqlServer/StudentStatusRepository.cs`

### 7.8 功能边界

- 当前学籍模块尚未接入独立 Service 层。
- 当前未实现批量导入导出。
- 当前未实现审批流，仅记录审批状态字段。

## 8. 学生成绩管理模块

### 8.1 模块职责

成绩管理用于按学年、学期维护学生课程成绩，并提供成绩导入导出。

### 8.2 已实现能力

- 按学年和学期查询成绩
- 新增成绩
- 修改成绩
- 删除成绩
- 查看成绩详情
- 平时成绩和考试成绩录入
- 自动计算总评成绩
- 自动计算成绩等级
- CSV 导入成绩
- CSV 导出当前成绩列表
- 保存前校验学号、姓名、学年、学期、课程、学分、成绩范围
- 导入时按学号、学年、学期、课程编号新增或更新
- 增删改、导入导出写入日志

### 8.3 用户流程

1. 进入学生成绩管理。
2. 选择起始学年和学期。
3. 点击查询刷新列表。
4. 点击录入成绩打开成绩编辑窗口。
5. 选择课程后自动带出学分。
6. 输入平时成绩和考试成绩后保存，系统计算总评和等级。
7. 点击导入成绩选择 CSV 文件。
8. 点击导出成绩保存当前列表为 CSV。

### 8.4 成绩等级规则

- 90 分及以上：优秀
- 80 分及以上：良好
- 70 分及以上：中等
- 60 分及以上：及格
- 60 分以下：不及格
- 未录入总评：未评定

### 8.5 数据流

1. `ScoreView` 处理列表、导入导出和按钮事件。
2. `ScoreEditWindow` 构造成绩对象并计算总评/等级。
3. `ScoreService` 执行业务校验、导入合并和日志记录。
4. `ScoreRepository` 持久化成绩和读取课程。

### 8.6 实现文件

- `StudentsManagerSystem/Views/Score/ScoreView.xaml`
- `StudentsManagerSystem/Views/Score/ScoreView.xaml.cs`
- `StudentsManagerSystem/Views/Score/ScoreEditWindow.xaml`
- `StudentsManagerSystem/Views/Score/ScoreEditWindow.xaml.cs`
- `StudentsManagerSystem/Models/Score.cs`
- `StudentsManagerSystem/Services/ScoreService.cs`
- `StudentsManagerSystem/Data/SqlServer/ScoreRepository.cs`

### 8.7 功能边界

- 当前仅支持 CSV 导入导出。
- 当前没有 Excel、PDF、打印功能。
- 当前没有成绩统计图表。

## 9. 基础数据管理模块

### 9.1 模块职责

基础数据管理用于维护院系、专业和班级等学校基础组织信息。

### 9.2 子模块

- 院系管理
- 专业管理
- 班级管理

### 9.3 已实现能力

- 院系列表展示、新增、修改、删除
- 专业列表展示、新增、修改、删除
- 班级列表展示、新增、修改、删除
- 基础必填校验
- 手机号格式校验
- 学制和学生人数数字校验
- 增删改写入日志
- 非 Admin 用户禁止进入基础数据管理

### 9.4 用户流程

1. Admin 用户进入基础数据管理。
2. 切换 Tab 选择院系、专业或班级。
3. 点击新增打开统一编辑窗口。
4. 点击修改带入选中记录。
5. 点击删除弹出确认框，确认后删除。
6. 点击刷新重新读取数据库。

### 9.5 数据流

1. `BasicDataView` 根据当前 Tab 加载对应列表。
2. `BasicDataEditWindow` 构造院系、专业或班级对象。
3. `BasicDataService` 执行业务校验和日志记录。
4. `BasicDataRepository` 访问 SQLite。

### 9.6 实现文件

- `StudentsManagerSystem/Views/BasicData/BasicDataView.xaml`
- `StudentsManagerSystem/Views/BasicData/BasicDataView.xaml.cs`
- `StudentsManagerSystem/Views/BasicData/BasicDataEditWindow.xaml`
- `StudentsManagerSystem/Views/BasicData/BasicDataEditWindow.xaml.cs`
- `StudentsManagerSystem/Models/BasicData.cs`
- `StudentsManagerSystem/Services/BasicDataService.cs`
- `StudentsManagerSystem/Data/SqlServer/BasicDataRepository.cs`

### 9.7 功能边界

- 当前未实现基础数据导入导出。
- 当前删除基础数据未自动联动更新学生表中的院系、专业、班级文本字段。
- 当前权限是入口级控制，不是按钮级控制。

## 10. 信息查询统计模块

### 10.1 模块职责

信息查询统计用于按条件查询学生信息，并显示基础统计摘要。

### 10.2 已实现能力

- 学生信息查询
- 按学号筛选
- 按姓名筛选
- 按院系筛选
- 按班级筛选
- 查询条件重置
- 查询结果统计
- 查询结果 CSV 导出
- 导出操作写入日志

### 10.3 统计内容

- 当前结果总数
- 男生数量
- 女生数量
- 人数最多的院系

### 10.4 用户流程

1. 进入信息查询统计。
2. 选择查询类型。
3. 输入学号、姓名或选择院系、班级。
4. 点击查询显示结果和统计摘要。
5. 点击重置清空条件。
6. 点击导出保存当前结果为 CSV。

### 10.5 数据流

1. `QueryView` 从 `StudentService` 获取学生列表。
2. 页面本地根据条件过滤学生集合。
3. 页面本地计算统计摘要。
4. `CsvExportHelper` 导出当前结果。

### 10.6 实现文件

- `StudentsManagerSystem/Views/Query/QueryView.xaml`
- `StudentsManagerSystem/Views/Query/QueryView.xaml.cs`
- `StudentsManagerSystem/Services/StudentService.cs`
- `StudentsManagerSystem/Common/CsvExportHelper.cs`

### 10.7 功能边界

- 当前统计以文本摘要为主，没有图表。
- 当前查询条件固定，尚未提供用户自定义组合字段。
- 当前只导出 CSV。

## 11. 数据库初始化模块

### 11.1 模块职责

数据库初始化模块负责创建 SQLite 数据库、创建表结构并写入基础种子数据。

### 11.2 初始化时机

程序启动时在登录窗口显示前执行初始化。

### 11.3 种子数据范围

- 院系
- 专业
- 班级
- 课程
- 学生
- 学生档案示例数据
- 学籍/奖助/毕业示例数据
- 成绩
- 用户

### 11.4 实现文件

- `StudentsManagerSystem/App.xaml.cs`
- `StudentsManagerSystem/Data/DatabaseInitializer.cs`
- `StudentsManagerSystem/Data/StudentsManagerDbContext.cs`
- `StudentsManagerSystem/Data/DatabaseConfiguration.cs`

### 11.5 功能边界

- 当前使用 `EnsureCreated` 创建数据库。
- 当前未使用 EF Core Migration 管理版本演进。

### 11.6 初始化规范

- 项目只能存在一个统一数据库初始化入口：`DatabaseInitializer.Initialize()`。
- 默认数据、演示数据、初始化数据只能维护在 `StudentsManagerSystem/Data/DatabaseInitializer.cs`。
- Repository、Service、View、ViewModel、测试代码不得插入硬编码种子数据。
- 如果缺少默认课程、学生、用户等数据，应修改 `DatabaseInitializer`，不能在业务代码中临时 `Add` 或写 `INSERT`。
- 业务运行期间 Repository 只负责读写业务数据，Service 只负责业务逻辑，界面只负责交互。

## 12. 日志模块

### 12.1 模块职责

日志模块负责记录系统运行和关键业务操作，便于排查问题和追踪操作。

### 12.2 已记录事件

- 应用启动
- 数据库初始化开始和完成
- 数据库初始化失败
- UI 线程未处理异常
- 非 UI 线程未处理异常
- 登录成功、失败、异常
- 学生增删改
- 成绩增删改
- 基础数据增删改
- 学生基本信息导入导出
- 成绩导入导出
- 查询结果导出

### 12.3 日志位置

日志文件输出到程序运行目录下的 `Logs/StudentsManagerSystem.log`。

### 12.4 实现文件

- `StudentsManagerSystem/Common/AppLogger.cs`
- `StudentsManagerSystem/App.xaml.cs`
- 各 Service 和导入导出页面

### 12.5 功能边界

- 当前没有日志查看界面。
- 当前没有日志分级配置、滚动切分和保留策略。

## 13. 导入导出模块

### 13.1 模块职责

导入导出模块用于把业务数据以 CSV 文件导入系统或导出备份。

### 13.2 已实现范围

- 学生基本信息 CSV 导入
- 学生基本信息 CSV 导出
- 成绩 CSV 导入
- 成绩 CSV 导出
- 查询结果 CSV 导出

### 13.3 导入规则

- 学生基本信息按学号识别已存在记录，存在则更新，不存在则新增。
- 成绩按学号、学年、学期、课程编号识别已存在记录，存在则更新，不存在则新增。
- 导入前会经过 Service 业务校验。

### 13.4 导出规则

- 学生基本信息导出当前搜索和排序后的结果。
- 成绩导出当前页面显示的成绩集合。
- 查询结果导出当前查询结果。

### 13.5 实现文件

- `StudentsManagerSystem/Common/CsvExportHelper.cs`
- `StudentsManagerSystem/Views/StudentArchive/StudentArchiveView.xaml.cs`
- `StudentsManagerSystem/Views/Score/ScoreView.xaml.cs`
- `StudentsManagerSystem/Views/Query/QueryView.xaml.cs`

### 13.6 功能边界

- 当前不支持 Excel 导入导出。
- 当前不支持 PDF 导出。
- 当前不支持打印。

## 14. 通用校验模块

### 14.1 模块职责

通用校验模块提供跨模块复用的输入格式校验能力。

### 14.2 已实现校验

- 学号：4 到 20 位字母或数字
- 身份证号：18 位数字或末位 X
- 手机号：11 位数字，以 1 开头
- 邮箱：可为空，非空时校验邮箱格式
- 姓名：2 到 50 位汉字或字母
- 学年：4 位年份
- 学期：`1`、`2`、`第一学期`、`第二学期`
- 金额：非负 decimal

### 14.3 实现文件

- `StudentsManagerSystem/Common/InputValidator.cs`

## 15. 功能完成度总览

| 模块 | 当前状态 | 主要缺口 |
| --- | --- | --- |
| 主界面 | 已实现 | 首页内容可继续丰富 |
| 登录与权限 | 部分实现 | 用户管理、密码修改、细粒度权限 |
| 学生基本信息 | 已接入真实 CRUD | Excel/PDF/打印 |
| 家庭信息 | 示例展示 | 完整 CRUD、搜索、导入导出 |
| 奖励记录 | 示例展示 | 完整 CRUD、搜索、导入导出 |
| 处分记录 | 示例展示 | 完整 CRUD、搜索、导入导出 |
| 体检信息 | 示例展示 | 完整 CRUD、搜索、导入导出 |
| 学籍管理 | 已接入真实 CRUD | Service 层统一、导入导出 |
| 成绩管理 | 已接入真实 CRUD 和 CSV | 图表、Excel/PDF/打印 |
| 基础数据 | 已接入真实 CRUD | 导入导出、关联数据一致性处理 |
| 查询统计 | 部分实现 | 图表、更多组合条件 |
| 日志 | 部分实现 | 日志查看、滚动策略 |

## 16. 后续开发原则

- 新增功能优先沿用当前模块划分。
- 数据访问优先通过 Repository，不在页面中直接操作 `DbContext`。
- 业务校验和保存逻辑优先放入 Service。
- 用户可见流程应保持“页面 -> 编辑窗口 -> Service -> Repository -> SQLite”的方向。
- 新增导入导出优先复用 `CsvExportHelper` 或在 Common 中扩展通用工具。
- 涉及权限时优先基于现有 `User.Role` 扩展，不新增独立权限系统，除非需求明确。
- 修改文档时应同步 README 和本文件中的功能边界。
- XAML、View、ViewModel、Service、Repository 不得硬编码院系、专业、班级、课程、学期、状态等可变业务数据。
- 可变业务选项应从业务表或 `LookupOptions` 字典表读取，默认字典数据统一在 `DatabaseInitializer` 中维护。
- 允许硬编码的占位选项仅限“全部”“请选择”“未选择”“无数据”。
