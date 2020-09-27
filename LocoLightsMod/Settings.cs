using UnityEngine;
using UnityModManagerNet;

namespace LocoLightsMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw(Label = "Verbose Logging", Type = DrawType.Toggle)]
        public bool verbose =
#if DEBUG
            true;
#else
            false;
#endif
        [Draw(Label = "Exterior Light Shdaows", Type = DrawType.ToggleGroup, Vertical = true)]
        public LightShadows exteriorShadows = LightShadows.Hard;
        [Draw(Label = "Interior Light Shadows", Type = DrawType.ToggleGroup, Vertical = true)]
        public LightShadows interiorShadows = LightShadows.None;

        override public void Save(UnityModManager.ModEntry entry)
        {
            Save<Settings>(this, entry);
        }

        public void OnChange()
        {
            var tcLights = GameObject.FindObjectsOfType<TrainCarLights>();
            foreach (var tcl in tcLights)
            {
                tcl.SetExteriorShadows(exteriorShadows);
                tcl.SetInteriorShadows(interiorShadows);
            }
        }
    }
}