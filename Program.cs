using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using System.IO;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;

namespace IzhAviaRPA
{
	class Program
	{
		private const string ParamFromCity = "Иж";
		private const string ParamToCity = "М";
		private const string ParamFamily = "Иванов";
		private const string ParamName = "Аркадий";
		private const string ParamPass = "9418558774";
		private const string ParamPhone = "79508856698";
		private const string ParamEmail = "shubin.it@mail.ru";

		//private const string From_str = "Туда";
		//private const string Till_str = "Обратно";

		public static void scrollWithOffset(IWebElement webElement, object driver, int x, int y)
		{

			String code = "window.scroll(" + (webElement.Location.X + x) + ","
			              + (webElement.Location.Y + y) + ");";

			((IJavaScriptExecutor)driver).ExecuteScript(code, webElement, x, y);

		}

		static void Main(string[] args)
		{
			DateTime fromDate = new DateTime(2018, 04, 04);
			DateTime tillDate = new DateTime(2018, 04, 06);

			DateTime birthDate = new DateTime(1984, 07, 06);


			ChromeOptions options = new ChromeOptions();
			//options.AddArguments("--headless");
			//			using (var driver = new ChromeDriver(options))
			//using (var driver = new FirefoxDriver())
			using (var driver = new ChromeDriver())
			{
				try
				{
					driver.Manage().Window.Maximize();
					driver.Navigate().GoToUrl("https://booking.izhavia.su/websky/#/search");
					

					// TODO Переделать XPath на CSS
					WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.XPath("//input[@placeholder='Откуда']")));

					var fromCity = driver.FindElement(By.XPath("//input[@placeholder='Откуда']"));
					var toCity = driver.FindElement(By.XPath("//input[@placeholder='Куда']"));
					fromCity.SendKeys(ParamFromCity);
					toCity.SendKeys(ParamToCity);

					driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']", 
							fromDate.Day, fromDate.Month - 1, fromDate.Year))).First().Click();

					driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']",
							tillDate.Day, tillDate.Month - 1, tillDate.Year))).Last().Click();

					driver.FindElement(By.CssSelector("button.btn.btn_search.btn_formSearch.btn_formSearch_js")).Click();

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("div.flightTable")));

					//TODO Переделать с copy-paste на метод красивый
					var toFlightTable = driver.FindElements(By.CssSelector("div.flightTable")).First();
					var toPrices = toFlightTable.FindElements(By.CssSelector("a wrap"));
					Dictionary<IWebElement, double> toPricesList = new Dictionary<IWebElement, double>();

					var resultStr = "";
					foreach (var price in toPrices)
					{
						var strPrice = price.Text;
						var priceDoubleStr = strPrice.Substring(0, strPrice.Length - 1).Trim();
						double priceDouble;
						if (double.TryParse(priceDoubleStr, out priceDouble))
						{
							toPricesList.Add(price, priceDouble);
							//prices.Add(price_double);
							resultStr += priceDouble + Environment.NewLine;
						}
					}

					var minimalPrice = toPricesList.OrderBy(p => p.Value).First();
					var maximalPrice = toPricesList.OrderByDescending(p => p.Value).First();

					resultStr += Environment.NewLine + "Minimal cost: " 
					                                  + minimalPrice + Environment.NewLine + "Maximal cost: " 
					                                  + maximalPrice;

					minimalPrice.Key.Click();

					resultStr += Environment.NewLine + Environment.NewLine;

					//$("div[class='flightTable']:not([class='chooseFlight__table'])")
					var fromFlightTable = driver.FindElements(By.CssSelector("div[class='flightTable']:not([class='chooseFlight__table'])")).Last();

					var fromPrices = fromFlightTable.FindElements(By.CssSelector("a wrap"));
					Dictionary<IWebElement, double> fromPricesList = new Dictionary<IWebElement, double>();

					foreach (var price in fromPrices)
					{
						var strPrice = price.Text;
						var priceDoubleStr = strPrice.Substring(0, strPrice.Length - 1).Trim();
						double priceDouble;
						if (double.TryParse(priceDoubleStr, out priceDouble))
						{
							fromPricesList.Add(price, priceDouble);
							//prices.Add(price_double);
							resultStr += priceDouble + Environment.NewLine;
						}
					}

					minimalPrice = fromPricesList.OrderBy(p => p.Value).First();
					maximalPrice = fromPricesList.OrderByDescending(p => p.Value).First();

					resultStr += Environment.NewLine + "Minimal cost: "
					                                  + minimalPrice + Environment.NewLine + "Maximal cost: "
					                                  + maximalPrice;

					

					Thread.Sleep(2000);

					minimalPrice.Key.Click();

					//$('a.btn_next')

					Thread.Sleep(2000);

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("a.btn_next")));

					

					driver.FindElement(By.CssSelector("a.btn_next")).Click();

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("input[ng-model='passenger.lastName']")));

					// Ввод данных о пассажире

					scrollWithOffset(driver.FindElement(By.CssSelector("h1.page-title")), driver, 0, 0);

					driver.FindElement(By.CssSelector("input[ng-model='passenger.lastName']")).SendKeys(ParamFamily);
					driver.FindElement(By.CssSelector("input[ng-model='passenger.firstName']")).SendKeys(ParamName);
					driver.FindElement(By.CssSelector("input[ng-model='passenger.dateOfBirth']")).SendKeys(
							birthDate.ToString("dd.MM.yyyy"));

					var gender_elm = driver.FindElement(By.CssSelector("div.inp div[ng-model='passenger.gender']"));
					gender_elm.Click();

					Thread.Sleep(1000);

					gender_elm.FindElements(By.CssSelector("div.ui-select-choices-row")).First().Click();

					var doc_elm = driver.FindElement(By.CssSelector("div[ng-model='passenger.documentType']"));
					doc_elm.Click();

					Thread.Sleep(1000);

					doc_elm.FindElements(By.CssSelector("div.ui-select-choices-row")).First().Click();
					
					//TODO все CSS селекторы переделать под ng-model
					driver.FindElement(By.CssSelector("input[ng-model='passenger.documentNumber']")).SendKeys(ParamPass);
					driver.FindElement(By.CssSelector("input[ng-model='passenger.phone']")).SendKeys(ParamPhone);
					driver.FindElement(By.CssSelector("input[ng-model='passenger.email']")).SendKeys(ParamEmail);

					driver.FindElement(By.CssSelector("div.iconfirm__i span")).Click();

					//scrollWithOffset(driver.FindElement(By.CssSelector("a.btn_next")), driver, 0, 0);






					Thread.Sleep(2000);

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("a.btn_next")));


					// Дальше
					driver.FindElement(By.CssSelector("a.btn_next")).Click();

					Thread.Sleep(2000);


					scrollWithOffset(driver.FindElement(By.CssSelector("h1.page-title")), driver, 0, 0);
					driver.ExecuteScript("document.body.style.zoom='65%'");

					Thread.Sleep(4000);
					driver.ExecuteScript("document.body.style.zoom='100%'");

					scrollWithOffset(driver.FindElement(By.CssSelector("div.iconfirm__i span")), driver, 0, 0);
					driver.FindElement(By.CssSelector("div.iconfirm__i span")).Click();

					Thread.Sleep(4000);
					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("a.btn_next")));

					driver.FindElement(By.CssSelector("a.btn_next")).Click();

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("span.orderNum")));
					

					resultStr += Environment.NewLine + "Бронирование: " + driver.Url;

					//Thread.Sleep(6000);

					//driver.FindElement(By.CssSelector("a.go_cancel websky-pay__footer__cancel__btn")).Click();

					File.WriteAllText(@"C:\tmp\to_prices.txt", resultStr);
					Thread.Sleep(10000);

					//TODO Сделать, чтобы драйвер нормально стопался
					Console.WriteLine();

				}
				finally
				{
					//driver.Close();
				}
			}
			
		}
	}
}
