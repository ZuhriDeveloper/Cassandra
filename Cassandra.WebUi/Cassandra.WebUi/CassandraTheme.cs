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
            Background       = "#F8FAFC",
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
            LinesDefault     = "#E2E8F0",
            LinesInputs      = "#CBD5E1",
            TableLines       = "#EEF2F7",
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
            LinesDefault     = "#334155",
            LinesInputs      = "#475569",
            TableLines       = "#293548",
            Success          = "#22C55E",
            Error            = "#EF4444",
            Warning          = "#F59E0B",
            Info             = "#38BDF8",
            TableHover       = "rgba(232,93,93,0.07)",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            DrawerWidthLeft     = "260px",
        },

        Shadows = BuildShadows(),

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"],
                FontSize   = ".875rem",
                LineHeight = "1.5",
            },
            H5        = new H5Typography        { FontSize = "1.25rem",   FontWeight = "700", LetterSpacing = "-.02em" },
            H6        = new H6Typography        { FontSize = "1.0625rem", FontWeight = "600", LetterSpacing = "-.01em" },
            Subtitle1 = new Subtitle1Typography { FontSize = ".9375rem",  FontWeight = "600" },
            Subtitle2 = new Subtitle2Typography { FontSize = ".8125rem",  FontWeight = "600" },
            Body2     = new Body2Typography     { FontSize = ".8125rem" },
            Caption   = new CaptionTypography   { FontSize = ".75rem" },
            Button    = new ButtonTypography
            {
                FontWeight    = "600",
                TextTransform = "none",
                LetterSpacing = ".01em",
            },
        },
    };

    // Softer, layered shadows in slate tint instead of Material's harsh black defaults.
    private static Shadow BuildShadows()
    {
        var s = new Shadow();
        s.Elevation[1]  = "0 1px 2px rgba(15,23,42,.05)";
        s.Elevation[2]  = "0 1px 2px rgba(15,23,42,.05), 0 2px 8px rgba(15,23,42,.06)";
        s.Elevation[3]  = "0 2px 4px rgba(15,23,42,.04), 0 4px 12px rgba(15,23,42,.08)";
        s.Elevation[4]  = "0 4px 8px rgba(15,23,42,.06), 0 8px 20px rgba(15,23,42,.08)";
        s.Elevation[8]  = "0 8px 16px rgba(15,23,42,.08), 0 16px 32px rgba(15,23,42,.10)";
        s.Elevation[16] = "0 12px 24px rgba(15,23,42,.10), 0 24px 48px rgba(15,23,42,.14)";
        return s;
    }
}
