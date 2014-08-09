﻿using System;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using System.Collections.Generic;
using RepositoryStumble.Core.Data;
using Xamarin.Utilities.Core.Services;
using System.Reactive.Linq;
using Akavache;

namespace RepositoryStumble.Core.ViewModels.Languages
{
    public class LanguagesViewModel : BaseViewModel, ILoadableViewModel
    {
        private const string LanguagesUrl = "http://trending.codehub-app.com/languages";
        private readonly ReactiveList<Language> _languages = new ReactiveList<Language>();

        public IReactiveCommand LoadCommand { get; private set; }

        private Language _selectedLanguage;
        public Language SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { this.RaiseAndSetIfChanged(ref _selectedLanguage, value); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        public IReadOnlyReactiveList<Language> Languages { get; private set; }

        public List<Language> ExtraLanguages { get; private set; }

        public LanguagesViewModel(IJsonSerializationService jsonSerializationService, INetworkActivityService networkActivity)
        {
            ExtraLanguages = new List<Language>();

            Languages = _languages.CreateDerivedCollection(
                x => x, 
                x => x.Name.StartsWith(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase), 
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
                var trendingData = await BlobCache.LocalMachine.DownloadUrl(LanguagesUrl, absoluteExpiration: DateTimeOffset.Now.AddDays(1));
                var langs = jsonSerializationService.Deserialize<List<Language>>(System.Text.Encoding.UTF8.GetString(trendingData));
                langs.InsertRange(0, ExtraLanguages);
                _languages.Reset(langs);
            });

            LoadCommand.TriggerNetworkActivity(networkActivity);
        }
    }
}

