using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SectorData
{
    private int focusedIndex = -1;
    private int despawnOffset = 0;
    public List<Prop> screen = new List<Prop>();
    
    public const float THRESHOLD_GOOD = 0.05F;
    public const float THRESHOLD_AVERAGE = 0.1F;
    public const float THRESHOLD_BAD = 0.2F;
    public const float THRESHOLD_MISS = 0.4F;

    public void Spawn(Prop prop) {
        // Add prop
        screen.Add(prop);
    }

    public void Update(float time) {
        // Keys
        List<Prop> props = new List<Prop>(screen);
        // For each prop
        foreach (Prop prop in props) {
            // Switch
            switch(prop.Move()) {
                case TonePosition.FINISHED:
                    // If can be despawned
                    if (prop.startTime + prop.length + THRESHOLD_BAD < time) {
                        // Destroy
                        SpawnedController.Destroy(prop.gameObject);
                        // Remove
                        screen.RemoveAt(0);

                        // Decrease offset
                        despawnOffset -= 1;
                        // Decrease index
                        focusedIndex -= 1;
                    } else {
                        // Increase despawn offset
                        despawnOffset += 1;
                    }
                    break;
                case TonePosition.PLAYING:
                    return;
                default:
                    return;
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

    public int HandlePress(float time) {
        // If there are no props
        if (screen.Count == 0) {
            // Set focused at none
            focusedIndex = -1;
            return 0;
        }

        // If there are no props despawning
        if (despawnOffset == 0) {
            // If too far away
            if (PropStartTooAway(time, screen[0])) {
                // No focus
                focusedIndex = -1;
                // No points
                return 0;
            }

            // Focus at the first prop
            focusedIndex = 0;
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }

        // If the current prop is still playing
        if (screen[despawnOffset].position == TonePosition.PLAYING) {
            // Focus
            focusedIndex = despawnOffset;
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }

        // NO TONE PLAYING CURRENTLY, BUT THERE IS AT LEAST ONE DESPAWNING (FINISHED) AND ONE WAITING

        // Calculate mid time between start of on screen vs first despawning prop
        float midDistance = (screen[despawnOffset].startTime + screen[despawnOffset - 1].startTime) / 2;

        // If nearer to the despawning prop
        if (time < midDistance) {
            // If too far away
            if (PropStartTooAway(time, screen[despawnOffset - 1])) {
                // No focus
                focusedIndex = -1;
                // No points
                return 0;
            }

            // Focus at the despawning
            focusedIndex = despawnOffset - 1;
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        } else {
            // If too far away
            if (PropStartTooAway(time, screen[despawnOffset])) {
                // No focus
                focusedIndex = -1;
                // No points
                return 0;
            }

            // Focus at the first on screen
            focusedIndex = despawnOffset;
            // Handle
            return CalculatePointsStart(time, screen[focusedIndex]);
        }
    }

    public int HandleRelease(float time) {
        // If no focused prop -> despawned, too early, no points
        if (focusedIndex == -1)
            return 0;

        // Calculate points
        int points = CalculatePointsEnd(time, screen[focusedIndex]);
        // If despawning
        if (focusedIndex < despawnOffset)
            // Reset
            focusedIndex = -1;
        else
            // Move to next index
            focusedIndex += 1;
        // Return
        return points;
    }

    public bool PropStartTooAway(float time, Prop prop) {
        return Math.Abs(time - prop.startTime) > THRESHOLD_MISS;
    }

    private int CalculatePointsStart(float time, Prop prop) {
        float diff = Math.Abs(prop.startTime - time);

        if (diff <= THRESHOLD_GOOD)
            return 300;
        else if (diff <= THRESHOLD_GOOD)
            return 200;
        else if (diff <= THRESHOLD_GOOD)
            return 50;
        else
            return 0;
    }

    private int CalculatePointsEnd(float time, Prop prop) {
        float diff = Math.Abs(prop.startTime + prop.length - time);

        if (diff <= THRESHOLD_GOOD)
            return 300;
        else if (diff <= THRESHOLD_GOOD)
            return 200;
        else if (diff <= THRESHOLD_GOOD)
            return 50;
        else
            return 0;
    }

    /*// Find next prop, on key press
    public Prop FindNextProp(float time) {
        // If not out of range
        if (nextIndex >= 0)
            return FocusedProp();
        
        // If there are no props available
        if (active.Length == 0)
            return null;
        
        // Index
        int i = 0;
        // While not applicable (passed already)
        while (active.Length > i && active[i].startTime < time - SpawnController.THRESHOLD_BAD)
            i += 1;

        // If out of range
        if (active.Length <= i)
            // No props available
            return null;
        
        // Return the prop
        nextIndex = i;
        return FocusedProp();
    }*/
}
