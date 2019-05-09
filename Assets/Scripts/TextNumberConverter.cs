using UnityEngine;
using TMPro;

public class TextNumberConverter : MonoBehaviour {
    public void ConvertNumberToText(float number) {
        GetComponent<TextMeshProUGUI>().text = number.ToString();
    }
}
