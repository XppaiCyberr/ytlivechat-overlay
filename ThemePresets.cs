namespace ytlivechatwedus
{
    public static class ThemePresets
    {
        public static readonly string BaseCSS = @"
            /* Hide YouTube chat header and input/action panel */
            yt-live-chat-header-renderer,
            #item-scroller #action-panel,
            yt-live-chat-message-input-renderer,
            #input-container,
            #separator {
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
        ";

        public static readonly string NeonCyberpunk = BaseCSS + @"
            /* Neon grid chat messages */
            yt-live-chat-text-message-renderer {
                background-color: rgba(10, 10, 15, 0.75) !important;
                border-left: 4px solid #ff007f !important;
                margin: 6px 10px !important;
                padding: 6px 12px !important;
                border-radius: 0 8px 8px 0 !important;
                box-shadow: 0 0 10px rgba(255, 0, 127, 0.2) !important;
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

            #message {
                color: #ffffff !important;
                font-family: 'Consolas', 'Courier New', monospace !important;
                font-size: 13px !important;
                text-shadow: 0 0 2px rgba(255, 255, 255, 0.3) !important;
            }
        ";

        public static readonly string ClearMonospace = BaseCSS + @"
            /* Minimal console styling */
            yt-live-chat-text-message-renderer {
                background: transparent !important;
                margin: 2px 5px !important;
                padding: 2px 5px !important;
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
        ";

        public static readonly string GlassmorphismLight = BaseCSS + @"
            /* Sleek light frost design */
            yt-live-chat-text-message-renderer {
                background-color: rgba(255, 255, 255, 0.25) !important;
                backdrop-filter: blur(8px) !important;
                border-radius: 16px !important;
                margin: 5px 10px !important;
                padding: 8px 14px !important;
                border: 1px solid rgba(255, 255, 255, 0.4) !important;
                box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.08) !important;
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
        ";
    }
}
