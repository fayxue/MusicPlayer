# 🎵 炫彩音乐播放器

基于 .NET 8 MAUI 开发的跨平台移动音乐播放器。

## ✨ 功能特性

- 🎵 **播放本地音乐** - 支持加载手机内的 MP3、WAV、FLAC、OGG 等常见音乐格式
- 🔀 **多种播放模式**
  - 顺序播放 🔁
  - 随机播放 🔀
  - 单曲循环 🔂
- 📜 **歌词显示** - 支持加载 LRC 格式歌词，并按播放进度高亮显示当前歌词
- 🎨 **炫酷界面** - 渐变色设计，简洁现代的UI风格
- ⏯️ **完整控制** - 播放/暂停、上一曲/下一曲、进度条拖动

## 📋 系统要求

- .NET 8 SDK
- Visual Studio 2022 或更高版本
- Android SDK（用于 Android 开发）
- Xcode（用于 iOS 开发，仅限 macOS）

## 🚀 快速开始

### 1. 克隆或下载项目

```bash
cd MusicPlayer
```

### 2. 还原 NuGet 包

```bash
dotnet restore
```

### 3. 运行项目

**Android:**
```bash
dotnet build -t:Run -f net8.0-android
```

**iOS:**
```bash
dotnet build -t:Run -f net8.0-ios
```

## 📱 使用说明

1. **添加歌曲**
   - 点击右上角的 ➕ 按钮
   - 从手机存储中选择音乐文件
   - 支持批量添加

2. **播放音乐**
   - 点击中央的 ▶️ 按钮开始播放
   - 使用 ⏸️ 暂停当前播放
   - 使用 ⏮️ 和 ⏭️ 切换上一曲/下一曲

3. **播放模式切换**
   - 点击左下角的图标切换播放模式
   - 🔁 顺序播放 → 🔀 随机播放 → 🔂 单曲循环

4. **查看播放列表**
   - 点击右下角的 📜 按钮打开/关闭播放列表
   - 点击列表中的歌曲直接播放

5. **歌词显示**
   - 在歌曲文件同目录下放置同名的 .lrc 文件
   - 播放时会自动加载并高亮显示当前歌词

## 📁 项目结构

```
MusicPlayer/
├── Models/                     # 数据模型
│   ├── Song.cs                # 歌曲模型
│   └── LrcLine.cs             # 歌词行模型
├── Services/                   # 服务层
│   ├── AudioPlayerService.cs  # 音频播放服务
│   └── LrcParserService.cs    # 歌词解析服务
├── ViewModels/                 # 视图模型
│   └── MainViewModel.cs       # 主视图模型
├── Views/                      # 界面
│   ├── MainPage.xaml          # 主页面（XAML）
│   └── MainPage.xaml.cs       # 主页面代码
├── Resources/                  # 资源文件
│   └── Styles/                # 样式文件
│       ├── Colors.xaml        # 颜色定义
│       └── Styles.xaml        # 样式定义
├── Platforms/                  # 平台特定代码
│   └── Android/               # Android 平台
│       └── AndroidManifest.xml # Android 权限配置
├── App.xaml                    # 应用程序资源
├── AppShell.xaml              # Shell 导航
└── MauiProgram.cs             # 程序入口
```

## 🎨 界面设计

- **深色主题** - 眼睛友好的深色背景
- **渐变色** - 紫色系渐变，彰显个性
- **圆角设计** - 现代化的圆角卡片布局
- **响应式** - 适配不同屏幕尺寸

## 🔒 权限说明

### Android
- `READ_EXTERNAL_STORAGE` - 读取存储中的音乐文件
- `READ_MEDIA_AUDIO` - Android 13+ 读取音频文件
- `WAKE_LOCK` - 保持屏幕唤醒以持续播放

### iOS
- 需要在 Info.plist 中配置媒体库访问权限

## 🛠️ 技术栈

- **.NET 8 MAUI** - 跨平台UI框架
- **CommunityToolkit.Mvvm** - MVVM 框架
- **XAML** - 声明式UI
- **C#** - 主要开发语言

## 📝 待优化功能

- [ ] 播放历史记录
- [ ] 收藏/喜欢功能
- [ ] 均衡器
- [ ] 在线歌词搜索
- [ ] 专辑封面显示
- [ ] 睡眠定时器
- [ ] 音频焦点管理

## 🐛 已知问题

- 在某些 Android 设备上，首次加载音乐文件可能需要额外的权限确认
- iOS 平台需要配置相应的权限才能访问音乐库

## 📄 许可证

本项目采用 MIT 许可证。

## 👨‍💻 开发者

欢迎提交 Issue 和 Pull Request！
