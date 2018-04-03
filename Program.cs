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
		private const string Param_FromCity = "Иж";
		private const string Param_ToCity = "М";
		private const string From_str = "Туда";
		private const string Till_str = "Обратно";

		static void Main(string[] args)
		{
			DateTime from_date = new DateTime(2018, 04, 09);
			DateTime till_date = new DateTime(2018, 04, 13);

			ChromeOptions options = new ChromeOptions();
			//options.AddArguments("--headless");
			//			using (var driver = new ChromeDriver(options))
			//using (var driver = new FirefoxDriver())
			using (var driver = new ChromeDriver())
			{
				try
				{
					driver.Navigate().GoToUrl("https://booking.izhavia.su/websky/#/search");

					// TODO Переделать XPath на CSS
					WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.XPath("//input[@placeholder='Откуда']")));

					var FromCity = driver.FindElement(By.XPath("//input[@placeholder='Откуда']"));
					var ToCity = driver.FindElement(By.XPath("//input[@placeholder='Куда']"));
					FromCity.SendKeys(Param_FromCity);
					ToCity.SendKeys(Param_ToCity);

					driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']", 
							from_date.Day, from_date.Month - 1, from_date.Year))).First().Click();

					driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']",
							till_date.Day, till_date.Month - 1, till_date.Year))).Last().Click();

					driver.FindElement(By.CssSelector("button.btn.btn_search.btn_formSearch.btn_formSearch_js")).Click();

					wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.CssSelector("div.flightTable")));

					//TODO Переделать с copy-paste на метод красивый
					var to_flightTable = driver.FindElements(By.CssSelector("div.flightTable")).First();
					var to_prices = to_flightTable.FindElements(By.CssSelector("a wrap"));
					Dictionary<IWebElement, double> to_prices_list = new Dictionary<IWebElement, double>();

					var result_str = "";
					foreach (var price in to_prices)
					{
						var str_price = price.Text;
						var price_double_str = str_price.Substring(0, str_price.Length - 1).Trim();
						double price_double;
						if (double.TryParse(price_double_str, out price_double))
						{
							to_prices_list.Add(price, price_double);
							//prices.Add(price_double);
							result_str += price_double + Environment.NewLine;
						}
					}

					var minimal_price = to_prices_list.OrderBy(p => p.Value).First();
					var maximal_price = to_prices_list.OrderByDescending(p => p.Value).First();

					result_str += Environment.NewLine + "Minimal cost: " 
					                                  + minimal_price + Environment.NewLine + "Maximal cost: " 
					                                  + maximal_price;

					minimal_price.Key.Click();

					result_str += Environment.NewLine + Environment.NewLine;

					//$("div[class='flightTable']:not([class='chooseFlight__table'])")
					var from_flightTable = driver.FindElements(By.CssSelector("div[class='flightTable']:not([class='chooseFlight__table'])")).Last();

					var from_prices = from_flightTable.FindElements(By.CssSelector("a wrap"));
					Dictionary<IWebElement, double> from_prices_list = new Dictionary<IWebElement, double>();

					foreach (var price in from_prices)
					{
						var str_price = price.Text;
						var price_double_str = str_price.Substring(0, str_price.Length - 1).Trim();
						double price_double;
						if (double.TryParse(price_double_str, out price_double))
						{
							from_prices_list.Add(price, price_double);
							//prices.Add(price_double);
							result_str += price_double + Environment.NewLine;
						}
					}

					minimal_price = from_prices_list.OrderBy(p => p.Value).First();
					maximal_price = from_prices_list.OrderByDescending(p => p.Value).First();

					result_str += Environment.NewLine + "Minimal cost: "
					                                  + minimal_price + Environment.NewLine + "Maximal cost: "
					                                  + maximal_price;

					File.WriteAllText(@"C:\tmp\to_prices.txt", result_str);

					Thread.Sleep(2000);

					minimal_price.Key.Click();

					//$('a.btn_next')

					Console.WriteLine();

				}
				finally
				{
					driver.Close();
				}
			}
			
		}
	}
}
