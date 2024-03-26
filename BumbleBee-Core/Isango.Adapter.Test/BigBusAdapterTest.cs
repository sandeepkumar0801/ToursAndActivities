using Autofac;
using Isango.Entities;
using Isango.Entities.BigBus;
using Isango.Entities.Enums;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.BigBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class BigBusAdapterTest : BaseTest
    {
        private IBigBusAdapter _bigBusAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _bigBusAdapter = scope.Resolve<IBigBusAdapter>();
            }
        }

        [Test]
        public void CreateBookingTest()
        {
            var selectedProducts = new List<SelectedProduct>
            {
                new BigBusSelectedProduct
                {
                    ProductId = 84572,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            IsSelected = true,
                            TravelInfo = new TravelInfo
                            {
                                StartDate=new DateTime(2019,09,30),
                                NoOfPassengers = new Dictionary<PassengerType, int>
                                {
                                    {PassengerType.Adult, 1},
                                    {PassengerType.Child, 1}
                                }
                            }
                        }
                    }
                }
            };

            var token = Guid.NewGuid().ToString();
            var result = _bigBusAdapter.CreateBooking(selectedProducts, token, out _, out _);
            Assert.AreEqual("BOOKED", result.FirstOrDefault()?.Status);
        }

        [Test]
        public void CancelBookingTest()
        {
            var selectedProducts = new List<SelectedProduct>
            {
                new BigBusSelectedProduct
                {
                    ProductId = 84572,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            IsSelected = true,
                            TravelInfo = new TravelInfo
                            {
                                StartDate=new DateTime(2019,09,30),
                                NoOfPassengers = new Dictionary<PassengerType, int>
                                {
                                    {PassengerType.Adult, 1},
                                    {PassengerType.Child, 1}
                                }
                            }
                        }
                    },
                    BookingReference = "faf3cf56-5afd-4592-afe5-97c0dced85ac"
                }
            };

            var token = Guid.NewGuid().ToString();
            var result = _bigBusAdapter.CancelBooking(selectedProducts, token, out _, out _);
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateReservationTest()
        {
            var selectedProducts = new List<SelectedProduct>
            {
                new BigBusSelectedProduct
                {
                    ProductId = 84572,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            IsSelected = true,
                            TravelInfo = new TravelInfo
                            {
                                StartDate=new DateTime(2019,09,30),
                                NoOfPassengers = new Dictionary<PassengerType, int>
                                {
                                    {PassengerType.Adult, 1},
                                    {PassengerType.Child, 1}
                                }
                            }
                        }
                    }
                }
            };

            var token = Guid.NewGuid().ToString();
            var result = _bigBusAdapter.CreateReservation(selectedProducts, token, out _, out _);
            Assert.AreEqual("RESERVED", ((BigBusSelectedProduct)result.FirstOrDefault())?.BookingStatus);
        }

        [Test]
        public void CancelReservationTest()
        {
            var selectedProducts = new List<SelectedProduct>
            {
                new BigBusSelectedProduct
                {
                    ProductId = 84572,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            IsSelected = true,
                            TravelInfo = new TravelInfo
                            {
                                StartDate=new DateTime(2019,09,30),
                                NoOfPassengers = new Dictionary<PassengerType, int>
                                {
                                    {PassengerType.Adult, 1},
                                    {PassengerType.Child, 1}
                                }
                            }
                        }
                    },
                    ReservationReference = "84fa9470-e160-4370-9ba0-f5938de17732"
                }
            };

            var token = Guid.NewGuid().ToString();
            var result = _bigBusAdapter.CancelReservation(selectedProducts, token, out _, out _);
            Assert.IsNotNull(result);
        }
    }
}