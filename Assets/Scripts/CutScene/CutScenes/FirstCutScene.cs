using System.Collections;
using Cinemachine;
using UnityEditor.Animations;
using UnityEngine;

public class FirstCutScene : CutScene
{
    [Header("Camers")]
    [SerializeField] CinemachineSmoothPath dollyTrack;

    [Header("Transform Camers")]
    [SerializeField] Transform TransformCamera_1;
    [SerializeField] Transform TransformCamera_2;
    [SerializeField] Transform TransformCamera_3;
    [SerializeField] Transform TransformCamera_4;

    [Header("Transforms")]
    [SerializeField] Transform head;
    [SerializeField] Transform transport;
    [SerializeField] Transform centerOfRotate;
    [SerializeField] Transform secondTransform;

    [Header("Animations")]
    [SerializeField] AnimatorController standController;
    [SerializeField] AnimatorController sitController;
    [SerializeField] Animator standAnimator;
    [SerializeField] Animator sitAnimator;

    [Header("Sounds")]
    [SerializeField] AudioClip stepsClip;

    readonly Plot plotProgress = Plot.Start;
    readonly string nameCutScene = "first";

    public override Plot PlotProgress => plotProgress;
    public override string Name { get => nameCutScene; }

    CinemachineVirtualCamera camera_1;
    CinemachineVirtualCamera camera_2;
    CinemachineVirtualCamera camera_3;
    CinemachineVirtualCamera camera_4;
    CinemachineVirtualCamera garageCamera;

    CinemachineTrackedDolly dolly;
    string standAnimName = "Stand";
    string sitAnimName = "Sit";

    string part2Key = "part 2";
    string firstOrderKey = "first order";

    Coroutine rotateCameraCoroutine;

    Vector3 startCameraPosition;
    Quaternion startCameraRotation;

    AudioSource audioSource;

    void OnEnable()
    {
        DialogManager.DialogEnd += Part1End;
        DialogManager.DialogEnd += Part2End;
    }

    void OnDisable()
    {
        DialogManager.DialogEnd -= Part1End;
        DialogManager.DialogEnd -= Part2End;
    }

    private void Start()
    {
        camera_1 = CameraManager.Instance.GetVirtualCamera(CameraType.CutScene1);
        camera_2 = CameraManager.Instance.GetVirtualCamera(CameraType.CutScene2);
        camera_3 = CameraManager.Instance.GetVirtualCamera(CameraType.CutScene3);
        camera_4 = CameraManager.Instance.GetVirtualCamera(CameraType.CutScene4);
        garageCamera = CameraManager.Instance.GetVirtualCamera(CameraType.GarageCamera);

        camera_1.transform.SetPositionAndRotation(TransformCamera_1.position, TransformCamera_1.rotation);
        camera_2.transform.SetPositionAndRotation(TransformCamera_2.position, TransformCamera_2.rotation);
        camera_3.transform.SetPositionAndRotation(TransformCamera_3.position, TransformCamera_3.rotation);
        camera_4.transform.SetPositionAndRotation(TransformCamera_4.position, TransformCamera_4.rotation);

        standAnimator.enabled = false;
        sitAnimator.enabled = false;
        standAnimator.runtimeAnimatorController = standController;
        sitAnimator.runtimeAnimatorController = sitController;

        audioSource = GetComponent<AudioSource>();
    }

    public override void StartCutScene()
    {
        StartCoroutine(SwingLegs());
        StartCoroutine(CutScenePart1());
        StartCoroutine(HeadRotation());
    }

    IEnumerator SwingLegs()
    {
        while (true)
        {
            sitAnimator.enabled = true;
            sitAnimator.Play(sitAnimName, 0 ,0f);

            float timeWait = sitController.animationClips[0].length * Random.Range(2, 4);
            yield return new WaitForSeconds(timeWait);
            sitAnimator.enabled = false;
            yield return new WaitForSeconds(Random.Range(1f, 8f));
        }
    }

    IEnumerator HeadRotation()
    {
        while (true)
        {
            head.LookAt(Camera.main.transform, Vector3.forward);
            head.localRotation = Quaternion.Euler(90 - head.eulerAngles.y, 0, -head.eulerAngles.x);

            yield return null;
        }
    }

