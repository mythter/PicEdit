using FontAwesome.Sharp;
using System;
using System.Reflection;
using System.Windows.Media;
using WpfFont = System.Windows.Media.FontFamily;

namespace PicEdit.Fonts.RegularPhosphor
{
    internal static class RegularPhosphorFont
    {
        public const string FontName = "Phosphor";
        public static readonly Lazy<WpfFont> Wpf = new(LoadWpfFont);

        private static readonly Assembly FontAssembly = typeof(RegularPhosphorFont).Assembly;
        private static WpfFont LoadWpfFont()
        {
            return FontAssembly.LoadFont("fonts", FontName);
        }
    }

    public class IconImage : IconImageBase<RegularPhosphorIcons>
    {
        protected override ImageSource ImageSourceFor(RegularPhosphorIcons icon)
        {
            var size = Math.Max(IconHelper.DefaultSize, Math.Max(ActualWidth, ActualHeight));
            return RegularPhosphorFont.Wpf.Value.ToImageSource(icon, Foreground, size);
        }
    }

    public class IconBlock : IconBlockBase<RegularPhosphorIcons>
    {
        public IconBlock() : base(RegularPhosphorFont.Wpf.Value)
        {
        }
    }

    public class Icon : IconBase<IconBlock, RegularPhosphorIcons>
    {
        public Icon(RegularPhosphorIcons icon) : base(icon)
        {
        }
    }
    
}