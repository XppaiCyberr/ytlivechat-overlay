namespace ytlivechatwedus
{
    public static class ThemePresets
    {
        public static readonly string BaseCSS = @"
            /* Hide YouTube chat header, footer, input panel, engagement messages, pinned messages and actions */
            yt-live-chat-header-renderer,
            yt-live-chat-footer-renderer,
            yt-live-chat-viewer-engagement-message-renderer,
            yt-live-chat-pinned-message-renderer,
            #item-scroller #action-panel,
            yt-live-chat-message-input-renderer,
            #input-panel,
            #input-container,
            #footer,
            #separator,
            #interaction-card,
            #viewer-engagement,
            yt-live-chat-ticker-renderer,
            #ticker,
            yt-live-chat-banner-manager {
                display: none !important;
            }

            /* Make everything transparent */
            html, body,
            yt-live-chat-renderer,
            #item-scroller,
            #items,
            #contents,
            #chat {
                background: transparent !important;
                background-color: transparent !important;
            }

            /* Hide scrollbar but keep scroll behavior */
            ::-webkit-scrollbar {
                display: none !important;
                width: 0px !important;
            }

            /* Author photo styling defaults */
            #author-photo,
            #author-photo img {
                width: 28px !important;
                height: 28px !important;
                border-radius: 50% !important;
                margin-right: 8px !important;
                vertical-align: middle !important;
                display: inline-block !important;
            }

            /* Badges styling defaults */
            yt-live-chat-author-badge-renderer {
                margin: 0 4px !important;
                vertical-align: middle !important;
                display: inline-block !important;
            }
            yt-live-chat-author-badge-renderer img {
                width: 16px !important;
                height: 16px !important;
            }

            /* Emojis size adjustment */
            img.emoji,
            .yt-emoji {
                height: 22px !important;
                width: 22px !important;
                vertical-align: middle !important;
            }
        ";

        public static readonly string MinimalistBubbles = BaseCSS + @"
            /* Style messages as bubbles */
            yt-live-chat-text-message-renderer {
                background-color: rgba(20, 20, 20, 0.6) !important;
                border-radius: 12px !important;
                margin: 4px 8px !important;
                padding: 8px 12px !important;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.15) !important;
                border: 1px solid rgba(255, 255, 255, 0.08) !important;
                transition: all 0.3s ease !important;
            }

            /* Hide avatars for a cleaner look */
            #author-photo {
                display: none !important;
            }

            /* Author name styling */
            #author-name {
                color: #58a6ff !important;
                font-family: 'Segoe UI', -apple-system, sans-serif !important;
                font-weight: 600 !important;
                font-size: 13px !important;
            }

            /* Owner / Moderator colors */
            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ffd700 !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #50fa7b !important;
            }

            yt-live-chat-text-message-renderer[author-type='member'] #author-name {
                color: #a371f7 !important;
            }

            /* Message text styling */
            #message {
                color: #f0f6fc !important;
                font-family: 'Segoe UI', -apple-system, sans-serif !important;
                font-size: 14px !important;
                line-height: 1.4 !important;
            }

            /* Super Chat / paid messages */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(255, 193, 7, 0.12) !important;
                border: 1px solid rgba(255, 193, 7, 0.35) !important;
                border-radius: 12px !important;
                margin: 6px 8px !important;
                padding: 8px 12px !important;
                box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #ffd700 !important;
                font-weight: bold !important;
            }

            yt-live-chat-paid-message-renderer #message,
            yt-live-chat-membership-item-renderer #message {
                color: #ffffff !important;
            }
        ";

        public static readonly string NeonCyberpunk = BaseCSS + @"
            /* Neon grid chat messages */
            yt-live-chat-text-message-renderer {
                background-color: rgba(10, 10, 15, 0.75) !important;
                border-left: 4px solid #ff007f !important;
                margin: 6px 10px !important;
                padding: 8px 12px !important;
                border-radius: 0 8px 8px 0 !important;
                box-shadow: 0 0 10px rgba(255, 0, 127, 0.2) !important;
            }

            /* Avatar neon frame */
            #author-photo img {
                border: 1.5px solid #ff007f !important;
                box-shadow: 0 0 5px #ff007f !important;
            }

            /* Cyberpunk names */
            #author-name {
                color: #00ffff !important;
                font-family: 'Consolas', 'Courier New', monospace !important;
                text-shadow: 0 0 5px rgba(0, 255, 255, 0.5) !important;
                font-weight: bold !important;
            }

            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ff00ff !important;
                text-shadow: 0 0 5px rgba(255, 0, 255, 0.5) !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #39ff14 !important;
                text-shadow: 0 0 5px rgba(57, 255, 20, 0.5) !important;
            }

            yt-live-chat-text-message-renderer[author-type='member'] #author-name {
                color: #e066ff !important;
                text-shadow: 0 0 5px rgba(224, 102, 255, 0.5) !important;
            }

            #message {
                color: #ffffff !important;
                font-family: 'Consolas', 'Courier New', monospace !important;
                font-size: 13px !important;
                text-shadow: 0 0 2px rgba(255, 255, 255, 0.3) !important;
            }

            /* Cyberpunk Super Chats & Paid events */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(0, 255, 255, 0.1) !important;
                border: 1px solid #00ffff !important;
                border-left: 4px solid #00ffff !important;
                border-radius: 0 8px 8px 0 !important;
                margin: 6px 10px !important;
                padding: 8px 12px !important;
                box-shadow: 0 0 15px rgba(0, 255, 255, 0.4) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #00ffff !important;
                text-shadow: 0 0 5px rgba(0, 255, 255, 0.8) !important;
            }
        ";

        public static readonly string ClearMonospace = BaseCSS + @"
            /* Minimal console styling */
            yt-live-chat-text-message-renderer {
                background: transparent !important;
                margin: 2px 5px !important;
                padding: 2px 5px !important;
            }

            /* Hide avatars */
            #author-photo {
                display: none !important;
            }

            #author-name {
                color: #8be9fd !important;
                font-family: 'Consolas', monospace !important;
                font-size: 13px !important;
            }

            #message {
                color: #f8f8f2 !important;
                font-family: 'Consolas', monospace !important;
                font-size: 13px !important;
            }

            #author-name::after {
                content: ': ' !important;
                color: #6272a4 !important;
            }

            /* Monospace Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(98, 114, 164, 0.1) !important;
                border: 1px solid #6272a4 !important;
                border-radius: 4px !important;
                margin: 4px 5px !important;
                padding: 6px 8px !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #50fa7b !important;
                font-family: 'Consolas', monospace !important;
            }
        ";

        public static readonly string GlassmorphismLight = BaseCSS + @"
            /* Sleek light frost design */
            yt-live-chat-text-message-renderer {
                background-color: rgba(255, 255, 255, 0.25) !important;
                backdrop-filter: blur(12px) !important;
                -webkit-backdrop-filter: blur(12px) !important;
                border-radius: 16px !important;
                margin: 6px 10px !important;
                padding: 8px 14px !important;
                border: 1px solid rgba(255, 255, 255, 0.4) !important;
                box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.08) !important;
            }

            #author-photo img {
                border: 1.5px solid rgba(255, 255, 255, 0.6) !important;
            }

            #author-name {
                color: #1f2328 !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-weight: 700 !important;
            }

            #message {
                color: #24292f !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-size: 14px !important;
            }

            /* Glassmorphic Light Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(255, 255, 255, 0.45) !important;
                backdrop-filter: blur(12px) !important;
                -webkit-backdrop-filter: blur(12px) !important;
                border-radius: 16px !important;
                margin: 8px 10px !important;
                padding: 10px 14px !important;
                border: 1.5px solid rgba(255, 223, 0, 0.5) !important;
                box-shadow: 0 8px 32px 0 rgba(255, 223, 0, 0.15) !important;
            }
        ";

        public static readonly string GlassmorphismDark = BaseCSS + @"
            /* Deep Dark Obsidian Frost design */
            yt-live-chat-text-message-renderer {
                background-color: rgba(10, 10, 12, 0.5) !important;
                backdrop-filter: blur(12px) !important;
                -webkit-backdrop-filter: blur(12px) !important;
                border-radius: 16px !important;
                margin: 6px 10px !important;
                padding: 8px 14px !important;
                border: 1px solid rgba(255, 255, 255, 0.06) !important;
                box-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.35) !important;
            }

            #author-photo img {
                border: 1.5px solid rgba(255, 255, 255, 0.15) !important;
            }

            #author-name {
                color: #00d2ff !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-weight: 700 !important;
            }

            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ff007f !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #00ff66 !important;
            }

            yt-live-chat-text-message-renderer[author-type='member'] #author-name {
                color: #bf5eff !important;
            }

            #message {
                color: #e2e8f0 !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-size: 14px !important;
            }

            /* Glassmorphic Dark Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(255, 215, 0, 0.1) !important;
                backdrop-filter: blur(12px) !important;
                -webkit-backdrop-filter: blur(12px) !important;
                border-radius: 16px !important;
                margin: 8px 10px !important;
                padding: 10px 14px !important;
                border: 1.5px solid rgba(255, 215, 0, 0.3) !important;
                box-shadow: 0 8px 32px 0 rgba(255, 215, 0, 0.15) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #ffd700 !important;
                font-weight: bold !important;
            }
        ";

        public static readonly string PastelSakura = BaseCSS + @"
            /* Soft pink sakura bubbles */
            yt-live-chat-text-message-renderer {
                background-color: rgba(255, 240, 245, 0.72) !important;
                border-radius: 18px !important;
                margin: 6px 10px !important;
                padding: 8px 14px !important;
                border: 1.5px solid rgba(255, 182, 193, 0.6) !important;
                box-shadow: 0 4px 12px rgba(255, 182, 193, 0.25) !important;
            }

            #author-photo img {
                border: 2px solid rgba(255, 182, 193, 0.8) !important;
            }

            #author-name {
                color: #d03a7c !important;
                font-family: 'Comic Sans MS', 'Segoe UI', sans-serif !important;
                font-weight: bold !important;
            }

            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ff1493 !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #9370db !important;
            }

            #message {
                color: #4a3b42 !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-size: 14px !important;
            }

            /* Sakura Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(255, 224, 230, 0.9) !important;
                border-radius: 18px !important;
                margin: 8px 10px !important;
                padding: 10px 14px !important;
                border: 2px dashed #ff69b4 !important;
                box-shadow: 0 4px 15px rgba(255, 105, 180, 0.35) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #ff1493 !important;
                font-weight: bold !important;
            }
        ";

        public static readonly string RetroArcade = BaseCSS + @"
            /* Blocky pixel arcade */
            yt-live-chat-text-message-renderer {
                background-color: #0f0f1b !important;
                border: 3px solid #ffcc00 !important;
                border-radius: 0px !important;
                margin: 6px 8px !important;
                padding: 6px 12px !important;
                box-shadow: 4px 4px 0px rgba(0, 0, 0, 1) !important;
            }

            #author-photo img {
                border: 2px solid #ff003c !important;
                border-radius: 0px !important;
            }

            #author-name {
                color: #00ff66 !important;
                font-family: 'Courier New', monospace !important;
                font-weight: 900 !important;
                text-transform: uppercase !important;
            }

            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ff003c !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #33ccff !important;
            }

            #message {
                color: #ffffff !important;
                font-family: 'Courier New', monospace !important;
                font-size: 13px !important;
                font-weight: bold !important;
            }

            /* Retro Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: #1a0f30 !important;
                border: 3px solid #ff00ff !important;
                border-radius: 0px !important;
                margin: 8px 8px !important;
                padding: 8px 12px !important;
                box-shadow: 4px 4px 0px rgba(0, 0, 0, 1) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #ff00ff !important;
                font-family: 'Courier New', monospace !important;
                font-weight: bold !important;
            }
        ";

        public static readonly string VibrantSunset = BaseCSS + @"
            /* Elegant Sunset Bubble with Gradient Accent border */
            yt-live-chat-text-message-renderer {
                background-color: rgba(24, 20, 36, 0.78) !important;
                border-left: 4px solid #ff5e62 !important;
                border-image: linear-gradient(to bottom, #ff9966, #ff5e62) 1 !important;
                border-radius: 0 12px 12px 0 !important;
                margin: 6px 10px !important;
                padding: 8px 14px !important;
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.25) !important;
            }

            #author-photo img {
                border: 1.5px solid #ff9966 !important;
            }

            #author-name {
                color: #ff9966 !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-weight: 700 !important;
            }

            yt-live-chat-text-message-renderer[author-type='owner'] #author-name {
                color: #ff5e62 !important;
            }

            yt-live-chat-text-message-renderer[author-type='moderator'] #author-name {
                color: #e157ff !important;
            }

            #message {
                color: #fceade !important;
                font-family: 'Segoe UI', sans-serif !important;
                font-size: 14px !important;
            }

            /* Sunset Super Chats */
            yt-live-chat-paid-message-renderer,
            yt-live-chat-membership-item-renderer {
                background-color: rgba(255, 94, 98, 0.15) !important;
                border: 1.5px solid #ff5e62 !important;
                border-left: 4px solid #ff9966 !important;
                border-radius: 0 12px 12px 0 !important;
                margin: 8px 10px !important;
                padding: 10px 14px !important;
                box-shadow: 0 6px 20px rgba(255, 94, 98, 0.2) !important;
            }

            yt-live-chat-paid-message-renderer #author-name,
            yt-live-chat-membership-item-renderer #author-name {
                color: #ff5e62 !important;
                font-weight: bold !important;
            }
        ";
    }
}
