# 🔍 Screen Translator(v1.0.3)

**Screen Translator AI** là một ứng dụng Windows mạnh mẽ, tinh gọn, cho phép bạn chụp bất kỳ vùng nào trên màn hình và dịch thuật ngay lập tức. Ứng dụng kết hợp công nghệ **Windows Native OCR** siêu tốc và trí tuệ nhân tạo **Google Gemini AI** để mang lại kết quả dịch thuật tự nhiên, chính xác nhất.

---

## ✨ Tính năng nổi bật

- ⚡ **Chụp ảnh tức thì**: Sử dụng phím tắt tùy chỉnh (Mặc định: `Ctrl + Shift + D`) để quét vùng cần dịch.
- 🤖 **Trí tuệ nhân tạo Gemini**: Sử dụng model **Gemini 2.0 Flash** mới nhất từ Google giúp hiểu ngữ cảnh và dịch mượt mà.
- 🎯 **Độ chính xác cao**: Tích hợp Windows Native OCR của Microsoft giúp nhận diện chữ viết cực nhanh và chính xác.
- 🇻🇳 **Hỗ Trợ Ngôn Ngữ**:
    - **Tiếng Nhật (JP) → Tiếng Việt (VI)** (Tối ưu cho Manga/Tài liệu kỹ thuật).
    - **Tiếng Anh (EN) → Tiếng Việt (VI)**.
- 🛠 **Tùy biến tối đa**:
    - Thay đổi phím tắt linh hoạt (Ctrl, Shift, Alt + bất kỳ phím nào).
    - Tự động lưu cài đặt: API Key, phím tắt, ngôn ngữ được ghi nhớ cho lần sau.
- 🚀 **Bản Portable duy nhất**: Chỉ một file `.exe` duy nhất, không cần cài đặt, không file rác.
- 📏 **Giao diện hiện đại**: Thiết kế Dark Mode thanh lịch, thu nhỏ gọn gàng dưới thanh Taskbar.

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
