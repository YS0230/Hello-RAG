# RAG 知識庫系統

基於 RAG（Retrieval-Augmented Generation）架構的系統規則知識庫，支援語意搜尋、AI 問答與中文全文搜尋。

## 功能

- **規則管理**：系統規則 CRUD（系統名稱、流水號、規則說明）
- **AI 語意搜尋**：Gemini Embedding + pgvector 向量相似度搜尋
- **混合搜尋**：向量搜尋 + Jieba 中文全文搜尋（RRF 融合排序）
- **AI 問答**：依語意搜尋結果，呼叫 Gemini 生成回答
- **Jieba 詞典管理**：可新增自訂詞條，影響全文搜尋精度
- **操作記錄**：自動記錄每次 API 操作（IP、Payload）
- **速率限制**：每 IP 每日請求上限，支援白名單

## Tech Stack

| 層 | 技術 |
|---|---|
| Frontend | Vue 3 + TypeScript + Pinia + Vue Router / Vite |
| Backend | ASP.NET Core (.NET 10) + EF Core 10 |
| 資料庫 | PostgreSQL + pgvector |
| AI | Google Gemini API (`gemini-embedding-001` / `gemini-2.0-flash-lite`) |
| 部署 | Docker Compose（multi-stage build） |

## 首次使用：替換機敏資訊

| 資訊 | 檔案 | 欄位 |
|------|------|------|
| DB 帳號 | `docker-compose.yml` | `POSTGRES_USER`、`ConnectionStrings__DefaultConnection` 的 `Username=` |
| DB 密碼 | `docker-compose.yml` | `POSTGRES_PASSWORD`、`ConnectionStrings__DefaultConnection` 的 `Password=` |
| DB 帳號（本機開發） | `Backend/appsettings.json` | `ConnectionStrings.DefaultConnection` 的 `Username=` |
| DB 密碼（本機開發） | `Backend/appsettings.json` | `ConnectionStrings.DefaultConnection` 的 `Password=` |
| Gemini API Key（本機開發） | `Backend/appsettings.json` 或 `Backend/appsettings.Development.json` | `Gemini.ApiKey` |
| Gemini API Key（Docker） | 環境變數 | `GEMINI_API_KEY`（見下方快速開始） |

> `docker-compose.yml` 的 `POSTGRES_USER` / `POSTGRES_PASSWORD` 與 `ConnectionStrings__DefaultConnection` 兩處必須一致。

## 快速開始

### 需求

- Docker & Docker Compose
- Google Gemini API Key（[取得](https://aistudio.google.com/app/apikey)）

### Docker 部署

```bash
# 複製範本並填入 API Key
cp .env.example .env
# 編輯 .env，將 your_api_key_here 替換為實際的 Gemini API Key

# 啟動
docker compose up --build
```

服務啟動後開啟 `http://localhost:8080`

### 本機開發

**Backend**

```bash
cd Backend
# 確保 PostgreSQL 在 localhost:5432
dotnet run
# API 位於 http://localhost:5037
```

**Frontend**

```bash
cd Frontend
npm install
npm run dev
# Dev server 位於 http://localhost:5173（proxy /api → :5037）
```

## 設定

### 環境變數 / appsettings.json

| 設定 | 說明 | 預設值 |
|---|---|---|
| `Gemini:ApiKey` | Google Gemini API Key | — |
| `ConnectionStrings:DefaultConnection` | PostgreSQL 連線字串 | localhost:5432 |
| `RateLimit:DailyLimit` | 每 IP 每日請求上限 | 100 |
| `RateLimit:AllowedIps` | 速率限制白名單 IP | `["127.0.0.1", "::1"]` |
| `Jieba:DictPath` | Jieba 自訂詞典路徑 | `/tmp/jieba_user_dict.txt` |

### Docker 端口

| 服務 | 對外 Port |
|---|---|
| Web 應用（前後端） | 8080 |
| PostgreSQL（除錯用） | 18888 |

## 搜尋架構

混合搜尋採用 **Reciprocal Rank Fusion (RRF)** 融合兩路結果：

```
向量搜尋（pgvector cosine distance < 0.35）  ──┐
                                                ├── RRF 融合 → 排序 → Top N
Jieba 全文搜尋（tsvector @@ tsquery）         ──┘
```

- 兩路命中：向量 × 0.4 + 全文 × 0.6 + bonus 0.005
- 僅向量命中 / 僅全文命中：各自分數計算
- 最終篩選 `rrf_score > 0.002`
