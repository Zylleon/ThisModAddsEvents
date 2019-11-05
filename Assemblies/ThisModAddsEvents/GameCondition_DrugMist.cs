using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;




namespace ThisModAddsEvents
{
    public class GameCondition_DrugMist : GameCondition
    {
        private const int LerpTicks = 5000;

        private const float MaxSkyLerpFactor = 0.5f;

        private SkyColorSet DrugMistColors;

        private List<SkyOverlay> overlays;

        private const int CheckInterval = 2719;

        private const float ToxicPerDay = 0.5f;


        public GameCondition_DrugMist()
        {
            ColorInt colorInt = new ColorInt(255, 191, 241);
            Color hilights = colorInt.ToColor;
            ColorInt colorInt2 = new ColorInt(112, 147, 179);
            Color shadows = colorInt2.ToColor;
            ColorInt colorInt3 = new ColorInt(230, 202, 229);
            Color overlay = colorInt2.ToColor;


            //this.DrugMistColors = new SkyColorSet(hilights, shadows, overlay, 0.65f);     // This was too saturated

            this.DrugMistColors = new SkyColorSet(hilights, shadows, overlay, 0.3f);
            this.overlays = new List<SkyOverlay>
            {
                new WeatherOverlay_DrugMist()
            };
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % CheckInterval == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    this.DoPawnsPsychiteInhalation(affectedMaps[i]);
                }
            }
            for (int j = 0; j < this.overlays.Count; j++)
            {
                for (int k = 0; k < affectedMaps.Count; k++)
                {
                    this.overlays[j].TickOverlay(affectedMaps[k]);
                }
            }
        }

        private void DoPawnsPsychiteInhalation(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];



                if (pawn.def.race.IsFlesh && pawn.Position.GetRoom(map, RegionType.Set_Passable).PsychologicallyOutdoors)
                {
                    // This number determines buildup speed, might need adjustment
                    float num = 0.156758334f;

                    float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                    num *= num2;

                    HediffDef teaHigh = DefDatabase<HediffDef>.GetNamed("PsychiteTeaHigh");

                    HealthUtility.AdjustSeverity(pawn, teaHigh, num);

                    // TODO: This doesn't adjust for or give tolerance
                    
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < this.overlays.Count; i++)
            {
                this.overlays[i].DrawOverlay(map);
            }
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(.95f, this.DrugMistColors, 1f, 1f));
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return this.overlays;
        }
    }




    public class WeatherOverlay_DrugMist : SkyOverlay
    {
        private static readonly Material FalloutOverlayWorld = MatLoader.LoadMat("Weather/SnowOverlayWorld", -1);

        public WeatherOverlay_DrugMist()
        {
            this.worldOverlayMat = WeatherOverlay_DrugMist.FalloutOverlayWorld;
            this.worldOverlayPanSpeed1 = 0.0008f;
            this.worldPanDir1 = new Vector2(-0.25f, -1f);
            this.worldPanDir1.Normalize();
            this.worldOverlayPanSpeed2 = 0.0012f;
            this.worldPanDir2 = new Vector2(-0.24f, -1f);
            this.worldPanDir2.Normalize();
        }
    }



}