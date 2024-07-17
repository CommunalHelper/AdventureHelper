using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.AdventureHelper {
    internal static class Utils {
        public static bool IsLineVisible(Vector2 start, Vector2 end) {
            float left, right;
            if (start.X > end.X) {
                left = end.X;
                right = start.X;
            } else {
                left = start.X;
                right = end.X;
            }

            float top, bot;
            if (start.Y > end.Y) {
                top = end.Y;
                bot = start.Y;
            } else {
                top = start.Y;
                bot = end.Y;
            }

            return CullHelper.IsRectangleVisible(left, top, right - left, bot - top);
        }
    }
}