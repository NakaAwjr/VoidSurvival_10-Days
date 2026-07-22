# Weather System (天気システム) — README

センサーデータ（放射線・ジャイロ・地磁気）を **Low / Mid / High** の3段階に分類し、
それらを合成して **天気（晴れ / 曇り / 雨 / 雷雨）** を算出するシステム。
他のスクリプトは `WeatherManager` を通じて、現在の天気・各レベルをいつでも取得できる。

---

## 1. 公開API（他プログラムが使うのはこれだけ）

すべて `WeatherManager`（シングルトン）経由でアクセスする。

```csharp
// --- 値を読む（いつでも） ---
Weather now   = WeatherManager.Instance.CurrentWeather;   // 現在の天気
Level   rad   = WeatherManager.Instance.RadiationLevel;   // 放射線 Low/Mid/High
Level   gyro  = WeatherManager.Instance.GyroLevel;        // ジャイロ Low/Mid/High
Level   mag   = WeatherManager.Instance.MagLevel;         // 地磁気 Low/Mid/High

// --- 変化を受け取る（イベント） ---
WeatherManager.Instance.OnWeatherChanged += (Weather w) => { /* ... */ };
```

### enum 定義

```csharp
public enum Level   { Low = 0, Mid = 1, High = 2 }            // 数値も持つ（点数計算用）
public enum Weather { Sunny, Cloudy, Rainy, Thunderstorm }   // 晴れ/曇り/雨/雷雨
```

---

## 2. 使用例

### 例A：天気の変化に反応する（推奨）

天気が「変わった瞬間」だけ処理したいとき（BGM切替、空の色変更、敵スポーン率変更など）。

```csharp
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    void Start()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged += OnWeatherChanged;
    }

    void OnDestroy()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= OnWeatherChanged; // 解除を忘れない
    }

    void OnWeatherChanged(Weather weather)
    {
        switch (weather)
        {
            case Weather.Sunny:        spawnRate = 1.0f; break;
            case Weather.Cloudy:       spawnRate = 1.5f; break;
            case Weather.Rainy:        spawnRate = 2.0f; break;
            case Weather.Thunderstorm: spawnRate = 3.0f; break;
        }
    }

    private float spawnRate = 1.0f;
}
```

### 例B：現在の値を毎フレーム読む（ポーリング）

```csharp
void Update()
{
    if (WeatherManager.Instance == null) return;

    // 放射線が High のときだけダメージ
    if (WeatherManager.Instance.RadiationLevel == Level.High)
        playerHp -= radiationDamage * Time.deltaTime;
}
```

### 例C：レベルを数値として使う

`Level` は `Low=0, Mid=1, High=2` の整数を持つので、そのまま計算に使える。

```csharp
int dangerScore = (int)WeatherManager.Instance.RadiationLevel
                + (int)WeatherManager.Instance.MagLevel;   // 0〜4
```

---

## 3. APIリファレンス

### WeatherManager（シングルトン / MonoBehaviour）
| メンバー | 型 | 説明 |
|---|---|---|
| `Instance` | `static WeatherManager` | 唯一の実体 |
| `CurrentWeather` | `Weather` | 現在の天気（読み取り専用） |
| `RadiationLevel` | `Level` | 放射線の段階 |
| `GyroLevel` | `Level` | ジャイロの段階 |
| `MagLevel` | `Level` | 地磁気の段階 |
| `OnWeatherChanged` | `event Action<Weather>` | 天気が**変化した時のみ**発火 |
| `ApplyRawValues(float rad, float gyro, float mag)` | `void` | 生値を渡して即評価（テスト/手動用） |
| `ForceEvaluate()` | `void` | 本番経路で手動再評価 |

### WeatherCalculator（static / 純粋ロジック）
| メソッド | 説明 |
|---|---|
| `Calculate(Level rad, Level gyro, Level mag)` → `Weather` | レベルを点数化して天気を決定。UnityEngine非依存なので流用可 |

### SensorThreshold（ScriptableObject）
| メンバー | 説明 |
|---|---|
| `midThreshold` / `highThreshold` | 段階の境界値（Inspectorで調整） |
| `Classify(float value)` → `Level` | 値を Low/Mid/High に変換 |

