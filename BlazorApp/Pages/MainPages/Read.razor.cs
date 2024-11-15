﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Services;
using System.Reflection.Metadata;

namespace BlazorApp.Pages.MainPages
{
    public partial class Read : ComponentBase
    {
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        [Inject] ILocalStorageService LocalStorageService { get; set; } = null!;
        [Inject] UserService UserService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null;
        [Parameter] public string BookId { get; set; } = null!;
        public string? BookLang { get; set; } = null!;
        //public string? UserMainLang { get; set; } = null!;
        private string BookFormat { get; set; } = null!;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            HttpInterceptorService.RegisterEvent();
            ParseQueryString();
            BookFormat = await LocalStorageService.GetItemAsStringAsync("bookFormat");      //19-30 msec
            //NativeLanguage = await LocalStorageService.GetItemAsStringAsync("NativeLanguage");
            //UserMainLang = await UserService.GetNativeLanguage();
            await base.OnInitializedAsync();
            _isLoading = false;
        }

        private void ParseQueryString()
        {
            var uri = new Uri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (query != null)
            {
                BookLang = query.Get("lang");       //Maybe exception or handle if lang is not set
            }
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
