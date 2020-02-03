using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DropItem : MonoBehaviour
{
    [SerializeField] particleControl particle;
    [SerializeField] bool isFriendly, isUnFriendly;
    public List<afterPickItem> afterPickItemsList = new List<afterPickItem>();
    NavMeshAgent agent;
    bool startMovingToPlayer = false;
    public DrainingItemWrapper itemWrapper;
    DrainingItemWrapper parentWrapper;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Invoke("CreateWrapperAsParent", 0.5f);
    }

    private void CreateWrapperAsParent()
    {
        DrainingItemWrapper wrapper = Instantiate(itemWrapper, transform.position, Quaternion.identity);
        transform.parent = wrapper.transform;
        wrapper.item = this;
        parentWrapper = wrapper;
    }

    public void getItem(unitHitbox player)
    {
        if (isFriendly)
        {
            foreach (afterPickItem aft in afterPickItemsList)
            {
                aft.doActionAPI(player);
            }
            player.showParticleEffect(particle);
            Destroy(gameObject);
        }
    }
    public particleControl getParticle()
    {
        return particle;
    }

    public void addAfterPickItems(afterPickItem aft)
    {
        if (!afterPickItemsList.Contains(aft))
        {
            afterPickItemsList.Add(aft);
        }
    }
    public void removeAfterPickItems(afterPickItem aft)
    {
        if (afterPickItemsList.Contains(aft))
        {
            afterPickItemsList.Remove(aft);
        }
    }
    public void dropMove(Vector3 des)
    {
        StartCoroutine(moveto(des));
    }

    IEnumerator moveto(Vector3 des)
    {
        while (Vector3.Distance(des, transform.position) >= 0.01 && !startMovingToPlayer)
        {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.Lerp(transform.position, des, 0.5f);
        }

        if (parentWrapper != null) parentWrapper.startDraining = false;
    }

    public IEnumerator moveToPicker(Transform player)
    {
        startMovingToPlayer = true;
        unitHitbox picker = player.GetComponent<unitHitbox>();
        while (Vector3.Distance(player.position, transform.position) >= 0.01f && parentWrapper.item.CanPlayerPickItem(picker))
        {
            yield return new WaitForFixedUpdate();
            agent.Warp(Vector3.Lerp(transform.position, player.position, 0.5f));
        }

        if (parentWrapper != null) parentWrapper.startDraining = false;
    }

    public bool CanPlayerPickItem(unitHitbox picker)
    {
        foreach (afterPickItem aft in afterPickItemsList)
        {
            if (!aft.checkPickItemCondition(picker)) return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (parentWrapper != null) Destroy(parentWrapper.gameObject);
    }

}
