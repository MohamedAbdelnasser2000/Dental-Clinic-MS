-- إنشاء قاعدة البيانات
USE master;
GO

-- حذف قاعدة البيانات إذا كانت موجودة
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'DentalClinicSystem')
BEGIN
    ALTER DATABASE DentalClinicSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DentalClinicSystem;
END
GO

-- إنشاء قاعدة البيانات الجديدة
CREATE DATABASE DentalClinicSystem;
GO

-- استخدام قاعدة البيانات الجديدة
USE DentalClinicSystem;
GO

PRINT 'تم إنشاء قاعدة البيانات DentalClinicSystem بنجاح';
GO
