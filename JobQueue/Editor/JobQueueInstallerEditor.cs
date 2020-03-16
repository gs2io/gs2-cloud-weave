using Gs2.Weave.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.JobQueue
{
    [CustomEditor(typeof(JobQueueInstaller))]   
    public class JobQueueInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as JobQueueInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<JobQueueSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        switch (output.name)
                        {
                            case "JobQueueNamespaceName":
                                installer.jobQueueNamespaceName = output.value;
                                setting.jobQueueNamespaceName = output.value;
                                EditorUtility.SetDirty(installer);
                                EditorUtility.SetDirty(setting);
                                break;
                            case "JobQueueNamespaceId":
                                setting.jobQueueNamespaceId = output.value;
                                EditorUtility.SetDirty(setting);
                                break;
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }
        
        protected override bool Validate()
        {
            var installer = target as JobQueueInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.jobQueueNamespaceName))
            {
                return false;
            }
            return true;
        }

        class JobQueuePostProcess : WeaveInstaller.PostProcess
        {
            private JobQueueInstaller _installer;
            
            public JobQueuePostProcess(
                JobQueueInstaller installer
            )
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::JobQueueNamespaceName}", _installer.jobQueueNamespaceName);
                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as JobQueueInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new JobQueuePostProcess(
                installer
            );
        }
    }
}