// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using Microsoft.Extensions.Configuration;
using Moq;
using TestTasks.SeCompany.Models;
using TestTasks.SeCompany.Services;

namespace TestTasks.SeCompany.Tests;

public class PhotographyCostCalculatorTests
{
    private static PhotographyCostParameters? _photoCostParams;
    private static PhotographyCostParameters PhotoCostParamsStub
    {
        get
        {
            // Just saving my time.
            if (_photoCostParams != null)
                return _photoCostParams;

            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, false)
            .Build();
            _photoCostParams = new PhotographyCostParametersRepository(configuration).Get();
            return _photoCostParams;
        }
    }

    public static IEnumerable<object[]> SingleBooking =>
        new List<object[]>
        {
            new object[] { new Booking(JobType.Video, DateTime.Parse("2023/06/21 13:00"), TimeSpan.FromHours(1)), 2000 }, // Holiday
            new object[] { new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 07:00"), TimeSpan.FromHours(2)), 0 },
            new object[] { new Booking(JobType.Photo, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), 2500 },
            new object[] { new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 15:00"), TimeSpan.FromHours(2)), 0 },
            new object[] { new Booking(JobType.Drone, DateTime.Parse("2023/06/15 06:00"), TimeSpan.FromHours(2)), 2500 }
        };

    public static IEnumerable<object[]> ManyBookings =>
        new List<object[]>
        {
            new object[]
            {
                new[]
                {
                    // 1300*0.25 + 1200*1 + 1100*05 + 1000*0.75 = 325 + 1200 + 550 + 750 = 1525 + 1300 = 2825
                    new Booking(JobType.Photo, DateTime.Parse("2023/06/15 06:45"), TimeSpan.FromHours(2.5)),
                    new Booking(JobType.VR, DateTime.Parse("2023/06/15 17:30"), TimeSpan.FromHours(0.5)), // 1100*0.5  = 550* 1.1 = 605
                    new Booking(JobType.Drone, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(0.75)), // 1200*0.75 = 900
                },
                4330
            },
            new object[]
            { 
                new[]
                {
                    new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 12:00"), TimeSpan.FromHours(2)), // 0
                    new Booking(JobType.Photo, DateTime.Parse("2023/06/15 16:00"), TimeSpan.FromHours(1)), // 1000*1.1 = 1100
                    new Booking(JobType.Video, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 1100*1.1 = 1210
                    new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 0
                    new Booking(JobType.Drone, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), // 2500
                },
                4810
            },
            new object[]
            { 
                new[]
                {
                    new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 05:00"), TimeSpan.FromHours(2)), // 0
                    new Booking(JobType.Photo, DateTime.Parse("2023/06/15 16:00"), TimeSpan.FromHours(1)), // 1000*1.1 = 1100
                    new Booking(JobType.Video, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 1100*1.1 = 1210
                    new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 0
                    new Booking(JobType.Drone, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), // 2500
                    new Booking(JobType.VR, DateTime.Parse("2023/06/15 21:00"), TimeSpan.FromHours(1)) // 1500
                },
                5000
            }
        };
    
    [Theory]
    [MemberData(nameof(SingleBooking))]
    public void GetCost_Should_Return_ExpectedCost(Booking booking, double expectedCost)
    {
        // Arrange
        var costParamsMock = new Mock<IPhotographyCostParametersRepository>();
        costParamsMock.Setup(mock => mock.Get()).Returns(PhotoCostParamsStub);
        var calculator = new PhotographyCostCalculator(costParamsMock.Object);

        // Act
        double result = calculator.GetCost(booking);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(expectedCost, result);
    }

    [Theory]
    [MemberData(nameof(ManyBookings))]
    public void GetCost_OfMany_Should_Return_ExpectedCost(Booking[] bookings, double expectedCost)
    {
        // Arrange
        var costParamsMock = new Mock<IPhotographyCostParametersRepository>();
        costParamsMock.Setup(mock => mock.Get()).Returns(PhotoCostParamsStub);
        var calculator = new PhotographyCostCalculator(costParamsMock.Object);

        // Act
        var result = calculator.GetCost(bookings);

        // Assert
        Assert.NotEmpty(bookings);
        Assert.Equal(expectedCost, result);
    }
}