// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;
using System.Collections.Generic;

namespace TestTasks.SeCompany.Models;

public sealed class Holidays
{
    public Holidays(DayOfWeek[] weekendDaysOfWeek, List<DateOnly> dates)
    {
        WeekendDaysOfWeek = weekendDaysOfWeek;
        Dates = dates;
    }

    public DayOfWeek[] WeekendDaysOfWeek { get; }

    public List<DateOnly> Dates { get; }
}
