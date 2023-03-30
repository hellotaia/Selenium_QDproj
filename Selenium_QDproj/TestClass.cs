using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V108.Network;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;

namespace Selenium_QDproj
{
    internal class SeleniumWebDriverTest
    {
    }
    public class SWebDriverTest
    {
        IWebDriver webDriver;

        [SetUp]
        public void SetUp()
        {
             webDriver = new ChromeDriver();
            //открыть сайт 
            webDriver.Navigate().GoToUrl("https://practice.automationtesting.in/shop/");
            webDriver.Manage().Window.Maximize();
        }

        public void AdResolver()
        {
            string closeButton1 = "//div[@id='dismiss-button']/div";
            string closeButton2 = "//div[@id='dismiss-button']/div";
            //если попапнулась реклама
            if (webDriver.FindElements(By.XPath(closeButton1)).Count > 0)
            {
                IWebElement closeAd = webDriver.FindElement(By.XPath(closeButton1));
                closeAd.Click();
            }
            //другой тип рекламы
            if (webDriver.FindElements(By.XPath(closeButton2)).Count > 0)
            {
                IWebElement closeAd2 = webDriver.FindElement(By.XPath(closeButton2));
                closeAd2.Click();
            }
        }

        public void WaitForJavaScriptLoad(byte MaxdelaySeconds = 10)
        {
            new Actions(webDriver); IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)webDriver;
            int num = MaxdelaySeconds; while (num > 0)
            {
                Thread.Sleep(3000);
                if (!(bool)javaScriptExecutor.ExecuteScript("return window.jQuery == undefined") && !(bool)javaScriptExecutor.ExecuteScript("return window.jQuery.active == 0"))
                {
                    num--; continue;
                }
                break;
            }
        }

        [Test]
        public void SearchTest() 
        {
            //в поле поиска ввести ключевое слово: 'html' и выполнить поиск
            IWebElement searchBox = webDriver.FindElement(By.Name("s"));
            searchBox.SendKeys("html");
            searchBox.Submit();
            var pageTitle = webDriver.Title;
            //проверить, что отображена страница результатов поиска, в тайтле которой отображается поисковый запрос - "HTML"
            StringAssert.Contains("html", pageTitle);

            //проверить, что все товары в выдаче содержат поисковый запрос и ссылку в своем названии (вебэлемент содержит href атрибут формата 'https://...')
            var items = webDriver.FindElements(By.XPath("//div/h2/a")).ToArray();
            foreach (var item in items)
            {
                string productLink = item.FindElement(By.XPath("//div/h2/a")).GetAttribute("href");
                Assert.That(productLink, Does.Contain("https://"));
            }

            //перейти на карточку товара 'Thinking in HTML' 
            AdResolver();
            IWebElement itemThink = items[0].FindElement(By.XPath("//h2/a[@title='Thinking in HTML']"));
            itemThink.Click();

            //и проверить, что отображена метка SALE на картинке товара и отображены две цены (обычная и цена со скидкой)
            Thread.Sleep(3000);
            IWebElement saleMark = webDriver.FindElement(By.XPath("//span[@class='onsale']"));
            Assert.IsTrue(saleMark.FindElement(By.XPath("//span[@class='onsale']")).Displayed);

            IWebElement regularPrice = webDriver.FindElement(By.XPath("//del/span[@class='woocommerce-Price-amount amount']"));
            IWebElement salePrice = webDriver.FindElement(By.XPath("//ins/span[@class='woocommerce-Price-amount amount']"));
            StringAssert.StartsWith("₹", regularPrice.Text, salePrice.Text);

            //найти в Related products 'HTML5 WebApp Develpment' и перейти на карточку товара
            IWebElement itemHtmlWebDev = webDriver.FindElement(By.XPath("//li/a[@class='woocommerce-LoopProduct-link']"));
            itemHtmlWebDev.Click();
            AdResolver();

            //добавить товар в корзину и запомнить полное название и цену
            //у пошук javascript, перевіряєш шо у тайтлі є "JavaScript",
            searchBox = webDriver.FindElement(By.Name("s"));
            searchBox.SendKeys("javascript");
            searchBox.Submit();
            AdResolver();
            pageTitle = webDriver.Title;
            StringAssert.Contains("javascript", pageTitle, "This string does not contain 'javascript'");
            //далі переходь на останній товар. А в ньому потім в релейтед перейдеш на цю сторінку де є товар.
            items = webDriver.FindElements(By.XPath("//div[@class='post-content']")).ToArray();
            IWebElement itemFuncJS = webDriver.FindElement(By.XPath("//h2/a[@title='Mastering JavaScript']"));
            itemFuncJS.Click();
            AdResolver();

            var itemName = webDriver.FindElement(By.XPath("//h1[@itemprop='name']")).Text;
            var itemprice = webDriver.FindElement(By.XPath("//p[@class='price']/span")).Text;
            var intitemprice = Convert.ToDouble(itemprice.Replace("₹", ""));

            IWebElement addItemToCart = webDriver.FindElement(By.XPath("//button[@type='submit']"));
            addItemToCart.Click();
            var cartMsg = webDriver.FindElement(By.XPath("//div[@class='woocommerce-message']"));

            StringAssert.Contains($"{itemName}", cartMsg.Text, $"Cart message does not contain {itemName}");

            //изменить количество товара в корзине на 3
            IWebElement viewBasket = cartMsg.FindElement(By.XPath("//a[contains(@class,'button')]"));
            viewBasket.Click();

            IWebElement qty = webDriver.FindElement(By.XPath("//div[@class='quantity']/input"));
            qty.Clear();
            qty.SendKeys("3");

            IWebElement updateCart = webDriver.FindElement(By.Name("update_cart"));
            updateCart.Click();
            //открыть корзину и сравнить название и цену в колонке "Total" у товара, на соответствие сохраненным значениям в соответствии с измененным количеством
            WaitForJavaScriptLoad();
            var prodName = webDriver.FindElement(By.XPath("//td[@class=\"product-name\"]/a")).Text;
            var totalPrice = webDriver.FindElement(By.XPath("//td[@class=\"product-subtotal\"]/span")).Text;
            var inttotalPrice = Convert.ToDouble(totalPrice.Replace("₹","").Replace(",",""));

            StringAssert.AreEqualIgnoringCase(prodName, itemName, "Actual/Expected items names are not equal");
            Assert.AreEqual(inttotalPrice, (intitemprice * 3), "Actual/Expected items prices are not equal");
        }
        [Test]
        [TestCase("html")]
        [TestCase("selenium")]
        [TestCase("web")]
        [TestCase("guide")]
        public void SearchExamples(string searchParametr)
        {
            //Используя Examples, добавить тест для проверки результата поиска для следующих запросов:
            //'selenium','web','guide'
              IWebElement searchBox = webDriver.FindElement(By.Name("s"));
            searchBox.SendKeys(searchParametr);
            searchBox.Submit();

            Assert.IsTrue(webDriver.Title.Contains(searchParametr));
        }

        [TearDown]
        public void TearDown()
        {
            webDriver.Close();
        }
    }
}
