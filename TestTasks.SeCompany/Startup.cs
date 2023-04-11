// Copyright (c) Volodymyr Kondratenko. All Rights Reserved.
// Licensed under the Apache License, version 2.0.

using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTasks.SeCompany.Services;

namespace TestTasks.SeCompany;

internal sealed class Startup
{
    private ServiceProvider? _serviceProvider;

    public ServiceProvider GetServiceProvider()
    {
        if (_serviceProvider is null)
            throw new ApplicationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    $"The instance is not configured. Use '{nameof(this.Configure)}' method at first."));
        return _serviceProvider!;
    }

    public Startup Configure()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, false)
        .Build();
        
        // TODO: Configure logging.
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<IPhotographyCostParametersRepository, PhotographyCostParametersRepository>();
        serviceCollection.AddSingleton<IPhotographyCostCalculator, PhotographyCostCalculator>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        return this;
    }
}
