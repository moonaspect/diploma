using Japanese.ViewModels;

namespace JapaneseTests
{
    [TestFixture]
    public class MatchGameViewModelTests
    {
        [Test]
        public void HandleMismatch_UpdatesGameMessage()
        {
            // Arrange
            var viewModel = new MatchGameViewModel();

            // Act
            viewModel.HandleMismatch();

            // Assert
            Assert.That(
                viewModel.GameMessage,
                Is.EqualTo("Incorrect, try again!"),
                "GameMessage should update on mismatch."
            );
        }

        [Test]
        public void HandleMatch_UpdatesGameMessageAndCounts()
        {
            // Arrange
            var viewModel = new MatchGameViewModel();
            viewModel.WordPairs.Add(new WordPair { Japanese = "こんにちは", Ukrainian = "Привіт" });

            // Act
            viewModel.HandleMatch();

            // Assert
            Assert.That(
                viewModel.GameMessage,
                Is.EqualTo("Correct! Keep going!"),
                "Game message should update on match."
            );
        }

        [Test]
        public void CheckAndIncrementMatchCount_EndsGameAfterMaxMatches()
        {
            // Arrange
            var viewModel = new MatchGameViewModel();
            for (int i = 0; i < 19; i++)
            {
                viewModel.CheckAndIncrementMatchCount();
            }

            // Act
            var result = viewModel.CheckAndIncrementMatchCount();

            // Assert
            Assert.That(
                result,
                Is.True,
                "CheckAndIncrementMatchCount should return true after reaching MaxMatches."
            );
        }

        [Test]
        public void OnPropertyChanged_TriggersEvent()
        {
            // Arrange
            var viewModel = new MatchGameViewModel();
            bool wasTriggered = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MatchGameViewModel.GameMessage))
                {
                    wasTriggered = true;
                }
            };

            // Act
            viewModel.GameMessage = "New Message";

            // Assert
            Assert.That(wasTriggered, Is.True, "PropertyChanged event should be triggered.");
        }
    }
}
