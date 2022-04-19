﻿using Microsoft.Extensions.DependencyInjection;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.ViewModels;

namespace TourPlanner.Client.UI.Services
{
    public class IoCContainerConfig
    {
        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        /// Builds the IoC service provider, see also App.xaml which instantiates it as a resource
        /// </summary>
        public IoCContainerConfig()
        {
            var services = new ServiceCollection();

            // same for ICommunicationHandler, IContentInterpreter, IFilterHandler
            services.AddSingleton<IApiService, TourPlannerApiService>();
            services.AddSingleton<ITourCollectionService, TourCollectionService>();
            services.AddSingleton<ITourReportGenerationService, TourPdfReportGenerationService>();
            services.AddSingleton<ISummarizeReportGenerationService, SummarizePdfReportGenerationService>();
            services.AddSingleton<ITourImageService, TourImageService>();

            // register the MainViewModel as well, the ServiceProvider will provide the constructor parameters
            // for the MainViewModel based on the configuration above
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<LogViewModel>();
            services.AddSingleton<TourViewModel>();
            services.AddTransient<NewTourViewModel>();
            services.AddSingleton<ListViewModel>();
            services.AddSingleton<SettingsViewModel>();

            // finish configuration and build the provider
            _serviceProvider = services.BuildServiceProvider();
        }

        public MainViewModel MainViewModel
            => _serviceProvider.GetService<MainViewModel>();

        public TourViewModel TourViewModel
            => _serviceProvider.GetService<TourViewModel>();

        public LogViewModel LogViewModel
            => _serviceProvider.GetService<LogViewModel>();

        public ListViewModel ListViewModel
            => _serviceProvider.GetService<ListViewModel>();

        public NewTourViewModel NewTourViewModel
            => _serviceProvider.GetService<NewTourViewModel>();


        public SettingsViewModel SettingsViewModel
            => _serviceProvider.GetService<SettingsViewModel>();
    }
}