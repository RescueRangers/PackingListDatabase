using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using MvvmDialogs.DialogTypeLocators;
using MvvmDialogs.FrameworkDialogs;
using Ninject.Modules;
using Packlists.Model;
using Packlists.Model.Printing;
using Packlists.Model.ProgressBar;
using Packlists.ViewModel;

namespace Packlists
{
    public class DependencyModule : NinjectModule
    {
        private readonly ReflectionDialogFactory _dialogFactory = new ReflectionDialogFactory();
        private readonly NamingConventionDialogTypeLocator _namingConvention = new NamingConventionDialogTypeLocator();
        private readonly DefaultFrameworkDialogFactory _defaultFrameworkDialog = new DefaultFrameworkDialogFactory();

        public override void Load()
        {
            Bind<IDataService>().To<DataService>().InSingletonScope();

            Bind<IPrintingService>().To<PrintingService>().WhenInjectedInto<MainViewModel>().InSingletonScope();

            Bind<IDialogService>().To<DialogService>().InSingletonScope().WithConstructorArgument("dialogFactory", _dialogFactory)
                .WithConstructorArgument("dialogTypeLocator", _namingConvention)
                .WithConstructorArgument("frameworkDialogFactory", _defaultFrameworkDialog);

            Bind<IProgressDialogService>().To<ProgressDialogService>().InSingletonScope();

            Bind<MainViewModel>().ToSelf().InSingletonScope();
            Bind<ItemsViewModel>().ToSelf().InTransientScope();
            Bind<MaterialsViewModel>().ToSelf().InTransientScope();
        }
    }
}
