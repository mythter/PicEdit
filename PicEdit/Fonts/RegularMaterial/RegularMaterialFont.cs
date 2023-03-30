using FontAwesome.Sharp;
using PicEdit.Fonts.RegularMaterial;
using System;
using System.Reflection;
using System.Windows.Media;
using WpfFont = System.Windows.Media.FontFamily;

namespace PicEdit.Fonts.RegularMaterial
{
    internal static class RegularMaterialFont
    {
        public const string FontName = "Material Icons";
        public static readonly Lazy<WpfFont> Wpf = new(LoadWpfFont);

        private static readonly Assembly FontAssembly = typeof(RegularMaterialFont).Assembly;
        private static WpfFont LoadWpfFont()
        {
            return FontAssembly.LoadFont("fonts", FontName);
        }
    }

    public class IconImage : IconImageBase<RegularMaterialIcons>
    {
        protected override ImageSource ImageSourceFor(RegularMaterialIcons icon)
        {
            var size = Math.Max(IconHelper.DefaultSize, Math.Max(ActualWidth, ActualHeight));
            return RegularMaterialFont.Wpf.Value.ToImageSource(icon, Foreground, size);
        }
    }

    public class IconBlock : IconBlockBase<RegularMaterialIcons>
    {
        public IconBlock() : base(RegularMaterialFont.Wpf.Value)
        {
        }
    }

    public class Icon : IconBase<IconBlock, RegularMaterialIcons>
    {
        public Icon(RegularMaterialIcons icon) : base(icon)
        {
        }
    }
    
}
