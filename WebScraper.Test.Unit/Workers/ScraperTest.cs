using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScraper.Builders;
using WebScraper.Data;
using WebScraper.Workers;

namespace WebScraper.Test.Unit
{
    
    [TestClass]
    public class ScraperTest
    {
        private readonly Scraper _scraper = new Scraper();

        [TestMethod]
        public void FindCollectionWithNoSegments()
        {
            var content = "Some fluff data<a href=\"https://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a> More fluff data";
            ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                .WithData(content)
                .WithRegex(@"<a href=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                .WithRegexOption(RegexOptions.ExplicitCapture)
                .Build();
            var foundElements = _scraper.Scrape(scrapeCriteria);

            Assert.IsTrue(foundElements.Count == 1);
            Assert.IsTrue(foundElements[0] == "<a href=\"https://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a>");

        }

        [TestMethod]
        public void FindCollectionWithTwoParts()
        {
            var content = "Some fluff data<a href=\"https://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a> More fluff data";
            ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                    .WithData(content)
                    .WithRegex(@"<a href=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                    .WithRegexOption(RegexOptions.ExplicitCapture)
                    .WithPart(new ScrapeCriteriaPartBuilder()
                             .WithRegex(@">(.*?)</a>")
                             .WithRegexOption(RegexOptions.Singleline)
                             .Build())
                    .WithPart(new ScrapeCriteriaPartBuilder()
                             .WithRegex(@"href=\""(.*?)\""")
                             .WithRegexOption(RegexOptions.Singleline)
                             .Build())
                    .Build();

            var foundElements = _scraper.Scrape(scrapeCriteria);

            Assert.IsTrue(foundElements.Count == 2);
            Assert.IsTrue(foundElements[0] == "some text");
            Assert.IsTrue(foundElements[1] == "https://domain.com");


        }
    }
}
