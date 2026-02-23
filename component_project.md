
---

## 4. Entities cần có (Domain/Entities/)

Tạo các class Entity (partial class, khớp chính xác với bảng DB):

- User.cs
- Category.cs
- Product.cs
- Cart.cs
- CartItem.cs
- Order.cs
- OrderItem.cs
- Payment.cs
- Message.cs
- StoreLocation.cs

---

## 5. DatabaseSchemaUpdater & DataSeeder

- Tạo 2 interface + implement trong `HomeStore.API/DependencyInjection/`:
  - `IDatabaseSchemaUpdater` + `DatabaseSchemaUpdater` → đọc file `DatabaseScripts/01_HomeStore_Schema.sql` và execute raw SQL khi startup (nếu bảng chưa tồn tại).
  - `IDatabaseDataSeeder` + `DatabaseDataSeeder` → seed Category + Product mẫu.

---

## 6. Packages & Dependencies

Đã có từ project cũ. Đảm bảo có:
- Microsoft.EntityFrameworkCore.SqlServer
- Swashbuckle.AspNetCore
- FluentValidation.AspNetCore
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Authentication.Google

---

## 7. Configuration cần kiểm tra

appsettings.json phải có đầy đủ:
- ConnectionStrings:DefaultConnection
- Jwt, Google, VNPay, Store (Latitude, Longitude, Address)

---

## 8. Những thứ cần tạo/chỉnh sửa

**Trong DAL:**
- `HomeStoreV2Context.cs` → thêm tất cả DbSet<...>()

**Trong Domain:**
- Tất cả Entities
- Tất cả DTOs (Request/Response cho từng chức năng)
- Tất cả I*Repository và I*Service

**Trong BLL:**
- Tất cả *Service.cs (AuthService, ProductService, CartService, OrderService, PaymentService, ChatService, StoreService)

**Trong API:**
- Tất cả Controllers (AuthController, ProductsController, CartsController, OrdersController, PaymentsController, ChatController, StoreController)
- Tất cả Validators
- Cập nhật Program.cs và DependencyInjection để đăng ký tất cả

---

## 9. Checklist chi tiết AI phải làm (theo thứ tự bắt buộc)

**Bước 1 – Domain**
- [ ] Tạo đầy đủ Entities (10 class)
- [ ] Tạo đầy đủ DTOs (Auth, Product, Cart, Order, Payment, Chat, Store)
- [ ] Tạo Interface Repository & Service tương ứng

**Bước 2 – DAL**
- [ ] Chỉnh sửa `HomeStoreV2Context.cs`: thêm 10 DbSet, OnModelCreating (nếu cần)
- [ ] Tạo đầy đủ *Repository.cs (implement I*Repository)

**Bước 3 – BLL**
- [ ] Tạo MappingProfile (AutoMapper)
- [ ] Tạo đầy đủ *Service.cs (logic business)
- [ ] Giữ nguyên VnpayHelper, chỉnh PaymentService hỗ trợ cả VNPay và COD

**Bước 4 – API**
- [ ] Tạo DatabaseSchemaUpdater (đọc file SQL từ DatabaseScripts và execute)
- [ ] Tạo DatabaseDataSeeder (seed Category + Product)
- [ ] Tạo tất cả Controllers + route + Swagger GroupName
- [ ] Tạo Validators (FluentValidation)
- [ ] Cập nhật Program.cs (AddControllers, AddSwagger, AddJwt, AddGoogle, CORS, SchemaUpdater, DataSeeder)
- [ ] Cập nhật DependencyInjection (đăng ký tất cả Repository, Service, Updater, Seeder)

**Bước 5 – Hoàn thiện**
- [ ] Thêm CORS cho localhost Android emulator
- [ ] Swagger có nhóm rõ ràng (01. Auth, 02. Products…)
- [ ] Test đầy đủ flow: Register → Login → Get Products → Add to Cart → Create Order → VNPay/COD → Chat → Map

**Hoàn thành thì project sẵn sàng connect với app Android.**

