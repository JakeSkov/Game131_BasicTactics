using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(Actor))]
public class Editor_Actor : Editor
{
    #region Variable Declaration
    bool editPosition = false;
    bool editImmunity = false;
    bool editAI = false;

    int maxHP = 100;
    int dmg = 50;
    int chanceToHit = 70;
    int init = 10;
    int currentHP = 100;

    SelectionList<Actor.ActionSource> immunityList;
    SelectionList<Actor.Position> positionList;
    string actName;

    Actor.ActionEffect effect = Actor.ActionEffect.Normal;
    Actor.ActionSource dmgType = Actor.ActionSource.Weapon;
    Actor.ActionSource[] immunities;
    Actor.ActionTarget targetType = Actor.ActionTarget.MeleeEnemy;
    Actor.Position boardPos = Actor.Position.left_front_center;

    DerivedStatList derivedStats;
    #endregion

    public override void OnInspectorGUI()
    {
        Actor editActor = target as Actor;

        Actor.ActionSource[] immTypeVal = Enum.GetValues(typeof(Actor.ActionSource)) as Actor.ActionSource[];
        string[] immTypeName = Enum.GetNames(typeof(Actor.ActionSource));
        for (int i = 0; i < immTypeName.Length; i++) immTypeName[i] += '\t';

        Actor.Position[] posVal = Enum.GetValues(typeof(Actor.Position)) as Actor.Position[];
        string[] posName = Enum.GetNames(typeof(Actor.Position));
        for (int i = 0; i < posName.Length; i++) posName[i] += '\t';

        immunityList = new SelectionList<Actor.ActionSource>(immTypeVal, immTypeName);
        positionList = new SelectionList<Actor.Position>(posVal, posName);

        #region Name, and HP
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name");
        actName = GUILayout.TextField(editActor.actorName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Hit Points");
        maxHP = EditorGUILayout.IntSlider(maxHP, 1, 10000);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Hit Points");
        currentHP = EditorGUILayout.IntSlider(currentHP, 0, maxHP);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Position
        editPosition = EditorGUILayout.Foldout(editPosition, "Position");
        if (editPosition)
        {
            boardPos = positionList.RadioList("", boardPos, 2);
        }
        #endregion

        #region Damage
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Damage");
        dmg = EditorGUILayout.IntSlider(dmg, 0, 180);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Effect");
        EditorGUILayout.EnumFlagsField(effect);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Damage Type");
        EditorGUILayout.EnumFlagsField(dmgType);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attack type");
        EditorGUILayout.EnumFlagsField(targetType);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Immunities
        editImmunity = EditorGUILayout.Foldout(editImmunity, "Immunities");
        if (editImmunity)
        {
            immunities = immunityList.CheckboxList("Immunities", immunities, 2);
        }
        #endregion

        #region AI
        editAI = EditorGUILayout.Foldout(editAI, "AI Options");
        if (editAI)
        {
            derivedStats = AssetDatabase.LoadAssetAtPath("Assets/DerivedProperties.asset", typeof(DerivedStatList)) as DerivedStatList;
            if (derivedStats == null)
            {
                derivedStats = ScriptableObject.CreateInstance<DerivedStatList>();
                AssetDatabase.CreateAsset(derivedStats, "Assets/DerivedProperties.asset");
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Chance to Hit");
            chanceToHit = EditorGUILayout.IntSlider(chanceToHit, 0, 100);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Inititive");
            init = EditorGUILayout.IntSlider(init, 5, 100);
            init = init / 5;
            init *= 5;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("AI Targeting Filters");
            EditorGUILayout.LabelField("Filter order does matter (Ordered top to bottom)");
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(80));
            EditorGUILayout.LabelField("Expression");
            EditorGUILayout.EndHorizontal();

            FilterLayout();

            EditorGUILayout.Separator();
            bool added = false;
            if (GUILayout.Button("Add Filter"))
            {
                added = true;
                AddButtonOnClick();
            }

            if (GUI.changed || added)
            {
                EditorUtility.SetDirty(derivedStats);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

        }
        #endregion

        #region Assignment
        editActor.actorName = actName;
        editActor.boardPosition = boardPos;
        editActor.maxHitPoints = maxHP;
        editActor.damage = dmg;
        editActor.actionEffect = effect;
        editActor.actionEffectSource = dmgType;
        editActor.immunities = immunities;
        editActor.actionTarget = targetType;
        editActor.percentChanceToHit = chanceToHit;
        editActor.initiative = init;
        editActor.hitPoints = currentHP;
        #endregion

        
    }

    void AddButtonOnClick()
    {
        if (derivedStats == null)
        {
            derivedStats = ScriptableObject.CreateInstance<DerivedStatList>();
        }
        List<DerivedStat> newDerivedStats;
        
        newDerivedStats = new List<DerivedStat>(derivedStats.stats);
        newDerivedStats.Add(new DerivedStat());

        derivedStats.stats = newDerivedStats.ToArray();
        this.Repaint();
    }

    void FilterLayout()
    {
        for (int i = 0; i < (derivedStats != null ? derivedStats.Length : 0); i++)
        {
            EditorGUILayout.BeginHorizontal();
            derivedStats.stats[i].name = EditorGUILayout.TextField(derivedStats.stats[i].name, GUILayout.Width(80));
            string derivedExpression = EditorGUILayout.TextField(derivedStats.stats[i].expression);
            derivedStats.stats[i].expression = derivedExpression;
            EditorGUILayout.EndHorizontal();
        }
    }
}

// @author Steven Smith
// Repo URL: https://github.com/onecrane/Game131_BasicTactics
class SelectionList<T> where T : IComparable
{
    int f = 9;
    T[] _values;
    string[] _labels;
    T _selectedValue;


    public T[] CheckboxList(string label, T[] initialSelections, int itemsPerCol)
    {
        List<T> selectedValues = new List<T>();
        List<int> initialSelectedIndexes = new List<int>();
        for (int i = 0; i < _values.Length; i++)
        {
            for (int j = 0; j < initialSelections.Length; j++)
            {
                if (_values[i].CompareTo(initialSelections[j]) == 0) initialSelectedIndexes.Add(i);
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.MaxWidth(100));

        EditorGUILayout.BeginVertical();
        for (int r = 0; r < _values.Length; r += itemsPerCol)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = r; i < r + itemsPerCol && i < _values.Length; i++)
            {
                if (GUILayout.Toggle(initialSelectedIndexes.Contains(i), _labels[i]))
                {
                    selectedValues.Add(_values[i]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        return selectedValues.ToArray();

    }

    public T RadioList(string label, T initialSelection, int itemsPerRow)
    {
        T originalSelectedValue = _selectedValue;
        _selectedValue = initialSelection;
        bool anyChecked = false;

        // TWo controls rendered: The label, and a vertical section
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(label, GUILayout.MaxWidth(100));

        EditorGUILayout.BeginVertical();
        for (int r = 0; r < _values.Length; r += itemsPerRow)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = r; i < r + itemsPerRow && i < _values.Length; i++)
            {
                if (_values[i].CompareTo(initialSelection) == 0) originalSelectedValue = initialSelection;
                if (GUILayout.Toggle(_values[i].CompareTo(_selectedValue) == 0, _labels[i]))
                {
                    _selectedValue = _values[i];
                    anyChecked = true;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        if (!anyChecked) _selectedValue = originalSelectedValue;
        return _selectedValue;
    }

    public SelectionList(T[] values, string[] labels)
    {
        _values = new T[values.Length];
        _labels = new string[labels.Length < values.Length ? values.Length : labels.Length];
        for (int i = 0; i < _values.Length; i++) _values[i] = values[i];
        for (int i = 0; i < _labels.Length; i++) _labels[i] = (i < labels.Length) ? labels[i] : values[i].ToString();
        _selectedValue = _values[0];
    }
}
