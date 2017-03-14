using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RainbowRainbow
{
    public class AreaSparksDoer : MonoBehaviour
    {
        public Vector3 localMin;
        public Vector3 localMax;
        public GlobalSparksDoer.SparksType sparksType;
        public Vector3 baseDirection = Vector3.up;
        public float magnitudeVariance = 0.5f;
        public float angleVariance = 180f;
        public float LifespanMin = 0.5f;
        public float LifespanMax = 0.9f;
        public int SparksPerSecond = 120;
        private Transform m_transform;
        private tk2dSprite sprite;

        private void Start()
        {
            this.m_transform = base.gameObject.transform;
            sprite = GetComponent<tk2dSprite>();
        }

        //*
        private void Update()
        {
            RoomHandler room;
            try
            {
                room = sprite.transform.position.GetAbsoluteRoom();
            }
            catch
            {
                room = null;
            }
            if (!(GameManager.Instance.IsPaused || room == null || (room.visibility == RoomHandler.VisibilityStatus.OBSCURED || room.visibility == RoomHandler.VisibilityStatus.REOBSCURED)) && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
            {
                //GlobalSparksDoer.DoRandomParticleBurst(Mathf.Max(1, (int)((float)this.SparksPerSecond * BraveTime.DeltaTime)), this.m_transform.localPosition + this.localMin, this.m_transform.localPosition + this.localMax, this.baseDirection, this.angleVariance, this.magnitudeVariance, null, new float?(UnityEngine.Random.Range(this.LifespanMin, this.LifespanMax)), null, this.sparksType);
                GlobalSparksDoer.DoRandomParticleBurst(
                    Mathf.Max(1, (int)((float)this.SparksPerSecond * BraveTime.DeltaTime)),
                    sprite.WorldTopLeft,
                    sprite.WorldBottomRight,
                    this.baseDirection,
                    this.angleVariance,
                    this.magnitudeVariance,
                    null,
                    new float?(UnityEngine.Random.Range(this.LifespanMin, this.LifespanMax)),
                    null,
                    this.sparksType);
            }
        }
        //*/
    }

    public class Main : ETGModule
    {
        public static Shader RainbowShader;

        private void overrideSpriteMaterial(tk2dBaseSprite sprite)
        {
            sprite.usesOverrideMaterial = true;
            sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
            sprite.renderer.material.shader = RainbowShader;
        }

        private void onNewAIActor(Component component)
        {
            AIActor actor = component as AIActor;
            overrideSpriteMaterial(actor.sprite);
            actor.sprite.gameObject.AddComponent<AreaSparksDoer>();
            
            //SimpleSparksDoer sparks = actor.GetComponent<SimpleSparksDoer>();
        }
        
        private void onNewGoopManager(Component component)
        {
            DeadlyDeadlyGoopManager goopman = component as DeadlyDeadlyGoopManager;

            var prop = goopman.GetType().GetField("m_shader", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);
            prop.SetValue(goopman, RainbowShader);
        }

        public override void Init()
        {
            RainbowShader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
            ETGMod.Objects.AddHook<AIActor>(onNewAIActor);
            ETGMod.Objects.AddHook<DeadlyDeadlyGoopManager>(onNewGoopManager);

        }

        public override void Start()
        {
        }

        public override void Exit()
        {
        }
    }
}
