using Isango.Entities;
using Isango.Entities.ConsoleApplication.RouteMap.Prio;
using Isango.Persistence.Contract;
using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class PrioPersistence : PersistenceBase, IPrioPersistence
    {
        private readonly ILogger _log;
        public PrioPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Sync data between databases
        /// </summary>
        public void SyncDataBetweenDataBases()
        {
            try
            {
                using (var ageGroupCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SyncPrioTicketMappingSp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(ageGroupCommand);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SyncDataBetweenDataBases",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all Prio Age Groups
        /// </summary>
        /// <param name="masterAgeGroups"></param>
        public void SaveAllAgeGroups(List<Entities.ConsoleApplication.AgeGroup.Prio.AgeGroup> masterAgeGroups)
        {
            try
            {
                if (masterAgeGroups?.Count > 0)
                {
                    var jsonResult = SerializeDeSerializeHelper.Serialize(masterAgeGroups);
                    using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertPrioTicketAgeGroupSP))
                    {
                        APIUploadDb.AddInParameter(command, Constant.PrioAgeGroupxml, DbType.String, jsonResult);
                        command.CommandType = CommandType.StoredProcedure;
                        APIUploadDb.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SaveAllAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(masterAgeGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SavePrioRouteMaps(List<RouteMap> prioRouteMaps)
        {
            try
            {
                if (prioRouteMaps?.Count > 0)
                {
                    var routeMapTable = SetPrioRouteMap(prioRouteMaps);
                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.API_Upload)))
                    {
                        SqlCommand insertCommand = new SqlCommand(Constant.InsertPrioTicketRouteMap, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter tvpParam = insertCommand.Parameters.AddWithValue(Constant.PrioTicketRouteMap, routeMapTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        try
                        {
                            connection.Open();
                            insertCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                        }
                        finally
                        {
                            connection?.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SavePrioRouteMaps",
                    Params = $"{SerializeDeSerializeHelper.Serialize(prioRouteMaps)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private DataTable SetPrioRouteMap(List<RouteMap> prioRouteMaps)
        {
            var routeMapTable = new DataTable { TableName = Constant.PrioRouteMapTableType };
            try
            {
                foreach (var property in prioRouteMaps[0].GetType().GetProperties())
                {
                    routeMapTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }
                foreach (var serviceDetail in prioRouteMaps)
                {
                    var newRow = routeMapTable.NewRow();
                    foreach (var property in serviceDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = serviceDetail.GetType().GetProperty(property.Name)?.GetValue(serviceDetail, null);
                    }

                    routeMapTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SetPrioRouteMap",
                    Params = $"{SerializeDeSerializeHelper.Serialize(prioRouteMaps)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return routeMapTable;
        }

        private DataTable SetPrioList(TicketListRs prioTicketListMaps)
        {
            var ticketListMapTable = new DataTable { TableName = Constant.PrioTicketListMapTableType };
            try
            {
                foreach (var property in prioTicketListMaps?.Data?.Tickets[0].GetType().GetProperties())
                {
                    if (property.Name.ToString().ToLowerInvariant() == "ticketid")
                    {
                        ticketListMapTable.Columns.Add(new DataColumn("ticket_id", typeof(string)));
                    }
                    else if (property.Name.ToString().ToLowerInvariant() == "tickettitle")
                    {
                        ticketListMapTable.Columns.Add(new DataColumn("ticket_title", typeof(string)));
                    }
                    else if (property.Name.ToString().ToLowerInvariant() == "venueid")
                    {
                        ticketListMapTable.Columns.Add(new DataColumn("venue_id", typeof(string)));
                    }
                    else if (property.Name.ToString().ToLowerInvariant() == "venuename")
                    {
                        ticketListMapTable.Columns.Add(new DataColumn("venue_name", typeof(string)));
                    }
                    else if (property.Name.ToString().ToLowerInvariant() == "locations")
                    {
                        ticketListMapTable.Columns.Add(new DataColumn("location_name", typeof(string)));
                        ticketListMapTable.Columns.Add(new DataColumn("country_name", typeof(string)));
                    }
                    else
                    {
                        ticketListMapTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                    }
                }
                foreach (var prioTicketData in prioTicketListMaps?.Data?.Tickets)
                {
                    var newRow = ticketListMapTable.NewRow();
                    foreach (var property in prioTicketData.GetType().GetProperties())
                    {
                        if (property.Name.ToString().ToLowerInvariant() == "ticketid")
                        {
                            newRow["ticket_id"] = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                        }
                        else if (property.Name.ToString().ToLowerInvariant() == "tickettitle")
                        {
                            newRow["ticket_title"] = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                        }
                        else if (property.Name.ToString().ToLowerInvariant() == "venueid")
                        {
                            newRow["venue_id"] = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                        }
                        else if (property.Name.ToString().ToLowerInvariant() == "venuename")
                        {
                            newRow["venue_name"] = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                        }
                        else if (property.Name.ToString().ToLowerInvariant() == "locations")
                        {
                            var list = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                            foreach (var item in (List<Location>)list)
                            {
                                newRow["location_name"] = item.LocationName;
                                newRow["country_name"] = item.CountryName;
                            }
                        }
                        else
                        {
                            newRow[property.Name] = prioTicketData.GetType().GetProperty(property.Name)?.GetValue(prioTicketData, null);
                        }
                    }

                    ticketListMapTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SetPrioRouteMap",
                    Params = $"{SerializeDeSerializeHelper.Serialize(prioTicketListMaps)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return ticketListMapTable;
        }


        public void SavePrioProductDetails(List<Entities.ConsoleApplication.AgeGroup.Prio.ProductDetails> prioProductDetails)
        {
            if (prioProductDetails?.Count > 0)
            {
                var productDetailsTable = SetPrioProductDetail(prioProductDetails);
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.API_Upload)))
                {
                    SqlCommand insertCommand = new SqlCommand(Constant.InsertPrioTicketDetails, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlParameter tvpParam = insertCommand.Parameters.AddWithValue(Constant.PrioProductDetailTableType, productDetailsTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        var abc = 0;
                        throw ex;
                    }
                    finally
                    {
                        connection?.Close();
                    }
                }
            }
        }

        public void SavePrioTicketList(TicketListRs prioTicketList)
        {
            try
            {
                if (prioTicketList?.Data?.Tickets?.Count > 0)
                {
                    var ticketListTable = SetPrioList(prioTicketList);
                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.API_Upload)))
                    {
                        SqlCommand insertCommand = new SqlCommand(Constant.InsertPrioTicketList, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter tvpParam = insertCommand.Parameters.AddWithValue(Constant.PrioTicketListParameter, ticketListTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        try
                        {
                            connection.Open();
                            insertCommand.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                        }
                        finally
                        {
                            connection?.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioPersistence",
                    MethodName = "SavePrioTicketList",
                    Params = $"{SerializeDeSerializeHelper.Serialize(prioTicketList)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private DataTable SetPrioProductDetail(List<Entities.ConsoleApplication.AgeGroup.Prio.ProductDetails> prioProductDetails)
        {
            var productDetailTable = new DataTable { TableName = Constant.PrioProductDetailTableType };

            foreach (var property in prioProductDetails[0].GetType().GetProperties())
            {
                productDetailTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }
            foreach (var productDetail in prioProductDetails)
            {
                var newRow = productDetailTable.NewRow();
                foreach (var property in productDetail.GetType().GetProperties())
                {
                    newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)?.GetValue(productDetail, null);
                }

                productDetailTable.Rows.Add(newRow);
            }
            return productDetailTable;
        }
    }
}