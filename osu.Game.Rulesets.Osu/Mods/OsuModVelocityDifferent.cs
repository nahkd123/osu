using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Logging;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Osu.Skinning.Default;
using osuTK;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModVelocityDifferent : Mod, IApplicableToDrawableHitObject, IApplicableToBeatmap
    {
        public override string Name => "Velocity Different";
        public override string Acronym => "VD";
        public override string Description => "Very funky sliders.";
        public override double ScoreMultiplier => 1;

        [SettingSource("Style", "Change the animation style of the slider ball.", 1)]
        public Bindable<Easing> Style { get; } = new Bindable<Easing>(Easing.InOutCubic);
        public IEasingFunction EasingFunction { get; private set; }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            EasingFunction = new DefaultEasingFunction(Style.Value);
        }

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            if (drawable is DrawableSlider slider) slider.ApplyCustomUpdateState += (dho, state) =>
            {
                slider.OnUpdate += (_) =>
                {
                    double progress = Math.Clamp((slider.Clock.CurrentTime - slider.HitObject.StartTime) / slider.HitObject.Duration, 0.0, 1.0);
                    progress = EasingFunction.ApplyEasing(slider.HitObject.ProgressAt(progress));
                    slider.Ball.Position = slider.HitObject.Path.PositionAt(progress);
                    slider.NestedHitObjects.OfType<SnakingSliderBody>().FirstOrDefault()?.UpdateProgress(progress);
                };
            };
        }

    }
}
