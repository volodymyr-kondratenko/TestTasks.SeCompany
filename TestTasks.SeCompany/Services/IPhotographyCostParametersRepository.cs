// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using TestTasks.SeCompany.Models;

namespace TestTasks.SeCompany.Services;

/// <summary>
///     Represents a repository of photography cost parameters.
/// </summary>
internal interface IPhotographyCostParametersRepository
{
    /// <summary>
    ///     Gets a photography cost parameters.
    /// </summary>
    /// <returns>The photography cost parameters.</returns>
    public PhotographyCostParameters Get();
}
