using Editor.Editor;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine
{
    internal class SFXInstance
    {
        public string Name { get; set; }
        public SoundEffectInstance Instance { get; set; }
        public static SFXInstance Create(GameEditor _game, string _assetName)
        {
            /*string fileName = Path.Combine(_game.Project.Folder, _game.Project.ContentFolder, _game.Project.AssetFolder, _assetName);
            SoundEffect ef = _game.Content.Load<SoundEffect>(fileName);*/
            SoundEffect ef = _game.Content.Load<SoundEffect>(_assetName);

            SoundEffectInstance efi = ef.CreateInstance();
            efi.Volume = 1;
            efi.IsLooped = false;
            return new SFXInstance() { Name = _assetName, Instance = efi };
        }
    }
}
