using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Config;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public abstract class CommandHandlerBase
    {
        #region "Properties"

        public MethodType Handles { get; set; }

        private readonly ILogger _log;

        #endregion "Properties"

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
        }

        public virtual object Execute(InputContext inputContext, string token, out string request, out string responseXml)
        {
            var watch = Stopwatch.StartNew();
            var inputXml = CreateInputRequest(inputContext);
            var xmlResults = GetXmlResults(inputXml);
            watch.Stop();
            request = inputXml.ToString();
            responseXml = xmlResults.ToString();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "HotelBeds", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputXml), xmlResults.ToString(), inputContext.MethodType.ToString(), token, "HotelBeds");
            if (Validate(xmlResults))
            {
                return GetResults(xmlResults);
            }
            return null;
        }

        public virtual object Execute(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var inputXml = CreateInputRequest(inputContext);
            var xmlResults = GetXmlResults(inputXml);
            watch.Stop();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "HotelBeds", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputXml), xmlResults?.ToString(), inputContext.MethodType.ToString(), token, "HotelBeds");
            if (Validate(xmlResults))
            {
                return GetResults(xmlResults);
            }
            return null;
        }

        public virtual async Task<object> ExecuteAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var inputXml = CreateInputRequest(inputContext);
            var xmlResults = await GetXmlResultsAsync(inputXml);
            watch.Stop();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "HotelBeds", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputXml), xmlResults?.ToString(), inputContext.MethodType.ToString(), token, "HotelBeds");
            if (Validate(xmlResults))
            {
                return GetResults(xmlResults);
            }
            return null;
        }

        public virtual object ExecuteWithoutResponse(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var inputXml = CreateInputRequest(inputContext);
            var xmlResults = GetXmlResults(inputXml);
            watch.Stop();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "HotelBeds", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputXml), xmlResults.ToString(), inputContext.MethodType.ToString(), token, "HotelBeds");
            return Validate(xmlResults);
        }

        protected abstract Task<object> GetXmlResultsAsync(object inputXml);

        public virtual async Task<object> ExecuteWithoutResponseAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var inputXml = CreateInputRequest(inputContext);
            var xmlResults = await GetXmlResultsAsync(inputXml);
            watch.Stop();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "HotelBeds", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputXml), xmlResults?.ToString(), inputContext.MethodType.ToString(), token, "HotelBeds");
            return Validate(xmlResults);
        }

        #region "Protected Virtual Functions"

        protected abstract object GetXmlResults(object inputXml);

        public abstract object GetResults(object xmlResults);

        protected virtual bool Validate(object xmlResults)
        {
            return xmlResults.ToString().IndexOf(XmlNodes.ErrorList, StringComparison.Ordinal) == -1;

            //var parser = XmlOperations.GetXmlParser();
            //parser.ResetParser();

            //parser.ParseXml(xmlResults.ToString());

            //return XmlOperations.GetElement(XmlNodes.ErrorList, 0, Parser) == null;
        }

        protected abstract object CreateInputRequest(InputContext inputContext);

        #endregion "Protected Virtual Functions"

        #region "Protected Virtual Properties"

        protected XmlParser MParser;

        protected virtual XmlParser Parser
        {
            get
            {
                if (MParser != null)
                    return MParser;

                MParser = XmlOperations.GetXmlParser();
                return MParser;
            }
        }

        #endregion "Protected Virtual Properties"

        public virtual string GetXMLForCustomers(List<Isango.Entities.Customer> customers)
        {
            var customerXml = new StringBuilder();
            customers = customers.OrderByDescending(x => x.IsLeadCustomer)
                .ThenBy(x => x.CustomerId)
                .ThenBy(x => x.PassengerType)
                .ToList();
            foreach (var customer in customers)
            {
                var age = !string.IsNullOrWhiteSpace(customer.Age.ToString()) ? customer.Age.ToString() : "30";
                var customerType = customer.PassengerType.Equals(PassengerType.Adult) ? Constant.Ad : Constant.Ch;
                customerXml.Append($"<Customer type=\"{customerType}\">");
                customerXml.Append($"{Constant.AgeStart}{age}{Constant.AgeEnd}");
                customerXml.Append($"{Constant.NameStart}{customer.FirstName}{Constant.NameEnd}");
                customerXml.Append($"{Constant.LastNameStart}{customer.LastName}{Constant.LastNameEnd}");
                customerXml.Append(Constant.CustomerTypeEnd);
            }
            return customerXml.ToString();
        }
    }
}