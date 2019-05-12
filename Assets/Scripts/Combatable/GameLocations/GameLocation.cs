using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLocation : Combatable {
    public GameObject connectionLinePrefab;

    public string locationName;
    public GameLocation[] reachableLocations;
    public List<GameEffect> locationEffects = new List<GameEffect>();
    public List<Building> buildings = new List<Building>();

    private bool wasOccupied = false;

    // Start is called before the first frame update
    new protected void Start() {
        base.Start();
        locationName = name;

        GetComponentInChildren<SpriteRenderer>().color = house.color;

        // Create GUI lines to reachable locations
        foreach (GameLocation location in reachableLocations) {
            // Check if line is duplicate
            bool lineExists = false;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("location_conn_line")) {
                LineRenderer lr = go.GetComponent<LineRenderer>();
                if (lr.GetPosition(0) == location.transform.position && lr.GetPosition(1) == transform.position) {
                    lineExists = true;
                    break;
                }
            }

            if (!lineExists) {
                GameObject cl = Instantiate(connectionLinePrefab);
                cl.name = "LocConnLine: " + name + "-" + location.name;
                LineRenderer lr = cl.GetComponent<LineRenderer>();

                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, location.transform.position);

                Gradient g = new Gradient();
                GradientColorKey[] g_colors = {
                    new GradientColorKey(house.color, 0.4f),
                    new GradientColorKey(location.house.color, 0.6f)
                };
                GradientAlphaKey[] g_alphas = {
                    new GradientAlphaKey(0.5f, 0.4f),
                    new GradientAlphaKey(0.5f, 0.6f)
                };
                g.SetKeys(g_colors, g_alphas);
                lr.colorGradient = g;
            }
        }
    }

    protected void Update() {
        if (wasOccupied) {
            Start();
            wasOccupied = false;
        }
    }

    public void OccupyBy(House house) {
        this.house = house;
        wasOccupied = true;

        // Remove all connection lines
        GameObject[] lines = GameObject.FindGameObjectsWithTag("location_conn_line");
        foreach (GameObject line in lines) {
            if (line.name.Contains(name)) {
                Destroy(line);
            }
        }
    }

    protected void GetEffectsFromBuildings() {
        foreach (Building b in buildings) {
            locationEffects.AddRange(b.gameEffects);
        }
    }
}
