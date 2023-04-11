// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using TestTasks.SeCompany.Models;

namespace TestTasks.SeCompany.Services;

/// <summary>
///     Represents a repository of photography cost parameters.
/// </summary>
internal sealed class PhotographyCostParametersRepository : IPhotographyCostParametersRepository
{
    private const string _holidaysConfigurationSectionName = "Holidays";
    private readonly IConfiguration _configuration;
    private PhotographyCostParameters? _photographyCostParameters;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PhotographyCostParametersRepository"/> class.
    /// </summary>
    /// <param name="IConfiguration">The configuration.</param>
    public PhotographyCostParametersRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PhotographyCostParameters Get()
    {
        if (_photographyCostParameters != null)
            return _photographyCostParameters!;

        var configurationHolidays =
            _configuration.GetSection(_holidaysConfigurationSectionName).Get<Configuration.Holidays>();
        ValidateHolidays(configurationHolidays);

        var photoCostParameters = new PhotographyCostParameters()
        {
            IncreaseCostWithinMinutes = _configuration.GetSection("IncreaseCostWithinMinutes").Get<int>(),
            IncreaseCostWithinMultiplier = _configuration.GetSection("IncreaseCostWithinMultiplier").Get<double>(),
            HolidayWorkCostMultiplier = _configuration.GetSection("HolidayWorkCostMultiplier").Get<double>(),
            PhotographerDayMaxCost = _configuration.GetSection("PhotographerDayMaxCost").Get<double>(),
            TimeCosts = _configuration.GetSection("PhotographerTimeCosts").Get<PhotographerTimeCost[]>()!,
            Holidays = Map(configurationHolidays!)
        };

        _photographyCostParameters = photoCostParameters;
        return _photographyCostParameters;
    }

    private static void ValidateHolidays(Configuration.Holidays? holidays)
    {
        if (holidays is null || holidays.WeekendDaysOfWeek.Length < 1 || holidays.Dates.Length < 1)
        {
            throw new ApplicationException(
                 $"Invalid configuration section: {_holidaysConfigurationSectionName}.");
        }
    }

    private static Models.Holidays Map(Configuration.Holidays holidays)
    {
        List<DateOnly> parsedDates = new(holidays.Dates.Length);
        foreach (string date in holidays.Dates)
        {
            if (!DateOnly.TryParse(date, CultureInfo.InvariantCulture, out DateOnly holidayDate))
            {
                throw new ApplicationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        $"Invalid holiday day format in configuration."));
            }

            parsedDates.Add(holidayDate);
        }

        return new Models.Holidays(holidays.WeekendDaysOfWeek, parsedDates);
    }
}
