// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using TestTasks.SeCompany.Models;

namespace TestTasks.SeCompany.Services;

/// <summary>
///     Calculates a cost of photographer jobs.
/// </summary>
internal interface IPhotographyCostCalculator
{
    /// <summary>
    ///     Calculates the total cost of a photographer jobs for one day.
    /// </summary>
    /// <param name="bookings">Bookings of all photography jobs in one day.</param>
    /// <returns>The total cost of the photography jobs for that day.</returns>
    public double GetCost(Booking[] bookings);

    /// <summary>
    ///     Calculates the total cost of a booking of some photographer job.
    /// </summary>
    /// <param name="booking">The booking of a photographer job.</param>
    /// <returns>The cost of the booked photographer job.</returns>
    public double GetCost(Booking booking);
}
