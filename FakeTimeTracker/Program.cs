using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace FakeTimeTracker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter username:");
            string user = ReadInput();

            Console.WriteLine("Enter password:");
            string password = ReadInput();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Username and password are mandatory");
                Environment.Exit(0);
            } 

            ValuesService valuesService = new();

            string loginResult = await valuesService.LoginData(user, password);
            if (!string.IsNullOrEmpty(loginResult))
            {
                Console.WriteLine($"Error during login: {loginResult}");
            }
            else
            {
                Console.WriteLine("Login successful and data inserted.");
            }

            valuesService.CloseBrowser();
        }
        static string ReadInput()
        {
            string inputString = string.Empty;
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(intercept: true);// Intercept key press to prevent displaying
                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    inputString += keyInfo.KeyChar;
                    Console.Write('*');// Display an asterisk for each character
                }
            } 
            while (keyInfo.Key != ConsoleKey.Enter);// Stop reading when Enter is pressed
            Console.WriteLine(); // Move to the next line
            return inputString;
        }
}
        
    public class ValuesService
    {
        private ChromeDriver _driver;

        public ValuesService()
        {
            _driver ??= new ChromeDriver();
        }

        public async Task<string> LoginData(string user, string password)
        {
            try
            {
                _driver.Navigate().GoToUrl("https://timetracker.mbdom.rbbh");

                IWebElement firstInputField = _driver.FindElement(By.Id("username"));
                firstInputField.Click();
                firstInputField.SendKeys(user);
                await Task.Delay(100);

                Actions actions = new(_driver);
                actions.SendKeys(Keys.Tab).Perform();
                await Task.Delay(100);
                actions.SendKeys(password).Perform();
                await Task.Delay(100);
                actions.SendKeys(Keys.Enter).Perform();

                await Task.Delay(500);

                await InsertDataAsync();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> InsertDataAsync()
        {
            try
            {
                IReadOnlyCollection<IWebElement> elements = _driver.FindElements(By.ClassName("border-td"));

                foreach (var element in elements)
                {
                    if (element.GetAttribute("class").Trim() == "border-td")
                    {
                        try
                        {
                            element.Click();
                            await Task.Delay(200);

                            // Find the 'task-holder' with the specified conditions
                            IWebElement taskHolder = _driver.FindElement(By.XPath("//div[contains(@class, 'task-holder') and .//div[contains(text(), 'Nema unesenih taskova')]]"));
                            await Task.Delay(200);

                            // Find and click the 'Unesi sate' button
                            IWebElement unesiSateButton = _driver.FindElement(By.XPath(".//button[contains(text(), 'Unesi sate')]"));
                            unesiSateButton.Click();
                            await UnosTaska(false);

                            // Find and click the 'Prekovremeni' button
                            IWebElement unesiPrekovremene = _driver.FindElement(By.XPath(".//button[contains(text(), 'Prekovremeni')]"));
                            unesiPrekovremene.Click();
                            await UnosTaska(true);
                        }
                        catch (NoSuchElementException)
                        {
                            // Skip to the next element if any element is not found
                            continue;
                        }
                    }
                }
                return "Insert done";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task UnosTaska(bool prekovremeni)
        {
            if (prekovremeni)
            {
                IWebElement mandayUtilityComponent1 = _driver.FindElement(By.XPath("//div[contains(@class, 'manday-utility-component') and .//div[contains(@class, 'manday-name') and contains(text(), 'Innovation')]]"));
                IWebElement mandayButton1 = mandayUtilityComponent1.FindElement(By.TagName("button"));
                mandayButton1.Click();
            }
            else
            {
                IWebElement mandayUtilityComponent1 = _driver.FindElement(By.XPath("//div[contains(@class, 'manday-utility-component') and .//div[contains(@class, 'manday-name') and contains(text(), 'Maintenance')]]"));
                IWebElement mandayButton1 = mandayUtilityComponent1.FindElement(By.TagName("button"));
                mandayButton1.Click();
            }

            IWebElement mandayUtilityComponent2 = _driver.FindElement(By.XPath("//div[contains(@class, 'manday-utility-component') and .//div[contains(@class, 'manday-name') and contains(text(), 'Streams')]]"));
            IWebElement mandayButton2 = mandayUtilityComponent2.FindElement(By.TagName("button"));
            mandayButton2.Click();

            IWebElement mandayUtilityComponent3 = _driver.FindElement(By.XPath("//div[contains(@class, 'manday-utility-component') and .//div[contains(@class, 'manday-name') and contains(text(), 'Digital Solutions for PI Stream')]]"));
            IWebElement mandayButton3 = mandayUtilityComponent3.FindElement(By.TagName("button"));
            mandayButton3.Click();

            IWebElement mandayUtilityComponent4 = _driver.FindElement(By.XPath("//div[contains(@class, 'manday-utility-component') and .//div[contains(@class, 'manday-name') and contains(text(), 'Business Applications/Services')]]"));
            IWebElement mandayButton4 = mandayUtilityComponent4.FindElement(By.TagName("button"));
            mandayButton4.Click();

            await Task.Delay(200);

            IWebElement searchCMInput = _driver.FindElement(By.XPath("//div[contains(@class, 'modal-body')]//input[@type='text' and @placeholder='Search in CM catalog']"));
            searchCMInput.Click();
            if (prekovremeni)
                searchCMInput.SendKeys("dig");
            else searchCMInput.SendKeys("mob");
            searchCMInput.SendKeys(Keys.Enter);

            if (prekovremeni)
            {
                IWebElement cmComponent = _driver.FindElement(By.XPath("//div[contains(@class, 'cm-component') and .//div[contains(@class, 'service-name') and starts-with(text(), 'Digital Admin')]]"));
                cmComponent.Click();
            }
            else
            {
                IWebElement cmComponent = _driver.FindElement(By.XPath("//div[contains(@class, 'cm-component') and .//div[contains(@class, 'service-name') and starts-with(text(), 'Mobilno')]]"));
                cmComponent.Click();
            }

            IWebElement formItem = _driver.FindElement(By.XPath("//div[contains(@class, 'form-item') and contains(text(), 'Sati')]"));
            IWebElement inputField = formItem.FindElement(By.XPath(".//input[@type='text']"));

            inputField.Click();
            inputField.Clear();
            inputField.SendKeys("8");

            Actions actions = new Actions(_driver);
            actions.SendKeys(Keys.Tab)
                   .SendKeys("DigitalForPI")
                   .SendKeys(Keys.Tab)
                   .SendKeys("DigitalForPI")
                   .Perform();

            await Task.Delay(200);

            IWebElement saveButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Spremi')]"));
            saveButton.Click();
            await Task.Delay(500);
        }

        public void CloseBrowser()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }
    }
}
