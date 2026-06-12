using MudBlazor;

namespace Cassandra.WebUi;

public static class CassandraTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary          = "#D62828",
            PrimaryDarken    = "#A41E1E",
            PrimaryLighten   = "#E85D5D",
            Secondary        = "#64748B",
            Background       = "#F1F5F9",
            Surface          = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText       = "#1E293B",
            DrawerBackground = "#1E293B",
            DrawerText       = "#94A3B8",
            DrawerIcon       = "#94A3B8",
            TextPrimary      = "#1E293B",
            TextSecondary    = "#64748B",
            ActionDefault    = "#64748B",
            Divider          = "#E2E8F0",
            DividerLight     = "#F1F5F9",
            Success          = "#16A34A",
            Error            = "#DC2626",
            Warning          = "#D97706",
            Info             = "#0284C7",
            TableHover       = "rgba(214,40,40,0.05)",
            TableStriped     = "rgba(0,0,0,0.018)",
        },
        PaletteDark = new PaletteDark
        {
            Primary          = "#E85D5D",
            PrimaryDarken    = "#D62828",
            Background       = "#0F172A",
            Surface          = "#1E293B",
            AppbarBackground = "#1E293B",
            AppbarText       = "#E2E8F0",
            DrawerBackground = "#0F172A",
            DrawerText       = "#94A3B8",
            DrawerIcon       = "#94A3B8",
            TextPrimary      = "#E2E8F0",
            TextSecondary    = "#94A3B8",
            ActionDefault    = "#94A3B8",
            Divider          = "#334155",
            DividerLight     = "#1E293B",
            Success          = "#22C55E",
            Error            = "#EF4444",
            Warning          = "#F59E0B",
            Info             = "#38BDF8",
            TableHover       = "rgba(232,93,93,0.07)",
        },
    };
}
