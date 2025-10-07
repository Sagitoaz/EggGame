using System;
using DG.Tweening;
using UnityEngine;

public enum EggAnimType { IdleBounce, MoveTo }

public struct EggAnimParams
{
    public Transform Root;          // Transform gốc của trứng (dùng để di chuyển vị trí)
    public Transform Visual;        // Transform phần "hiển thị" để tween scale (squash & stretch)
    public Transform Shadow;        // Transform bóng (đặt cố định ở mặt đất, chỉ tween scale/alpha), có thể null
    public SpriteRenderer ShadowSR; // SpriteRenderer của bóng để tween màu/alpha, có thể null
    public Vector3 BaseScale;       // Scale gốc của phần Visual (để hồi về sau mỗi nhịp)
    public Vector3 GroundPos;       // Vị trí "mặt đất" (node + offset), gốc để so sánh khi nảy lên/xuống
    public int Level;               // Level hiện tại của trứng
    public int TweenId;             // Id duy nhất cho DOTween.Kill (thường dùng GetInstanceID())
}

public static class EggAnimRunner
{
    public static event Action<EggAnimType, EggAnimParams> OnAnimStarted;

    public static event Action<EggAnimType, EggAnimParams> OnAnimCompleted;

    public static void Kill(EggAnimParams p)
    {
        DOTween.Kill(p.TweenId);
        if (p.Root) p.Root.DOKill();
        if (p.Visual) p.Visual.DOKill();
        if (p.Shadow) p.Shadow.DOKill();
    }

