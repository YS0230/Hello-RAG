const BASE = '/api/jiebadict'

export interface JiebaDictEntry {
  id: number
  word: string
  frequency: number
  partOfSpeech: string
  isActive: boolean
  createdAt: string
}

export interface JiebaDictPayload {
  word: string
  frequency: number
  partOfSpeech: string
  isActive: boolean
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text().catch(() => `HTTP ${res.status}`)
    throw new Error(text || `HTTP ${res.status}`)
  }
  return res.json()
}

async function handleEmpty(res: Response): Promise<void> {
  if (!res.ok) {
    const text = await res.text().catch(() => `HTTP ${res.status}`)
    throw new Error(text || `HTTP ${res.status}`)
  }
}

export const jiebaDictApi = {
  getAll: () => fetch(BASE).then(r => handleResponse<JiebaDictEntry[]>(r)),

  create: (payload: JiebaDictPayload) =>
    fetch(BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    }).then(r => handleResponse<JiebaDictEntry>(r)),

  update: (id: number, payload: JiebaDictPayload) =>
    fetch(`${BASE}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    }).then(handleEmpty),

  delete: (id: number) =>
    fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(handleEmpty),

  rebuild: () =>
    fetch(`${BASE}/rebuild`, { method: 'POST' }).then(r =>
      handleResponse<{ message: string }>(r),
    ),
}
