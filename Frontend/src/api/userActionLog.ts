export interface UserActionLog {
  id: number
  timestamp: string
  action: string
  resource: string | null
  resourceId: string | null
  httpMethod: string
  requestPath: string
  statusCode: number
  ipAddress: string | null
}

export interface LogQueryParams {
  page?: number
  pageSize?: number
  resource?: string
  action?: string
  from?: string
  to?: string
  ip?: string
}

export interface LogPageResult {
  total: number
  page: number
  pageSize: number
  items: UserActionLog[]
}

export const userActionLogApi = {
  async query(params: LogQueryParams = {}): Promise<LogPageResult> {
    const qs = new URLSearchParams()
    if (params.page) qs.set('page', String(params.page))
    if (params.pageSize) qs.set('pageSize', String(params.pageSize))
    if (params.resource) qs.set('resource', params.resource)
    if (params.action) qs.set('action', params.action)
    if (params.from) qs.set('from', params.from)
    if (params.to) qs.set('to', params.to)
    if (params.ip) qs.set('ip', params.ip)
    const res = await fetch(`/api/useractionlog?${qs}`)
    if (!res.ok) throw new Error('查詢失敗')
    return res.json()
  },
}
