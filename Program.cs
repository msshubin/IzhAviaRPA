using System;
using System.Collections.Generic;
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
			DateTime from_date = new DateTime(2018, 04, 04);
			DateTime till_date = new DateTime(2018, 04, 10);




			ChromeOptions options = new ChromeOptions();
			//options.AddArguments("--headless");
			//			using (var driver = new ChromeDriver(options))
			//using (var driver = new FirefoxDriver())
			using (var driver = new ChromeDriver())
			{
				try
				{
					driver.Navigate().GoToUrl("https://booking.izhavia.su/websky/#/search");

					var window_pos = driver.Manage().Window.Position;
					




					WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						By.XPath("//input[@placeholder='Откуда']")));


					var FromCity = driver.FindElement(By.XPath("//input[@placeholder='Откуда']"));
					var ToCity = driver.FindElement(By.XPath("//input[@placeholder='Куда']"));
					FromCity.SendKeys(Param_FromCity);
					ToCity.SendKeys(Param_ToCity);
					/*
					JavascriptExecutor js = (JavascriptExecutor)webDriver;
					js.executeScript("javascript:document.getElementById(\"sliderWidget\").value=1.5;");

					System.out.println(slider.getAttribute("value"));
					*/
					//$('input[class="textInp calendarInp calendarInp_js ng-touched ng-not-empty ng-dirty ng-valid-parse ng-valid ng-valid-required"]').click()

					//var from_button = driver.FindElement(By.Id("book_from"));
					//from_button.Click();

					//var FromDate = driver.FindElement(By.CssSelector(String.Format("input[placeholder='{0}'] button[data-pika-day='{1}'][data-pika-month='{2}'][data-pika-year='{3}']", From_str, from_date.Day, from_date.Month, from_date.Year)));
					//FromDate.Click();
					//driver.FindElement(By.Id("book_from")).SendKeys("04.04.2018");

					var FromDate_elm = driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']", 
							from_date.Day, from_date.Month - 1, from_date.Year))).First();
					var point1 = FromDate_elm.Location;
					//Actions builder = new Actions(driver);
					//builder.MoveToElement(FromDate_elm).Perform();

					FromDate_elm.Click();
					/*
					FromDate_elm = driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']",
							from_date.Day, from_date.Month - 1, from_date.Year))).First();
*/
					

					//driver.FindElement(By.Id("book_to")).Click();


					//Actions builder = new Actions(driver);
					//builder.MoveToElement(TillDate_elm).Perform();

					var TillDate_elm = driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']",
							till_date.Day, till_date.Month - 1, till_date.Year))).Last();
					var point2 = TillDate_elm.Location;
					TillDate_elm.Click();

					File.WriteAllText(@"C:\tmp\points.txt", "Window: " + window_pos + Environment.NewLine+ point1 + Environment.NewLine + point2);

					driver.FindElement(By.CssSelector("button.btn.btn_search.btn_formSearch.btn_formSearch_js")).Click();


					//driver.Mouse.MouseMove((ICoordinates)point2);

					/*
					Actions action = new Actions(driver);

					// First, go to your start point or Element
					action.MoveToElement(FromDate_elm);
					action.Perform();

					// Then, move the mouse
					action.MoveByOffset(100, 150);
					action.Perform();

					// Then, move again (you can implement your one code to follow your curve...)
					//action.MoveByOffset(-10, -10);
					//action.Perform();

					// Finaly, click
					action.Click();
					action.Perform();
					*/

					/*var TillDate_elm = driver.FindElements(By.CssSelector(
						String.Format("button[data-pika-day='{0}'][data-pika-month='{1}'][data-pika-year='{2}']", 
							till_date.Day, till_date.Month - 1, till_date.Year))).First();

					TillDate_elm.Click();*/



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
