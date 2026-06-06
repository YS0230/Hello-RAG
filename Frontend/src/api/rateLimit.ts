export interface IpDailyCount {
  ip: string
  count: number
}

export const rateLimitApi = {
  async getDaily(): Promise<IpDailyCount[]> {
    const res = await fetch('/api/ratelimit/daily')
    if (!res.ok) throw new Error('查詢失敗')
    return res.json()
  },
}
