﻿using System;

namespace Pasta.Core
{
    public interface IEffect
    {
        void Apply(EffectContext context);
    }
}
