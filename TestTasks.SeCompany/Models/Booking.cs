// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;

namespace TestTasks.SeCompany.Models;

/// <summary>
///     Represents a booking for a specific job.
/// </summary>
public sealed class Booking
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Booking"/> class.
    /// </summary>
    /// <param name="jobType">The type of a job that is booked.</param>
    /// <param name="dateTimeUtc">The date and the time of the booking, in UTC.</param>
    /// <param name="duration">The duration of the booking.</param>
    public Booking(JobType jobType, DateTime dateAndTimeUtc, TimeSpan duration)
    {
        JobType = jobType;
        DateTimeUtc = dateAndTimeUtc;
        Duration = duration;
    }

    /// <summary>
    ///     The type of a job that is booked.
    /// </summary>
    public JobType JobType { get; }

    /// <summary>
    ///     The date and the time of the booking, in UTC.
    /// </summary>
    public DateTime DateTimeUtc { get; }
    
    /// <summary>
    ///     The duration of the booking.
    /// </summary>
    public TimeSpan Duration { get; }
}
