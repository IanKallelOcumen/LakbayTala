namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Centralized canvas layer sort orders for consistent visual hierarchy.
    /// Used by UIManager when creating layers and when assigning panel sort order within a layer.
    /// </summary>
    public static class UICanvasLayers
    {
        public const int Background = 0;
        public const int Gameplay = 100;
        public const int HUD = 200;
        public const int Menu = 300;
        public const int Lore = 400;
        public const int Popup = 500;
        public const int Overlay = 600;

        /// <summary>Offset applied to child canvases within a layer (e.g. modal on top of menu).</summary>
        public const int SubLayerOffset = 10;

        public static int GetSortOrderForPanelType(UIPanelType type)
        {
            switch (type)
            {
                case UIPanelType.HUD: return HUD;
                case UIPanelType.Menu: return Menu;
                case UIPanelType.Lore: return Lore;
                case UIPanelType.Popup: return Popup;
                case UIPanelType.Overlay: return Overlay;
                case UIPanelType.Leaderboard:
                case UIPanelType.Stats:
                case UIPanelType.Achievements:
                case UIPanelType.Settings: return Menu;
                default: return Menu;
            }
        }
    }
}
