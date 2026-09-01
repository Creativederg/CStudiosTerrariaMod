using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace CStudios.Content.Effects
{
    public struct GoldTrail
    {
        private static VertexStrip _vertexStrip = new VertexStrip();

        public void Draw(Projectile proj)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(4f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(
                proj.oldPos, proj.oldRot, StripColors, StripWidth,
                -Main.screenPosition + proj.Size / 2f);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(new Color(255, 240, 160), new Color(255, 190, 40), progressOnStrip);
            result.A = 0;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            float num = 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 36f, num);
        }
    }
}
