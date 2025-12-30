# 🔍 Screen Translator (v1.0.3)

A lightweight, powerful Windows application that captures screen regions and translates text instantly using **Windows Native OCR** and **Google Gemini AI**.

![App Preview](C:/Users/nguyentung/Desktop/apps/ScreenTranslator.UI/Resources/app_icon.png) 
*(Note: Replace with a real screenshot in your repo)*

## 🚀 Key Features

- **Instant Capture**: Use a customizable hotkey (default: `Ctrl+Shift+D`) to select any area on your screen.
- **AI-Powered Translation**: Leverages **Gemini 2.0 Flash** for high-context, natural translations.
- **Multi-Language Support**:
  - Japanese 🇯🇵 → Vietnamese 🇻🇳
  - English 🇺🇸 → Vietnamese 🇻🇳
- **Persistent Settings**: Your API Key, Hotkeys, and Language preferences are automatically saved.
- **Native Performance**: Uses Windows 10/11 built-in OCR for lightning-fast text recognition.
- **Professional Tray UI**: Minimalist design that stays out of your way in the system tray.
- **Single Instance Protection**: Prevents multiple background processes for better efficiency.

## 🛠️ Setup & Installation

1. **Download**: Grab the latest `ScreenTranslator.UI.exe` from the `publish_capture` folder.
2. **Requirements**: 
   - Windows 10 or 11.
   - .NET 6.0/7.0/8.0 Runtime (depending on build).
3. **API Key**: 
   - Right-click the Tray Icon -> **Settings** -> **Set API Key**.
   - Get your free key from [Google AI Studio](https://aistudio.google.com/app/apikey).

## 📖 How to Use

1. **Launch** the app. Look for the search icon in your System Tray.
2. **Configure**: Set your preferred Hotkey and Translation Provider (Standard or AI).
3. **Capture**: Press your hotkey and drag the mouse over the text you want to translate.
4. **View**: The result window will pop up near your selection with the translated text.

## ⚙️ Development

This project is built with:
- **WPF (.NET)**: For a modern Windows UI experience.
- **CommunityToolkit.Mvvm**: Robust architecture.
- **Hardcodet.NotifyIcon**: Advanced tray integration.

```bash
# Clone the repository
git clone https://github.com/your-username/screen-translator-ai.git

# Build
dotnet build
```

## 📄 License
MIT License - Feel free to use and contribute!
