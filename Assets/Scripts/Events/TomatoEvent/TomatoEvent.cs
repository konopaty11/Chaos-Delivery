using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class TomatoEvent : GameEvent
{
    [SerializeField] GameObject prefabTomato;
    [SerializeField] List<Transform> tomatoTransfroms;

    float throwForce = 2.7f;
    const int countEvents = 3;

    public override int CountEvents => countEvents;

    public override IEnumerator Event()
    {
        while (true)
        {
            if (player == null) yield break;

            foreach (Transform tomatoTransform in tomatoTransfroms)
            {
                GameObject tomato = Instantiate(prefabTomato, tomatoTransform, false);

                Rigidbody rg = tomato.GetComponent<Rigidbody>();
                rg.linearVelocity = CalculateThrowVelocity(tomato.transform.position, player.transform.position, throwForce);
                rg.useGravity = true;

                yield return new WaitForSeconds(Random.Range(0.1f, 0.8f));
            }

            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }

    Vector3 CalculateThrowVelocity(Vector3 start, Vector3 end, float force)
    {
        float gravity = Physics.gravity.magnitude;
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);

        // �������� 1: �������� ������� �� ����
        if (displacementXZ.magnitude < 0.1f)
        {
            // ���� ������� ����� � ����, ������� ����������� �����
            return Vector3.up * force;
        }

        // �������� 2: ������������ ������ ���� ���� �����
        float minHeight = Mathf.Abs(displacementY) + 0.1f;

        // �������� 3: ��������� ����� ������ � ������� �� NaN
        float timeUp = Mathf.Sqrt(2 * minHeight / gravity);
        float timeDown = Mathf.Sqrt(2 * Mathf.Max(0, minHeight - displacementY) / gravity);

        float totalTime = timeUp + timeDown;

        // �������� 4: �������� ������� �� ����
        if (totalTime < 0.001f)
        {
            totalTime = 0.001f;
        }

        // ��������� ��������
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * minHeight);
        Vector3 velocityXZ = displacementXZ / totalTime;

        // �������� 5: ���������� ��� ��� NaN
        if (float.IsNaN(velocityXZ.x) || float.IsNaN(velocityXZ.y) || float.IsNaN(velocityXZ.z))
        {
            velocityXZ = displacementXZ.normalized * force;
        }

        return velocityXZ + velocityY * force;
    }
}