using UnityEngine;
using TMPro;
using Platformer.Model;
using Platformer.Core;

namespace LakbayTala.UI
{
    /// <summary>
    /// Placeholder: Shows current Tala count (and optional required) in the HUD. Assign to a TMP or UI Text.
    /// </summary>
    public class TalaCountDisplay : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public string format = "{0}";           // e.g. "{0}" or "{0} / {1}"
        public bool showRequired = false;

        PlatformerModel _model;

        void Start()
        {
            _model = Simulation.GetModel<PlatformerModel>();
        }

        void Update()
        {
            if (_model == null) _model = Simulation.GetModel<PlatformerModel>();
            if (label == null || _model == null) return;
            if (showRequired)
                label.text = string.Format(string.IsNullOrEmpty(format) ? "{0} / {1}" : format, _model.currentTalaCollected, _model.requiredTalaToFinish);
            else
                label.text = string.Format(string.IsNullOrEmpty(format) ? "{0}" : format, _model.currentTalaCollected);
        }
    }
}
