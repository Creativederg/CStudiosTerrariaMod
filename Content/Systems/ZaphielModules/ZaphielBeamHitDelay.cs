namespace CStudios.Content.Systems.ZaphielModules
{
    public static class ZaphielBeamHitDelay
    {
        // Smaller = more hits per second. 1 = every tick.
        public static int For(ZaphielShootContext ctx)
        {
            if (ctx.FinalityCoreActive || ctx.FinalityMode || ctx.CocoonBitsActive || ctx.FinalityEdgeActive)
                return 1;

            if (ctx.AuthorityCoreActive || ctx.AuthorityMode)
                return 3;

            if (ctx.PhantomBitsActive || ctx.PermetAfterimageActive)
                return 4;

            if (ctx.FractureCoreActive || ctx.FractureMode)
                return 5;

            if (ctx.ScoreMode || ctx.FeedbackHeartActive || ctx.LivingGaugeActive)
                return 7;

            if (ctx.HerrscherDriveActive || ctx.SkybladeManifestActive)
                return 8;

            return 8;
        }
    }
}
