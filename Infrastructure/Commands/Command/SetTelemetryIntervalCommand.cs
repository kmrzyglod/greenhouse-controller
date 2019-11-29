﻿using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Validation;
using Infrastructure.Services.Telemetry;

namespace Infrastructure.Commands.Command
{
    internal class SetTelemetryIntervalCommand : CommandBase
    {
        //Telemetry send interval in seconds
        public int Interval { get; }

        public SetTelemetryIntervalCommand(string correlationId, int interval) : base(correlationId)
        {
            Interval = interval;
        }

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();

            if (Interval <= 0)
            {
                errors.Add(new ValidationError(nameof(Interval), $"{nameof(Interval)} field cannot has 0 value"));
            }

            return ConvertToArray(errors);
        }

        public override string PartitionKey { get; } = nameof(TelemetryService);
    }
}