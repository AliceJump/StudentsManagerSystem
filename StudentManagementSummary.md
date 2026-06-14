# 学生管理 - 增删改查

当前学生档案管理采用 `StudentArchiveView` + `StudentService` + `StudentRepository` 的调用链路。数据库为 SQLite，数据访问通过 EF Core 完成；当前仓储位于 `Data/Repositories`，通过 `StudentsManagerDbContextFactory` 创建 EF Core 上下文。

## 查询（Read）
```csharp
public List<Student> GetAll()
{
    using var context = StudentsManagerDbContextFactory.CreateDbContext();
    return context.Students.AsNoTracking()
        .OrderBy(student => student.StudentNo)
        .ToList();
}

public List<Student> Search(string keyword)
{
    using var context = StudentsManagerDbContextFactory.CreateDbContext();
    return context.Students.AsNoTracking()
        .Where(student =>
            EF.Functions.Like(student.StudentNo, $"%{keyword}%") ||
            EF.Functions.Like(student.Name, $"%{keyword}%") ||
            EF.Functions.Like(student.Department, $"%{keyword}%") ||
            EF.Functions.Like(student.Major, $"%{keyword}%") ||
            EF.Functions.Like(student.Class, $"%{keyword}%"))
        .OrderBy(student => student.StudentNo)
        .ToList();
}
```

家庭信息、奖励记录、处分记录和体检信息通过 `GetFamilyInfos()`、`GetRewardRecords()`、`GetPunishmentRecords()`、`GetHealthRecords()` 读取数据库列表。

## 新增（Create）
```csharp
public int Add(Student student)
{
    using var context = StudentsManagerDbContextFactory.CreateDbContext();
    context.Students.Add(student);
    context.SaveChanges();
    return student.Id;
}
```

## 修改（Update）
```csharp
public void Update(Student student)
{
    using var context = StudentsManagerDbContextFactory.CreateDbContext();
    context.Students.Update(student);
    context.SaveChanges();
}
```

学生档案子表新增和修改共用保存方法：`SaveFamilyInfo`、`SaveRewardRecord`、`SavePunishmentRecord`、`SaveHealthRecord`。保存前由 `StudentService` 校验学号存在性、必填字段和格式，保存后写入业务日志。

## 删除（Delete - 删除关联数据）
```csharp
public void Delete(int id)
{
    using var context = StudentsManagerDbContextFactory.CreateDbContext();
    var student = context.Students.FirstOrDefault(item => item.Id == id);
    if (student == null) return;

    context.FamilyInfos.RemoveRange(context.FamilyInfos.Where(item => item.StudentNo == student.StudentNo));
    context.RewardRecords.RemoveRange(context.RewardRecords.Where(item => item.StudentNo == student.StudentNo));
    context.PunishmentRecords.RemoveRange(context.PunishmentRecords.Where(item => item.StudentNo == student.StudentNo));
    context.HealthRecords.RemoveRange(context.HealthRecords.Where(item => item.StudentNo == student.StudentNo));
    context.StudentRegistrations.RemoveRange(context.StudentRegistrations.Where(item => item.StudentId == id));
    context.StatusChangeRecords.RemoveRange(context.StatusChangeRecords.Where(item => item.StudentId == id));
    context.ScholarshipInfos.RemoveRange(context.ScholarshipInfos.Where(item => item.StudentId == id));
    context.GraduationInfos.RemoveRange(context.GraduationInfos.Where(item => item.StudentId == id));
    context.Scores.RemoveRange(context.Scores.Where(item => item.StudentId == id));
    context.Students.Remove(student);
    context.SaveChanges();
}
```

---

## 表现层调用

**查询显示**
```csharp
students = txtSearch.Text.Trim().Length > 0 
    ? studentService.Search(txtSearch.Text) 
    : studentService.GetAll();
ApplySortAndPaging();
```

**新增/修改**
```csharp
private void btnAdd_Click(object sender, RoutedEventArgs e)
{
    var win = new StudentEditWindow();
    if (win.ShowDialog() == true && win.ResultStudent != null)
    {
        var result = studentService.Add(win.ResultStudent);
        if (result.Succeeded) LoadStudentData();
    }
}
```

家庭、奖励、处分、体检子模块共用 `ArchiveRecordEditWindow`，由当前 Tab 决定表单字段和保存目标。

**删除**
```csharp
if (MessageBox.Show("确认删除吗?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
{
    studentService.Delete(selected.Id);
    LoadStudentData();
}
```

**排序/分页**
```csharp
var sortedStudents = SortStudents(filteredStudents).ToList();
var pageStudents = sortedStudents
    .Skip((currentPage - 1) * pageSize)
    .Take(pageSize)
    .ToList();
dataGrid.ItemsSource = pageStudents;
```

**CSV 导入/导出**
```csharp
var importedStudents = ImportStudents(dialog.FileName);
foreach (var student in importedStudents) studentService.SaveImported(student);
CsvExportHelper.ExportToCsv(SortStudents(filteredStudents), dialog.FileName);
```

---

## 关键点
- 查询通过 EF Core LINQ 和 `EF.Functions.Like` 生成参数化 SQL
- 新增、修改、删除通过 `StudentService` 统一做业务校验和日志记录
- 删除学生时会按学号清理家庭、奖励、处分、体检档案，并清理关联学籍、奖助、毕业、成绩等数据
- 学生基本信息页面支持搜索、排序、分页、CSV 导入和 CSV 导出
- 家庭、奖励、处分、体检支持新增、修改、删除和查看详情，当前未接入独立搜索、分页、导入导出
- `using` 自动释放 EF Core `DbContext` 资源
