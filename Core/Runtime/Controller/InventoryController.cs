using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Core;
using Gs2.Core.Model;
using Gs2.Core.Net;
using Gs2.Gs2Inventory;
using Gs2.Gs2Inventory.Request;
using Gs2.Unity;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2Inventory.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class InventoryController: MonoBehaviour
    {
        public static IEnumerator GetInventoryModel(
            Client client,
            string inventoryNamespaceName,
            string inventoryModelName,
            GetInventoryModelEvent onGetInventoryModel,
            ErrorEvent onError
        )
        {
            EzInventoryModel inventoryModel;
            {
                AsyncResult<EzGetInventoryModelResult> result = null;
                yield return client.Inventory.GetInventoryModel(
                    r => { result = r; },
                    inventoryNamespaceName,
                    inventoryModelName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                inventoryModel = result.Result.Item;
            }
            List<EzItemModel> itemModels;
            {
                AsyncResult<EzListItemModelsResult> result = null;
                yield return client.Inventory.ListItemModels(
                    r => { result = r; },
                    inventoryNamespaceName,
                    inventoryModelName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                itemModels = result.Result.Items;
            }

            onGetInventoryModel.Invoke(inventoryModelName, inventoryModel, itemModels);
        }

        public static IEnumerator GetInventory(
            Client client,
            GameSession session,
            string inventoryNamespaceName,
            string inventoryName,
            GetInventoryEvent onGetInventory,
            ErrorEvent onError
        )
        {
            EzInventory inventory;
            {
                AsyncResult<EzGetInventoryResult> result = null;
                yield return client.Inventory.GetInventory(
                    r => { result = r; },
                    session,
                    inventoryNamespaceName,
                    inventoryName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                inventory = result.Result.Item;
            }
            var itemSets = new List<EzItemSet>();
            string nextPageToken;
            do
            {
                AsyncResult<EzListItemsResult> result = null;
                yield return client.Inventory.ListItems(
                    r => { result = r; },
                    session,
                    inventoryNamespaceName,
                    inventoryName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                itemSets.AddRange(result.Result.Items);
                nextPageToken = result.Result.NextPageToken;
            } while (nextPageToken != null);

            onGetInventory.Invoke(inventory, itemSets);
        }

        public static IEnumerator Acquire(
            GameSession session,
            string identifierAcquireItemClientId,
            string identifierAcquireItemClientSecret,
            string inventoryNamespaceName,
            string inventoryModelName,
            string itemModelName,
            int value,
            AcquireEvent onAcquire,
            ErrorEvent onError
        )
        {
            // このコードは実際にアプリケーションで使用するべきではありません。
            // アプリ内から課金通貨の残高を加算できるようにすることは、事業に多大な悪い影響を及ぼす可能性があります。
            var restSession = new Gs2RestSession(
                new BasicGs2Credential(
                    identifierAcquireItemClientId,
                    identifierAcquireItemClientSecret
                )
            );
            var error = false;
            yield return restSession.Open(
                r =>
                {
                    if (r.Error != null)
                    {
                        onError.Invoke(r.Error);
                        error = true;
                    }
                }
            );

            if (error)
            {
                yield return restSession.Close(() => { });
                yield break;
            }

            var restClient = new Gs2InventoryRestClient(
                restSession
            );

            yield return restClient.AcquireItemSetByUserId(
                new AcquireItemSetByUserIdRequest()
                    .WithNamespaceName(inventoryNamespaceName)
                    .WithUserId(session.AccessToken.userId)
                    .WithInventoryName(inventoryModelName)
                    .WithItemName(itemModelName)
                    .WithAcquireCount(value),
                r =>
                {
                    if (r.Error != null)
                    {
                        onError.Invoke(r.Error);
                        error = true;
                    }
                    else
                    {
                        onAcquire.Invoke(
                            new EzInventory(r.Result.inventory),
                            r.Result.items.Select(item => new EzItemSet(item)).ToList(),
                            value
                        );
                    }
                }
            );
            
            yield return restSession.Close(() => { });
        }

        public static IEnumerator Consume(
            Client client,
            GameSession session,
            string inventoryNamespaceName,
            string inventoryModelName,
            string itemModelName,
            int consumeValue,
            ConsumeEvent onConsume,
            ErrorEvent onError,
            string itemSetName = null
        )
        {
            AsyncResult<EzConsumeResult> result = null;
            yield return client.Inventory.Consume(
                r =>
                {
                    result = r;
                },
                session,
                inventoryNamespaceName,
                inventoryModelName,
                itemModelName,
                consumeValue,
                itemSetName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var inventory = result.Result.Inventory;
            var itemSets = result.Result.Items;

            onConsume.Invoke(inventory, itemSets, consumeValue);
        }
    }
}