# HomeStore_BE
HomeStore quản lý kinh doanh đồ gia dụng

**Backend API cho ứng dụng bán đồ gia dụng**  
Dự án ASP.NET Core Web API phục vụ app Android khách hàng xem, mua sắm đồ gia dụng (nồi cơm điện, máy giặt, tủ lạnh, nồi chiên không dầu...).

---

**Seed data (for testing):**
| Email                    | Password   | Role     |
|--------------------------|------------|----------|
| admin@homestore.com      | Admin@123  | Admin    |
| nguyenvana@gmail.com     | User@123   | Customer |
| tranthib@gmail.com       | User@123   | Customer |

---

## Công nghệ sử dụng (chưa hoàn thiện)

- **Framework**: ASP.NET Core 8.0 (Web API)
- **Kiến trúc**: N-Tier (API → BLL → DAL → Domain)
- **Database**: SQL Server (Database First)
- **Authentication**: JWT + Google OAuth
- **Payment**: VNPay + COD
- **ORM**: Entity Framework Core
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **API Docs**: Swagger / OpenAPI

---

## Tính năng chính

- Đăng ký / Đăng nhập (Email + Google)
- Quản lý sản phẩm & danh mục
- Giỏ hàng (Cart)
- Đặt hàng & Thanh toán (VNPay + COD)
- Lịch sử đơn hàng
- Chat với cửa hàng
- Bản đồ vị trí cửa hàng
- Thông báo giỏ hàng khi mở app
- Admin quản lý (sẽ mở rộng sau)

---
