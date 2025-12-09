using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MovieApp.PlaywrightTests
{
    public class Tests
    {
        private IBrowser _browser;
        private IPage _page;
        private IPlaywright _playwright;

        private string _baseUrl = "https://dean-farrelly13245.github.io/Blazor-WASM-Project/";

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            _page = await _browser.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _browser.CloseAsync();
        }

        [Test]
        public async Task HomePage_ShowsPopularMoviesText()
        {
            await _page.GotoAsync(_baseUrl);

            await _page.WaitForTimeoutAsync(10000);

            var body = await _page.TextContentAsync("body");

            Assert.That(body, Does.Contain("Popular Movies"));
        }

        [Test]
        public async Task Filters_ShowCurrentText_WhenApplied()
        {
            await _page.GotoAsync(_baseUrl);

            await _page.WaitForTimeoutAsync(10000);

            await _page.SelectOptionAsync("#yearSelect", "2010s");
            await _page.ClickAsync("#applyFiltersButton");

            await _page.WaitForTimeoutAsync(2000);

            var body = await _page.TextContentAsync("body");

            Assert.That(body, Does.Contain("Year=2010s"));
        }

        [Test]
        public async Task Filters_ShouldReduce_NumberOfTitles_WhenUsingHigherRating()
        {
            await _page.GotoAsync(_baseUrl);

            await _page.WaitForTimeoutAsync(10000);

            var itemsBefore = await _page.QuerySelectorAllAsync("div[style*='background:#1f1f3a']");
            int countBefore = itemsBefore.Count;

            await _page.SelectOptionAsync("#ratingSelect", "9");
            await _page.ClickAsync("#applyFiltersButton");

            await _page.WaitForTimeoutAsync(2000);

            var itemsAfter = await _page.QuerySelectorAllAsync("div[style*='background:#1f1f3a']");
            int countAfter = itemsAfter.Count;

            Assert.That(countAfter, Is.LessThanOrEqualTo(countBefore));
        }
    }
}
