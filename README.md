# CNPM_Nhom12
# 💰 Personal Finance Manager

Ứng dụng quản lý chi tiêu cá nhân — Đồ án môn Công nghệ Phần mềm, Nhóm 12.

**Repo:** https://github.com/gounchy/Personal-finance

---

## ⚙️ Cài đặt và chạy

### Yêu cầu
- Visual Studio 2022
- SQL Server (bất kỳ phiên bản nào, kể cả SQL Server Express)
- SQL Server Management Studio (SSMS)

### Bước 1: Clone repo

```bash
https://github.com/gounchy/Personal-finance.git
```

### Bước 2: Restore database

Mở **SSMS**, tạo database mới:

```sql
CREATE DATABASE PersonalFinanceDB;
```

Sau đó mở file `PersonalFinance.sql` trong SSMS và nhấn **Execute (F5)**.

### Bước 3: Kiểm tra connection string

Mở file `appsettings.json`, đảm bảo connection string trỏ đúng SQL Server của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PersonalFinanceDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Nếu SQL Server của bạn không phải `localhost`, hãy thay bằng tên instance đúng (ví dụ: `Server=.\SQLEXPRESS`).

### Bước 4: Chạy ứng dụng

Mở solution `CNPM_Nhom12.sln` bằng Visual Studio, nhấn **F5** hoặc nút **Run**.

---