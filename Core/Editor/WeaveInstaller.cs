using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gs2.Core;
using Gs2.Editor.Core;
using Gs2.Gs2Deploy;
using Gs2.Gs2Deploy.Request;
using Gs2.Gs2Deploy.Result;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Event = Gs2.Gs2Deploy.Model.Event;

namespace Gs2.Weave.Core.Editor
{
    public class WeaveInstaller
    {
        private static string ToStackName(Manifest manifest, string template)
        {
            return manifest.package.Replace(".", "_") + "_" + manifest.name + "_" + template.Replace(".", "_");
        }

        private static bool _statusGetting = false;

        public static IEnumerator GetStatus(Manifest manifest, UnityAction<AsyncResult<GetStackStatusResult>> callback)
        {
            if(_statusGetting) yield break;

            _statusGetting = true;
            
            foreach (var template in manifest.templates)
            {
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).GetStackStatus(
                    new GetStackStatusRequest()
                        .WithStackName(ToStackName(manifest, template)),
                    r =>
                    {
                        _statusGetting = false;
                        callback.Invoke(r);
                    }
                );
            }
        }

        public class PostProcess
        {
            public virtual string Execute(string original)
            {
                return original;
            }
        }

        public static IEnumerator Install(Manifest manifest, UnityAction<List<Event>> updateEventCallback, UnityAction callback, PostProcess postProcess = null)
        {
            var exit = false;
            foreach (var template in manifest.templates)
            {
                var templateData = new StreamReader(manifest.basePath + template, Encoding.UTF8).ReadToEnd();

                if (postProcess != null)
                {
                    templateData = postProcess.Execute(templateData);
                }
                
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).CreateStack(
                    new CreateStackRequest()
                        .WithName(ToStackName(manifest, template))
                        .WithTemplate(templateData),
                    r =>
                    {
                        if (r.Error != null)
                        {
                            EditorUtility.DisplayDialog("Error", r.Error.Message, "OK");
                            exit = true;
                        }
                    }
                );
                if (exit)
                {
                    yield break;
                }
            }
            
            string status = null;
            foreach (var template in manifest.templates)
            {
                do
                {
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).GetStackStatus(
                        new GetStackStatusRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                status = r.Result.status;
                            }
                        }
                    );

                    yield return new WaitForSeconds(5);
                    
                    var end = false;
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).DescribeEvents(
                        new DescribeEventsRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                updateEventCallback.Invoke(r.Result.items);
                            }

                            end = true;
                        }
                    );

                    while (!end)
                    {
                        yield return new WaitForSeconds(1);
                    }

                } while (!status.EndsWith("_COMPLETE"));
            }
            
            callback.Invoke();
        }

        public static IEnumerator Validate(Manifest manifest, UnityAction callback, PostProcess postProcess = null)
        {
            var exit = false;
            foreach (var template in manifest.templates)
            {
                var templateData = new StreamReader(manifest.basePath + template, Encoding.UTF8).ReadToEnd();

                if (postProcess != null)
                {
                    templateData = postProcess.Execute(templateData);
                }
                
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).Validate(
                    new ValidateRequest()
                        .WithTemplate(templateData),
                    r =>
                    {
                        if (r.Error != null)
                        {
                            EditorUtility.DisplayDialog("Error", r.Error.Message, "OK");
                            exit = true;
                        }
                    }
                );
                if (exit)
                {
                    yield break;
                }
            }
            
            callback.Invoke();
        }

        public static IEnumerator Update(Manifest manifest, UnityAction<List<Event>> updateEventCallback, UnityAction callback, PostProcess postProcess = null)
        {
            var exit = false;
            foreach (var template in manifest.templates)
            {
                var templateData = new StreamReader(manifest.basePath + template, Encoding.UTF8).ReadToEnd();

                if (postProcess != null)
                {
                    templateData = postProcess.Execute(templateData);
                }
                
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).UpdateStack(
                    new UpdateStackRequest()
                        .WithStackName(ToStackName(manifest, template))
                        .WithTemplate(templateData),
                    r =>
                    {
                        if (r.Error != null)
                        {
                            EditorUtility.DisplayDialog("Error", r.Error.Message, "OK");
                            exit = true;
                        }
                    }
                );
                if (exit)
                {
                    yield break;
                }
            }
            
            string status = null;
            foreach (var template in manifest.templates)
            {
                do
                {
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).GetStackStatus(
                        new GetStackStatusRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                status = r.Result.status;
                            }
                        }
                    );

                    yield return new WaitForSeconds(5);
                    
                    var end = false;
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).DescribeEvents(
                        new DescribeEventsRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                updateEventCallback.Invoke(r.Result.items);
                            }

                            end = true;
                        }
                    );

                    while (!end)
                    {
                        yield return new WaitForSeconds(1);
                    }

                } while (!status.EndsWith("_COMPLETE"));
            }
            
            callback.Invoke();
        }

        public static IEnumerator Uninstall(Manifest manifest, UnityAction<List<Event>> updateEventCallback, UnityAction callback)
        {
            var exit = false;
            foreach (var template in manifest.templates)
            {
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).DeleteStack(
                    new DeleteStackRequest()
                        .WithStackName(ToStackName(manifest, template)),
                    r =>
                    {
                        if (r.Error != null)
                        {
                            EditorUtility.DisplayDialog("Error", r.Error.Message, "OK");
                            exit = true;
                        }
                    }
                );
                if (exit)
                {
                    yield break;
                }
            }
            
            string status = null;
            foreach (var template in manifest.templates)
            {
                do
                {
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).GetStackStatus(
                        new GetStackStatusRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                status = r.Result.status;
                            }
                        }
                    );

                    yield return new WaitForSeconds(5);

                    var end = false;
                    yield return new Gs2DeployRestClient(
                        Context.Session
                    ).DescribeEvents(
                        new DescribeEventsRequest()
                            .WithStackName(ToStackName(manifest, template)),
                        r =>
                        {
                            if (r.Error == null)
                            {
                                updateEventCallback.Invoke(r.Result.items);
                            }

                            end = true;
                        }
                    );

                    while (!end)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    
                } while (!status.EndsWith("_COMPLETE"));
            }
            
            callback.Invoke();
        }
        
        public static IEnumerator GetOutputs(Manifest manifest, UnityAction<AsyncResult<DescribeOutputsResult>> callback)
        {
            foreach (var template in manifest.templates)
            {
                yield return new Gs2DeployRestClient(
                    Context.Session
                ).DescribeOutputs(
                    new DescribeOutputsRequest()
                        .WithStackName(ToStackName(manifest, template)),
                    callback
                );
            }
        }
    }
}