    public static void PlayIdleBounce(EggAnimParams p,
                                      float waitMin = 5f, float waitMax = 7f,
                                      float amp = 0.2f,
                                      float tAnt = 0.08f, float tUp = 0.18f,
                                      float tDown = 0.16f, float tImp = 0.10f, float tRec = 0.12f,
                                      float squashX = 1.10f, float squashY = 0.90f,
                                      float stretchX = 0.92f, float stretchY = 1.08f,
                                      float impactX = 1.14f, float impactY = 0.86f,
                                      float shUpX = 0.75f, float shUpY = 0.85f,
                                      float shImpX = 1.20f, float shImpY = 0.90f,
                                      float shAlphaUp = 0.40f, float shAlphaImp = 0.80f)
    {
        if (p.Level <= 1 || p.Root == null || p.Visual == null) return;

        Kill(p);

        OnAnimStarted?.Invoke(EggAnimType.IdleBounce, p);

        var id = p.TweenId;

        // Lấy scale gốc của bóng (nếu có) để tính tỷ lệ lên/xuống
        var baseX = p.Shadow ? p.Shadow.localScale.x : 1f;
        var baseY = p.Shadow ? p.Shadow.localScale.y : 1f;

        // Màu/alpha gốc của bóng (nếu có) để về trạng thái ban đầu
        var shBase = p.ShadowSR ? p.ShadowSR.color : Color.white;

        // Tính sẵn các vector scale cho các pha của trứng
        var squash = new Vector3(p.BaseScale.x * squashX, p.BaseScale.y * squashY, 1f); // dẹt (anticipation)
        var stretch = new Vector3(p.BaseScale.x * stretchX, p.BaseScale.y * stretchY, 1f); // căng (bay lên)
        var impact = new Vector3(p.BaseScale.x * impactX, p.BaseScale.y * impactY, 1f); // dẹt mạnh (chạm đất)

        // Tính sẵn scale của bóng theo từng pha
        var shUp = new Vector3(baseX * shUpX, baseY * shUpY, 1f); // bóng nhỏ lại khi trứng lên cao
        var shImp = new Vector3(baseX * shImpX, baseY * shImpY, 1f); // bóng bè khi trứng đập xuống

        // Tính sẵn màu/alpha của bóng theo từng pha
        var colUp = new Color(shBase.r, shBase.g, shBase.b, shAlphaUp);   // mờ hơn khi trứng lên
        var colImp = new Color(shBase.r, shBase.g, shBase.b, shAlphaImp);  // đậm hơn khi trứng chạm đất

        // Chờ ngẫu nhiên trước khi bắt đầu một nhịp (trông tự nhiên, không đồng bộ cứng)
        float wait = UnityEngine.Random.Range(waitMin, waitMax);

        // Lên lịch chạy sau "wait" giây
        DOVirtual.DelayedCall(wait, () =>
        {
            // Tạo một sequence mới cho nhịp nảy
            var seq = DOTween.Sequence().SetId(id);

            // 1) Anticipation (hơi dẹt xuống trước khi bật)
            seq.Append(p.Visual.DOScale(squash, tAnt).SetEase(Ease.OutQuad).SetId(id));
            if (p.Shadow && p.ShadowSR)
            {
                // Bóng thu nhẹ theo
                seq.Join(
                    p.Shadow.DOScale(new Vector3(baseX * 0.95f, baseY * 0.95f, 1f), tAnt)
                           .SetEase(Ease.OutQuad)
                           .SetId(id)
                );
            }

            // 2) Lên (trứng bay lên + stretch)
            seq.Append(
                p.Root.DOMoveY(p.GroundPos.y + amp, tUp) // Root di chuyển lên theo trục Y
                     .SetEase(Ease.OutQuad)
                     .SetId(id)
            );
            seq.Join(
                p.Visual.DOScale(stretch, tUp)          // Visual căng ra theo thời gian bay lên
                        .SetEase(Ease.OutQuad)
                        .SetId(id)
            );
            if (p.Shadow && p.ShadowSR)
            {
                // Bóng nhỏ và mờ hơn khi trứng lên cao
                seq.Join(p.Shadow.DOScale(shUp, tUp).SetEase(Ease.OutQuad).SetId(id));
                seq.Join(p.ShadowSR.DOColor(colUp, tUp).SetEase(Ease.OutQuad).SetId(id));
            }

            // 3) Xuống (trứng rơi xuống + impact)
            seq.Append(
                p.Root.DOMoveY(p.GroundPos.y, tDown) // Trở lại mặt đất
                     .SetEase(Ease.InQuad)
                     .SetId(id)
            );
            seq.Join(
                p.Visual.DOScale(impact, tImp)      // Dẹt mạnh khi chạm đất (impact)
                        .SetEase(Ease.InQuad)
                        .SetId(id)
            );
            if (p.Shadow && p.ShadowSR)
            {
                // Bóng bè & đậm khi chạm đất
                seq.Join(p.Shadow.DOScale(shImp, tImp).SetEase(Ease.InQuad).SetId(id));
                seq.Join(p.ShadowSR.DOColor(colImp, tImp).SetEase(Ease.InQuad).SetId(id));
            }

            // 4) Recover
            seq.Append(
                p.Visual.DOScale(p.BaseScale, tRec)
                        .SetEase(Ease.OutQuad)
                        .SetId(id)
            );
            if (p.Shadow && p.ShadowSR)
            {
                // Bóng về lại scale/màu ban đầu
                seq.Join(p.Shadow.DOScale(new Vector3(baseX, baseY, 1f), tRec).SetEase(Ease.OutQuad).SetId(id));
                seq.Join(p.ShadowSR.DOColor(shBase, tRec).SetEase(Ease.OutQuad).SetId(id));
            }

            // Khi hoàn tất một nhịp: bắn sự kiện và hẹn tiếp nhịp mới (loop "nhẹ nhàng")
            seq.OnComplete(() =>
            {
                OnAnimCompleted?.Invoke(EggAnimType.IdleBounce, p);

                // Đệ quy
                if (p.Level > 1 && p.Root && p.Visual)
                {
                    PlayIdleBounce(p, waitMin, waitMax, amp, tAnt, tUp, tDown, tImp, tRec,
                                   squashX, squashY, stretchX, stretchY, impactX, impactY,
                                   shUpX, shUpY, shImpX, shImpY, shAlphaUp, shAlphaImp);
                }
            });

        }).SetId(id);
    }
    public static void PlayMoveTo(EggAnimParams p, Vector3 targetPos, float duration, Action onComplete = null)
    {
        Kill(p);

        OnAnimStarted?.Invoke(EggAnimType.MoveTo, p);

        // Di chuyển Root tới vị trí đích trong "duration" giây
        p.Root.DOMove(targetPos, duration)
              .SetId(p.TweenId)
              .OnComplete(() =>
              {
                  onComplete?.Invoke();

                  OnAnimCompleted?.Invoke(EggAnimType.MoveTo, p);
              });
    }
}
