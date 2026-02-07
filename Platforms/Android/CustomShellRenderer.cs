using Android.Content;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace CrocoManager.Platforms.Android
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new CustomBottomNavViewAppearanceTracker(this, shellItem);
        }
    }

    public class CustomBottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public CustomBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
            : base(shellContext, shellItem)
        {
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);

            bottomView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;
        }
    }
}