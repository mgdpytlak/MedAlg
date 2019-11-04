using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace MedAlg1
{
    public class Tests : IDisposable
    {

        public IWebDriver driver;

        public Tests()
        {
            try{
                driver = new ChromeDriver();
            }
            catch (Exception e){
                Console.WriteLine("Exeption while starting browser" + e);
            }
        }

        [Fact]

        public void ValidateDownload()
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.medicalgorithmics.pl/");
            //driver.FindElement(By.CssSelector(".qode-page-loading-effect-holder")).Click(); 
            driver.FindElement(By.CssSelector("a[id$='cn-accept-cookie']")).Click();
            var contact_button = driver.FindElement(By.CssSelector("#mega-menu-item-29 > a"));

            Actions actions = new Actions(driver);
            var before_hover = contact_button.GetCssValue("color");
            actions.MoveToElement(contact_button).Build().Perform();
            var after_hover = driver.FindElement(By.CssSelector("#mega-menu-item-29 > a")).GetCssValue("color");
            //Sprawdzenie czy Zakładka Kontakt zmienia kolor czcionki po najechaniu
            Assert.NotEqual(before_hover, after_hover);

            contact_button.Click();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            var media_button = driver.FindElement(By.CssSelector("img[title$='ico-circle-media']"));
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("img[title$='ico-circle-media']"))); //Timeout exception
            //media_button.Click(); //Other element would receive the click: <div class="qode-page-loading-effect-holder"...
            new Actions(driver).MoveToElement(media_button).Click().Build().Perform();
            
            driver.Navigate().GoToUrl("https://www.medicalgorithmics.pl/media-pack"); //wyjście awaryne
            IWebElement logo_zip = driver.FindElement(By.XPath("/html/body/div[3]/div/div/div/div[2]/div/div[2]/div/div/div/div/div/div/div/div[1]/div/div/div/div[2]/div/div/div[1]/div/h1/a/strong"));
            new Actions(driver).Click(logo_zip).Build().Perform();
            //IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            //executor.ExecuteScript("arguments[0].click();", logo_zip);
            //logo_zip.Click(); //Other element would receive the click: <div class="qode-page-loading-effect-holder"...
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            string directoryPath = @"C:\Users\";
            string userName = Environment.UserName;
            string relativePath = @"\Downloads\logotypy.zip";
            string path = Path.GetFullPath(directoryPath + userName + relativePath);
            //Sprawdzenie czy plik pobrał się na lokalny komputer
            Assert.True(File.Exists(path));
        }

        [Fact]

        public void ValidateSearch()
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.medicalgorithmics.pl/");
            var search_field = driver.FindElement(By.CssSelector("input[class$='qode_search_field']"));
            search_field.SendKeys("Pocket ECG CRS");
            search_field.SendKeys(Keys.Enter);
            Assert.Equal("Wyniki wyszukiwania \"Pocket ECG CRS\" - Medicalgorithmics", driver.Title);

            //Sprawdzenie czy wyszukiwanie po powyższej frazie zwraca dokładnie 10 wyników na pierwszej stronie
            var results_sum = driver.FindElements(By.ClassName("latest_post_custom")).Count;
            Assert.Equal(10, results_sum);

            //Sprawdzenie czy wynik wyszukiwania zwraca dokładnie 1 rezultat, w którym znajduje się fraza “PocketECG CRS – telerehabilitacja kardiologiczna"
            var one_result = driver.FindElements(By.PartialLinkText("PocketECG CRS – telerehabilitacja kardiologiczna")).Count;
            Assert.Equal(1, one_result);

            //Sprawdzenie czy wyniki występują na dwóch stronach
            var inactive_sum = driver.FindElements(By.ClassName("inactive")).Count;
            Assert.Equal(1, inactive_sum);

            //Sprawdzenie czy strona załadowała się w całości poprawnie
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            Assert.Equal(js.ExecuteScript("return document.readyState").ToString(), "complete");

            //Przykłady innych sposobów
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("latest_post_custom")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("footer_top")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("wpb_wrapper")));

        }

        public void Dispose()
        {
            try{
                driver.Quit();
            }
            catch (Exception e){
                Console.WriteLine("Exeption while stopping browser" + e);
            }
        }
    }
}
