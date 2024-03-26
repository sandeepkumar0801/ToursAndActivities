using EmailSuitConsole.Models;
using Logger.Contract;
using System.Diagnostics;
using System.Text;
using constant = EmailSuitConsole.Constant.Constants;

namespace EmailSuitConsole.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig emailConfig;
        private readonly ILogger _log;

        public EmailService(EmailConfig emailConfig, ILogger log)
        {
            this.emailConfig = emailConfig;
            _log = log;
        }
        public async Task SendtiqetStatusEmail(UserEmailOptions userEmailOptions)
        {
            var watchSendEmail = Stopwatch.StartNew();

            try
            {
                StringBuilder productsTable_offline = new StringBuilder();
                StringBuilder producttable_active = new StringBuilder();

                List<Product> products = emailConfig.GetProductData(constant.CheckTiqetStatus);
                    foreach (var product in products)
                    {
                        if (product.StatusType == constant.OfflineStatus)
                        {
                            productsTable_offline.Append("<tr><td>" + product.SERVICEID + "</td><td>" + product.Servicename + "</td><td>" + product.RegionName + "</td><td>" + constant.OFFLINE);
                        }
                        else
                        {
                            producttable_active.Append("<tr><td>" + product.SERVICEID + "</td><td>" + product.Servicename + "</td><td>" + product.RegionName + "</td><td>" + constant.Active);

                        }
                    }
                    string emailSubject = constant.EmailSubject;
                    string emailBodyTemplate = constant.emailBodyTemplate;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable_offline, producttable_active);
                watchSendEmail.Stop();
                _log.WriteTimer(constant.SendtiqetStatusEmail, constant.SendtiqetStatusEmail, "Tiqets", watchSendEmail.Elapsed.ToString());

            }
            catch (Exception ex)
            {
                watchSendEmail.Stop();
                _log.WriteTimer(constant.SendtiqetStatusEmail, ex.Message, "Tiqets", watchSendEmail.Elapsed.ToString());
                throw new InvalidOperationException(constant.TiqetError, ex);
            }
        }
        public async Task SendPrioStatusEmail(UserEmailOptions userEmailOptions)
        { 
            try
            {
                StringBuilder productsTable_offline = new StringBuilder();
                StringBuilder producttable_active = new StringBuilder();

                List<Product> products = emailConfig.GetProductData(constant.CheckPrioStatus);
                foreach (var product in products)
                {
                    if (product.StatusType == constant.OfflineStatus)
                    {
                        productsTable_offline.Append("<tr><td>" + product.SERVICEID + "</td><td>" + product.Servicename + "</td><td>" + product.RegionName + "</td><td>" + constant.OFFLINE);
                    }
                    else
                    {
                        producttable_active.Append("<tr><td>" + product.SERVICEID + "</td><td>" + product.Servicename + "</td><td>" + product.RegionName + "</td><td>" + constant.Active);

                    }
                }
                    string emailSubject = constant.PrioEmailSubject;
                    string emailBodyTemplate = constant.emailBodyTemplate;
                    await emailConfig.SendEmail(userEmailOptions,  emailSubject, emailBodyTemplate, productsTable_offline, producttable_active);
                
             }
            catch(Exception ex)
            {
                throw new InvalidOperationException(constant.PrioError, ex);
            }
        }
        public async Task SendTiqetChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<TiqetsProduct> products = emailConfig.GetProductData1(constant.CheckTiqetTempStatus);
                foreach (var product in products)
                {
                    productsTable.Append("<tr><td>" + product.serviceid + "</td><td>" + product.SERVICELONGNAME + "</td><td>" + product.CityName + "</td><td>" + product.productid + "</td><td>" + product.OldStatus + "</td><td>" + product.NewStatus + "</td><td>" + product.Sale_Status_Reason + "</td><td>" + product.Sale_Status_Expected_Reopen);
                }
                string emailSubject = "Tiqet Temporarily Unavailable Status Change";
                string emailBodyTemplate = constant.TiqetTemplate;
                await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Encountered SendTiqetChange error ", ex);
            }
        }
        public async Task SendVariantChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<Variant> products = emailConfig.GetProductData_Variant(constant.CheckTiqetTempVariantStatus);
                if (products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>" + product.SERVICEID + "</td><td>" + product.productid + "</td><td>" + product.variantid + "</td><td>" + product.OldLabel + "</td><td>" + product.NewLabel + "</td><td>");
                    }
                    string emailSubject = "Tiqet Variant Status Change";
                    string emailBodyTemplate = "TiqetVariant";
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
                else
                {
                    Console.WriteLine("No Variant Recorded today");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendVariantChange error ", ex);
            }
        }

        public async Task SendTourCMSChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<TourCMSProduct> products = emailConfig.GetProductDetai1TourCMS(constant.CheckTourCMSTempStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.serviceid + "</td><td>"
                            + product.serviceoptioninserviceid + "</td><td>"
                            + product.SERVICELONGNAME + "</td><td>"
                            + product.AccountId + "</td><td>"
                            + product.ChannelId + "</td><td>"
                            + product.tourid + "</td><td>"
                            + product.OldSaleStatus + "</td><td>"
                            + product.NewSaleStatus + "</td><td>"
                            + product.ProductStatus + "</td><td></tr>"
                            );
                    }

                    string emailSubject = "TourCMS Temporarily Unavailable Status Change";
                    string emailBodyTemplate = constant.TourCMSTemplate;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendTourCMSChange error ", ex);
            }
        }
        public async Task SendRaynaChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<RaynaProduct> products = emailConfig.GetProductDetai1Rayna(constant.CheckRaynaTempStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.serviceid + "</td><td>"
                            + product.serviceoptioninserviceid + "</td><td>"
                            + product.SERVICELONGNAME + "</td><td>"
                            + product.tourid + "</td><td>"
                            + product.ProductStatus + "</td><td></tr>"
                           );
                    }
                    string emailSubject = "Rayna Temporarily Unavailable Status Change";
                    string emailBodyTemplate = constant.RaynaTemplate;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendRaynaChange error ", ex);
            }
        }
        public async Task SendGlobalTixV3Change(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<GlobalTixV3Product> products = emailConfig.GetProductDetai1GlobalTixV3(constant.CheckGlobaTixV3TempStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.serviceid + "</td><td>"
                            + product.SERVICELONGNAME + "</td><td>"
                            + product.Product_id + "</td><td>"
                            + product.ProductStatus + "</td><td></tr>"
                           );
                    }
                    string emailSubject = "GlobalTixV3 Temporarily Unavailable Status Change";
                    string emailBodyTemplate = constant.GlobalTixV3Template;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendGlobalTixV3 error ", ex);
            }
        }

        public async Task SendTourCMSLabelChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<TourCMSPax> products = emailConfig.GetProductData_TourCMSPax(constant.CheckTourCMSTempPaxStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.SERVICEID + "</td><td>" 
                            + product.serviceoptioninserviceid + "</td><td>"
                            + product.AccountId + "</td><td>"
                            + product.ChannelId + "</td><td>"
                            + product.TourId + "</td><td>"
                            + product.rate_id + "</td><td>" 
                            + product.OldLabel + "</td><td>"
                            + product.NewLabel + "</td><td></tr>");
                    }
                    string emailSubject = "TourCMS Pax Status Change";
                    string emailBodyTemplate = "TourCMSLabel";
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
              
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendTourCMSChange error ", ex);
            }
        }

        public async Task SendRaynaOptionsChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<RaynaProductOptions> products = emailConfig.GetProductDetai1RaynaOptions(constant.CheckRaynaOptionsTempStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.serviceid + "</td><td>"
                            + product.serviceoptioninserviceid + "</td><td>"
                            + product.SERVICELONGNAME + "</td><td>"
                            + product.tourid + "</td><td>"
                            + product.touroptionid + "</td><td>"
                            + product.ProductStatus + "</td><td></tr>"
                           );
                    }
                    string emailSubject = "RaynaOptions Temporarily Unavailable Status Change";
                    string emailBodyTemplate = constant.RaynaOptionsTemplate;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendRaynaChangeOptions error ", ex);
            }
        }
        public async Task SendGlobalTixV3OptionsChange(UserEmailOptions userEmailOptions)
        {
            try
            {
                StringBuilder productsTable = new StringBuilder();

                List<GlobalTixV3ProductOptions> products = emailConfig.GetProductDetai1GlobalTixV3Options(constant.CheckGlobaTixV3OptionsTempStatus);
                if (products != null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        productsTable.Append("<tr><td>"
                            + product.serviceid + "</td><td>"
                            + product.SERVICELONGNAME + "</td><td>"
                            + product.Product_id + "</td><td>"
                            + product.Option_Id + "</td><td>"
                            + product.ProductStatus + "</td><td></tr>"
                           );
                    }
                    string emailSubject = "GlobalTixV3Options Temporarily Unavailable Status Change";
                    string emailBodyTemplate = constant.GlobalTixV3OptionsTemplate;
                    await emailConfig.SendEmail(userEmailOptions, emailSubject, emailBodyTemplate, productsTable);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encountered SendGlobalTixV3Options error ", ex);
            }
        }

    }
}

