
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
public class GameManager : MonoBehaviour
{
    public static GameManager Ins;
    [Header("Reference")]
    [SerializeField] CharacterDatabase characterDatabase;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] SeedDatabase seedDataBase; // Cơ sở dữ liệu cây trồng chung cho toàn game
    public event Action<PlayerData, CharacterDataSO> onLoadDataCompleted;

    [SerializeField] int _coins;

    //phần mới animal
    public AnimalDataSO AnimalData;
    private Transform animalSpanwPoint;
    
    public int Coin 
    {
        get => _coins;
        set
        {
            if (_coins != value)
            {
                _coins = value;
                GameEventManager.Ins.CoinChange(_coins);
            }
        }
    }
    private Vector3 nextPlayerPosition; // vị trí người chơi sau khi chuyển scene
    private async void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        var task = await Save_Load_Firebase.LoadData("Coins");
        if (task.Exists)
        {
            Coin = Convert.ToInt32(task.Value);
        }
        else
        {
            Coin = 500;// số tiền mặc định cho beginer
        }
        GameEventManager.Ins.CoinChange(_coins);
    }
    //Để Script giúp cho GManager "lắng nghe" sự kiện khi scene thay đổi không bị mất đi
    // Đăng ký "lắng nghe" sự kiện khi scene thay đổi


    private async void Start()
    {
        try
        {
            var task = await Save_Load_Firebase.LoadData("PlayerData");
            if (task.Exists)
            {
                PlayerData data = JsonConvert.DeserializeObject<PlayerData>(task.Value.ToString());
                CharacterDataSO characterDataSO = characterDatabase.characters[data.indexAsset];
                onLoadDataCompleted?.Invoke(data, characterDataSO);
            }
            else
            {
                onLoadDataCompleted?.Invoke(null, characterDatabase.characters[0]);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Hủy đăng ký để tránh lỗi
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   

    /// <summary>
    /// Hàm này sẽ được tự động gọi MỖI KHI một scene mới được tải xong.
    /// </summary>
    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //tải lại cây trồng khi load scene
        string path = scene.path;
        //chỉ load khi vào scene Play;
        if (!path.StartsWith("Assets/Scenes/ScenePlay"))
        {
            return;
        }
        if(scene.name == "Farm"){
        var task = await Save_Load_Firebase.LoadData("SeedData");
            if (task.Exists)
            {
                List<SeedSaveData> seedDatas = JsonConvert.DeserializeObject<List<SeedSaveData>>(task.Value.ToString());
                LoadCropsForScene(scene.name, seedDatas);
            }
        }
        MovePlayerToPosition();


        // Kiểm tra nếu đây là scene game (không phải menu) (test animal)
        if (scene.name != "PersistentSystems" && scene.name != "CustomizeCharacter" /*...*/)
        {
            FindSpawnPoint(scene.name);
        }
    }
    //Hàm được Portal gọi để bắt đầu chuyển scene
    public async void StartSceneTransition(string sceneName, Vector3 newPos)
    {
        await SaveCurrentSceneState(); //nếu cần lưu trạng thái hiện tại thì làm ở đây
        //Lưu lại vị trí mà người chơi sẽ đến
        this.nextPlayerPosition = newPos;

        // Gọi UIManager để bật hiệu ứng fade-out đen màn hình ở đây) // làm sau
        if (LoadingScene.Ins != null)
            LoadingScene.Ins.LoadScene(sceneName, "Loading...", LoadSceneMode.Single, true);
        else SceneManager.LoadScene(sceneName);
    }
    //hàm để di chuyển người chơi đến vị trí đã lưu sau khi tải xong scene mới
    private void MovePlayerToPosition()
    {
        // Chúng ta chỉ di chuyển nếu nextPlayerPosition đã được thiết lập (khác (0,0,0))
        if (this.nextPlayerPosition != Vector3.zero)
        {
            // Tìm GameObject người chơi bằng Tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position = this.nextPlayerPosition;
                Debug.Log($"Đã di chuyển Player đến vị trí: {nextPlayerPosition}");

                // Reset lại để lần sau không bị di chuyển nhầm
                this.nextPlayerPosition = Vector3.zero;
            }
            else
            {
                Instantiate(playerPrefab, this.nextPlayerPosition, Quaternion.identity);
            }
        }
    }
    private async Task SaveCurrentSceneState()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        //chỉ lưu khi là game scene có thể trồng cây
        Debug.Log(currentScene);
        if(currentScene.Equals("Farm"))
        {
            List<SeedSaveData> seedSaveDatas = new List<SeedSaveData>();
            Seed[] cropsInScene = FindObjectsByType<Seed>(FindObjectsSortMode.None);
            Debug.Log(cropsInScene.Length);
            if (cropsInScene == null || cropsInScene.Length == 0)
            {
                await Save_Load_Firebase.RemoveAsync("SeedData");
                return;
            }
            foreach (Seed crop in cropsInScene)
            {
                seedSaveDatas.Add(crop.GetSaveData());
            }
         
            await Save_Load_Firebase.SaveData("SeedData", JsonConvert.SerializeObject(seedSaveDatas));
        }
    }

    public async void LoadCropsForScene(string sceneName, List<SeedSaveData> data)
    {
        if(seedDataBase == null)
        {
            Debug.LogError("Crop Database is not assigned in GameManager!");
            return;
        }
        var task = await Save_Load_Firebase.GetSeverDateTime();
        DateTime curTime;
        if (task.HasValue)
        {
            curTime = task.Value;
        }
        else
        {
            curTime = DateTime.UtcNow;
        }
        if (data == null) return;

        foreach(SeedSaveData cropData in data)
        {
            if(cropData.SceneName == sceneName)
            {
                SeedDataSO dataAsset = seedDataBase.GetSeedDataByID(cropData.cropDataID);
                var cropPrefab = dataAsset.cropData.prefab;
                if(dataAsset != null && cropPrefab != null)
                {
                    GameObject cropInstance = Instantiate(cropPrefab, cropData.worldPosition.ToVector3(), Quaternion.identity);
                    Seed cropScript = cropInstance.GetComponent<Seed>();
                    if(cropScript != null)
                    {
                        cropScript.LoadCropState(dataAsset, cropData.timeHarvest, curTime);
                    }
                }
            }
        }

     
    }
    private async void OnDestroy()  
    {
        await SaveCurrentSceneState();
        await Save_Load_Firebase.SaveData("Coins", _coins);
    }

    private void OnValidate()
    {
        if(characterDatabase == null)
        {
            characterDatabase = Resources.Load<CharacterDatabase>("CharacterData");
        }
    }


    //animal test
    private void Update()
    {
        // Khi nhấn phím F9 (ví dụ)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (AnimalData != null && animalSpanwPoint != null)
            {
                BuyAnimal(AnimalData);
            }
        }
    }
    public void BuyAnimal(AnimalDataSO animal)
    {
        // Kiểm tra xem đã tìm thấy spawn point chưa
        if (animalSpanwPoint == null)
        {
            Debug.LogError($"Không thể mua {animal._itemName}: AnimalSpawnPoint chưa được tìm thấy trong scene này!");
            return; // Dừng lại nếu không có spawn point
        }

        Debug.Log($"Đang mua {animal._itemName}...");
        Vector3 spawnPos = animalSpanwPoint.position;
        spawnPos.z = 0f; // Đảm bảo Z=0
        GameObject animalObj = Instantiate(animal.animalPrefab, spawnPos, Quaternion.identity);
        animalObj.GetComponent<FarmAnimal>().Spawn(animal);
    }
    /// Tự động tìm Spawn Point trong scene mới dựa vào TÊN.
    /// </summary>
    private void FindSpawnPoint(string sceneName)
    {
        // Chỉ tìm nếu chúng ta ở đúng scene
        if (sceneName == "Farm") // Thay "Farm" bằng tên scene farm của bạn
        {
            // Tìm GameObject bằng TÊN
            // Đảm bảo tên này KHỚP 100% với tên GameObject trong Hierarchy
            GameObject spawnObj = GameObject.Find("AnimalSpawnPoint");

            if (spawnObj != null)
            {
                animalSpanwPoint = spawnObj.transform;
                Debug.Log("GameManager đã tự động tìm thấy AnimalSpawnPoint!");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy 'AnimalSpawnPoint' trong scene Farm! Hãy kiểm tra lại tên.");
                animalSpanwPoint = null;
            }
        }
        else
        {
            // Nếu là scene khác (Town, Mine...), chúng ta không cần spawn point
            animalSpanwPoint = null;
        }
    }
}
