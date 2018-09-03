/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PacklistDatabase.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MvvmDialogs;
using Ninject;
using Packlists.Model;
using Packlists.Model.Printing;
using Packlists.Model.ProgressBar;

namespace Packlists.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        private IKernel _kernel;

        public ViewModelLocator()
        {
            _kernel = new StandardKernel(new NinjectSettings{AllowNullInjection = true}, new DependencyModule());
            

            //Main = _kernel.Get<MainViewModel>();
            //Items = _kernel.Get<ItemsViewModel>();
            //Materials = _kernel.Get<MaterialsViewModel>();

            //var container = new ContainerBuilder();


            //ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container.Build()));
            ////ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //container.RegisterType<DataService>().As<IDataService>().SingleInstance();
            //container.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            //container.RegisterType<ProgressDialogService>().As<IProgressDialogService>().SingleInstance();
            //container.RegisterType<PrintingService>().As<IPrintingService>().SingleInstance();

            //container.RegisterType<MainViewModel>().SingleInstance();
            //container.RegisterType<ItemsViewModel>().InstancePerDependency();
            //container.RegisterType<MaterialsViewModel>().InstancePerDependency();
            //container.RegisterType<ProgressDialog>().InstancePerDependency();



            ////SimpleIoc.Default.Register<MainViewModel>();
            ////SimpleIoc.Default.Register<ItemsViewModel>();
            ////SimpleIoc.Default.Register<MaterialsViewModel>();
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

        //public ProgressDialogViewModel ProgressDialog => _kernel.Get<ProgressDialogViewModel>();


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}