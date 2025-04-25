﻿namespace RuanFa.Shop.SharedKernel.Interfaces;
public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
