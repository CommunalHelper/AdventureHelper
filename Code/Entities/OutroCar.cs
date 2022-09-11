﻿using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.AdventureHelper.Entities {
    [CustomEntity("AdventureHelper/OutroCar")]
    public class OutroCar : IntroCar {
        private readonly DynamicData origData;

        public OutroCar(EntityData data, Vector2 offset) 
            : base(data, offset) {
            origData = new DynamicData(typeof(IntroCar), this);
            Image bodySprite = origData.Get<Image>("bodySprite");
            bodySprite.FlipX = true;
            Hitbox hitbox = new(25f, 4f, -7f, -17f);
            Hitbox hitbox2 = new(19f, 4f, -24f, -11f);
            Collider = new ColliderList(hitbox, hitbox2);
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Entity wheels = origData.Get<Entity>("wheels");
            wheels.Components.Get<Image>().FlipX = true;
            wheels.Position.X += 6;
        }
    }
}
