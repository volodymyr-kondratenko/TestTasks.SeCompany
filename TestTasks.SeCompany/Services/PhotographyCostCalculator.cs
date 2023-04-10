// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TestTasks.SeCompany.Models;

namespace TestTasks.SeCompany.Services;

/// <summary>
///     Calculates a cost of photographer jobs.
/// </summary>
internal sealed class PhotographyCostCalculator : IPhotographyCostCalculator
{
    private readonly IPhotographyCostParametersRepository _photographyCostParametersRepository;
    private PhotographyCostParameters? _photographyCostParameters;

    private PhotographyCostParameters PhotographyCostParameters
    {
        get
        {
            if (_photographyCostParameters == null)
                _photographyCostParameters = _photographyCostParametersRepository.Get();
            
            return _photographyCostParameters!;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PhotographyCostCalculator"/> class.
    /// </summary>
    /// <param name="photographyCostParametersRepository">The repository of photography cost parameters.</param>
    public PhotographyCostCalculator(IPhotographyCostParametersRepository photographyCostParametersRepository)
    {
        _photographyCostParametersRepository = photographyCostParametersRepository;
    }

    /// <inheritdoc />
    public double GetCost(Booking[] bookings)
    {
        List<Booking> billingItems = bookings
            .Where(booking => !IsCostFree(booking.JobType))
            .OrderBy(booking => booking.DateTimeUtc)
            .ToList();

        double totalCost = 0;
        for (int i = 0; i < billingItems.Count - 1; i++)
        {
            Booking currentBooking = billingItems[i];
            Booking nextBooking = billingItems[i + 1];
            double currentBookingCost = GetCostInternal(currentBooking);
            double nextBookingCost = GetCostInternal(nextBooking);

            double diffInMinutes = (nextBooking.DateTimeUtc - currentBooking.DateTimeUtc).TotalMinutes;
            if (diffInMinutes < PhotographyCostParameters.IncreaseCostWithinMinutes)
            {
                totalCost +=
                    Math.Min(currentBookingCost, nextBookingCost)
                    * PhotographyCostParameters.IncreaseCostWithinMultiplier;
            }

            totalCost += currentBookingCost;
        }
        totalCost += GetCostInternal(billingItems[^1]);

        if (totalCost > PhotographyCostParameters.PhotographerDayMaxCost)
            totalCost = PhotographyCostParameters.PhotographerDayMaxCost;

        return Math.Round(totalCost, 2);
    }

    /// <inheritdoc />
    public double GetCost(Booking booking)
    {
        if (IsCostFree(booking.JobType))
            return 0;

        return GetCostInternal(booking);
    }

    private static bool IsCostFree(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.Default:
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, $"'{nameof(jobType)}' is not set."));
            case JobType.PickUpKey:
            case JobType.PlanMeasurement:
            case JobType.Transport:
                return true;
            default:
                return false;
        }
    }

    private double GetCostInternal(Booking booking)
    {
        PhotographerTimeCost[] timeCosts = PhotographyCostParameters.TimeCosts;
        var bookingTime = TimeOnly.FromDateTime(booking.DateTimeUtc);

        double finalCost = default;
        foreach (PhotographerTimeCost timeCost in timeCosts)
        {
            if (bookingTime.IsBetween(timeCost.StartTime, timeCost.EndTime))
                finalCost = timeCost.CostPerHour;
        }

        if (finalCost == default)
            finalCost = PhotographyCostParameters.UbnormalTimePhotographerCost;

        if (IsHoliday(booking.DateTimeUtc))
            return finalCost * PhotographyCostParameters.HolidayWorkCostMultiplier;

        return finalCost;
    }

    private bool IsHoliday(DateTime dateTimeUtc)
    {
        Holidays holidays = PhotographyCostParameters.Holidays;

        if (holidays.WeekendDaysOfWeek.Contains(dateTimeUtc.DayOfWeek) ||
            holidays.Dates.Contains(DateOnly.FromDateTime(dateTimeUtc)))
            return true;

        return false;
    }
}