using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FameCSharp;
using FameCore.Util;

public enum SelectionMode
{
    group,
    individual
}

public static class UnitSelection
{
    private static SelectionMode unitSelectionMode = SelectionMode.group;
    public static SelectionMode UnitSelectionMode
    {
        get { return unitSelectionMode; }
        set
        {
            if (unitSelectionMode != value)
            {
                DeselectUnit();
            }
            unitSelectionMode = value;
        }
    }

    public static void DeselectUnit()
    {
        foreach (SelectableUnit s in currentlySelectedUnits)
        {
            s.IsSelected = false;
        }
        currentlySelectedUnits.Clear();
    }

    static List<SelectableUnit> currentlySelectedUnits = new List<SelectableUnit>();
    public static int SelectionCount()
    {
        return currentlySelectedUnits.Count;
    }
    public static SelectableUnit[] GetCurrentlySelectedUnit()
    {
        return currentlySelectedUnits.ToArray();
    }

    public static void SetSelected(SelectableUnit unit)
    {
        for (int i = 0; i < currentlySelectedUnits.Count; i++)
        {
            if (currentlySelectedUnits[i] == unit)
            {
                return;
            }
        }
        currentlySelectedUnits.Add(unit);
        unit.IsSelected = true;
    }

    public static void SetSelectedUnits(Vector3[] points)
    {
        DeselectUnit();
        switch (UnitSelectionMode)
        {
            case SelectionMode.group:
                {
                    List<int> selectedGroup = new List<int>();
                    int[] selectedAgents = FameManager.QueryAgents(points);
                    foreach (int i in selectedAgents)
                    {
                        FlockMember unit = FameManager.GetFlockMember(i);
                        if (unit is SelectableUnit)
                        {
                            SelectableUnit avatar = (SelectableUnit)unit;
                            if (!selectedGroup.Contains(avatar.ID))
                            {
                                if (avatar.ID != -1)
                                {
                                    selectedGroup.Add(avatar.ID);
                                }
                            }
                        }
                    }

                    foreach (int i in selectedGroup)
                    {
                        FlockGroup g = FameManager.GetFlockGroup(i);
                        if (g is SelectableUnit)
                        {
                            SelectableUnit selectableUnit = (SelectableUnit)g;
                            currentlySelectedUnits.Add(selectableUnit);
                            selectableUnit.IsSelected = true;
                        }

                    }
                }
                break;
            case SelectionMode.individual:
                {
                    FVec3[] point3FVec = FameUnityUtil.Vec3ToFVec3(points);
                    int[] selectedAgents = FAME.Singleton.QueryAgents(point3FVec);
                    foreach (int i in selectedAgents)
                    {
                        FlockMember unit = FameManager.GetFlockMember(i);
                        if (unit is SelectableUnit)
                        {
                            SelectableUnit selectableUnit = (SelectableUnit)unit;
                            currentlySelectedUnits.Add(selectableUnit);
                            selectableUnit.IsSelected = true;
                        }
                    }
//                    Debug.Log("Selected " + selectedAgents.Length + " " + currentlySelectedUnits.Count);
                }
                break;
        }
    }

    ///// <summary>
    ///// Set selected unit based on current mouse position
    ///// </summary>
    //public static void SetSelectedUnits(Vector3 mousePosition)
    //{
    //    DeselectUnit();
    //    switch (UnitSelectionMode)
    //    {
    //        case SelectionMode.group:
    //            {
    //            }
    //            break;
    //        case SelectionMode.individual:
    //            {
    //                //RaycastHit hit = new RaycastHit();
    //                //Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    //                List<GameAvatar> hitByClick = new List<GameAvatar>();
    //                int[] agentIDs = FlockController.GetFlockMemberIDs();
    //                for (int i = 0; i < agentIDs.Length; i++)
    //                {
    //                    FlockMember member = FlockController.GetAgent(agentIDs[i]);
    //                    if (member is GameAvatar)
    //                    {
    //                        GameAvatar avatar = (GameAvatar)member;
    //                        if (avatar.Contain(mousePosition))
    //                        {
    //                            hitByClick.Add(avatar);
    //                        }
    //                    }
    //                }
    //                int nearestIndex = -1;
    //                float nearestDistSq = float.MaxValue;
    //                for (int i = 0; i < hitByClick.Count; i++)
    //                {
    //                    float unitDist = (Camera.main.transform.position - hitByClick[i].transform.position).sqrMagnitude;
    //                    if (unitDist < nearestDistSq)
    //                    {
    //                        nearestDistSq = unitDist;
    //                        nearestIndex = i;
    //                    }
    //                }
    //                if (nearestIndex != -1)
    //                {
    //                    hitByClick[nearestIndex].IsSelected = true;
    //                    FlockMember flockMember = FlockController.GetAgent(hitByClick[nearestIndex].AgentID);
    //                    if (flockMember is SelectableUnit)
    //                    {
    //                        currentlySelectedUnits.Add(flockMember as SelectableUnit);
    //                    }
    //                }
    //            }
    //            break;
    //    }
    //}
}
