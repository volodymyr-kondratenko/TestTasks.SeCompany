// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;

namespace TestTasks.SeCompany.Models;

internal sealed class PhotographerTimeCost
{
    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int CostPerHour { get; set; }
}
