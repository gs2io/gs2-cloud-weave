using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Unity.Gs2Formation.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class FormationController: MonoBehaviour
    {
        public static IEnumerator GetMoldModel(
            Client client,
            string formationNamespaceName,
            string moldModelName,
            GetMoldModelEvent onGetMoldModel,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetMoldModelResult> result = null;
            yield return client.Formation.GetMoldModel(
                r =>
                {
                    result = r;
                },
                formationNamespaceName,
                moldModelName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var moldModel = result.Result.Item;

            onGetMoldModel.Invoke(moldModelName, moldModel);
        }

        public static IEnumerator ListForms(
            Client client,
            GameSession session,
            string formationNamespaceName,
            EzMoldModel moldModel,
            GetFormEvent onGetForm,
            ErrorEvent onError
        )
        {
            var forms = new List<EzForm>();
            string pageToken = null;
            while (true)
            {
                AsyncResult<EzListFormsResult> result = null;
                yield return client.Formation.ListForms(
                    r =>
                    {
                        result = r;
                    },
                    session,
                    formationNamespaceName,
                    moldModel.Name,
                    pageToken
                );
            
                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }
                
                forms.AddRange(result.Result.Items);
                pageToken = result.Result.NextPageToken;

                if (pageToken == null)
                {
                    break;
                }
            }

            for (var i = 0; i < forms.Count; i++)
            {
                onGetForm.Invoke(moldModel, i, forms[i]);
            }
        }

        public static IEnumerator SetForm(
            Client client,
            GameSession session,
            string formationNamespaceName,
            EzMoldModel moldModel,
            int index,
            List<EzSlotWithSignature> slots,
            string signatureKeyId,
            GetFormEvent onGetForm,
            UpdateFormEvent onUpdateForm,
            ErrorEvent onError
        )
        {
            AsyncResult<EzSetFormResult> result = null;
            yield return client.Formation.SetForm(
                r =>
                {
                    result = r;
                },
                session,
                formationNamespaceName,
                moldModel.Name,
                index,
                slots,
                signatureKeyId
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var form = result.Result.Item;

            onUpdateForm.Invoke(moldModel, index, form);
            onGetForm.Invoke(moldModel, index, form);
        }
    }
}