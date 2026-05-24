# Changelog

All notable changes to this project will be documented in this file.

---

## [1.1.0] - 2026-05-24

### Added
- **4 Brand-New Premium Style Presets**:
  - **Glassmorphic Dark (Eclipse)**: Sleek Obsidian frosting glass bubbles with thin translucent outer borders and glowing neon cyan/magenta/emerald username highlights.
  - **Pastel Sakura Blossom**: Warm pink and lavender bubble gradients, cute rounded typography, and soft pink drop-shadows.
  - **Retro Arcade (8-Bit)**: Monospace pixel aesthetics, blocky borders, retro game highlights (yellow, cyan, magenta) and hard drop shadows.
  - **Vibrant Sunset Gradient**: Sophisticated message containers with a rich left-accent sunset gradient (warm orange to magenta/indigo), modern sans-serif fonts, and soft warm-toned highlights.
- Full custom styling support for **Super Chats** and **Membership Milestones** across all presets to match each theme's visual design instead of rendering YouTube's default opaque blue/green/yellow cards.
- Intrusive YouTube banners (`yt-live-chat-banner-manager`), tickers (`yt-live-chat-ticker-renderer`, `#ticker`), and contextual artifacts hidden by default in `BaseCSS`.

### Changed
- **Minimalist Bubbles**: Hidden user avatars for a cleaner look, and integrated gold/amber styles for Super Chats.
- **Neon Cyberpunk**: Enhanced neon pink glow box-shadows, introduced holographic grid styling for Super Chats, and wrapped author profile photos in glowing neon-pink borders.
- **Clean Monospace**: Hidden avatars for absolute zero clutter, aligned text, and added high-contrast retro borders around Super Chats.
- **Glassmorphic Light (Frost)**: Translucent white-frosted light-themed glass bubble design with silver/frost glow and soft pastel borders for Super Chats.
- **WPF Controller UI**:
  - Expanded ComboBox items to support all 8 premium styles + Custom CSS Editor.
  - Dynamically switches the visibility of the Custom CSS textbox based on selection index 8.
  - Updated configuration settings serialization to support the new theme indices.

---

## [1.0.0] - 2026-05-22

### Added
- Initial release of **XCYoutube Live Chat Overlay**.
- Zero API Key required rendering utilizing Microsoft Edge WebView2 popout chat.
- Lock chat (Click-Through and mouse transparency) using global hotkey `Ctrl + Shift + L`.
- Editable boundary framing (move and resize HUD dynamically).
- Adjust transparency, font zoom scale, and settings persistence.
