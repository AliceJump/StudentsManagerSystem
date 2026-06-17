import sqlite3

db_path = r"D:\items\project\CS_project\StudentsManagerSystem\StudentsManagerSystem\bin\Debug\net8.0-windows\StudentsManagerSystem.db"

conn = sqlite3.connect(db_path)

try:
    conn.execute("DELETE FROM StudentRegistrations WHERE Id = 1")
    conn.commit()
    print("已删除")
except Exception as e:
    print(f"失败: {e}")
finally:
    conn.close()