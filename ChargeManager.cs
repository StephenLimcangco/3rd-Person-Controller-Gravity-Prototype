using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeManager : MonoBehaviour
{
    public float chargeCounter;
    public float chargeSpeed = 10;
    public float drainSpeed = 5;

    public Text textBox;
    public Slider slider;

    private void Awake()
    {
        chargeCounter = 100;
    }
    public void UpdateChargeCounter(bool isGrounded)
    {
        if (isGrounded)
        {
            if (chargeCounter < 100)
            {
                chargeCounter += chargeSpeed * Time.deltaTime;
            }
        }

        textBox.text = "Charge: " + (int) chargeCounter + "%";
        slider.value = chargeCounter / 100;
    }

}
