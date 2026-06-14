# 数据库设计汇总

## 1. 数据库定位

系统当前使用 SQLite + EF Core。数据库初始化入口统一为 `DatabaseInitializer.Initialize()`，表结构由 `StudentsManagerDbContext` 映射，默认数据和演示数据集中维护在 `StudentsManagerSystem/Data/DatabaseInitializer.cs`。

## 2. 主键与业务键原则

- `Id`：数据库内部主键，用于 EF Core 实体标识。
- `StudentNo`：学生业务唯一键，必须唯一，学生档案、学籍、成绩等涉及学生身份展示和业务关联时优先使用学号。
- `IdCard`：学生身份证号唯一键。
- `CourseNo`：课程编号唯一键。
- `DepartmentNo`、`MajorNo`、`ClassNo`：基础数据编号唯一键。

## 3. 学生档案相关表

### 3.1 Students

用途：学生基本信息主表。

关键字段：
- `Id`：内部主键
- `StudentNo`：学号，唯一业务键
- `Name`：姓名
- `Gender`：性别
- `BirthDate`：出生日期
- `IdCard`：身份证号，唯一
- `Department`：院系名称
- `Major`：专业名称
- `Class`：班级名称
- `EnrollmentDate`：入学日期

### 3.2 FamilyInfos

用途：学生家庭成员信息。

关键字段：
- `Id`：内部主键
- `StudentNo`：学号，关联 `Students.StudentNo`
- `RelationName`：关系人姓名
- `Relationship`：关系
- `PhoneNumber`：联系电话
- `WorkUnit`：工作单位
- `Address`：地址

说明：学生档案页面不展示 `StudentId`，统一展示和使用 `StudentNo`。

### 3.3 RewardRecords

用途：学生奖励记录。

关键字段：
- `Id`：内部主键
- `StudentNo`：学号，关联 `Students.StudentNo`
- `RewardDate`：奖励日期
- `RewardType`：奖励类型
- `RewardLevel`：奖励等级
- `RewardReason`：奖励原因
- `RewardUnit`：颁发单位

### 3.4 PunishmentRecords

用途：学生处分记录。

关键字段：
- `Id`：内部主键
- `StudentNo`：学号，关联 `Students.StudentNo`
- `PunishmentDate`：处分日期
- `PunishmentType`：处分类型
- `PunishmentLevel`：处分等级
- `PunishmentReason`：处分原因
- `CancelDate`：撤销日期
- `Status`：处分状态

### 3.5 HealthRecords

用途：学生体检记录。

关键字段：
- `Id`：内部主键
- `StudentNo`：学号，关联 `Students.StudentNo`
- `CheckDate`：体检日期
- `Height`：身高
- `Weight`：体重
- `BloodType`：血型
- `Vision`：视力
- `HealthStatus`：健康状况

## 4. 学籍管理相关表

### 4.1 StudentRegistrations

用途：每学期学籍注册记录。

关键字段：`StudentNo`、`StudentName`、`RegistrationDate`、`AcademicYear`、`Semester`、`Status`。

### 4.2 StatusChangeRecords

用途：学籍变动记录。

关键字段：`StudentNo`、`StudentName`、`ChangeDate`、`ChangeType`、`OriginalInfo`、`NewInfo`、`Reason`、`ApprovalStatus`。

### 4.3 ScholarshipInfos

用途：奖助学金记录。

关键字段：`StudentNo`、`StudentName`、`AcademicYear`、`Semester`、`ScholarshipType`、`ScholarshipLevel`、`Amount`、`AwardDate`、`Status`。

### 4.4 GraduationInfos

用途：毕业信息记录。

关键字段：`StudentNo`、`StudentName`、`GraduationDate`、`GraduationType`、`DegreeType`、`CertificateNo`、`DegreeNo`。

## 5. 成绩管理相关表

### 5.1 Courses

用途：课程基础信息。

关键字段：
- `CourseNo`：课程编号，唯一
- `CourseName`：课程名称
- `Credit`：学分
- `CourseType`：课程类型
- `Hours`：课时
- `Department`：开课院系

### 5.2 Scores

用途：学生课程成绩。

关键字段：`StudentNo`、`StudentName`、`AcademicYear`、`Semester`、`CourseNo`、`CourseName`、`Credit`、`RegularScore`、`ExamScore`、`TotalScore`、`Grade`、`Status`。

导入合并规则：按 `StudentNo + AcademicYear + Semester + CourseNo` 判断是否已存在。

## 6. 基础数据相关表

### 6.1 Departments

用途：院系基础数据。

关键字段：`DepartmentNo`、`DepartmentName`、`DepartmentHead`、`PhoneNumber`。

### 6.2 Majors

用途：专业基础数据。

关键字段：`MajorNo`、`MajorName`、`DepartmentName`、`Duration`、`DegreeType`。

### 6.3 Classes

用途：班级基础数据。

关键字段：`ClassNo`、`ClassName`、`DepartmentName`、`MajorName`、`Grade`、`ClassTeacher`、`StudentCount`。

## 7. 系统支撑表

### 7.1 Users

用途：登录用户。

关键字段：`Username`、`PasswordHash`、`DisplayName`、`Role`、`IsActive`、`LastLoginAt`。

### 7.2 LookupOptions

用途：通用业务字典。

关键字段：`Category`、`Value`、`SortOrder`、`IsActive`。

当前字典范围：性别、政治面貌、学期、奖助状态、审批状态。

## 8. 删除与关联规则

删除学生时：
- 家庭、奖励、处分、体检档案按 `StudentNo` 清理。
- 学籍、奖助、毕业、成绩等现有模块仍保留 `StudentId` 和 `StudentNo` 字段，当前删除逻辑继续兼容现有数据。

后续开发要求：新增涉及学生身份的业务表必须包含 `StudentNo`，页面展示不得出现 `StudentId`。

## 9. 初始化与迁移规则

- 默认数据只能写在 `DatabaseInitializer`。
- 业务代码不得硬编码默认院系、专业、班级、课程、学生、教师等数据。
- 当前项目使用 `EnsureCreated`，对既有 SQLite 数据库的轻量结构补齐由 `DatabaseInitializer` 执行。
- 后续如引入 EF Core Migration，应保持统一初始化入口，不得在业务层补种子数据。
