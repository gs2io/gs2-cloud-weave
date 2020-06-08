using System.Collections.Generic;
using System.Text;
using Gs2.Gs2Formation.Model;
using Gs2.Weave.Core.Editor;
using Gs2.Util.LitJson;
using UnityEditor;

namespace Gs2.Weave.Party
{
    [CustomEditor(typeof(PartyInstaller))]   
    public class PartyInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as PartyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<PartySetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "FormationNamespaceName")
                        {
                            installer.formationNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.formationNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "MoldModelName")
                        {
                            installer.moldModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.moldModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "FormModelName")
                        {
                            installer.formModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.formModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "NumberOfSaveArea")
                        {
                            installer.numberOfSaveArea = int.Parse(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "NumberOfUnit")
                        {
                            installer.numberOfUnit = int.Parse(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "PartyKeyName")
                        {
                            installer.keyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "PartyKeyId")
                        {
                            setting.partyKeyId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        protected override bool Validate()
        {
            var installer = target as PartyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.formationNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.moldModelName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.formModelName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.keyNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.keyName))
            {
                return false;
            }
            if (installer.numberOfSaveArea == 0)
            {
                return false;
            }
            if (installer.numberOfUnit == 0)
            {
                return false;
            }
            return true;
        }

        public class PartyMaster
        {
            public string version = "2019-09-09";
            public List<MoldModel> moldModels;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.moldModels != null)
                {
                    writer.WritePropertyName("moldModels");
                    writer.WriteArrayStart();
                    foreach(var item in this.moldModels)
                    {
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        class PartyMasterPostProcess : WeaveInstaller.PostProcess
        {
            private PartyInstaller _installer;
            
            public PartyMasterPostProcess(PartyInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::FormationNamespaceName}", _installer.formationNamespaceName);
                original = original.Replace("${Gs2::Weave::MoldModelName}", _installer.moldModelName);
                original = original.Replace("${Gs2::Weave::FormModelName}", _installer.formModelName);
                original = original.Replace("${Gs2::Weave::PartyKeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::PartyKeyName}", _installer.keyName);
                original = original.Replace("${Gs2::Weave::NumberOfSaveArea}", _installer.numberOfSaveArea.ToString());
                original = original.Replace("${Gs2::Weave::NumberOfUnit}", _installer.numberOfUnit.ToString());

                {
                    var slots = new List<SlotModel>();
                    for (var i = 0; i < _installer.numberOfUnit; i++)
                    {
                        slots.Add(new SlotModel
                        {
                            name = i.ToString(),
                        });
                    }
                    var master = new PartyMaster
                    {
                        moldModels = new List<MoldModel>
                        {
                            new MoldModel
                            {
                                name = _installer.moldModelName,
                                initialMaxCapacity = _installer.numberOfSaveArea,
                                maxCapacity = _installer.numberOfSaveArea,
                                formModel = new FormModel
                                {
                                    name = _installer.formModelName,
                                    slots = slots,
                                },
                            }
                        }
                    };
                
                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["FormationSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as PartyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new PartyMasterPostProcess(installer);
        }
    }
}