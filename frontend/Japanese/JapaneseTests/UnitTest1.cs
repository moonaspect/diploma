using NUnit.Framework;
using System.Threading.Tasks;

namespace Japanese.ViewModels.Tests
{
    [TestFixture]
    public class RecordsViewModelTests
    {
        [Test]
        public async Task LoadRecordsAsync_ValidResponse_PopulatesGameRecords()
        {
            // Arrange
            var viewModel = new RecordsViewModel();
            var initialCount = viewModel.GameRecords.Count;

            // Act
            await viewModel.LoadRecordsAsync();

            // Assert
            Assert.That(viewModel.GameRecords.Count, Is.GreaterThan(initialCount), "Records should be populated.");
        }

        [Test]
        public async Task SaveRecordAsync_ValidRecord_SavesSuccessfully()
        {
            // Arrange
            var viewModel = new RecordsViewModel();
            var newRecord = new GameRecord { PlayerId = "TestPlayer", Score = 100 };

            // Act
            await viewModel.SaveRecordAsync(newRecord);

            // Assert
            Assert.That(viewModel.GameRecords, Does.Contain(newRecord), "Record should be saved successfully.");
        }
    }
}
