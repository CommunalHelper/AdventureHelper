using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.AdventureHelper.Entities {
    [CustomEntity("AdventureHelper/LinkedZipMoverNoReturn")]
    public class LinkedZipMoverNoReturn : Solid {
        public const string defaultPath = "objects/AdventureHelper/noreturnzipmover";

        private static readonly SoundSource sfx = new();

        private readonly MTexture[,] edges;
        private readonly Sprite streetlight;
        private readonly BloomPoint bloom;
        private readonly List<MTexture> innerCogs;
        private readonly MTexture temp;
        private readonly string syncFlagCode;
        private readonly string spritePath;
        private readonly float speedMultiplier;
        private readonly Color ropeColor;
        private readonly Color ropeLightColor;
        private bool firstDirection;
        private ZipMoverPathRenderer pathRenderer;
        private Vector2 start;
        private Vector2 target;
        private float percent;

        public LinkedZipMoverNoReturn(Vector2 position, int width, int height, Vector2 target, string colorCode, float speedMultiplier, string spritePath) 
            : base(position, width, height, false) {
            spritePath.Trim('/');
            spritePath.Trim('\\');
            if (spritePath == string.Empty) {
                spritePath = defaultPath;
            }

            this.spritePath = spritePath;
            this.speedMultiplier = speedMultiplier;
            edges = new MTexture[3, 3];
            List<MTexture> tempInnerCogs = GFX.Game.GetAtlasSubtextures(spritePath + "/innercog");
            innerCogs = tempInnerCogs.Count > 0 ? tempInnerCogs : GFX.Game.GetAtlasSubtextures(defaultPath + "/innercog");
            temp = new MTexture();
            percent = 0f;
            Depth = -9999;
            start = Position;
            this.target = target;
            ColorCode = colorCode;
            syncFlagCode = $"ZipMoverSync:{ColorCode}";
            ropeColor = Calc.HexToColor(ColorCode);
            ropeLightColor = Calc.HexToColor(ColorCode) * 1.1f;
            firstDirection = true;
            Add(new Coroutine(Sequence(), true));
            Add(new LightOcclude(1f));
            Add(streetlight = new Sprite(GFX.Game, GFX.Game.GetAtlasSubtexturesAt(spritePath + "/light", 0)?.AtlasPath is not ("__fallback" or null) ? spritePath + "/light" : "objects/zipmover/light"));
            streetlight.Add("frames", "", 1f);

            streetlight.Play("frames");
            streetlight.Active = false;
            streetlight.SetAnimationFrame(1);
            streetlight.Position = new Vector2((Width / 2f) - (streetlight.Width / 2f), (Height / 2f) - (streetlight.Height / 2f));
            Add(bloom = new BloomPoint(1f, 6f));
            bloom.Position = new Vector2(Width / 2f, (Height / 2f) - (streetlight.Height / 2f) + 3f);
            string path = GFX.Game.Has(spritePath + "/block") ? spritePath : defaultPath;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    edges[i, j] = GFX.Game[path + "/block"].GetSubtexture(i * 8, j * 8, 8, 8);
                }
            }

            SurfaceSoundIndex = 7;
            sfx.Position = new Vector2(Width, Height) / 2f;
            Add(sfx);
        }

        public LinkedZipMoverNoReturn(EntityData data, Vector2 offset) 
            : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset, data.Attr("colorCode", "000000"), data.Float("speedMultiplier", 1f), data.Attr("spritePath", defaultPath)) { 
        }

        public string ColorCode { get; }

        public override void Added(Scene scene) {
            base.Added(scene);
            scene.Add(pathRenderer = new ZipMoverPathRenderer(this, spritePath));
        }

        public override void Removed(Scene scene) {
            scene.Remove(pathRenderer);
            pathRenderer = null;
            SyncFlag = false;
            ZipMoverSoundController.StopSound(ColorCode, ZipMoverSoundController.SoundType.NonReturning);
            base.Removed(scene);
        }

        public override void Update() {
            base.Update();
            bloom.Y = (float)((Height / 2f) - (streetlight.Height / 2f) + (streetlight.CurrentAnimationFrame * 3));
        }

        public override void Render() {
            if (!CullHelper.IsRectangleVisible(X, Y, Width, Height))
                return;
            
            Vector2 position = Position;
            Position += Shake;
            Draw.Rect(X, Y, Width, Height, Color.Black);
            int num = 1;
            float num2 = 0f;
            int count = innerCogs.Count;
            int num3 = 4;
            while (num3 <= Height - 4f) {
                int num4 = num;
                int num5 = 4;
                while (num5 <= Width - 4f) {
                    int index = (int)(Mod((num2 + (num * percent * 3.14159274f * 4f)) / 1.57079637f, 1f) * count);
                    MTexture mtexture = innerCogs[index];
                    Rectangle rectangle = new(0, 0, mtexture.Width, mtexture.Height);
                    Vector2 zero = Vector2.Zero;
                    bool flag = num5 <= 4;
                    if (flag) {
                        zero.X = 2f;
                        rectangle.X = 2;
                        rectangle.Width -= 2;
                    } else {
                        bool flag2 = num5 >= Width - 4f;
                        if (flag2) {
                            zero.X = -2f;
                            rectangle.Width -= 2;
                        }
                    }

                    bool flag3 = num3 <= 4;
                    if (flag3) {
                        zero.Y = 2f;
                        rectangle.Y = 2;
                        rectangle.Height -= 2;
                    } else {
                        bool flag4 = num3 >= Height - 4f;
                        if (flag4) {
                            zero.Y = -2f;
                            rectangle.Height -= 2;
                        }
                    }

                    mtexture = mtexture.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, temp);
                    mtexture.DrawCentered(Position + new Vector2(num5, num3) + zero, Color.White * ((num < 0) ? 0.5f : 1f));
                    num = -num;
                    num2 += 1.04719758f;
                    num5 += 8;
                }

                bool flag5 = num4 == num;
                if (flag5) {
                    num = -num;
                }

                num3 += 8;
            }

            int num6 = 0;
            while (num6 < Width / 8f) {
                int num7 = 0;
                while (num7 < Height / 8f) {
                    int num8 = (num6 == 0) ? 0 : ((num6 == (Width / 8f) - 1f) ? 2 : 1);
                    int num9 = (num7 == 0) ? 0 : ((num7 == (Height / 8f) - 1f) ? 2 : 1);
                    bool flag6 = num8 != 1 || num9 != 1;
                    if (flag6) {
                        edges[num8, num9].Draw(new Vector2(X + (num6 * 8), Y + (num7 * 8)));
                    }

                    num7++;
                }

                num6++;
            }

            base.Render();
            Position = position;
        }

        private void ScrapeParticlesCheck(Vector2 to) {
            bool flag = Scene.OnInterval(0.03f);
            if (flag) {
                bool flag2 = to.Y != ExactPosition.Y;
                bool flag3 = to.X != ExactPosition.X;
                bool flag4 = flag2 && !flag3;
                if (flag4) {
                    int num = Math.Sign(to.Y - ExactPosition.Y);
                    bool flag5 = num == 1;
                    Vector2 value = flag5 ? BottomLeft : TopLeft;
                    int num2 = 4;
                    bool flag6 = num == 1;
                    if (flag6) {
                        num2 = Math.Min((int)Height - 12, 20);
                    }

                    int num3 = (int)Height;
                    bool flag7 = num == -1;
                    if (flag7) {
                        num3 = Math.Max(16, (int)Height - 16);
                    }

                    bool flag8 = Scene.CollideCheck<Solid>(value + new Vector2(-2f, num * -2));
                    if (flag8) {
                        for (int i = num2; i < num3; i += 8) {
                            SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2(0f, i + (num * 2f)), (num == 1) ? -0.7853982f : 0.7853982f);
                        }
                    }

                    bool flag9 = Scene.CollideCheck<Solid>(value + new Vector2(Width + 2f, num * -2));
                    if (flag9) {
                        for (int j = num2; j < num3; j += 8) {
                            SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopRight + new Vector2(-1f, j + (num * 2f)), (num == 1) ? -2.3561945f : 2.3561945f);
                        }
                    }
                } else {
                    bool flag10 = flag3 && !flag2;
                    if (flag10) {
                        int num4 = Math.Sign(to.X - ExactPosition.X);
                        bool flag11 = num4 == 1;
                        Vector2 value2 = flag11 ? TopRight : TopLeft;
                        int num5 = 4;
                        bool flag12 = num4 == 1;
                        if (flag12) {
                            num5 = Math.Min((int)Width - 12, 20);
                        }

                        int num6 = (int)Width;
                        bool flag13 = num4 == -1;
                        if (flag13) {
                            num6 = Math.Max(16, (int)Width - 16);
                        }

                        bool flag14 = Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, -2f));
                        if (flag14) {
                            for (int k = num5; k < num6; k += 8) {
                                SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2(k + (num4 * 2f), -1f), (num4 == 1) ? 2.3561945f : 0.7853982f);
                            }
                        }

                        bool flag15 = Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, Height + 2f));
                        if (flag15) {
                            for (int l = num5; l < num6; l += 8) {
                                SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, BottomLeft + new Vector2(l + (num4 * 2f), 0f), (num4 == 1) ? -2.3561945f : -0.7853982f);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator Sequence() {
            Vector2 start = Position;
            for (; ; )
            {
                while (!HasPlayerRider() && !SyncFlag) {
                    yield return null;
                }
                // Signaling other movers
                SyncFlag = true;
                // End signaling other movers
                ZipMoverSoundController.PlaySound(ColorCode, ZipMoverSoundController.SoundType.NonReturning, this);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
                StartShaking(0.1f);
                yield return 0.1f;
                SyncFlag = false;
                streetlight.SetAnimationFrame(3);
                StopPlayerRunIntoAnimation = false;
                float at = 0f;
                while (at < 1f) {
                    yield return null;
                    at = Calc.Approach(at, 1f, 2f * Engine.DeltaTime * speedMultiplier);
                    percent = Ease.SineIn(at);
                    Vector2 to = firstDirection ? Vector2.Lerp(start, target, percent) : Vector2.Lerp(target, start, percent);
                    ScrapeParticlesCheck(to);
                    bool flag = Scene.OnInterval(0.1f);
                    if (flag) {
                        pathRenderer.CreateSparks();
                    }

                    MoveTo(to);
                }

                StartShaking(0.2f);
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                SceneAs<Level>().Shake(0.3f);
                StopPlayerRunIntoAnimation = true;
                streetlight.SetAnimationFrame(2);
                yield return 0.5f;
                ZipMoverSoundController.StopSound(ColorCode, ZipMoverSoundController.SoundType.NonReturning);
                StopPlayerRunIntoAnimation = false;
                streetlight.SetAnimationFrame(1);
                firstDirection = !firstDirection;
            }
        }

        private float Mod(float x, float m) {
            return ((x % m) + m) % m;
        }

        private bool SyncFlag {
            get => (Scene as Level).Session.GetFlag(syncFlagCode);
            set => (Scene as Level).Session.SetFlag(syncFlagCode, value);
        }

        private class ZipMoverPathRenderer : Entity {
            public LinkedZipMoverNoReturn TargetZip;

            private readonly MTexture cog;
            private Vector2 from;
            private Vector2 to;
            private Vector2 sparkAdd;
            private readonly float sparkDirFromA;
            private readonly float sparkDirFromB;
            private readonly float sparkDirToA;
            private readonly float sparkDirToB;

            public ZipMoverPathRenderer(LinkedZipMoverNoReturn zipMover, string spritePath) : base() {
                cog = GFX.Game.Has(spritePath + "/cog") ? GFX.Game[spritePath + "/cog"] : GFX.Game[defaultPath + "/cog"];
                Depth = 5000;
                TargetZip = zipMover;
                from = TargetZip.start + new Vector2(TargetZip.Width / 2f, TargetZip.Height / 2f);
                to = TargetZip.target + new Vector2(TargetZip.Width / 2f, TargetZip.Height / 2f);
                sparkAdd = (from - to).SafeNormalize(5f).Perpendicular();
                float num = (from - to).Angle();
                sparkDirFromA = num + 0.3926991f;
                sparkDirFromB = num - 0.3926991f;
                sparkDirToA = num + 3.14159274f - 0.3926991f;
                sparkDirToB = num + 3.14159274f + 0.3926991f;
            }

            public void CreateSparks() {
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromB);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToB);
            }

            public override void Render() {
                if (Utils.IsLineVisible(from, to)) {
                    DrawCogs(Vector2.UnitY, Color.Black);
                    DrawCogs(Vector2.Zero);
                }
                
                Draw.Rect(new Rectangle((int)(TargetZip.X - 1f), (int)(TargetZip.Y - 1f), (int)TargetZip.Width + 2, (int)TargetZip.Height + 2), Color.Black);
            }

            private void DrawCogs(Vector2 offset, Color? colorOverride = null) {
                Vector2 vector = (to - from).SafeNormalize();
                Vector2 value = vector.Perpendicular() * 3f;
                Vector2 value2 = -vector.Perpendicular() * 4f;
                float rotation = TargetZip.percent * 3.14159274f * 2f;
                Draw.Line(from + value + offset, to + value + offset, colorOverride ?? TargetZip.ropeColor);
                Draw.Line(from + value2 + offset, to + value2 + offset, colorOverride ?? TargetZip.ropeColor);
                for (float num = 4f - (TargetZip.percent * 3.14159274f * 8f % 4f); num < (to - from).Length(); num += 4f) {
                    Vector2 value3 = from + value + vector.Perpendicular() + (vector * num);
                    Vector2 value4 = to + value2 - (vector * num);
                    Draw.Line(value3 + offset, value3 + (vector * 2f) + offset, colorOverride ?? TargetZip.ropeLightColor);
                    Draw.Line(value4 + offset, value4 - (vector * 2f) + offset, colorOverride ?? TargetZip.ropeLightColor);
                }

                cog.DrawCentered(from + offset, colorOverride ?? Color.White, 1f, rotation);
                cog.DrawCentered(to + offset, colorOverride ?? Color.White, 1f, rotation);
            }
        }
    }
}
