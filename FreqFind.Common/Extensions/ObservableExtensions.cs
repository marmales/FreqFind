using System.Windows;

namespace FreqFind.Common.Extensions
{
    public static class ObservableExtensions
    {
        public static T ResolveDialog<T>(this T viewModel) where T : Observable
        {
            var window = viewModel.ResolveWindow();
            window.ShowDialog();
            return viewModel;
        }
        public static Window ResolveWindow(this Observable viewModel)
        {
            var view = viewModel.ResolveView(onlyWindows: true);
            return view as Window;
        }

        public static FrameworkElement ResolveView(this Observable viewModel, bool onlyWindows = false)
        {
            return UIAssemblies.ResolveView(viewModel, fromFallback: false, skipViewModelPart: false, onlyWindows: onlyWindows);
        }
    }
}
