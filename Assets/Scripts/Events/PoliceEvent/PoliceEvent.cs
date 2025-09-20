using System.Collections;
using UnityEngine;

public class PoliceEvent : GameEvent
{
    const int countEvent = 4;
    public override int CountEvents => countEvent;

    public override IEnumerator Event()
    {
        Rigidbody rg = player.GetComponent<TransportController>().Rg;

        float maxSpeed = 15f;
        while (true)
        {
            float speeding = rg.linearVelocity.magnitude - maxSpeed;
            if (speeding > 0)
            {
                float smallFinePercent = 0.1f;
                float bigFinePercent = 0.2f;
                int fine = (int) (CoinsManager.Instance.Coins * (speeding > 5f ? bigFinePercent : smallFinePercent));

                PromptManager.Instance.ShowPrompt($"Вы превысили скорость! Списан штраф в размере {fine}.", 6f, true);
                CoinsManager.Instance.Coins -= fine;

                yield break;
            }

            yield return null;
        }
    }
}
