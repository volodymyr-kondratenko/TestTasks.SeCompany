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
            new object[] { new Booking(JobType.Video, DateTime.Parse("2023/06/15 13:00"), TimeSpan.FromHours(1)), 1000 },
            new object[] { new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 07:00"), TimeSpan.FromHours(2)), 0 },
            new object[] { new Booking(JobType.Photo, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), 1200 },
            new object[] { new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 15:00"), TimeSpan.FromHours(2)), 0 },
            new object[] { new Booking(JobType.Drone, DateTime.Parse("2023/06/15 06:00"), TimeSpan.FromHours(2)), 1300 }
        };

    public static IEnumerable<object[]> ManyBookings =>
        new List<object[]>
        {
            new object[]
            { 
                new[]
                {
                    new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 12:00"), TimeSpan.FromHours(2)), // 0
                    new Booking(JobType.Photo, DateTime.Parse("2023/06/15 16:00"), TimeSpan.FromHours(1)), // 1000*1.1 = 1100
                    new Booking(JobType.Video, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 1100*1.1 = 1210
                    new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 0
                    new Booking(JobType.Drone, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), // 1200
                },
                3510
            },
            new object[]
            { 
                new[]
                {
                    new Booking(JobType.PickUpKey, DateTime.Parse("2023/06/15 05:00"), TimeSpan.FromHours(2)), // 0
                    new Booking(JobType.Photo, DateTime.Parse("2023/06/15 16:00"), TimeSpan.FromHours(1)), // 1000*1.1 = 1100
                    new Booking(JobType.Video, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 1100*1.1 = 1210
                    new Booking(JobType.PlanMeasurement, DateTime.Parse("2023/06/15 17:00"), TimeSpan.FromHours(1)), // 0
                    new Booking(JobType.Drone, DateTime.Parse("2023/06/15 18:00"), TimeSpan.FromHours(2)), // 1200
                    new Booking(JobType.VR, DateTime.Parse("2023/06/15 21:00"), TimeSpan.FromHours(1)) // 1500
                },
                5000
            }
        };
    
    [Theory]
    [MemberData(nameof(SingleBooking))]
    public void GetCost_Should_Succeed(Booking booking, double expectedCost)
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
    public void GetCost_OfMany_Should_Succeed(Booking[] bookings, double expectedCost)
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