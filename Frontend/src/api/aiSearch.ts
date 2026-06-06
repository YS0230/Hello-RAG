import type { RuleSearchResult } from '@/api/systemRule'

export interface AiSearchRequest {
  query: string
  topN?: number
}

export interface AiSearchResponse {
  answer: string
  sources: RuleSearchResult[]
}

const BASE = '/api/aisearch'

async function extractError(res: Response): Promise<string> {
  try {
    const text = await res.text()
    if (!text) return `HTTP ${res.status}`
    try {
      const json = JSON.parse(text)
      return json.detail ?? json.title ?? json.message ?? json.error?.message ?? text
    } catch {
      return text
    }
  } catch {
    return `HTTP ${res.status}`
  }
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) throw new Error(await extractError(res))
  return res.json()
}

export const aiSearchApi = {
  search: (req: AiSearchRequest) =>
    fetch(BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req),
    }).then(r => handleResponse<AiSearchResponse>(r)),
}
