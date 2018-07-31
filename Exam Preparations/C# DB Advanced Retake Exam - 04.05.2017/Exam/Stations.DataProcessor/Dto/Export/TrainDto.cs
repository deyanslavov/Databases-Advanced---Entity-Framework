﻿namespace Stations.DataProcessor.Dto.Export
{
    using System;

    public class TrainDto
    {
        public string TrainNumber { get; set; }

        public int DelayedTimes { get; set; }

        public TimeSpan MaxDelayedTime { get; set; }
    }
}
