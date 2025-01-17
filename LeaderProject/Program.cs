﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Data.SQLite;

namespace Leader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var directory = Environment.CurrentDirectory;
            var filePath = Path.GetFullPath(directory);
            IWebDriver driver = new ChromeDriver(filePath);
            driver.Navigate().GoToUrl("https://www.leader.ir/");
            Console.WriteLine("opened browser");

            string path = "dataTable.db";
            
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                using (var sqlite = new SQLiteConnection(@"Data Source=" + path))
                {
                    sqlite.Open();
                    string sql = "create table leader(statements TEXT)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite);
                    command.ExecuteNonQuery();
                }

            }


            var dataList = driver.FindElements(By.XPath("/html/body/footer/nav[1]/div/div[2]/div/div[1]/div[1]/ul/li"));
            int countDataListTopics = dataList.Count();
            List<string> listOfTopics = new List<string>();

            for (int i = 1; i <= countDataListTopics; i++)
            {
                var topics = driver.FindElement(By.XPath($"/html/body/footer/nav[1]/div/div[2]/div/div[1]/div[1]/ul/li[{i}]")).Text;
                Console.WriteLine(topics);
                listOfTopics.Add(topics);


                if (topics == "بیانات")
                {
                    var selectTopics = driver.FindElement(By.XPath($"/html/body/footer/nav[1]/div/div[2]/div/div[1]/div[1]/ul/li[{i}]"));
                    Thread.Sleep(1000);
                    selectTopics.Click();
                }
            }

            var years = driver.FindElements(By.XPath("/html/body/main/div[1]/section[1]/main/div[2]/ul/li"));
            var month = driver.FindElements(By.XPath("/html/body/main/div[1]/section[1]/main/div[3]/ul/li"));

            for (int i = 2; i <= 2; i++)
            {
                var selectedYear = driver.FindElement(By.XPath($"/html/body/main/div[1]/section[1]/main/div[2]/ul/li[{i}]"));
                Thread.Sleep(1000);
                selectedYear.Click();

                for (int j = 1; j <= month.Count; j++)
                {
                    Thread.Sleep(1000);
                    var selectedMonth = driver.FindElement(By.XPath($"/html/body/main/div[1]/section[1]/main/div[3]/ul/li[{j}]"));
                    Thread.Sleep(1000);
                    selectedMonth.Click();
                    Thread.Sleep(1000);

                    var statements = driver.FindElements(By.XPath("/html/body/main/div[1]/section[2]/main/ul"));
                    Thread.Sleep(1000);

                    if (statements.Count == 0)
                    {
                        driver.Navigate().Refresh();
                    }

                    for (int k = 1; k <= statements.Count; k++)
                    {
                        Thread.Sleep(1000);

                        var selectedStatements = driver.FindElement(By.XPath($"/html/body/main/div[1]/section[2]/main/ul[{k}]/li/div[2]/h6/a[2]"));
                        selectedStatements.Click();


                        var newsTime = driver.FindElement(By.XPath("/html/body/main/div[1]/section/main/article[1]/div[1]/time/h6")).Text;
                        var newsTopic = driver.FindElement(By.XPath("/html/body/main/div[1]/section/main/article[1]/div[2]/h3")).Text;
                        var newsText = driver.FindElement(By.XPath("/html/body/main/div[1]/section/main/article[2]/div[2]")).Text;
                        newsTime = new string(newsTime.Where(c => !char.IsPunctuation(c)).ToArray());

                        string statementsDetails = $"{newsTopic}\n\n + {newsText}";
                        Thread.Sleep(1000);
                        //File.WriteAllText($@"C:\Users\erfan\source\repos\LeaderProject\LeaderProject\{newsTime}.txt", statementsDetails);

                        try
                        {
                            SQLiteConnection con = new SQLiteConnection("Data Source = dataTable.db");
                            con.Open();
                            SQLiteCommand cmd = new SQLiteCommand();
                            cmd.CommandText = "insert into leader(statements) VALUES(@statements)";
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@statements", statementsDetails);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }


                        try
                        {
                            driver.Navigate().Back();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }


                        var selectedDefultYears = driver.FindElement(By.XPath($"/html/body/main/div[1]/section[1]/main/div[2]/ul/li[{i}]"));
                        Thread.Sleep(500);

                        selectedDefultYears.Click();


                        Thread.Sleep(500);
                        var selectedDefultMonth = driver.FindElement(By.XPath($"/html/body/main/div[1]/section[1]/main/div[3]/ul/li[{j}]"));

                        selectedDefultMonth.Click();
                        Thread.Sleep(500);
                    }
                }
            }
            //driver.Quit();
        }
    }
}
