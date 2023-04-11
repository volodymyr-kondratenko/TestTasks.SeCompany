// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;

namespace TestTasks.SeCompany.Configuration;

internal sealed class Holidays
{
    public DayOfWeek[] WeekendDaysOfWeek { get; set; } = Array.Empty<DayOfWeek>();

    public string[] Dates { get; set; } = Array.Empty<string>();
}
