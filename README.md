# 💣 Bomberman Game

Unity ile geliştirilmiş, **Design Pattern**'ler kullanılarak yapılandırılmış klasik Bomberman oyunu. Bu proje, yazılım mühendisliği prensiplerine uygun olarak temiz, sürdürülebilir ve genişletilebilir bir mimari sunmaktadır.

## 📋 İçindekiler
- [Proje Hakkında](#proje-hakkında)
- [Kullanılan Design Pattern'ler](#kullanılan-design-patternler)
- [Kurulum](#kurulum)
- [Oynanış](#oynanış)
- [Teknik Detaylar](#teknik-detaylar)
- [İletişim](#iletişim)

## 🎮 Proje Hakkında

Bu proje, klasik Bomberman oyununun modern yazılım mimarisi prensipleri kullanılarak geliştirilmiş bir versiyonudur. Oyun, **nesne yönelimli programlama (OOP)** ve **design pattern**'ler kullanılarak kod kalitesinin, sürdürülebilirliğinin ve genişletilebilirliğinin artırılması amacıyla tasarlanmıştır.

### Özellikler
- Klasik Bomberman oyun mekaniği
- Çok oyunculu mod desteği
- Power-up sistemi (bomba sayısı, patlama menzili, hız artırma)
- Farklı stratejilerle düşman yapay zekası
- Kullanıcı kayıt ve giriş sistemi
- Skor kaydetme ve kazananı belirleme
- Yıkılabilir ve güçlendirilmiş duvarlar
- Temiz ve modüler kod yapısı

## 🏗️ Kullanılan Design Pattern'ler

### 1. **Singleton Pattern**
**Amaç:** Oyun genelinde tek bir instance'ı olan yönetici sınıfların kontrolü.

**Kullanım Alanları:**
- `GameManager`: Oyun durumunu, oyuncu kontrolünü ve kazanan belirleme mantığını yönetir
- `SessionManager`: Giriş yapan oyuncu bilgisini session boyunca tutar

**Faydaları:**
- Global erişim noktası sağlar
- Oyun durumunun tutarlılığını garanti eder
- Sahne geçişlerinde veri kaybını önler (DontDestroyOnLoad)

**Kod Örneği:**
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) 
        { 
            DestroyImmediate(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        }
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
- GameManager oyun akışını kontrol eder ve oyuncu ölüm olaylarını dinler
- SessionManager giriş yapan oyuncunun adını saklar ve sahneler arası taşır

---

### 2. **Decorator Pattern**
**Amaç:** Bomba özelliklerini runtime'da dinamik olarak genişletmek.

**Kullanım Alanları:**
- `BombStructures.cs`: Bomba patlama menzilini artırmak için decorator yapısı kullanılır
- `RadiusEnhancer`: BasicBombStats'ı dekore ederek patlama yarıçapını artırır

**Faydaları:**
- Bomba özelliklerini kalıcı olarak değiştirmeden geçici iyileştirmeler yapılabilir
- Yeni özellikler eklemek için mevcut kodu değiştirmeye gerek yoktur (Open/Closed Principle)
- Power-up'lar aracılığıyla bomba yetenekleri katmanlar halinde artırılabilir

**Kod Örneği:**
```csharp
public interface IBombStats
{
    int GetRadius(); 
}

public class BasicBombStats : IBombStats
{
    public int GetRadius() => 1; 
}

public abstract class BombDecorator : IBombStats
{
    protected IBombStats _wrappedBomb;
    
    public BombDecorator(IBombStats bomb)
    {
        _wrappedBomb = bomb;
    }
    
    public virtual int GetRadius() => _wrappedBomb.GetRadius();
}

public class RadiusEnhancer : BombDecorator
{
    public RadiusEnhancer(IBombStats bomb) : base(bomb) { }
    
    public override int GetRadius()
    {
        return _wrappedBomb.GetRadius() + 1;
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// BombController.cs içinde
public void IncreaseBlastRadius()
{
    _currentBombStats = new RadiusEnhancer(_currentBombStats);
    Debug.Log("PowerUp Alındı! Menzil: " + _currentBombStats.GetRadius());
}
```
Her power-up alındığında mevcut bomba istatistiği yeni bir decorator ile sarılır ve menzil artar.

---

### 3. **Factory Pattern**
**Amaç:** Bomba nesnelerinin oluşturulmasını merkezileştirmek ve farklı bomba türleri eklemeyi kolaylaştırmak.

**Kullanım Alanları:**
- `BombFactory`: Soyut fabrika sınıfı bomba üretim arayüzünü tanımlar
- `BasicBombFactory`: Temel bombaları üreten concrete factory implementasyonu

**Faydaları:**
- Bomba oluşturma mantığını tek noktada toplar
- Yeni bomba türleri eklemek için sadece yeni factory oluşturmak yeterlidir
- ScriptableObject kullanımı sayesinde Unity Editor'den kolayca ayarlanabilir

**Kod Örneği:**
```csharp
// Soyut Factory
public abstract class BombFactory : ScriptableObject
{
    public abstract GameObject CreateBomb(Vector2 position);
}

// Concrete Factory
[CreateAssetMenu(fileName = "BasicBombFactory", menuName = "Bomb/Basic Factory")]
public class BasicBombFactory : BombFactory
{
    public GameObject bombPrefab;
    
    public override GameObject CreateBomb(Vector2 position)
    {
        if (bombPrefab == null) return null;
        return Instantiate(bombPrefab, position, Quaternion.identity);
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// BombController.cs içinde
[Header("Factory")]
public BombFactory bombFactory;

private IEnumerator PlaceBomb()
{
    Vector2 position = transform.position;
    GameObject bomb = bombFactory.CreateBomb(position);
    // ...
}
```

---

### 4. **Strategy Pattern**
**Amaç:** Düşman davranışlarını dinamik olarak değiştirilebilir algoritmalar olarak tanımlamak.

**Kullanım Alanları:**
- `IEnemyStrategy`: Düşman hareket stratejilerinin arayüzü
- `FoolStrategy`: Rastgele hareket eden basit düşman stratejisi
- `NormalStrategy`: Belirli aralıklarla yön değiştiren orta seviye strateji
- `CleverStrategy`: Oyuncuyu takip eden akıllı düşman stratejisi

**Faydaları:**
- Her düşman türü için farklı hareket algoritmaları kullanılabilir
- Strateji runtime'da değiştirilebilir
- Yeni düşman davranışları eklemek için mevcut kodu değiştirmeye gerek yok
- Kod tekrarını önler ve test edilebilirliği artırır

**Kod Örneği:**
```csharp
public interface IEnemyStrategy
{
    Vector2 GetDirection(); 
    void OnHitWall();
}

// Basit rastgele hareket stratejisi
public class FoolStrategy : IEnemyStrategy
{
    private Vector2 _direction;
    private Vector2[] _allDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    
    public FoolStrategy()
    {
        _direction = _allDirections[Random.Range(0, _allDirections.Length)];
    }
    
    public Vector2 GetDirection() => _direction;
    
    public void OnHitWall()
    {
        // Duvara çarpınca rastgele yön değiştir
        _direction = _allDirections[Random.Range(0, _allDirections.Length)];
    }
}

// Oyuncuyu takip eden akıllı strateji
public class CleverStrategy : IEnemyStrategy
{
    private Transform _enemyTransform;
    private Transform _playerTransform;
    
    public CleverStrategy(Transform enemy)
    {
        _enemyTransform = enemy;
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    public Vector2 GetDirection()
    {
        if (_playerTransform == null) return Vector2.right;
        
        Vector2 toPlayer = (_playerTransform.position - _enemyTransform.position).normalized;
        
        // X veya Y ekseninde daha yakın olana göre hareket et
        if (Mathf.Abs(toPlayer.x) > Mathf.Abs(toPlayer.y))
            return toPlayer.x > 0 ? Vector2.right : Vector2.left;
        else
            return toPlayer.y > 0 ? Vector2.up : Vector2.down;
    }
    
    public void OnHitWall()
    {
        // Duvara çarpınca perpendicular yöne geç
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// EnemyController.cs içinde
private IEnemyStrategy _moveStrategy;

private void Start()
{
    _moveStrategy = new NormalStrategy(); // Strateji seçimi
    _currentDirection = _moveStrategy.GetDirection();
}

private void FixedUpdate()
{
    if (_moveStrategy != null)
    {
        RaycastHit2D hit = Physics2D.Raycast(_rb.position, _currentDirection, 0.6f, wallLayer);
        
        if (hit.collider != null)
        {
            _moveStrategy.OnHitWall(); // Strateji kendi mantığını uygular
            _currentDirection = _moveStrategy.GetDirection();
        }
    }
}
```

---

### 5. **Observer Pattern**
**Amaç:** Oyun olaylarında gevşek bağlı iletişim sağlamak ve bağımlılıkları azaltmak.

**Kullanım Alanları:**
- `IGameSubject`: Observer'ları yöneten Subject arayüzü
- `IGameObserver`: Olayları dinleyen Observer arayüzü
- `MovementController`: Oyuncu ölümü gibi olayları bildirir
- `GameManager`: Oyuncu ölüm olaylarını dinler ve kazananı belirler

**Faydaları:**
- Sınıflar arasında gevşek bağlantı (loose coupling) sağlar
- Bir olay gerçekleştiğinde birden fazla nesne bilgilendirilebilir
- Yeni observer'lar eklemek mevcut kodu değiştirmeyi gerektirmez

**Kod Örneği:**
```csharp
public interface IGameObserver
{
    void OnNotify(string eventName, object data);
}

public interface IGameSubject
{
    void AddObserver(IGameObserver observer);
    void RemoveObserver(IGameObserver observer);
    void NotifyObservers(string eventName);
}

// Subject Implementation
public class MovementController : MonoBehaviour, IGameSubject
{
    private List<IGameObserver> _observers = new List<IGameObserver>();
    
    public void DeathSequence()
    {
        this.enabled = false;
        // Ölüm animasyonu...
        Invoke(nameof(OnDeathComplete), 1.25f);
    }
    
    private void OnDeathComplete()
    {
        gameObject.SetActive(false);
        NotifyObservers("PlayerDied"); // Tüm observer'lara bildir
    }
    
    public void AddObserver(IGameObserver observer)
    {
        if (!_observers.Contains(observer)) 
            _observers.Add(observer);
    }
    
    public void NotifyObservers(string eventName)
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnNotify(eventName, this);
        }
    }
}

// Observer Implementation
public class GameManager : MonoBehaviour, IGameObserver
{
    public void OnNotify(string eventName, object data)
    {
        if (eventName == "PlayerDied")
        {
            CheckWinState(); // Kazananı kontrol et
        }
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// GameManager.cs içinde
private void Start()
{
    players = GameObject.FindGameObjectsWithTag("Player");
    
    // Her oyuncuyu observe et
    foreach (GameObject playerObj in players)
    {
        MovementController playerController = playerObj.GetComponent<MovementController>();
        if (playerController != null)
        {
            playerController.AddObserver(this);
        }
    }
}
```

---

### 6. **State Pattern**
**Amaç:** Oyuncu davranışlarını farklı durumlara göre organize etmek.

**Kullanım Alanları:**
- `IPlayerState`: Oyuncu durumlarının arayüzü
- `PlayerAliveState`: Oyuncu hayattayken input işleme durumu
- `PlayerDeadState`: Oyuncu öldüğünde input'ları devre dışı bırakan durum

**Faydaları:**
- Karmaşık if-else yapılarını ortadan kaldırır
- Her durumun mantığını izole eder
- Yeni durumlar eklemek kolaydır
- Kod okunabilirliğini artırır

**Kod Örneği:**
```csharp
public interface IPlayerState
{
    void HandleInput();
    void UpdateState();
}

// Oyuncu hayattayken
public class PlayerAliveState : IPlayerState
{
    private MovementController _player;
    
    public PlayerAliveState(MovementController player)
    {
        _player = player;
    }
    
    public void HandleInput()
    {
        if (_player != null)
        {
            _player.HandleInput(); // Normal input işleme
        }
    }
    
    public void UpdateState() { }
}

// Oyuncu öldüğünde
public class PlayerDeadState : IPlayerState
{
    private MovementController _player;
    
    public PlayerDeadState(MovementController player)
    {
        _player = player;
    }
    
    public void HandleInput() 
    { 
        // Ölüyken input kabul etme
    }
    
    public void UpdateState() { }
}
```

**Kullanım Senaryosu:**
Bu pattern, oyuncu durumlarını (alive, dead, stunned, invincible vb.) yönetmek için kullanılabilir. Her durum kendi input ve update mantığına sahiptir.

---

### 7. **Adapter Pattern**
**Amaç:** Farklı input sistemlerini (klavye, gamepad, AI) tek bir arayüz üzerinden yönetmek.

**Kullanım Alanları:**
- `IInputAdapter`: Genel input arayüzü
- `KeyboardInputAdapter`: Klavye girdilerini adapter arayüzüne çevirir

**Faydaları:**
- Input sistemi kolayca değiştirilebilir (klavye, gamepad, AI)
- Çok oyunculu modda her oyuncu farklı input kullanabilir
- Test için mock input adapter'lar oluşturulabilir
- Input remapping kolaylaşır

**Kod Örneği:**
```csharp
public interface IInputAdapter
{
    Vector2 GetDirection(); 
    bool IsActionPressed(); 
}

public class KeyboardInputAdapter : IInputAdapter
{
    private KeyCode _up, _down, _left, _right, _action;
    
    public KeyboardInputAdapter(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode action)
    {
        _up = up;
        _down = down;
        _left = left;
        _right = right;
        _action = action;
    }
    
    public Vector2 GetDirection()
    {
        if (Input.GetKey(_up)) return Vector2.up;
        if (Input.GetKey(_down)) return Vector2.down;
        if (Input.GetKey(_left)) return Vector2.left;
        if (Input.GetKey(_right)) return Vector2.right;
        return Vector2.zero;
    }
    
    public bool IsActionPressed()
    {
        return Input.GetKeyDown(_action);
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// MovementController.cs içinde
private IInputAdapter _inputAdapter;

private void Awake()
{
    // Klavye input adapter'ı oluştur
    _inputAdapter = new KeyboardInputAdapter(inputUp, inputDown, inputLeft, inputRight, inputBomb);
}

public void HandleInput()
{
    Vector2 inputDir = _inputAdapter.GetDirection(); // Adapter üzerinden input al
    
    if (inputDir == Vector2.up)
        SetDirection(Vector2.up, spriteRendererUp);
    // ...
}
```

**Gelecekte Eklenebilecek Adapter'lar:**
- `GamepadInputAdapter`: Xbox/PlayStation controller desteği
- `AIInputAdapter`: Bot oyuncular için yapay zeka kontrolü
- `TouchInputAdapter`: Mobil cihazlar için dokunmatik kontrol

---

### 8. **Repository Pattern**
**Amaç:** Veri erişim katmanını soyutlamak ve oyun verilerinin kalıcılığını yönetmek.

**Kullanım Alanları:**
- `IPlayerRepository`: Oyuncu verilerinin soyut erişim arayüzü
- `FileRepository`: Dosya sistemine yazma/okuma implementasyonu

**Faydaları:**
- Veri kaynağı değiştirilebilir (dosya, veritabanı, cloud)
- Business logic'i veri erişiminden ayırır
- Test edilebilirliği artırır (mock repository kullanılabilir)
- Veri işleme mantığını merkezileştirir

**Kullanım Örneği:**
```csharp
public interface IPlayerRepository
{
    bool RegisterUser(string username, string password);
    bool LoginUser(string username, string password);
    void SaveWin();
    int GetHighScore();
}

public class FileRepository : IPlayerRepository
{
    // Dosya sistemine yazma/okuma implementasyonu
    public bool RegisterUser(string username, string password)
    {
        // Kullanıcıyı dosyaya kaydet
    }
    
    public void SaveWin()
    {
        // Kazanma kaydını dosyaya yaz
    }
}
```

**Proje İçindeki Gerçek Kullanım:**
```csharp
// GameManager.cs içinde
private IPlayerRepository _playerRepository;

private void Awake()
{
    _playerRepository = new FileRepository();
}

public void CheckWinState()
{
    // Kazanan belirlendikten sonra
    if (_playerRepository != null)
    {
        _playerRepository.SaveWin(); // Repository üzerinden kaydet
    }
}
```

## 🚀 Kurulum

1. Projeyi klonlayın:
```bash
git clone https://github.com/Merttyillmazz/bomberman.git
```

2. Unity Hub'dan projeyi açın (Unity 2021.3 veya üzeri önerilir)

3. Unity Editor'de projeyi açtıktan sonra ilgili sahneyi çalıştırın

## 🎯 Oynanış

### Kontroller
**Oyuncu 1:**
- **WASD:** Hareket
- **Space:** Bomba Yerleştir

### Power-Up'lar
- 🎈 **Extra Bomb:** Aynı anda daha fazla bomba koyabilirsiniz
- 💥 **Blast Radius:** Bomba patlama menzili artar
- ⚡ **Speed Increase:** Hareket hızınız artar

### Amaç
Tüm düşmanları yok edin ve son kalan oyuncu olun!

## 🔧 Teknik Detaylar

### Kullanılan Teknolojiler
- **Engine:** Unity 2021.3+
- **Programlama Dili:** C#
- **Mimari:** Component-based with Design Patterns
- **Fizik:** Rigidbody2D, Collision Detection
- **Rendering:** 2D Sprite Renderer, Tilemap System

### Proje Yapısı
```
Assets/
├── Scripts/
│   ├── BombStructures.cs          # Decorator Pattern (Bomba özellikleri)
│   ├── BombFactory.cs             # Factory Pattern (Bomba üretimi)
│   ├── BasicBombFactory.cs        # Concrete Factory
│   ├── BombController.cs          # Bomba mantığı ve patlama sistemi
│   ├── EnemyStrategies.cs         # Strategy Pattern (Düşman AI)
│   ├── EnemyController.cs         # Düşman kontrolü
│   ├── IInputAdapter.cs           # Adapter Pattern arayüzü
│   ├── KeyboardInputAdapter.cs    # Klavye adapter implementasyonu
│   ├── MovementController.cs      # Observer Subject (Oyuncu hareketi)
│   ├── IPlayerState.cs            # State Pattern arayüzü
│   ├── PlayerAliveState.cs        # Alive state implementasyonu
│   ├── PlayerDeadState.cs         # Dead state implementasyonu
│   ├── GameManager.cs             # Singleton & Observer (Oyun yönetimi)
│   ├── SessionManager.cs          # Singleton (Session yönetimi)
│   ├── LoginController.cs         # Kullanıcı girişi
│   ├── Explosions.cs              # Patlama efektleri
│   ├── ItemPickup.cs              # Power-up sistemi
│   ├── HardWall.cs                # Güçlendirilmiş duvar
│   ├── Destructible.cs            # Yıkılabilir nesneler
│   └── AnimatedSpriteRenderer.cs  # Animasyon sistemi
├── Prefabs/
├── Scenes/
└── Materials/
```

### Design Pattern Özeti
| Pattern | Kullanım Yeri | Amaç |
|---------|---------------|------|
| **Singleton** | GameManager, SessionManager | Tek instance garantisi |
| **Decorator** | BombStructures (RadiusEnhancer) | Bomba özelliklerini dinamik artırma |
| **Factory** | BombFactory, BasicBombFactory | Bomba üretimini merkezileştirme |
| **Strategy** | EnemyStrategies (Fool, Normal, Clever) | Düşman davranışlarını değiştirilebilir yapma |
| **Observer** | MovementController → GameManager | Oyuncu olaylarını dinleme |
| **State** | PlayerAliveState, PlayerDeadState | Oyuncu durumlarını yönetme |
| **Adapter** | KeyboardInputAdapter | Farklı input sistemlerini tek arayüze çevirme |
| **Repository** | FileRepository | Veri erişimini soyutlama |

## 📊 Mimari Prensipler

Bu projede uygulanan SOLID prensipleri:

- **Single Responsibility:** Her sınıf tek bir sorumluluğa sahip
- **Open/Closed:** Decorator ve Strategy pattern'ler sayesinde genişlemeye açık, değişime kapalı
- **Liskov Substitution:** Interface'ler sayesinde sınıflar birbirinin yerine kullanılabilir
- **Interface Segregation:** Küçük, özel amaçlı interface'ler (IInputAdapter, IEnemyStrategy)
- **Dependency Inversion:** Somut sınıflar yerine soyutlamalara bağımlılık

## 📞 İletişim

Sorularınız veya geri bildirimleriniz için benimle iletişime geçebilirsiniz.

**GitHub:** [@Merttyillmazz](https://github.com/Merttyillmazz)
instagram : https://www.instagram.com/mmertyillmaz/

---

⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın!

## 📄 Lisans

Bu proje eğitim amaçlıdır ve özgürce kullanılabilir.
