# 学生管理 - 增删改查

## 查询（Read）
```csharp
public List<Student> GetAll()
{
    using var connection = SqlServerConnectionFactory.CreateConnection();
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = "SELECT Id, StudentNo, Name FROM dbo.Students ORDER BY StudentNo;";
    using var reader = command.ExecuteReader();
    while (reader.Read()) students.Add(MapStudent(reader));
    return students;
}

public List<Student> Search(string keyword)
{
    command.CommandText = "WHERE StudentNo LIKE @Keyword OR Name LIKE @Keyword ...";
    command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
}
```

## 新增（Create）
```csharp
public int Add(Student student)
{
    command.CommandText = @"INSERT INTO dbo.Students (...) OUTPUT INSERTED.Id VALUES (@StudentNo, @Name, ...);";
    AddStudentParameters(command, student);
    return Convert.ToInt32(command.ExecuteScalar());  // 返回ID
}
```

## 修改（Update）
```csharp
public void Update(Student student)
{
    command.CommandText = "UPDATE dbo.Students SET StudentNo = @StudentNo, Name = @Name, ... WHERE Id = @Id;";
    AddStudentParameters(command, student);
    command.Parameters.AddWithValue("@Id", student.Id);
    command.ExecuteNonQuery();
}
```

## 删除（Delete - 含事务级联）
```csharp
public void Delete(int id)
{
    using var connection = SqlServerConnectionFactory.CreateConnection();
    using var transaction = connection.BeginTransaction();
    try
    {
        // 先删关联表：FamilyInfos, RewardRecords, PunishmentRecords, HealthRecords, Scores...
        DeleteRelatedRecords(connection, transaction, id, "dbo.FamilyInfos");
        // 最后删学生
        command.CommandText = "DELETE FROM dbo.Students WHERE Id = @Id;";
        transaction.Commit();
    }
    catch { transaction.Rollback(); throw; }
}
```

---

## 表现层调用

**查询显示**
```csharp
students = txtSearch.Text.Trim().Length > 0 
    ? repo.Search(txtSearch.Text) 
    : repo.GetAll();
dataGrid.ItemsSource = students;
```

**新增/修改**
```csharp
private void btnAdd_Click(object sender, RoutedEventArgs e)
{
    var win = new StudentEditWindow();
    if (win.ShowDialog() == true) LoadStudentData();  // 刷新
}
```

**删除**
```csharp
if (MessageBox.Show("确认删除吗?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
{
    repo.Delete(selected.Id);
    LoadStudentData();
}
```

---

## 关键点
- 参数化查询 `@Keyword` 防SQL注入
- 删除用事务级联，`Rollback()` 回滚错误
- `using` 自动释放连接和资源
