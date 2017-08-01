using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Jarvis
{
    /// <summary>
    /// The main Jarvis Driver
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CoreDispatcher _dispatcher;
        private SpeechSynthesizer _speechSynthesizer;
        private object media;

        private HandleTags _tagsHandler; // handles the tags that jarvis recognizes (ie: weather, news, podcast, etc)

        // Instantiate the two Speech Recognition Engines (The Wake-Engine and the Main-Engine)
        SpeechRecognizer _mainSpeechRecognizer;
        SpeechRecognizer _wakeRecognizer;

        private bool _isPlaying;
        private bool _isAwake;

        public MainPage()
        {
            InitializeComponent();

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _speechSynthesizer = new SpeechSynthesizer();

            _wakeRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);

            _mainSpeechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);

            SpeechResponse.MediaEnded += SpeechResponse_MediaEnded;

            // obtain current coordinates from public ip
            InitializeHandlerAndGetCoordinates();
        }

        private async void InitializeHandlerAndGetCoordinates()
        {
            _tagsHandler = new HandleTags();
            await _tagsHandler.GetCoordinates();

            // start the Wake Recognizer and begin listening
            await InitializeWakeRecognizer();

            // initialize the main Recognition engine
            await InitializeMainRecognizer();
        }



        #region Wake Recognizer Methods

        private async Task InitializeWakeRecognizer()
        {
            ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");

            #region Timeouts (currently unused)
            // set timeouts
            //_speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(8.0);
            //_speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(8.0);
            //_speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(2.0);
            #endregion

            // add and compile the grammar constraints
            await SetupWakeConstraints();

            // begin continuous recognition
            await _wakeRecognizer.ContinuousRecognitionSession.StartAsync();

            // capture results from speech recognition
            _wakeRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_WakeResultGenerated;

            // capture the state when voice is captured
            _wakeRecognizer.StateChanged += _wakeRecognizer_StateChanged;
        }

        private void _wakeRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Wake Recognizer: " + args.State);
        }

        private async void ContinuousRecognitionSession_WakeResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Result.Text);

            var wakeTag = args.Result.Constraint?.Tag ?? "";

            if (wakeTag.ToLower() == "wake")
            {
                if (_isPlaying)
                {
                    await _dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        SpeechResponse.Stop();

                        _isPlaying = false;
                    });
                }

                HandleWake(args.Result);

                _isAwake = true;

                return;
            }
            else if(wakeTag.ToLower() == "stop" && _isAwake)
            {
                // stop the current speech
                HandleResult(args.Result);
            }
        }

        private async Task SetupWakeConstraints()
        {
            // wake constraint
            var wakeFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/Wake.xml"));
            var wakeConstraint = new SpeechRecognitionGrammarFileConstraint(wakeFile, "wake");
            _wakeRecognizer.Constraints.Add(wakeConstraint);

            var compilationResult = await _wakeRecognizer.CompileConstraintsAsync();

            if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine(compilationResult.Status);
            }
        }

        /// <summary>
        /// Handles a recognized result from the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Result.Text);

            var wakeTag = args.Result.Constraint?.Tag ?? "";

            if (wakeTag.ToLower() == "wake")
            {
                HandleWake(args.Result);

                return;
            }
        }

        /// <summary>
        /// This is response from Jarvis that he is currently listening for commands
        /// </summary>
        /// <param name="wakeResult"></param>
        private async void HandleWake(SpeechRecognitionResult wakeResult)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                System.Diagnostics.Debug.WriteLine("Jarvis is listening:");

                PlayListeningBeep();

                var recSession = await _mainSpeechRecognizer.RecognizeAsync();

                if (recSession.Status == SpeechRecognitionResultStatus.Success && !string.IsNullOrEmpty(recSession.Text))
                {
                    HandleResult(recSession);
                }
            });
        }

        #endregion




        #region Main Speech Recognition Methods

        private async Task InitializeMainRecognizer()
        {
            ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");

            #region Timeouts
            // set timeouts
            _mainSpeechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(8.0);
            _mainSpeechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(8.0);
            _mainSpeechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(2.0);
            #endregion

            // add and compile the grammar constraints
            await SetupMainConstraits();

            // capture the state when voice is captured
            _mainSpeechRecognizer.StateChanged += _mainSpeechRecognizer_StateChanged;

            _mainSpeechRecognizer.HypothesisGenerated += _mainSpeechRecognizer_HypothesisGenerated;
        }

        private void _mainSpeechRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Hypothesis.Text);
        }

        /// <summary>
        /// Recognize the state of the main speech recognition engine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void _mainSpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Main Speech Recognizer: " + args.State);

            if (args.State == SpeechRecognizerState.Processing)
            {
                //await _mainSpeechRecognizer.StopRecognitionAsync();
                //System.Diagnostics.Debug.WriteLine("Main recognition has been stopped");
            }
        }


        /// <summary>
        /// Sets up grammar constraints
        /// </summary>
        /// <returns></returns>
        private async Task SetupMainConstraits()
        {           
            // weather constraint
            var weatherRootFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/WeatherGrammar/WeatherRootGrammar.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(weatherRootFile, "weather"));

            // weather type constraints (for more specific requests)
            var weatherTypeRootFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/WeatherGrammar/WeatherTypeGrammar.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(weatherTypeRootFile, "weatherType"));

            // budget constraints
            var getBudgetRootFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/BudgetGrammar/Get_Budget.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(getBudgetRootFile, "getBudget"));

            // recalibration constraints (for finding updated coordinates)
            var calibrationRootFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/ReCalibrate.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(calibrationRootFile, "recalibrate"));

            // sleep constraint
            var sleepRootFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/Sleep.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(sleepRootFile, "sleep"));

            // Spotify
            //var musicSearchConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "Song Name");
            var musicGrammarFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Grammar/SpotifyGrammar/SpotifySearch.xml"));
            _mainSpeechRecognizer.Constraints.Add(new SpeechRecognitionGrammarFileConstraint(musicGrammarFile, "spotify"));
            //_mainSpeechRecognizer.Constraints.Add(musicSearchConstraint);

            // compile constraints

            var compilationResult = await _mainSpeechRecognizer.CompileConstraintsAsync();

            if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine(compilationResult.Status);
                await Talk("Grammar compilation failure. Please restart.");
            }
        }

