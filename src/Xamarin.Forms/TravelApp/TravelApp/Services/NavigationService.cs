﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.ViewModels;
using TravelApp.Views;
using Xamarin.Forms;

namespace TravelApp.Services
{
    public class NavigationService
    {
        protected readonly Dictionary<Type, Type> _mappings;

        static NavigationService _instance;

        public NavigationService()
        {
            _mappings = new Dictionary<Type, Type>();

            CreatePageViewModelMappings();
        }

        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NavigationService();

                return _instance;
            }
        }

        protected Application CurrentApplication
        {
            get { return Application.Current; }
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : BindableObject
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BindableObject
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

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }

            return _mappings[viewModelType];
        }

        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"Mapping type for {viewModelType} is not a page");
            }

            Page page = Activator.CreateInstance(pageType) as Page;

            return page;
        }

        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is HomeView)
            {
                CurrentApplication.MainPage = page;
            }
            else
            {
                if (CurrentApplication.MainPage is NavigationPage navigationPage)
                {
                    await navigationPage.PushAsync(page);
                }
            }
        }

            void CreatePageViewModelMappings()
            {
                _mappings.Add(typeof(MainViewModel), typeof(HomeView));
                _mappings.Add(typeof(DetailViewModel), typeof(DetailView));
            }
        }
    
}