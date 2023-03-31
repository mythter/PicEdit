using FontAwesome.Sharp;
using System;
using System.Reflection;
using System.Windows.Media;
using WpfFont = System.Windows.Media.FontFamily;

namespace PicEdit.Fonts.FillPhosphor
{
    internal static class FillPhosphorFont
    {
        public const string FontName = "Phosphor-Fill";
        public static readonly Lazy<WpfFont> Wpf = new(LoadWpfFont);

        private static readonly Assembly FontAssembly = typeof(FillPhosphorFont).Assembly;
        private static WpfFont LoadWpfFont()
        {
            return FontAssembly.LoadFont("fonts", FontName);
        }
    }

    public class IconImage : IconImageBase<FillPhosphorIcons>
    {
        protected override ImageSource ImageSourceFor(FillPhosphorIcons icon)
        {
            var size = Math.Max(IconHelper.DefaultSize, Math.Max(ActualWidth, ActualHeight));
            return FillPhosphorFont.Wpf.Value.ToImageSource(icon, Foreground, size);
        }
    }

    public class IconBlock : IconBlockBase<FillPhosphorIcons>
    {
        public IconBlock() : base(FillPhosphorFont.Wpf.Value)
        {
        }
    }

    public class Icon : IconBase<IconBlock, FillPhosphorIcons>
    {
        public Icon(FillPhosphorIcons icon) : base(icon)
        {
        }
    }
    
}
