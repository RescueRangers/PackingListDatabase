/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PacklistDatabase.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using Ninject;

namespace Packlists.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public sealed class ViewModelLocator : IDisposable
    {
        private readonly IKernel _kernel;

        public ViewModelLocator()
        {
            _kernel = new StandardKernel(new NinjectSettings{AllowNullInjection = true}, new DependencyModule());
        }

        
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public MainViewModel Main => _kernel.Get<MainViewModel>();

        public ItemsViewModel Items => _kernel.Get<ItemsViewModel>();

        public MaterialsViewModel Materials => _kernel.Get<MaterialsViewModel>();

        public ImportViewModel Import => _kernel.Get<ImportViewModel>();

        //public ProgressDialogViewModel ProgressDialog => _kernel.Get<ProgressDialogViewModel>();


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }
    }
}