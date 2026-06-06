export interface SystemRule {
  id: number
  systemName: string
  serialNumber: string
  system: string
  ruleDescription: string
  recorder: string
  createdAt: string
  updatedAt: string
}

export interface SystemRulePayload {
  systemName: string
  serialNumber: string
  system: string
  ruleDescription: string
  recorder: string
}

export interface RuleSearchResult extends Omit<SystemRule, 'updatedAt'> {
  similarity: number
  vectorSimilarity: number
  textScore: number
  searchVector?: string
}

export interface RuleSearchRequest {
  query: string
  topN?: number
}

const BASE = '/api/systemrule'
const SEARCH_BASE = '/api/rulesearch'

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

async function handleEmpty(res: Response): Promise<void> {
  if (!res.ok) throw new Error(await extractError(res))
}

export const systemRuleApi = {
  getAll: () =>
    fetch(BASE).then(r => handleResponse<SystemRule[]>(r)),

  getById: (id: number) =>
    fetch(`${BASE}/${id}`).then(r => handleResponse<SystemRule>(r)),

  create: (payload: SystemRulePayload) =>
    fetch(BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    }).then(r => handleResponse<SystemRule>(r)),

  update: (id: number, payload: SystemRulePayload) =>
    fetch(`${BASE}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ id, ...payload }),
    }).then(handleEmpty),

  delete: (id: number) =>
    fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(handleEmpty),
}

export const ruleSearchApi = {
  search: (req: RuleSearchRequest) =>
    fetch(SEARCH_BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req),
    }).then(r => handleResponse<RuleSearchResult[]>(r)),
}
