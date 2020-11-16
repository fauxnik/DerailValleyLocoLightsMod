using UnityEngine;
using System;

namespace LocoLightsMod
{
    public class LocoLights : TrainCarLights
    {
        public override bool IsOn { get; protected set; }
        public float Direction { get; private set; }

        public override void Init(TrainCar car, LocoLightData[] exterior, LocoLightData[] interior)
        {
            base.Init(car, exterior, interior);
            Direction = 0f;
        }

        public void SetLights(bool isOn)
        {
            Main.Log($"Setting lights for {SafeCarID}... (prev={(IsOn ? "on" : "off")}, next={(isOn ? "on" : "off")})");
            IsOn = isOn;
            UpdateLights();
        }

        public void SetDirection(float direction)
        {
            Main.Log($"Setting direction for {SafeCarID}... (prev={Direction}, next={direction})");
            Direction = direction;
            base.UpdateFlickerers();
            base.UpdateLights();
        }

        protected override float GetDirectionalScalar(LocoLightData datum)
        {
            return Direction > 0.05f ? datum.fwd : Direction < -0.05f ? datum.rev : datum.ntl;
        }
    }
}
