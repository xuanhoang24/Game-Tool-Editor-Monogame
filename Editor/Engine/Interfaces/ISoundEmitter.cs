using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine.Interfaces
{
    enum SoundEffectTypes
    {
        OnSelect = 0,
        OnTakeDamaged = 1,
    }

    internal interface ISoundEmitter
    {
        public SFXInstance[] SoundEffects { get; }
    }
}