### WeatherDataManager（シングルトン / データ供給）
PlayFabから取得した「現在時刻のセンサー値」を保持する。通常は直接触らない。
| メンバー | 型 |
|---|---|
| `Instance` | `static WeatherDataManager` |
| `CurrentGyro` | `GyroData` |
| `CurrentMag1` / `CurrentMag2` / `CurrentMag3` | `MagData` |
| `CurrentRadiation` | `RadiationData` |
| `IsDataLoaded` | `bool` |

---

## 4. ファイル構成と役割

| ファイル | 種類 | 役割 |
|---|---|---|
| `WeatherManager.cs` | シングルトン | **中心。**天気の保持・通知。他はここを使う |
| `SensorEnums.cs` | enum | `Level` と `Weather` の定義 |
| `SensorThreshold.cs` | ScriptableObject | しきい値設定（Inspectorで調整） |
| `SensorClassifier.cs` | static | 生データ→数値化（エラー値除去込み） |
| `WeatherCalculator.cs` | static | レベル→天気の判定ロジック |
| `WeatherDataManager.cs` | シングルトン | PlayFabからデータ取得・現在値を供給 |
| `WeatherDisplay.cs` | UI | 現在の天気を画面表示 |
| `WeatherListenerExample.cs` | 見本 | 天気を受け取る側のサンプル |
| `WeatherTester.cs` | テスト | スライダーで天気を手動確認（PlayFab不要） |

---

## 5. セットアップ

1. `SensorThreshold` アセットを3つ作る（`Create > VoidSurvival > SensorThreshold`）。
   - Radiation: Mid=40 / High=70
   - Gyro:      Mid=0.8 / High=1.6
   - Mag:       Mid=30000 / High=60000
2. 空GameObjectに `WeatherManager` を付け、Inspectorの3欄にアセットを割り当てる。
3. 空GameObjectに `WeatherDataManager` を付ける（本番データ用）。
4. 必要に応じて `WeatherDisplay` / `WeatherTester` を付ける。

---

## 6. データの流れ

```
[実機センサー]
   ↓ CSV/JSON
[Google Apps Script]  ── 生の数値を送信 ──▶ [PlayFab TitleData : "GameMasterData"]
                                                   │
                                                   ▼
                                    [WeatherDataManager]  ← 現在時刻の値を抽出
                                                   │ CurrentGyro / CurrentMag1..3 / CurrentRadiation
                                                   ▼
                                       [WeatherManager]
                                         ├ SensorThreshold で Low/Mid/High に分類
                                         └ WeatherCalculator で天気を合成
                                                   │ CurrentWeather / OnWeatherChanged
                                                   ▼
                            [他のスクリプト：敵・UI・BGM・WeatherDisplay …]
```

要点：**分類や天気判定はUnity側で行う**。Apps Scriptは生の数値を送るだけ（しきい値を変えても再アップロード不要）。

---

## 7. 天気の決まり方（調整ポイント）

`WeatherCalculator.Calculate` が点数式で判定している。

```
score = (int)radiation * 2 + (int)gyro + (int)mag   // 範囲 0〜8（放射線を重く）
score >= 7 → Thunderstorm（雷雨）
score >= 5 → Rainy（雨）
score >= 2 → Cloudy（曇り）
それ未満   → Sunny（晴れ）
```

- 重みや境界を変えたい → `WeatherCalculator.cs` を編集。
- 各センサーの段階の境界を変えたい → 対応する `SensorThreshold` アセットの値を変更。

---

## 8. 注意点・ハマりどころ

- **時間が進まないとデータも天気も変わらない。** `TimeManager` はタイマー開始に
  `ResumeTimer()` の呼び出しが必要（または Awake で `StartCoroutine`）。テスト中は
  `minutesPerDay` を 1〜2 に下げると速く確認できる。
- **地磁気の `-1,600,000` はエラー値。** `SensorClassifier` が自動除外するので、
  しきい値計算には影響しない。
- **OnWeatherChanged は「変化時のみ」発火。** 現在値が常に欲しい場合は
  `CurrentWeather` をポーリングする。
- **日本語が □ になる場合**はTMPフォントに日本語が無いだけ。日本語フォントアセットを
  作って割り当てる（ロジックとは無関係）。
- 購読したら `OnDestroy` で**必ず解除**する。
