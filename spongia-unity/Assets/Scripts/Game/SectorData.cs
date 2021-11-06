using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SectorData
{

    // Controller
    public SpawnedController controller;

    private int focusedIndex = -1;
    private int despawnOffset = 0;
    public List<Prop> screen = new List<Prop>();
    
    public const float THRESHOLD_GOOD = 0.05F;
    public const float THRESHOLD_AVERAGE = 0.1F;
    public const float THRESHOLD_BAD = 0.2F;
    public const float THRESHOLD_MISS = 0.4F;

    public SectorData(SpawnedController c) {
        // Set
        controller = c;
    }

    public void Spawn(Prop prop) {
        // Add prop
        screen.Add(prop);
    }

    public void Update(float time) {
        // Keys
        List<Prop> props = new List<Prop>(screen);
        // For each prop
        foreach (Prop prop in props) {
            // Previous position
            TonePosition previous = prop.position;

            // Switch
            switch(prop.Move(time)) {
                case TonePosition.FINISHED:
                    // If can be despawned
                    if (prop.startTime + prop.length + THRESHOLD_BAD < time) {
                        // If was not pressed
                        if (!prop.pressed) {
                            // If still focused and pressed, add 300, if not pressed at all 600
                            controller.AddToMaxScore((int) ((prop.startedPressing ? 300 : 600)*controller.multiplier));
                            // Reset combo
                            controller.HandleScore(Sector.NORTH_EAST, -1);
                            // If did not start pressing
                            if (!prop.startedPressing)
                                controller.HandleScore(Sector.NORTH_EAST, -1);
                        }

                        // Destroy
                        SpawnedController.Destroy(prop.gameObject);
                        // Remove
                        screen.RemoveAt(0);

                        // Decrease offset
                        despawnOffset -= 1;
                        // Decrease index
                        focusedIndex = focusedIndex == -1 ? -1 : focusedIndex - 1;
                    } else if (previous == TonePosition.PLAYING) {
                        // Increase despawn offset -> the prop is now despawning
                        despawnOffset += 1;
                    }
                    break;
            }
            /*// Remove
            if (prop.GetComponent<Prop>().Move()) {
                // Destroy
                Destroy(prop);
                // Remove
                active[sector].RemoveAt(0);
            }*/
        }
    }

    public void Failed() {
        // For each prop
        foreach (Prop prop in screen) {
            // If finished
            if (prop.position == TonePosition.FINISHED)
                continue;
                
            // Apply gravity
            prop.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            // Set sorting order in layer
            prop.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    public void Reset() {
        // For each prop
        foreach (Prop prop in screen)
            // Destroy
            SpawnedController.Destroy(prop.gameObject);
        // Reset list
        screen = new List<Prop>();
        // Reset indexes
        focusedIndex = -1;
        despawnOffset = 0;
    }

    // -2: 0 score, 0 combo
    // -1: 0 score, reset combo -> MISS
    // 0+: 0 score, +1 combo
    public int HandlePress(float time) {
        // If there are no props at all
        if (screen.Count == 0) {
            // Reset just in case
            focusedIndex = -1;
            // Can not focus
            return -2;
        }

        // If not -1, some is selected
        if (focusedIndex != -1) {
            // If too far away
            if (PropStartTooAway(time, screen[focusedIndex]))
                // No points
                return -2;

            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }

        // NO PROP FOCUSED CURRENTLY

        // If there are no props FINISHED
        if (despawnOffset == 0) {
            // The first prop
            Prop first = screen[0];

            // If too far away from the first prop, or pressed already
            if (PropStartTooAway(time, first) || first.pressed)
                // No points
                return -2;

            // Focus at the first prop
            focusedIndex = 0;
            first.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            // Handle
            return CalculatePointsStart(time, first);
        }

        // THERE IS AT LEAST ONE PROP FINISHED (DESPAWNING)

        // If there are no non-FINISHED props available (only FINISHED/DESPAWNING props)
        if (screen.Count - despawnOffset == 0) {
            // The last despawning prop
            Prop prop = screen[despawnOffset - 1];

            // If missed or pressed already
            if (PropStartTooAway(time, prop) || prop.pressed)
                // Can't focus
                return -2;
            
            // Focus
            focusedIndex = despawnOffset - 1;
            prop.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            // Handle
            return CalculatePointsStart(time, prop);
        }

        // THERE ARE AT LEAST 2 PROPS - AT LEAST ONE DESPAWNING (FINISHED) AND ONE WAITING

        // If the first non-FINISHED prop is still PLAYING
        if (screen[despawnOffset].position == TonePosition.PLAYING) {
            // If too far away
            if (PropStartTooAway(time, screen[despawnOffset])) {
                // No focus
                focusedIndex = -1;
                // No points
                return -2;
            }

            // At this time, the tone should not have been pressed, in such case the focused index would not be -1
            // Focus
            focusedIndex = despawnOffset;
            screen[focusedIndex].GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }

        // NO TONE PLAYING CURRENTLY, BUT THERE ARE AT LEAST 2 - AT LEAST ONE DESPAWNING (FINISHED) AND ONE WAITING

        // Calculate mid time between start of on screen vs first despawning prop
        float midDistance = (screen[despawnOffset].startTime + screen[despawnOffset - 1].startTime) / 2;

        // If nearer to the despawning prop
        if (time < midDistance) {
            // If too far away
            if (PropStartTooAway(time, screen[despawnOffset - 1])) {
                // No focus
                focusedIndex = -1;
                // No points
                return -2;
            }

            // Focus at the despawning
            focusedIndex = despawnOffset - 1;
            screen[focusedIndex].GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        } else {
            // If too far away
            if (PropStartTooAway(time, screen[despawnOffset])) {
                // No focus
                focusedIndex = -1;
                // No points
                return -2;
            }

            // Focus at the first on screen
            focusedIndex = despawnOffset;
            screen[focusedIndex].GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }
    }

    // -2: 0 score, 0 combo
    // -1: 0 score, reset combo
    // 0+: 0 score, +1 combo
    public int HandleRelease(float time) {
        // If no focused prop -> despawned, too early, no points
        if (focusedIndex == -1)
            return -2;

        // Calculate points
        int points = CalculatePointsEnd(time, screen[focusedIndex]);
        screen[focusedIndex].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        screen[focusedIndex].pressed = true;
        // If despawning
        if (focusedIndex < despawnOffset)
            // Reset
            focusedIndex = -1;
        else {
            // Move to next index (or if no prop available, none)
            focusedIndex = screen.Count > focusedIndex + 1 ? focusedIndex + 1 : -1;
            if (focusedIndex > -1)
                screen[focusedIndex].GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        }
        // Return
        return points;
    }

    public bool PropStartTooAway(float time, Prop prop) {
        return Math.Abs(time - prop.startTime) > THRESHOLD_MISS;
    }

    private int CalculatePointsStart(float time, Prop prop) {
        float diff = Math.Abs(prop.startTime - time);

        controller.AddToMaxScore((int) (300*controller.multiplier));
        prop.startedPressing = true;
        if (diff <= THRESHOLD_GOOD)
            return 300;
        else if (diff <= THRESHOLD_AVERAGE)
            return 200;
        else if (diff <= THRESHOLD_BAD)
            return 50;
        else if (diff <= THRESHOLD_MISS)
            return -1;
        else
            return 0;
    }

    private int CalculatePointsEnd(float time, Prop prop) {
        float diff = Math.Abs(prop.startTime + prop.length - time);

        controller.AddToMaxScore((int) (300*controller.multiplier));
        if (diff <= THRESHOLD_GOOD)
            return 300;
        else if (diff <= THRESHOLD_AVERAGE)
            return 200;
        else if (diff <= THRESHOLD_BAD)
            return 50;
        else
            return -1;
    }
    
}
