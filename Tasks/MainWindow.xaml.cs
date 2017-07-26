using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Tasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AsyncExamples _asyncExamples;

        public MainWindow()
        {
            InitializeComponent();
            _asyncExamples = new AsyncExamples();
        }

        #region Asynchronous Programming
        private void Download(object sender, RoutedEventArgs e)
        {
            Html.Text = string.Empty;
            Html.Text = _asyncExamples.Download();
        }

        private async void DownloadAsync(object sender, RoutedEventArgs e)
        {
            Html.Text = string.Empty;
            await _asyncExamples.DownloadAsync();
        }

        public void ContinueWithChain(object sender, RoutedEventArgs e)
        {
            _asyncExamples.ContinueWithChain().ContinueWith(t => Console.WriteLine("Task finished"));
        }

        private void CalculateAsync(object sender, RoutedEventArgs e)
        {
            _asyncExamples.CalculateAsync();
        }
        #endregion

        #region Cancellation
        private CancellationTokenSource _tokenSource;

        private async void RunInfinitiveLoop(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();
            await _asyncExamples.WithCancellation(_tokenSource.Token);
        }

        private void StopInfinitiveLoop(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();
        }
        #endregion

        #region Exceptions
        private void HandleExceptions(object sender, RoutedEventArgs e)
        {
            _asyncExamples.HandleExceptions();
        }
        #endregion
    }
}
