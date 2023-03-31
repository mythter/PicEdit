using FontAwesome.Sharp;
using System;
using System.Reflection;
using System.Windows.Media;
using WpfFont = System.Windows.Media.FontFamily;

namespace PicEdit.Fonts.RegularMaterialRound
{
    internal static class RegularMaterialRoundFont
    {
        public const string FontName = "Material Icons Round Regular";
        public static readonly Lazy<WpfFont> Wpf = new(LoadWpfFont);

        private static readonly Assembly FontAssembly = typeof(RegularMaterialRoundFont).Assembly;
        private static WpfFont LoadWpfFont()
        {
            return FontAssembly.LoadFont("fonts", FontName);
        }
    }

    public class IconImage : IconImageBase<RegularMaterialRoundIcons>
    {
        protected override ImageSource ImageSourceFor(RegularMaterialRoundIcons icon)
        {
            var size = Math.Max(IconHelper.DefaultSize, Math.Max(ActualWidth, ActualHeight));
            return RegularMaterialRoundFont.Wpf.Value.ToImageSource(icon, Foreground, size);
        }
    }

    public class IconBlock : IconBlockBase<RegularMaterialRoundIcons>
    {
        public IconBlock() : base(RegularMaterialRoundFont.Wpf.Value)
        {
        }
    }

    public class Icon : IconBase<IconBlock, RegularMaterialRoundIcons>
    {
        public Icon(RegularMaterialRoundIcons icon) : base(icon)
        {
        }
    }
    
}
