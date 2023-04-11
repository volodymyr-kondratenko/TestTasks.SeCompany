// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TestTasks.SeCompany.Models;
using TestTasks.SeCompany.Services;

[assembly: InternalsVisibleTo("TestTasks.SeCompany.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace TestTasks.SeCompany;

internal sealed class Program
{
    public static void Main(string[] args)
    {
        using var serviceProvider = new Startup().Configure().GetServiceProvider();
        var calculator = serviceProvider.GetRequiredService<IPhotographyCostCalculator>();
        var cost = calculator.GetCost(
            new[]
            {
                new Booking(JobType.PlanMeasurement, DateTime.Parse("12/12/2022"), TimeSpan.FromHours(2)),
                new Booking(JobType.Photo, DateTime.Parse("12/12/2022"), TimeSpan.FromHours(2)),
                new Booking(JobType.PickUpKey, DateTime.Parse("12/12/2022"), TimeSpan.FromHours(2)),
                new Booking(JobType.Drone, DateTime.Parse("12/12/2022"), TimeSpan.FromHours(2))
            });

        Console.WriteLine(cost);
        Console.WriteLine("We are done!");
    }
}