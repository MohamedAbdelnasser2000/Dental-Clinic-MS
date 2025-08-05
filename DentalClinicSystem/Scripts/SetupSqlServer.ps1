# Script لإعداد SQL Server لنظام إدارة عيادة الأسنان

Write-Host "=== إعداد SQL Server لنظام إدارة عيادة الأسنان ===" -ForegroundColor Green

# التحقق من وجود SQL Server
Write-Host "التحقق من اتصال SQL Server..." -ForegroundColor Yellow
try {
    $connectionString = "Server=.;Database=master;Trusted_Connection=true;TrustServerCertificate=true;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $connection.Close()
    Write-Host "✓ تم الاتصال بـ SQL Server بنجاح" -ForegroundColor Green
}
catch {
    Write-Host "✗ فشل في الاتصال بـ SQL Server: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "تأكد من أن SQL Server يعمل على الجهاز" -ForegroundColor Yellow
    exit 1
}

# إنشاء قاعدة البيانات
Write-Host "إنشاء قاعدة البيانات..." -ForegroundColor Yellow
try {
    sqlcmd -S "." -E -i "Scripts\CreateDatabase.sql"
    Write-Host "✓ تم إنشاء قاعدة البيانات بنجاح" -ForegroundColor Green
}
catch {
    Write-Host "✗ فشل في إنشاء قاعدة البيانات: $($_.Exception.Message)" -ForegroundColor Red
}

# تطبيق Migrations
Write-Host "تطبيق Migrations..." -ForegroundColor Yellow
try {
    dotnet ef database update
    Write-Host "✓ تم تطبيق Migrations بنجاح" -ForegroundColor Green
}
catch {
    Write-Host "✗ فشل في تطبيق Migrations: $($_.Exception.Message)" -ForegroundColor Red
}

# تشغيل التطبيق
Write-Host "تشغيل التطبيق..." -ForegroundColor Yellow
Write-Host "يمكنك الآن تسجيل الدخول باستخدام:" -ForegroundColor Cyan
Write-Host "Admin: admin@dentalclinic.com / Admin123!" -ForegroundColor White
Write-Host "Doctor: doctor@dentalclinic.com / Doctor123!" -ForegroundColor White
Write-Host "Receptionist: receptionist@dentalclinic.com / Reception123!" -ForegroundColor White

Write-Host "=== انتهى الإعداد ===" -ForegroundColor Green
