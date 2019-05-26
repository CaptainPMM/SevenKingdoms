using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSizeUnifier : MonoBehaviour {
    public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

    public void UnifyTextSizes() {
        enabled = true;
    }

    private void Update() {
        // Determine the minimum font size of all texts
        float minTxtSize = 999f;
        foreach (var txt in texts) {
            if (txt.fontSize < minTxtSize) minTxtSize = txt.fontSize;
        }

        // Set all text font sizes to the minimum
        foreach (var txt in texts) {
            txt.enableAutoSizing = false;
            txt.fontSize = minTxtSize;
            txt.ForceMeshUpdate();
        }

        enabled = false;
    }
}
