using Microsoft.Extensions.Configuration;
using EmailSuitConsole.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace EmailSuitConsole.Data
{
    public class IsangoDataBaseLive
    {
        private readonly string _connectionString;

        public IsangoDataBaseLive(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("IsangoLiveEntities");
        }
        public List<Product> ExecuteReader(string storedProcedureName = "")
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.SERVICEID = (int)reader["SERVICEID"];
                            product.Servicename = reader["Servicename"].ToString();
                            product.RegionName = reader["RegionName"].ToString();
                            product.StatusType = reader["StatusType"].ToString();
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }
        public List<TiqetsProduct> ExecuteReader_Get(string storedProcedureName = "")
        {
            List<TiqetsProduct> tiqproducts = new List<TiqetsProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TiqetsProduct tiqproduct = new TiqetsProduct();
                            tiqproduct.serviceid = (int)reader["SERVICEID"];
                            tiqproduct.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();
                            tiqproduct.CityName = reader["CityName"].ToString();

                            tiqproduct.productid = reader["productid"].ToString();
                            tiqproduct.NewStatus = reader["NewStatus"].ToString();
                            tiqproduct.OldStatus = reader["OldStatus"].ToString();
                            tiqproduct.Sale_Status_Expected_Reopen = reader["Sale_Status_Expected_Reopen"].ToString();
                            tiqproduct.Sale_Status_Reason = reader["Sale_Status_Reason"].ToString();


                            tiqproducts.Add(tiqproduct);
                        }
                    }
                }
            }

            return tiqproducts;
        }

        public List<TourCMSProduct> ExecuteReader_GetTourCMS(string storedProcedureName = "")
        {
            var tourCMSproducts = new List<TourCMSProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tourCMSproduct = new TourCMSProduct();
                            tourCMSproduct.serviceid = (int)reader["serviceid"];
                            tourCMSproduct.serviceoptioninserviceid = (int)reader["serviceoptioninserviceid"];

                            tourCMSproduct.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();

                            tourCMSproduct.AccountId = (int)reader["AccountId"];
                            tourCMSproduct.ChannelId = (int)reader["ChannelId"];
                            tourCMSproduct.tourid = (int)reader["tourid"];


                            tourCMSproduct.NewSaleStatus = reader["NewSaleStatus"].ToString();
                            tourCMSproduct.OldSaleStatus = reader["OldSaleStatus"].ToString();
                           
                            tourCMSproduct.ProductStatus = reader["ProductStatus"].ToString();


                            tourCMSproducts.Add(tourCMSproduct);
                        }
                    }
                }
            }

            return tourCMSproducts;
        }

        public List<RaynaProduct> ExecuteReader_GetRayna(string storedProcedureName = "")
        {
            var raynaProducts = new List<RaynaProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var raynaProduct = new RaynaProduct();
                            raynaProduct.serviceid = (int)reader["serviceid"];
                            raynaProduct.serviceoptioninserviceid = (int)reader["serviceoptioninserviceid"];
                            raynaProduct.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();
                            raynaProduct.tourid = (int)reader["tourid"];
                            raynaProduct.ProductStatus = reader["ProductStatus"].ToString();
                            raynaProducts.Add(raynaProduct);
                        }
                    }
                }
            }

            return raynaProducts;
        }


        public List<GlobalTixV3Product> ExecuteReader_GetGlobalTixV3(string storedProcedureName = "")
        {
            var globalTixV3Products = new List<GlobalTixV3Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var globalTixV3Product = new GlobalTixV3Product();
                            globalTixV3Product.serviceid = (int)reader["serviceid"];
                            globalTixV3Product.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();
                            globalTixV3Product.Product_id = (int)reader["Product_id"];
                            globalTixV3Product.ProductStatus = reader["ProductStatus"].ToString();
                            globalTixV3Products.Add(globalTixV3Product);
                        }
                    }
                }
            }

            return globalTixV3Products;
        }




        public List<Variant> ExecuteReader_VariantData(string storedProcedureName = "")
        {
            List<Variant> products = new List<Variant>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Variant product = new Variant();
                            product.SERVICEID = (int)reader["SERVICEID"];
                            product.variantid = (int)reader["variantid"];
                            product.productid = reader["productid"].ToString();
                            product.NewLabel = reader["NewLabel"].ToString();
                            product.OldLabel = reader["OldLabel"].ToString();
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public List<TourCMSPax> ExecuteReader_TourCMSPaxData(string storedProcedureName = "")
        {
            List<TourCMSPax> products = new List<TourCMSPax>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new TourCMSPax();
                            product.SERVICEID = (int)reader["SERVICEID"];
                            product.serviceoptioninserviceid = (int)reader["serviceoptioninserviceid"];
                            product.TourId = (int)reader["TourId"];
                            product.AccountId = (int)reader["AccountId"];
                            product.ChannelId = (int)reader["ChannelId"];
                            product.rate_id = reader["rate_id"].ToString();
                            product.NewLabel = reader["NewLabel"].ToString();
                            product.OldLabel = reader["OldLabel"].ToString();
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public List<RaynaProductOptions> ExecuteReader_GetRaynaOptions(string storedProcedureName = "")
        {
            var raynaProducts = new List<RaynaProductOptions>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var raynaProduct = new RaynaProductOptions();
                            raynaProduct.serviceid = (int)reader["serviceid"];
                            raynaProduct.serviceoptioninserviceid = (int)reader["serviceoptioninserviceid"];
                            raynaProduct.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();
                            raynaProduct.tourid = (int)reader["tourid"];
                            raynaProduct.touroptionid = (int)reader["touroptionid"];
                            raynaProduct.ProductStatus = reader["ProductStatus"].ToString();
                            raynaProducts.Add(raynaProduct);
                        }
                    }
                }
            }

            return raynaProducts;
        }


        public List<GlobalTixV3ProductOptions> ExecuteReader_GetGlobalTixV3Options(string storedProcedureName = "")
        {
            var globalTixV3Products = new List<GlobalTixV3ProductOptions>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var globalTixV3Product = new GlobalTixV3ProductOptions();
                            globalTixV3Product.serviceid = (int)reader["serviceid"];
                            globalTixV3Product.SERVICELONGNAME = reader["SERVICELONGNAME"].ToString();
                            globalTixV3Product.Product_id = (int)reader["Product_id"];
                            globalTixV3Product.Option_Id = (int)reader["Option_Id"];
                            globalTixV3Product.ProductStatus = reader["ProductStatus"].ToString();
                            globalTixV3Products.Add(globalTixV3Product);
                        }
                    }
                }
            }

            return globalTixV3Products;
        }
    }
}
