using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using nmpsc = System.Configuration;

namespace RpmProgrammingExercise.Models
{
    class GasPriceDAL
    {
        string ConnectionString = nmpsc.ConfigurationManager.AppSettings["connectionString"];
        public List<GasPrice> GetGasPricesFromAPI(int daysCount)
        {
            List<GasPrice> gasPrices = new List<GasPrice>();
            DateTime today = DateTime.Today;
            var gasPriceWebRequest = WebRequest.Create("http://api.eia.gov/series/?api_key=ec92aacd6947350dcb894062a4ad2d08&series_id=PET.EMD_EPD2D_PTE_NUS_DPG.W") as HttpWebRequest;
            gasPriceWebRequest.ContentType = "application/json";
            try
            {
                using (var s = gasPriceWebRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var pricesAsJson = sr.ReadToEnd();
                        var prices = JsonConvert.DeserializeObject<IncomingData>(pricesAsJson);
                        foreach (var priceList in prices.series[0].data)
                        {
                            string recordDate = (string)priceList[0];
                            DateTime recordDateDateTime = DateTime.ParseExact(recordDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                            //check if record is within the time range specified
                            if ((today - recordDateDateTime).TotalDays > daysCount)
                            {
                                break;
                            }
                            double price = 0;
                            //The API can return both an int or a double for the price, check here to remove chance for error
                            if (priceList[1] is Int64)
                            {
                                price = Convert.ToDouble((Int64)priceList[1]);
                            }
                            else if (priceList[1] is Double)
                            {
                                price = (double)priceList[1];
                            }
                            gasPrices.Add(new GasPrice() { recordDate = recordDate, price = price });
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error reading Json");
                return new List<GasPrice>(); //return empty list if there is any error reading the Json
            }
               
            return gasPrices;
        }

        public void InsertGasPrice(GasPrice gasPrice)
        {
            //sql statement to insert only if record does not already exist
            string sql = "IF NOT EXISTS(SELECT 1 FROM GasPrices WHERE RecordDate = @RecordDate) " +
                "INSERT INTO GasPrices VALUES(@RecordDate, @Price)";
            using (SqlConnection cnn = new SqlConnection(
                ConnectionString))
            {
                try
                {
                    cnn.Open();
                    using (SqlCommand command = new SqlCommand(sql, cnn))
                    {
                        command.Parameters.Add(new SqlParameter("RecordDate", gasPrice.recordDate));
                        command.Parameters.Add(new SqlParameter("Price", gasPrice.price));
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Error inserting record into SQL");
                }
            }
        }
    }
}
