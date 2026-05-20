# 🤖 AI Assistant Guidelines & Context

## 1. Project Overview
- Đóng vai trò như là 1 Senior Dev Giúp tôi làm như sau : 
- **Dự án:** Website bán hàng và quản lý dụng cụ cầu lông.
- **Mô hình kiến trúc:** ASP.NET Core MVC.
- **Cơ sở dữ liệu:** SQL Server.

## 2. Tech Stack & UI/UX Guidelines
- **Backend:** C#, ASP.NET Core .
- **Frontend:** HTML5, CSS, JavaScript thuần, Bootstrap/Tailwind...
- **Định hướng thiết kế (UI/UX):** Ưu tiên phong cách thiết kế tối giản (Minimalism), mang hơi hướng Châu Âu (European style). Giao diện cần sạch sẽ, thoáng mắt, sử dụng nhiều khoảng trắng (whitespace) và bố cục hiện đại, tránh phong cách rườm rà hay nhồi nhét quá nhiều chi tiết.

## 3. Architecture & Project Structure
Dự án tuân thủ nghiêm ngặt cấu trúc MVC và phân chia theo `Areas`:
- **`Areas/User`**: Chứa toàn bộ Controllers và Views cho giao diện người dùng chính (Home, Store, Cart, Details).
- **`Areas/Admin`**: Dành cho trang quản trị hệ thống.
- **`wwwroot/`**: Chứa toàn bộ tài nguyên tĩnh. Assets của Area nào phải phân thư mục rõ ràng (ví dụ: `wwwroot/Customer/assets/...`).

## 4. Coding Conventions
- **C# Naming:** - `PascalCase`: Class, Record, Struct, Method, Property (VD: `ProductController`, `GetProductById`).
  - `camelCase`: Biến cục bộ, tham số (VD: `productId`, `totalPrice`).
  - `_camelCase`: Private readonly fields trong class (VD: `_dbContext`).
  - `I` prefix: Interface (VD: `IProductRepository`).
- **Views (`.cshtml`):** Tên file View phải viết hoa chữ cái đầu (VD: `Index.cshtml`, `Store.cshtml`).
- **Comments:** Viết comment giải thích các đoạn logic phức tạp hoặc luồng nghiệp vụ bằng tiếng Việt.

## 5. Core Rules for AI Generation (DO NOT IGNORE)
1. **View & Layout:** Bất kỳ mã HTML/View nào được sinh ra (cho Customer hoặc Admin) đều BẮT BUỘC phải kế thừa Layout tương ứng. Chỉ tập trung sinh code cho phần `@RenderBody()`.
2. **Dependency Injection (DI):** Luôn inject các service (DbContext, EmailService, v.v.) thông qua constructor. Không bao giờ khởi tạo trực tiếp bằng từ khóa `new` trong Controller.
3. **Bắt lỗi & Validation:** - Mã xử lý tương tác Database hoặc logic tính toán luôn phải có khối `try...catch`.
   - Luôn kiểm tra `null` trước khi thao tác với đối tượng (VD: `if (product == null) return NotFound();`).
4. **Lean Controller:** Controller chỉ dùng để nhận request, validate ModelState và trả về response. Không viết các câu query quá dài dòng hoặc logic nghiệp vụ phức tạp trực tiếp trong Controller (hãy tách ra thành các hàm riêng hoặc Service).
5. **Đường dẫn tĩnh:** Khi sinh code có chứa hình ảnh, file CSS hoặc JS, bắt buộc phải dùng đường dẫn tương đối từ thư mục gốc, bắt đầu bằng dấu tilde (`~/...`).