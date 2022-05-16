using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Drawing;

namespace DiscordBot_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public string[] login;
        public string[] password;
        public string path;
        public string channelURL;
        public IWebDriver driver;
        public IWebElement element;
        public Thread thread;
        public bool working = false;

        public void Run()
        {
            login = GetData("login");
            password = GetData("password");

            driver = new ChromeDriver();
            driver.Navigate().GoToUrl($"{channelURL}");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);

            for (int j = 0; j < login.Length - 1; ++j)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.open('{channelURL}');");
            }

            for (int i = 0; i < login.Length; ++i)
            {
                // логин
                element = driver.FindElement(By.XPath("//*[@id='app-mount']/div[2]/div/div/div/div/form/div/div/div[1]/div[2]/div[1]/div/div[2]/input"));
                element.SendKeys(login[i]);

                // пароль
                element = driver.FindElement(By.XPath("//*[@id='app-mount']/div[2]/div/div/div/div/form/div/div/div[1]/div[2]/div[2]/div/input"));
                element.SendKeys(password[i]);

                // войти
                driver.FindElement(By.XPath("//*[@id='app-mount']/div[2]/div/div/div/div/form/div/div/div[1]/div[2]/button[2]")).Click();

                Thread.Sleep(1488);

                if (i < login.Length - 1)
                    driver.SwitchTo().Window(driver.WindowHandles[i + 1]);
            }

            while (working)
            {
                if (pause)
                {
                    for (int i = 0; i < login.Length; ++i)
                    {
                        Thread.Sleep(10);
                        driver.SwitchTo().Window(driver.WindowHandles[i]);
                        SendMessage();
                        //Thread.Sleep(Convert.ToInt32(textBox5.Text));
                        DeleteMessage();
                    }
                }
            }
        }

        public string[] GetData(string data)
        {
            path = textBox1.Text;
            StreamReader sr = new StreamReader(path);
            int count = File.ReadAllLines(path).Length;
            string[] login = new string[count];
            string[] password = new string[count];

            for (int i = 0; i < count; ++i)
            {
                string temp = sr.ReadLine();

                if (temp == null) 
                    break;

                login[i] = temp.Split(' ')[0];
                password[i] = temp.Split(' ')[1];
            }

            if (data == "login") 
                return login;
            else 
                return password;
        }

        public void SendMessage()
        {
            element = driver.FindElement(By.CssSelector("#app-mount > div.app-3xd6d0 > div > div.layers-OrUESM.layers-1YQhyW > div > div > div > div > div.chat-2ZfjoI > div.content-1jQy2l > main > form > div > div > div > div.scrollableContainer-15eg7h.webkit-QgSAqd > div > div.textArea-2CLwUE.textAreaSlate-9-y-k2.slateContainer-3x9zil > div.markup-eYLPri.slateTextArea-27tjG0.fontSize16Padding-XoMpjI > div"));

            for (int i = 0; i < Convert.ToInt32(textBox3.Text); ++i)
                element.SendKeys(textBox4.Text);

            element.SendKeys(OpenQA.Selenium.Keys.Enter);
        }

        public void DeleteMessage()
        {
            Actions actions = new Actions(driver);

            element = driver.FindElement(By.CssSelector("#app-mount > div.app-3xd6d0 > div > div.layers-OrUESM.layers-1YQhyW > div > div > div > div > div.chat-2ZfjoI"));
            actions.MoveToElement(element).MoveByOffset(0, 300).Click().Build().Perform();
            //Thread.Sleep(10);
            // try catch finally
            driver.FindElement(By.XPath("//div[@aria-label='Ещё']")).Click(); // ещё
            driver.FindElement(By.CssSelector("#message-actions-delete")).Click(); // удалить?
            driver.FindElement(By.XPath("/html/body/div[1]/div[4]/div[2]/div/div/div[3]/button[1]")).Click(); // удаление
        }

        // start
        private void button1_Click(object sender, EventArgs e)
        {
            channelURL = textBox2.Text;
            working = true;
            thread = new Thread(Run);
            thread.Start();
        }

        // stop
        private void button2_Click(object sender, EventArgs e)
        {
            thread.Abort();
            working = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread != null && thread.IsAlive) 
                thread.Abort();
        }

        public bool pause = false;
        private void button3_Click(object sender, EventArgs e)
        {
            if (pause) 
                pause = false;
            else 
                pause = true;
        }

    }
}
