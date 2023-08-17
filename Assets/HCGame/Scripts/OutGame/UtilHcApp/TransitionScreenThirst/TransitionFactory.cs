using System;
using Prime31.TransitionKit;
using UnityEngine;
using Random = UnityEngine.Random;

//NHÀ MÁY NƠI TẠO INSTANCE CÁC KIỂU TRANSITION TRƯỚC VỚI THÔNG SỐ CỦA NÓ
public static class TransitionFactorys
{
    private static readonly WindTransition _WindTransition = new WindTransition()
    {
        duration = 0.8f,
        size = 0.3f,
        windVerticalSegments = -100,
    };

    private static readonly WindTransition _CurveWindTransition = new WindTransition()
    {
        duration = 1f,
        size = 0.3f,
        useCurvedWind = true,
        windVerticalSegments = -300,
    };

    private static readonly DoorwayTransition _DoorwayTransition = new DoorwayTransition()
    {
        duration = 0.8f,
        perspective = 1.1f,
        runEffectInReverse = false
    };

    private static readonly RippleTransition _RippleTransition = new RippleTransition()
    {
        duration = 0.8f,
        amplitude = 1500f,
        speed = 20f
    };

    private static readonly TriangleSlicesTransition _TriangleSlicesTransition = new TriangleSlicesTransition()
    {
        divisions = Random.Range(2, 10),
        duration = 0.8f
    };

    private static readonly VerticalSlicesTransition _VerticalSlicesTransition = new VerticalSlicesTransition()
    {
        divisions = Random.Range(2, 10),
        duration = 0.8f
    };

    private static readonly FishEyeTransition _FishEyeTransition = new FishEyeTransition()
    {
        duration = 0.8f,
        size = 0.08f,
        zoom = 10.0f,
        colorSeparation = 3.0f
    };

    private static readonly PixelateTransition _PixelateTransition =
        new PixelateTransition()
        {
            finalScaleEffect = PixelateTransition.PixelateFinalScaleEffect.Zoom,
            duration = 1.0f,
        };

    private static readonly SquaresTransition _SquaresTransition =
        new SquaresTransition()
        {
            duration = 1.0f,
            squareSize = new Vector2(32f, 25f),
            squareColor = Color.black,
            smoothness = 0.1f
        };

    public static void RunTransition(TransitionType type, Action loadCallBack, Func<bool> checkLoadComplete)
    {
        TransitionKitDelegate t = null;
        switch (type)
        {
            case TransitionType.None:
                break;
            case TransitionType.Wind:
                t = _WindTransition;
                break;
            case TransitionType.CurveWind:
                t = _CurveWindTransition;
                break;
            case TransitionType.FishEye:
                t = _FishEyeTransition;
                break;
            case TransitionType.Ripple:
                t = _RippleTransition;
                break;
            case TransitionType.DoorWay:
                t = _DoorwayTransition;
                break;
            case TransitionType.TriangleSlices:
                t = _TriangleSlicesTransition;
                break;
            case TransitionType.Pixelate:
                t = _PixelateTransition;
                break;
            case TransitionType.VerticalSlices:
                t = _VerticalSlicesTransition;
                break;
            case TransitionType.Square:
                t = _SquaresTransition;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (t == null)
        {
            loadCallBack?.Invoke();
            return;
        }

        t.LoadingScreenTransition = loadCallBack;
        t.FuncLoadingScreen = checkLoadComplete;
        TransitionKit.instance.TransitionWithDelegate(t);
    }

    public static void RunRandomTransition(Action loadCallBack, Func<bool> checkLoadComplete)
    {
        var t = (TransitionType)Random.Range(0, Enum.GetValues(typeof(TransitionType)).Length);
        RunTransition(t, loadCallBack, checkLoadComplete);
    }
}

public enum TransitionType
{
    None,
    Wind,
    CurveWind,
    FishEye,
    Ripple,
    DoorWay,
    TriangleSlices,
    Pixelate,
    VerticalSlices,
    Square
}