﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collector.ViewModels;
using Collector.ViewModels.Home;
using Collector.ViewModels.Home.QuemSomos;
using Collector.ViewModels.Home.Usuarios;
using Collector.ViewModels.Login;
using Collector.ViewModels.ViewModelLocator;
using Collector.Views;
using Collector.Views.Home;
using Collector.Views.Home.QuemSomos;
using Collector.Views.Home.Usuarios;
using Collector.Views.Login;
using Xamarin.Forms;

namespace Collector.Services.Navigation
{
    public class NavigationService : INavigationService
    {

        protected readonly Dictionary<Type, Type> _mappings;

        protected Application CurrentApplication
        {
            get { return Application.Current; }
        }

        public NavigationService()
        {
            _mappings = new Dictionary<Type, Type>();
            CreatePageViewModelMappings();
        }

        public Task InitializeAsync()
        {
            return NavigateToAsync<BannerViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : BaseVM
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseVM
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        /// <summary>
        /// NavigationPage 
        /// </summary>
        /// <param name="page">Page.</param>
        public void NavPage(Page page)
        {
            Application.Current.MainPage = new NavigationPage(page);
        }


        /// <summary>
        /// Navega para próxima pagina async
        /// </summary>
        /// <param name="page">Page.</param>
        public void NavAsyncPage(Page page)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(page);
            });

        }

        //Singleton navigation service
        private static NavigationService instance = null;
        private static readonly object padlock = new object();

        public static NavigationService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new NavigationService();
                    }
                    return instance;
                }
            }
        }

        public virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is AccessView)
            {
                CurrentApplication.MainPage = page;
            }
            else if (page is CreateAcounteView)
            {
                CurrentApplication.MainPage = new NavigationPage(page);
            }
            else if(page is BannerView)
            {
                CurrentApplication.MainPage = page;
            }
            else if (page is MainTabbedPageView)
            {
                CurrentApplication.MainPage = new NavigationPage(page);
            }
            else
            {
                var nav = CurrentApplication.MainPage as NavigationPage;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await nav.PushAsync(page);
                });
            }
            await (page.BindingContext as BaseVM).InitializeAsync(parameter);
        }

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"não existe mapeamento para ${viewModelType} por isso a navegação não está funcionando, mapeie a view model no método CreatePageViewModelMappings");
            }
            return _mappings[viewModelType];
        }


        /// <summary>
        /// Cria o bind das paginas automaticamente
        /// </summary>
        /// <returns>The and bind page.</returns>
        /// <param name="viewModelType">View model type.</param>
        /// <param name="parameter">Parameter.</param>
        public Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"A view model {viewModelType} não esta mapeado para uma page");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            BaseVM viewModel = Locator.Instance.Resolve(viewModelType) as BaseVM;
            page.BindingContext = viewModel;
            return page;
        }

        public void CreatePageViewModelMappings()
        {
            _mappings.Add(typeof(BannerViewModel), typeof(BannerView));
            _mappings.Add(typeof(CreateAcounteViewModel), typeof(CreateAcounteView));
            _mappings.Add(typeof(AccessViewModel), typeof(AccessView));
            _mappings.Add(typeof(MainMapaViewModel), typeof(MainMapaView));
            _mappings.Add(typeof(MainTabbedPageViewModel), typeof(MainTabbedPageView));
            _mappings.Add(typeof(ListaDeUsuariosViewModel), typeof(ListaDeUsuariosView));
            _mappings.Add(typeof(DetalhesDosUsuarioViewModel), typeof(DetalhesDosUsuariosView));
            _mappings.Add(typeof(QuemSomosViewModel), typeof(QuemSomosView));
            _mappings.Add(typeof(ParaOndeVAmosViewModel), typeof(ParaOndeVamosView));
            _mappings.Add(typeof(ColaboreViewModel), typeof(ColaboreView));
            _mappings.Add(typeof(TermsOfUseViewModel), typeof(TermsOfUseView));
        }
    }
}