﻿#if UNITY_2017_1_OR_NEWER

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelScriptedImporter)), CanEditMultipleObjects]
    public class VoxelScriptedImporterEditor : ScriptedImporterEditor
    {
        private static bool advancedMode;
        private static bool generateLightmapUVsAdvanced = false;

        private SerializedProperty legacyVoxImportProp;
        private SerializedProperty importModeProp;
        private SerializedProperty importScaleProp;
        private SerializedProperty importOffsetProp;
        private SerializedProperty combineFacesProp;
        private SerializedProperty ignoreCavityProp;
        private SerializedProperty outputStructureProp;
        private SerializedProperty generateLightmapUVsProp;
        private SerializedProperty generateLightmapUVsAngleErrorProp;
        private SerializedProperty generateLightmapUVsAreaErrorProp;
        private SerializedProperty generateLightmapUVsHardAngleProp;
        private SerializedProperty generateLightmapUVsPackMarginProp;
        private SerializedProperty generateTangentsProp;
        private SerializedProperty meshFaceVertexOffsetProp;
        private SerializedProperty loadFromVoxelFileProp;
        private SerializedProperty generateMipMapsProp;
        private SerializedProperty colliderTypeProp;
#pragma warning disable 0414
        private SerializedProperty remappedMaterialsProp;
#pragma warning restore 0414

        private readonly GUIContent[] ImportModeStrings =
        {
            new GUIContent(VoxelBase.ImportMode.LowTexture.ToString()),
            new GUIContent(VoxelBase.ImportMode.LowPoly.ToString()),
        };
        private readonly int[] ImportModeValues =
        {
            (int)VoxelBase.ImportMode.LowTexture,
            (int)VoxelBase.ImportMode.LowPoly,
        };
        private readonly GUIContent[] ColliderTypeStrings =
        {
            new GUIContent(VoxelScriptedImporter.ColliderType.None.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Box.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Sphere.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Capsule.ToString()),
            new GUIContent(VoxelScriptedImporter.ColliderType.Mesh.ToString()),
        };
        private readonly int[] ColliderTypeValues =
        {
            (int)VoxelScriptedImporter.ColliderType.None,
            (int)VoxelScriptedImporter.ColliderType.Box,
            (int)VoxelScriptedImporter.ColliderType.Sphere,
            (int)VoxelScriptedImporter.ColliderType.Capsule,
            (int)VoxelScriptedImporter.ColliderType.Mesh,
        };

        public override void OnEnable()
        {
            base.OnEnable();

            legacyVoxImportProp = serializedObject.FindProperty("legacyVoxImport");
            importModeProp = serializedObject.FindProperty("importMode");
            importScaleProp = serializedObject.FindProperty("importScale");
            importOffsetProp = serializedObject.FindProperty("importOffset");
            combineFacesProp = serializedObject.FindProperty("combineFaces");
            ignoreCavityProp = serializedObject.FindProperty("ignoreCavity");
            outputStructureProp = serializedObject.FindProperty("outputStructure");
            generateLightmapUVsProp = serializedObject.FindProperty("generateLightmapUVs");
            generateLightmapUVsAngleErrorProp = serializedObject.FindProperty("generateLightmapUVsAngleError");
            generateLightmapUVsAreaErrorProp = serializedObject.FindProperty("generateLightmapUVsAreaError");
            generateLightmapUVsHardAngleProp = serializedObject.FindProperty("generateLightmapUVsHardAngle");
            generateLightmapUVsPackMarginProp = serializedObject.FindProperty("generateLightmapUVsPackMargin");
            generateTangentsProp = serializedObject.FindProperty("generateTangents");
            meshFaceVertexOffsetProp = serializedObject.FindProperty("meshFaceVertexOffset");
            loadFromVoxelFileProp = serializedObject.FindProperty("loadFromVoxelFile");
            generateMipMapsProp = serializedObject.FindProperty("generateMipMaps");
            colliderTypeProp = serializedObject.FindProperty("colliderType");
            remappedMaterialsProp = serializedObject.FindProperty("remappedMaterials");
        }

        public override void OnInspectorGUI()
        {
            var vtarget = target as VoxelScriptedImporter;
            if (vtarget == null)
            {
                base.OnInspectorGUI();
                return;
            }

            #region Simple
            {
                EditorGUI.BeginChangeCheck();
                var mode = GUILayout.Toolbar(advancedMode ? 1 : 0, VoxelBaseEditor.Edit_AdvancedModeStrings);
                if (EditorGUI.EndChangeCheck())
                {
                    advancedMode = mode != 0 ? true : false;
                }
            }
            #endregion

            #region Settings
            {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    if (advancedMode)
                    {
                        if (vtarget.fileType == VoxelBase.FileType.vox)
                        {
                            EditorGUILayout.PropertyField(legacyVoxImportProp, new GUIContent("Legacy Vox Import", "Import with legacy behavior up to Version 1.1.2.\nMultiple objects do not correspond.\nIt is deprecated for future use.\nThis is left for compatibility."));
                        }
                        EditorGUILayout.IntPopup(importModeProp, ImportModeStrings, ImportModeValues, new GUIContent("Import Mode"));
                    }
                    EditorGUILayout.PropertyField(importScaleProp);
                    EditorGUILayout.PropertyField(importOffsetProp);
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Optimize
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Optimize", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(combineFacesProp, new GUIContent("Combine Voxel Faces"));
                    EditorGUILayout.PropertyField(ignoreCavityProp, new GUIContent("Ignore Cavity"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Output
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(outputStructureProp, new GUIContent("Voxel Structure", "Save the structure information."));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Mesh
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(generateLightmapUVsProp, new GUIContent("Generate Lightmap UVs", "Generate lightmap UVs into UV2."));
                    if (generateLightmapUVsProp.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        generateLightmapUVsAdvanced = EditorGUILayout.Foldout(generateLightmapUVsAdvanced, new GUIContent("Advanced"));
                        if (generateLightmapUVsAdvanced)
                        {
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.Slider(generateLightmapUVsHardAngleProp, 0f, 180f, new GUIContent("Hard Angle", "Angle between neighbor triangles that will generate seam."));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    generateLightmapUVsHardAngleProp.floatValue = Mathf.Round(generateLightmapUVsHardAngleProp.floatValue);
                                }
                            }
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.Slider(generateLightmapUVsPackMarginProp, 1f, 64f, new GUIContent("Pack Margin", "Measured in pixels, assuming mesh will cover an entire 1024x1024 lightmap."));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    generateLightmapUVsPackMarginProp.floatValue = Mathf.Round(generateLightmapUVsPackMarginProp.floatValue);
                                }
                            }
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.Slider(generateLightmapUVsAngleErrorProp, 1f, 75f, new GUIContent("Angle Error", "Measured in percents. Angle error measures deviation of UV angles from geometry angles. Area error measures deviation of UV triangles area from geometry triangles if they were uniformly scaled."));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    generateLightmapUVsAngleErrorProp.floatValue = Mathf.Round(generateLightmapUVsAngleErrorProp.floatValue);
                                }
                            }
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.Slider(generateLightmapUVsAreaErrorProp, 1f, 75f, new GUIContent("Area Error"));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    generateLightmapUVsAreaErrorProp.floatValue = Mathf.Round(generateLightmapUVsAreaErrorProp.floatValue);
                                }
                            }
                        }
                        EditorGUI.indentLevel--;

                    }
                    EditorGUILayout.PropertyField(generateTangentsProp, new GUIContent("Generate Tangents", "Generate Tangents."));
                    EditorGUILayout.Slider(meshFaceVertexOffsetProp, 0f, 0.01f, new GUIContent("Vertex Offset", "Increase this value if flickering of polygon gaps occurs at low resolution."));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Material
            {
                EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    if (advancedMode)
                        EditorGUILayout.PropertyField(loadFromVoxelFileProp, new GUIContent("Load From Voxel File"));
#if UNITY_2017_3_OR_NEWER
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Materials");
                        bool disableExtract = false;
                        {
                            if (vtarget.materials == null || vtarget.materials.Length <= 0)
                                disableExtract = true;
                            else
                            {
                                disableExtract = true;
                                for (int i = 0; i < vtarget.materials.Length; i++)
                                {
                                    if(vtarget.materials[i] != null)
                                    {
                                        disableExtract = false;
                                        break;
                                    }
                                }
                            }
                        }
                        EditorGUI.BeginDisabledGroup(disableExtract);
                        if (GUILayout.Button(new GUIContent("Extract Materials...", "Click on this button to extract the embedded materials.")))
                        {
                            string path = vtarget.assetPath;
                            path = EditorUtility.SaveFolderPanel("Select Materials Folder", Path.GetDirectoryName(path), "");
                            if (!string.IsNullOrEmpty(path))
                            {
                                path = FileUtil.GetProjectRelativePath(path);
                                if (string.IsNullOrEmpty(path))
                                {
                                    EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                }
                                else
                                {
                                    try
                                    {
                                        AssetDatabase.StartAssetEditing();
                                        {
                                            foreach (var t in targets)
                                            {
                                                var importer = t as VoxelScriptedImporter;
                                                if (importer == null) continue;
                                                SerializedObject so = new SerializedObject(importer);
                                                SerializedProperty rmatProp = so.FindProperty("remappedMaterials");
                                                var materials = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath).Where(x => x.GetType() == typeof(Material));
                                                foreach (var material in materials)
                                                {
                                                    if (EditorCommon.IsMainAsset(material))
                                                        continue;
                                                    var assetPath = string.Format("{0}/{1}_{2}.mat", path, Path.GetFileNameWithoutExtension(importer.assetPath), material.name);
                                                    assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                                                    AssetDatabase.CreateAsset(Material.Instantiate(material), assetPath);
                                                    {
                                                        var saveMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                                                        var remap = rmatProp.GetArrayElementAtIndex(rmatProp.arraySize++);
                                                        remap.FindPropertyRelative("name").stringValue = material.name;
                                                        remap.FindPropertyRelative("material").objectReferenceValue = saveMaterial;
                                                    }
                                                }
                                                so.ApplyModifiedProperties();
                                                importer.SaveAndReimport();
                                            }
                                            return; //Force Exit
                                        }
                                    }
                                    finally
                                    {
                                        AssetDatabase.StopAssetEditing();
                                    }
                                }
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }
                    if (targets.Length == 1)    //single selection only
                    {
                        EditorGUILayout.LabelField(new GUIContent("Remapped Materials", "External materials to use for each embedded material."), EditorStyles.boldLabel);

                        EditorGUI.indentLevel++;
                        for (int index = 0; index < vtarget.materialNames.Length; index++)
                        {
                            SerializedProperty materialProp = null;
                            Material material = null;
                            int propertyIdx = 0;
                            for (int i = 0, count = remappedMaterialsProp.arraySize; i < count; i++)
                            {
                                var remap = remappedMaterialsProp.GetArrayElementAtIndex(i);
                                var name = remap.FindPropertyRelative("name").stringValue;
                                var prop = remap.FindPropertyRelative("material");
                                if (vtarget.materialNames[index] == name)
                                {
                                    materialProp = prop;
                                    material = prop.objectReferenceValue as Material;
                                    propertyIdx = i;
                                    break;
                                }
                            }

                            if (materialProp != null)
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.ObjectField(materialProp, typeof(Material), new GUIContent(vtarget.materialNames[index]));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (materialProp.objectReferenceValue == null)
                                    {
                                        remappedMaterialsProp.DeleteArrayElementAtIndex(propertyIdx);
                                    }
                                }
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                material = EditorGUILayout.ObjectField(vtarget.materialNames[index], material, typeof(Material), false) as Material;
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (material != null)
                                    {
                                        var remap = remappedMaterialsProp.GetArrayElementAtIndex(remappedMaterialsProp.arraySize++);
                                        remap.FindPropertyRelative("name").stringValue = vtarget.materialNames[index];
                                        remap.FindPropertyRelative("material").objectReferenceValue = material;
                                    }
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
#else
                    {
                        EditorGUILayout.HelpBox("It is impossible to change the material due to issue of Issue ID 1012200.\nWhen using Material change please use Unity 2017.4.1 or later.", MessageType.Warning);
                    }
#endif
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Texture
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(generateMipMapsProp, new GUIContent("Generate Mip Maps"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            #region Collider
            if (advancedMode)
            {
                EditorGUILayout.LabelField("Collider", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.IntPopup(colliderTypeProp, ColliderTypeStrings, ColliderTypeValues, new GUIContent("Generate Colliders"));
                }
                EditorGUI.indentLevel--;
            }
            #endregion

            ApplyRevertGUI();
        }
    }
}
#endif