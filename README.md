# 🔍 Screen Translator AI (v1.2.2)

**Screen Translator AI** là một ứng dụng Windows mạnh mẽ, tinh gọn, cho phép bạn chụp bất kỳ vùng nào trên màn hình và dịch thuật ngay lập tức. Ứng dụng kết hợp công nghệ **Windows Native OCR** siêu tốc và trí tuệ nhân tạo **Google Gemini AI** để mang lại kết quả dịch thuật tự nhiên, chính xác nhất.

## ✨ Tính năng nổi bật

-   **📸 Chụp & Dịch**: Chỉ cần kéo chuột chọn vùng, văn bản sẽ được dịch ngay lập tức.
-   **⚡ Smart Realtime (v1.2.0)**: Chế độ dịch tự động cực nhanh (0.5s) nhưng tiết kiệm CPU nhờ thuật toán so sánh hình ảnh thông minh.
-   **🤖 AI Translation**: Dịch thuật ngữ cảnh cực hay nhờ Gemini 2.0 Flash.
-   **🎨 Giao diện hiện đại**: Dark Mode, Glassmorphism, tuỳ chỉnh độ mờ, cỡ chữ.
-   **💾 Tự động lưu**: Ghi nhớ mọi cài đặt của bạn.

---

## 🛠 Hướng dẫn thiết lập (Setup)

### 1. Tải và Chạy
- Giải nén tệp `ScreenTranslator_v1.0.3_Portable.zip`.
- Chạy trực tiếp file `ScreenTranslator.UI.exe`.

### 2. Cấu hình API Key (Bắt buộc cho chế độ AI)
- Chuột phải vào icon kính lúp ở thanh Taskbar (Tray Icon) -> **Settings** -> **Set API Key**.
- Lấy khóa API miễn phí tại: [Google AI Studio](https://aistudio.google.com/app/apikey).
- Dán mã vào ứng dụng và nhấn **Save**.

### 3. Cài đặt phím tắt
- Chuột phải vào Tray Icon -> **Settings** -> **Set Hotkey**.
- Chọn tổ hợp phím bạn muốn và lưu lại.

---

## 📖 Cách sử dụng

1. **Khởi động**: Sau khi mở app, icon sẽ xuất hiện ở góc dưới bên phải màn hình.
2. **Kích hoạt**: Nhấn tổ hợp phím tắt (VD: `Ctrl+Shift+D`). Màn hình sẽ mờ đi để bạn chọn vùng.
3. **Quét**: Nhấp và kéo chuột qua vùng văn bản cần dịch.
4. **Xem kết quả**: Một cửa sổ nhỏ sẽ hiện ra ngay bên cạnh với nội dung dịch thuật. Bạn có thể nhấn ra ngoài để đóng cửa số kết quả.

---

## 📂 Cấu trúc dự án (Dành cho nhà phát triển)

Dự án được xây dựng trên nền tảng **.NET 7.0 (WPF)** với kiến trúc **MVVM**:
- `ScreenTranslator.UI`: Giao diện chính và logic ViewModel.
- `ScreenTranslator.Core`: Các interface và model dữ liệu dùng chung.
- `ScreenTranslator.Infrastructure`: Xử lý OCR, gọi API Gemini và quản lý phím tắt Native.

---

## ❓ Xử lý sự cố (Troubleshooting)

- **Lỗi không đăng ký được phím tắt**: Đảm bảo phím tắt bạn chọn không bị trùng với ứng dụng khác đang chạy.
- **Lỗi AI không dịch được**: Kiểm tra lại API Key và đảm bảo bạn có kết nối Internet ổn định.
- **App không hiện cửa sổ**: App chạy ngầm dưới dạng Tray Icon, hãy kiểm tra danh sách icon ở góc phải Taskbar.

---

## 📄 Bản quyền & Đóng góp
Sản phẩm được phát hành dưới giấy phép **MIT**. Mọi đóng góp xin vui lòng tạo Pull Request hoặc Issue trên GitHub.

---
*Phát triển bởi Google DeepMind Team - Advanced Agentic Coding.*
