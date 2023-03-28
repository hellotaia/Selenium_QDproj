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

        [Test]
        public void SearchTest() 
        {
            //в поле поиска ввести ключевое слово: 'html' и выполнить поиск
            IWebElement searchBox = webDriver.FindElement(By.Name("s"));
            searchBox.SendKeys("html");
            searchBox.Submit();
            //проверить, что отображена страница результатов поиска, в тайтле которой отображается поисковый запрос - "HTML"
            Assert.IsTrue(webDriver.Title.Contains("html"));

            //проверить, что все товары в выдаче содержат поисковый запрос и ссылку в своем названии (вебэлемент содержит href атрибут формата 'https://...')
            IWebElement[] items = webDriver.FindElements(By.XPath("//div[@class='post-content']")).ToArray();
            foreach (IWebElement item in items)
            {
                IWebElement productLink = item.FindElement(By.XPath("//h2/a"));
                Assert.IsTrue(productLink.Text.ToLower().Contains("html"));
                Assert.IsTrue(productLink.GetAttribute("href").StartsWith("https://"));
            }

            //перейти на карточку товара 'Thinking in HTML' 
            IWebElement itemThink = items[0].FindElement(By.XPath("//h2/a[@title='Thinking in HTML']"));
            itemThink.Click();

            /*//если попапнулась реклама
            if (webDriver.FindElements(By.XPath("//div[@id='dismiss-button']")).Count > 0)
            {
                IWebElement closeAd = webDriver.FindElement(By.XPath("//div[@id='dismiss-button']"));
                Thread.Sleep(1000);
                closeAd.Click();
            }
            //другой тип рекламы
            if (webDriver.FindElements(By.XPath("//div[@id='dismiss-button']")).Count > 0)
            {
                IWebElement closeAd2 = webDriver.FindElement(By.XPath("//div[contains(@class,'close-button')]"));
                Thread.Sleep(1000);
                closeAd2.Click();
            }*/

            //и проверить, что отображена метка SALE на картинке товара и отображены две цены (обычная и цена со скидкой)
            Thread.Sleep(3000);
            IWebElement saleMark = webDriver.FindElement(By.XPath("//span[@class='onsale']"));
            Assert.IsTrue(saleMark.FindElement(By.XPath("//span[@class='onsale']")).Displayed);

            IWebElement regularPrice = webDriver.FindElement(By.XPath("//del/span[@class='woocommerce-Price-amount amount']"));
            IWebElement salePrice = webDriver.FindElement(By.XPath("//ins/span[@class='woocommerce-Price-amount amount']"));
            Assert.IsTrue(regularPrice.Text.StartsWith("₹"));
            Assert.IsTrue(salePrice.Text.StartsWith("₹"));

            //найти в Related products 'HTML5 WebApp Develpment' и перейти на карточку товара
            Thread.Sleep(3000);
            IWebElement[] relatedProds = webDriver.FindElements(By.XPath("//ul[@class='products']")).ToArray();
            IWebElement itemHtmlWebDev = relatedProds[0].FindElement(By.XPath("//li/a[@class='woocommerce-LoopProduct-link']"));

            itemHtmlWebDev.Click();


            //добавить товар в корзину и запомнить полное название и цену
            webDriver.Navigate().GoToUrl("https://practice.automationtesting.in/product/mastering-javascript/");
            IWebElement itemName = webDriver.FindElement(By.XPath("//h1[@itemprop='name']"));
/*            IWebElement itemprice = webDriver.FindElement(By.XPath("//p[@class='price']/span/text()"));*/

            IWebElement addItemToCart = webDriver.FindElement(By.XPath("//button[@type='submit']"));
            addItemToCart.Click();
            IWebElement cartMsg = webDriver.FindElement(By.XPath("//div[@class='woocommerce-message']"));

            /*            Assert.IsTrue(cartMsg.Text.Contains($"{itemName}"));*/

            //изменить количество товара в корзине на 3
            IWebElement viewBasket = cartMsg.FindElement(By.XPath("//a[contains(@class,'button')]"));
            viewBasket.Click();

            IWebElement qty = webDriver.FindElement(By.XPath("//div[@class='quantity']/input"));
            qty.Click();
            qty.Clear();
            qty.SendKeys("3");
            IWebElement updateCart = webDriver.FindElement(By.Name("update_cart"));
            updateCart.Click();
            //открыть корзину и сравнить название и цену в колонке "Total" у товара, на соответствие сохраненным значениям в соответствии с измененным количеством


        }
        /*  [Test]
          [TestCase("html")]
          [TestCase("selenium")]
          [TestCase("web")]
          [TestCase("guide")]
          public void SearchExamples(string searchParametr)
          {
              //Используя Examples, добавить тест для проверки результата поиска для следующих запросов:
              *//*'selenium','web','guide'*//*
              IWebElement searchBox = webDriver.FindElement(By.Name("s"));
              searchBox.SendKeys(searchParametr);
              searchBox.Submit();

              Assert.IsTrue(webDriver.Title.Contains(searchParametr));
          }*/

        [TearDown]
        public void TearDown()
        {
           /* webDriver.Close();*/
        }
    }
}
