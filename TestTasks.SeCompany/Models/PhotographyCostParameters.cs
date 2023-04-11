// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

namespace TestTasks.SeCompany.Models;

internal sealed class PhotographyCostParameters
{
    public required int IncreaseCostWithinMinutes { get; init; }

    public required double IncreaseCostWithinMultiplier { get; init; }

    public required double HolidayWorkCostMultiplier { get; init; }

    public required double PhotographerDayMaxCost { get; init; }

    public required PhotographerTimeCost[] TimeCosts { get; init; }

    public required Holidays Holidays { get; init; }
}