#endregion



        /// <summary>
        /// Pulls thread from UIDispatcher with grammar tag for handling
        /// </summary>
        /// <param name="result"></param>
        private async void HandleResult(SpeechRecognitionResult result)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

                var tag = result?.Constraint.Tag;
                // get properties of utterance
                IReadOnlyDictionary<string, IReadOnlyList<string>> properties = result.SemanticInterpretation.Properties;

                // identify which tag was recognized by Jarvis

                var returnSpeech = await _tagsHandler.IdentifyTag(properties, tag);

                if (!returnSpeech.Equals("I didn't quite get that"))
                {
                    if (tag == "recalibrate")
                    {
                        //await DestroyAndReInit();
                    }
                    else if (tag == "spotify")
                    {
                    }
                    else if (tag == "sleep")
                    {
                        await _mainSpeechRecognizer.StopRecognitionAsync();
                        System.Diagnostics.Debug.WriteLine("Jarvis is asleep");
                    }
                    else if (tag == "stop")
                    {
                        if (_isPlaying)
                        {
                            // stop the voice output
                            SpeechResponse.Stop();

                            _isPlaying = false;
                        }
                    }
                    await Talk(returnSpeech);
                }
                else
                {
                    await Talk(returnSpeech); // this will be the utterance that Jarvis didn't understand the speech
                }
            });
        }



        /// <summary>
        /// Speech synthesis for Jarvis
        /// </summary>
        /// <param name="message"></param>
        private async Task Talk(string message)
        {
            var stream = await Task.Run(async() =>
            {
                return await _speechSynthesizer.SynthesizeTextToStreamAsync(message);
            });
            SpeechResponse.SetSource(stream, stream.ContentType);

            SpeechResponse.Play();

            _isPlaying = true;
        }

        /// <summary>
        /// Set flag that media object has stopped playing speech synthesis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechResponse_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
        }

        private void PlayListeningBeep()
        {
            var beepSource = new Uri("ms-appx:///Assets/beep-07.mp3");
            ListeningSound.Source = beepSource;

            ListeningSound.Play();
        }
    }
}
