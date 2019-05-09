using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : GameLocation {
    // Test soldier recruiting (normally not in outposts)
    [SerializeField] private float recruitmentSpeed = 1f;

    private float elapsedTime;
    // --

    // Start is called before the first frame update
    new void Start() {
        base.Start();

        locationEffects.Add(GameEffect.LOCATION_DEFENDER_CASUALTIES_MODIFIER);

        elapsedTime = 0f;
    }

    // Update is called once per frame
    new void Update() {
        base.Update();

        if (combat == null) {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= recruitmentSpeed) {
                elapsedTime = 0;
                // soldiers.AddSoldierTypeNum(SoldierType.CONSCRIPTS, 1); // TEST
                // soldiers.AddSoldierTypeNum(SoldierType.CAV_KNIGHTS, 1); // TEST
                UpdateGUI();
            }
        }
    }
}