    IEnumerator CutScenePart1()
    {
        CameraManager.Instance.TransitionCamera(CameraType.CutScene2);
        yield return new WaitForSeconds(0.2f);

        CameraManager.Instance.SetTransition(CinemachineBlendDefinition.Style.EaseInOut, 1.5f);
        CameraManager.Instance.TransitionCamera(CameraType.CutScene3);
        yield return new WaitForSeconds(2.5f);

        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Что-то скучно.", MessageID.CutScene1.PLAYER_1);

        standAnimator.enabled = true;
        standAnimator.Play(standAnimName);
        yield return new WaitForSeconds(2.5f);

        CameraManager.Instance.TransitionCamera(CameraType.CutScene1);
        yield return new WaitForSeconds(2f);

        SetTrackedDolly();

        audioSource.clip = stepsClip;
        audioSource.Play();

        float factorMove = 0.13f;
        float currentTimeMove = 0f;
        float maxTimeMove = 4f;
        while (currentTimeMove < maxTimeMove)
        {
            currentTimeMove += Time.deltaTime;
            dolly.m_PathPosition += Time.deltaTime * factorMove;
            yield return null;
        }

        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Ты принят.", MessageID.CutScene1.KAZAK_1);
        maxTimeMove = 4.5f;
        while (currentTimeMove < maxTimeMove)
        {
            currentTimeMove += Time.deltaTime;
            dolly.m_PathPosition += Time.deltaTime * factorMove;
            yield return null;
        }

        audioSource.Stop();

        CameraManager.Instance.SetTransition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
        yield return new WaitForSeconds(0.8f);
        CameraManager.Instance.TransitionCamera(CameraType.CutScene4);

        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Что?", MessageID.CutScene1.PLAYER_2);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Правильнее будет сказать - Куда?", MessageID.CutScene1.KAZAK_2);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Куда?", MessageID.CutScene1.PLAYER_3);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "В лучший сервис доставки.", MessageID.CutScene1.KAZAK_3);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Доставки чего?", MessageID.CutScene1.PLAYER_4);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Всего.", MessageID.CutScene1.KAZAK_4);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "А ты вообще кто?", MessageID.CutScene1.PLAYER_5);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Данияр.", MessageID.CutScene1.KAZAK_5);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Что у тебя с голосом, ты же ребёнок.", MessageID.CutScene1.PLAYER_6);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Традиционный казакский ритуал по сжиганию гланд.", MessageID.CutScene1.KAZAK_6);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Окак.", MessageID.CutScene1.PLAYER_7);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Можешь начать работать прямо сейчас.", MessageID.CutScene1.KAZAK_7);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Нууу... Хорошо, давай.", MessageID.CutScene1.PLAYER_8);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Ура!", MessageID.CutScene1.KAZAK_8, part2Key);
    }
    
    void Part1End(string key)
    {
        if (key != part2Key) return;

        StartCoroutine(CutScenePart2());
    }

    IEnumerator CutScenePart2()
    {
        Upgrades transportUpgrade = transport.GetComponent<Upgrades>();
        if (transportUpgrade != null)
            GarageManager.Instance.TransportEnableControl(true, transportUpgrade.UpgradeData.transportType);
        else
            Debug.Log("Не найден upgrades");

        float transitionDuartion = 1.5f;
        CameraManager.Instance.SetTransition(CinemachineBlendDefinition.Style.EaseInOut, transitionDuartion);
        CameraManager.Instance.TransitionCamera(CameraType.GarageCamera);
        yield return new WaitForSeconds(transitionDuartion);

        Transform standTransform = standAnimator.transform;
        standAnimator.enabled = false;
        standTransform.SetPositionAndRotation(secondTransform.position, secondTransform.rotation);

        rotateCameraCoroutine = StartCoroutine(RotateGarageCamera());

        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Это твой первый транспорт!", MessageID.CutScene1.KAZAK_9);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "А если я не хочу на этом ехать.", MessageID.CutScene1.PLAYER_9);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "У тебя есть казах-коины?", MessageID.CutScene1.KAZAK_10);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Нет. Я в них не верю.", MessageID.CutScene1.PLAYER_10);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "Зря! Скоро один казах-коин будет стои...", MessageID.CutScene1.KAZAK_11);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Ладно, ладно. Как мне их получить?", MessageID.CutScene1.PLAYER_11);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "За каждый доставленный заказ, ты будешь получать монеты - казах-коины.", MessageID.CutScene1.KAZAK_12);
        DialogManager.Instance.ShowDialog(Person.KAZAK, PersonID.Kazak, "На них ты сможешь купить новый транспорт или улучшить текущий.", MessageID.CutScene1.KAZAK_13);
        DialogManager.Instance.ShowDialog(Person.PLAYER, PersonID.Player, "Хорошо.", MessageID.CutScene1.PLAYER_12, firstOrderKey);

        yield return null;
    }

    IEnumerator RotateGarageCamera()
    {
        garageCamera = CameraManager.Instance.GetVirtualCamera(CameraType.GarageCamera);
        garageCamera.LookAt = transport;

        startCameraPosition = garageCamera.transform.position;
        startCameraRotation = garageCamera.transform.rotation;

        CinemachineComposer composer = garageCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer == null)
            composer = garageCamera.AddCinemachineComponent<CinemachineComposer>();

        composer.m_HorizontalDamping = 0f;
        composer.m_VerticalDamping = 0f;
        composer.m_TrackedObjectOffset = new Vector3(0f, 1.2f, 0f);

        float radiusRotate = Vector3.Distance(garageCamera.transform.position, centerOfRotate.position);

        float angleStep = 0.08f * Mathf.Deg2Rad;

        Vector3 direction = garageCamera.transform.position - centerOfRotate.position;
        float currentAngle = Mathf.Atan2(direction.x, direction.y);

        while (true)
        {
            currentAngle += angleStep;

            garageCamera.transform.position = Vector3.Lerp
                (
                garageCamera.transform.position, 
                GetCirclePosition(currentAngle, garageCamera.transform.position.y, radiusRotate), 
                Time.deltaTime
                );

            yield return null;
        }
    }

    Vector3 GetCirclePosition(float angle, float yCoord, float radius)
    {
        return new Vector3
            (
            centerOfRotate.position.x + Mathf.Cos(angle) * radius,
            yCoord,
            centerOfRotate.position.z + Mathf.Sin(angle) * radius
            );
    }

    void SetTrackedDolly()
    {
        // Получаем или добавляем компонент Tracked Dolly
        dolly = camera_1.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (dolly == null)
        {
            // Если компонента нет - добавляем
            dolly = camera_1.AddCinemachineComponent<CinemachineTrackedDolly>();
        }

        // Настраиваем Tracked Dolly
        dolly.m_Path = dollyTrack;
        dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
        dolly.m_PathPosition = 0f; // Начальная позиция

        // Меняем тип Body на Tracked Dolly
        camera_1.m_Lens.Dutch = 0f;
    }

    void Part2End(string key)
    {
        if (key != firstOrderKey) return;
        StartCoroutine(CutScenePart3());
    }

    IEnumerator CutScenePart3()
    {
        UIManager.Instance.ShowUI("LoadScreen");
        yield return new WaitForSeconds(0.5f);

        StopCoroutine(rotateCameraCoroutine);
        garageCamera.transform.SetPositionAndRotation(startCameraPosition, startCameraRotation);
        garageCamera.LookAt = null;
        garageCamera.DestroyCinemachineComponent<CinemachineComposer>();
        GarageManager.Instance.TransportEnableControl(false, TransportType.Bike);

        ExitFromCutScene();

        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.HideUI("LoadScreen");

    }

    void ExitFromCutScene()
    {
        standAnimator.gameObject.SetActive(false);
        CameraManager.Instance.SetTransition(CinemachineBlendDefinition.Style.Cut);

        TransportManager.Instance.TransportEnableControl(true, TransportType.Bike);

        UIManager.Instance.ShowUI("Start work");
        UIManager.Instance.ShowUI("Main");
    }

    public override void SkipCutScene()
    {
        ExitFromCutScene();
    }
}
