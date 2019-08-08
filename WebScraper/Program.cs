using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using WebScraper.Data;
using WebScraper.Builders;
using WebScraper.Workers;

namespace WebScrapper
{
    class Program
    {

        private const string Method = "search";

        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Please enter which city you would like to scrape information from: ");
                var craigslistCity = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Please enter the CraigsList category that you would like to scrape: ");
                var craigslistCategoryName = Console.ReadLine() ?? string.Empty;

                using (WebClient webClient = new WebClient())
                {
                    string content = webClient.DownloadString($"https://{craigslistCity.Replace(" ", string.Empty)}.craigslist.org/{Method}/{craigslistCategoryName}");
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

                    Scraper scraper = new Scraper();

                    var scrapedElements = scraper.Scrape(scrapeCriteria);

                    if (scrapedElements.Any())
                    {
                        foreach (var scrapedElement in scrapedElements) Console.WriteLine(scrapedElement);
                    }

                    else
                    {
                        Console.WriteLine("There are no matches for the spicified criteria");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            /*MatchCollection matches = Regex.Matches("blah blah blah...This is my cat...bla", "This is my [a-z]at");

            foreach (var match in matches)
            {
                Console.WriteLine(match);
            }
            */

            Console.ReadKey();
        }
    }
}

