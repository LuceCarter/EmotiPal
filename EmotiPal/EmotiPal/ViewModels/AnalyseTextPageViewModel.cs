﻿using EmotiPal.Helpers;
using EmotiPal.Models;
using EmotiPal.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Realms;
using Realms.Sync;
using System.Linq;

namespace EmotiPal.ViewModels
{
    public class AnalyseTextPageViewModel : BaseViewModel
    {
        private string _textForAnalysis;
        private string _sentimentAnalysisResult;
        private Color _sentimentResultColour;
        private ObservableCollection<SentimentResult> _sentimentResults;
        private Realms.Sync.App app;
        private User user;
        private Realm realm;
        private SyncConfiguration config;
        

        public AnalyseTextPageViewModel()
        {
            AnalyseTextCommand = new Command(async () => await AnalyseText(TextForAnalysis));
            SentimentResultColour = Color.Black;                                  
        }        

        
        public string TextForAnalysis
        {
            get => _textForAnalysis;
            set => SetProperty(ref _textForAnalysis, value);
        }

        public ICommand AnalyseTextCommand { get; }

        public string SentimentAnalysisResult
        {
            get => _sentimentAnalysisResult;
            set => SetProperty(ref _sentimentAnalysisResult, value);
        }

        public Color SentimentResultColour
        {
            get => _sentimentResultColour;
            set => SetProperty(ref _sentimentResultColour, value);
        }

        public ObservableCollection<SentimentResult> SentimentResults
         {
            get => _sentimentResults;
            set => SetProperty(ref _sentimentResults, value);
         }

        async Task AnalyseText(string textForAnalysis)
        {
            TextForAnalysis = "";
            var textAnalyser = new TextAnalyser();

            SentimentResult result = new SentimentResult();
            result.AnalysedText = textForAnalysis;

            switch (await textAnalyser.AnalyseSentiment(textForAnalysis))
            {
                case "Unknown":
                    result.Sentiment = "Unknown";
                    SentimentResultColour = Color.Blue;
                    SentimentAnalysisResult = "😕 Hmmm I can't seem to tell the sentiment!";
                    break;
                case "Normal":
                    result.Sentiment = "Normal";
                    SentimentAnalysisResult = "😐 This text seems neutral to me!";
                    break;
                case "Positive":
                    result.Sentiment = "Positive";
                    SentimentResultColour = Color.Green;
                    SentimentAnalysisResult = "😁 Yay! This text seems happy!";
                    break;
                case "Negative":
                    result.Sentiment = "Negative";
                    SentimentResultColour = Color.Red;
                    SentimentAnalysisResult = "😥 This text appears to be not happy";
                    break;
                default:
                    result.Sentiment = "Unknown";
                    SentimentResultColour = Color.Blue;
                    SentimentAnalysisResult = "😕 Hmmm I can't seem to tell the sentiment!";
                    break;
            }

            try
            {
                result.PartitionKey = "myPartition";
                realm = await Realm.GetInstanceAsync(config);
                realm.Write(() =>
                {
                    realm.Add(result);
                });

                    
            } catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Cannot write to database", ex.Message, "OK");
            }                       
        }

        // Realm calls have to be run on the same thread so this is called from OnAppearing
        // in code-behind as this always uses a single thread.
        public async Task InitialiseRealm()
        {
            app = Realms.Sync.App.Create(APIKeys.RealmAppId);
            user = await app.LogInAsync(Credentials.Anonymous());
            config = new SyncConfiguration("myPartition", user)
            {
                SchemaVersion = 3
            };
            realm = await Realm.GetInstanceAsync(config);
            SentimentResults = new ObservableCollection<SentimentResult>(realm.All<SentimentResult>().ToList());
        }
    }
}
